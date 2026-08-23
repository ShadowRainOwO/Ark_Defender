using UnityEngine;

/// <summary>普通容器之间转移失败的原因，供 UI 显示对应提示。</summary>
public enum TransferFailureReason
{
    None,             // 没有失败
    InvalidContainer, // 来源或目标容器为空
    InvalidSlot,      // 来源格子索引无效
    InvalidAmount,    // 请求数量小于等于 0
    EmptySlot,        // 来源格子没有物品
    SameContainer,    // 当前逻辑不允许容器内部转移
    TargetFull        // 目标容器没有可用容量
}

/// <summary>一次普通物品转移的执行结果。</summary>
public struct TransferResult
{
    public bool Success { get; }
    public int TransferredAmount { get; }
    public TransferFailureReason FailureReason { get; }

    private TransferResult(bool success, int amount, TransferFailureReason reason)
    {
        Success = success;
        TransferredAmount = amount;
        FailureReason = reason;
    }

    public static TransferResult Succeeded(int amount)
    {
        return new TransferResult(true, amount, TransferFailureReason.None);
    }

    public static TransferResult Failed(TransferFailureReason reason)
    {
        return new TransferResult(false, 0, reason);
    }
}

/// <summary>
/// 负责背包、仓库和场景容器之间的普通物品转移。
/// 不处理商店交易，因为购买和出售必须额外校验金币与价格。
/// </summary>
public class ItemTransferManager : MonoBehaviour
{
    public static ItemTransferManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 从来源格子向目标容器转移指定数量。
    /// 如果目标容量不足会尽可能部分转移，并返回实际数量。
    /// </summary>
    public TransferResult Transfer(
        IItemContainer source,
        int sourceSlot,
        IItemContainer target,
        int amount)
    {
        if (source == null || target == null)
            return TransferResult.Failed(TransferFailureReason.InvalidContainer);
        if (ReferenceEquals(source, target))
            return TransferResult.Failed(TransferFailureReason.SameContainer);
        if (amount <= 0)
            return TransferResult.Failed(TransferFailureReason.InvalidAmount);

        ItemStack sourceStack = source.GetItem(sourceSlot);
        if (sourceStack == null)
            return TransferResult.Failed(TransferFailureReason.InvalidSlot);
        if (sourceStack.IsEmpty)
            return TransferResult.Failed(TransferFailureReason.EmptySlot);

        ItemData item = sourceStack.Item;
        int transferAmount = Mathf.Min(amount, sourceStack.Amount, target.GetAddableAmount(item));
        if (transferAmount <= 0)
            return TransferResult.Failed(TransferFailureReason.TargetFull);

        // 先从来源移除，再加入目标；若目标实际接收量不足，将差额回滚给来源。
        int removed = source.RemoveAt(sourceSlot, transferAmount);
        int added = target.Add(item, removed);

        if (added < removed)
        {
            source.Add(item, removed - added);
        }

        return added > 0
            ? TransferResult.Succeeded(added)
            : TransferResult.Failed(TransferFailureReason.TargetFull);
    }
}
