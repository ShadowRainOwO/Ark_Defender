using System;
using UnityEngine;

/// <summary>商店交易失败的原因，供界面显示具体提示。</summary>
public enum ShopFailureReason
{
    None,           // 没有失败
    InvalidRequest, // 商店、商品、容器或数量无效
    OutOfStock,     // 商品库存不足
    NotEnoughMoney, // 玩家金币不足
    TargetFull,     // 接收商品的容器已满
    EmptySlot       // 出售来源格为空
}

/// <summary>一次购买或出售的执行结果。</summary>
public struct ShopResult
{
    public bool Success { get; }
    public int Amount { get; }
    public int MoneyChanged { get; }
    public ShopFailureReason FailureReason { get; }

    private ShopResult(bool success, int amount, int moneyChanged, ShopFailureReason reason)
    {
        Success = success;
        Amount = amount;
        MoneyChanged = moneyChanged;
        FailureReason = reason;
    }

    public static ShopResult Succeeded(int amount, int moneyChanged)
    {
        return new ShopResult(true, amount, moneyChanged, ShopFailureReason.None);
    }

    public static ShopResult Failed(ShopFailureReason reason)
    {
        return new ShopResult(false, 0, 0, reason);
    }
}

/// <summary>
/// 处理购买、出售、价格、库存与玩家金币。
/// ShopData 只保存商品目录；实际交易必须经过本类，不能直接用 ItemTransferManager。
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("玩家货币（临时实现）")]
    [Min(0)]
    [Tooltip("当前直接保存在 ShopManager 中；以后接入存档时可替换为独立 WalletData。")]
    [SerializeField] private int playerMoney;

    public int PlayerMoney => playerMoney;
    public event Action<int> MoneyChanged;

    /// <summary>
    /// 从商店购买商品并放入目标容器。
    /// 会同时受请求数量、商店库存、玩家金币和目标容量限制。
    /// </summary>
    public ShopResult Buy(ShopData shop, int entryIndex, IItemContainer target, int amount)
    {
        ShopEntry entry = shop != null ? shop.GetEntry(entryIndex) : null;
        if (entry == null || entry.Item == null || target == null || amount <= 0)
            return ShopResult.Failed(ShopFailureReason.InvalidRequest);
        if (!entry.HasInfiniteStock && entry.Stock <= 0)
            return ShopResult.Failed(ShopFailureReason.OutOfStock);
        if (entry.BuyPrice <= 0)
            return ShopResult.Failed(ShopFailureReason.InvalidRequest);

        int availableStock = entry.HasInfiniteStock ? amount : Mathf.Min(amount, entry.Stock);
        int affordable = playerMoney / entry.BuyPrice;
        if (affordable <= 0)
            return ShopResult.Failed(ShopFailureReason.NotEnoughMoney);

        int buyAmount = Mathf.Min(availableStock, affordable, target.GetAddableAmount(entry.Item));
        if (buyAmount <= 0)
            return ShopResult.Failed(ShopFailureReason.TargetFull);

        // 先扣库存再加入背包；若背包未完全接收，则把差额退回商店。
        int taken = shop.TakeStock(entryIndex, buyAmount);
        int added = target.Add(entry.Item, taken);
        if (added < taken) shop.AddStock(entryIndex, taken - added);

        int cost = added * entry.BuyPrice;
        playerMoney -= cost;
        MoneyChanged?.Invoke(playerMoney);
        return ShopResult.Succeeded(added, -cost);
    }

    /// <summary>
    /// 从普通容器的指定格子出售物品。
    /// 当前按 ItemData 基础价值的一半收购，尚未把售出物加入商店回购列表。
    /// </summary>
    public ShopResult Sell(IItemContainer source, int sourceSlot, int amount)
    {
        ItemStack stack = source?.GetItem(sourceSlot);
        if (source == null || amount <= 0)
            return ShopResult.Failed(ShopFailureReason.InvalidRequest);
        if (stack == null || stack.IsEmpty)
            return ShopResult.Failed(ShopFailureReason.EmptySlot);

        int sellAmount = Mathf.Min(amount, stack.Amount);
        int unitPrice = Mathf.Max(0, stack.Item.BaseMoneyValue / 2);
        int removed = source.RemoveAt(sourceSlot, sellAmount);
        int income = removed * unitPrice;
        playerMoney += income;
        MoneyChanged?.Invoke(playerMoney);
        return ShopResult.Succeeded(removed, income);
    }
}
