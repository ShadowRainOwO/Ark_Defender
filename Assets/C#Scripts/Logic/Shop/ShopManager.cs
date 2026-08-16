using System;
using UnityEngine;

public enum ShopFailureReason
{
    None,
    InvalidRequest,
    OutOfStock,
    NotEnoughMoney,
    TargetFull,
    EmptySlot
}

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

public class ShopManager : MonoBehaviour
{
    [Min(0)]
    [SerializeField] private int playerMoney;

    public int PlayerMoney => playerMoney;
    public event Action<int> MoneyChanged;

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

        int taken = shop.TakeStock(entryIndex, buyAmount);
        int added = target.Add(entry.Item, taken);
        if (added < taken) shop.AddStock(entryIndex, taken - added);

        int cost = added * entry.BuyPrice;
        playerMoney -= cost;
        MoneyChanged?.Invoke(playerMoney);
        return ShopResult.Succeeded(added, -cost);
    }

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
