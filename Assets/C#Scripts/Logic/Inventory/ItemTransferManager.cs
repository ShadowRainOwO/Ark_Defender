using UnityEngine;

public enum TransferFailureReason
{
    None,
    InvalidContainer,
    InvalidSlot,
    InvalidAmount,
    EmptySlot,
    SameContainer,
    TargetFull
}

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
