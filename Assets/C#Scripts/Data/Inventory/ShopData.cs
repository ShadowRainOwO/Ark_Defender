using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopEntry
{
    [SerializeField] private ItemData item;
    [Tooltip("-1 表示无限库存")]
    [SerializeField] private int stock = -1;
    [Min(0)]
    [SerializeField] private int buyPrice;
    [Min(0)]
    [SerializeField] private int sellPrice;

    public ItemData Item => item;
    public int Stock => stock;
    public int BuyPrice => buyPrice > 0 ? buyPrice : item != null ? item.BaseMoneyValue : 0;
    public int SellPrice => sellPrice > 0 ? sellPrice : Mathf.Max(0, BuyPrice / 2);
    public bool HasInfiniteStock => stock < 0;

    internal int TakeStock(int amount)
    {
        if (amount <= 0 || item == null)
        {
            return 0;
        }

        if (HasInfiniteStock)
        {
            return amount;
        }

        int taken = Mathf.Min(amount, stock);
        stock -= taken;
        return taken;
    }

    internal void AddStock(int amount)
    {
        if (!HasInfiniteStock && amount > 0)
        {
            stock += amount;
        }
    }
}

public class ShopData : MonoBehaviour
{
    [SerializeField] private List<ShopEntry> entries = new List<ShopEntry>();

    public IReadOnlyList<ShopEntry> Entries => entries;
    public event Action Changed;

    public ShopEntry GetEntry(int index)
    {
        return index >= 0 && index < entries.Count ? entries[index] : null;
    }

    public int TakeStock(int index, int amount)
    {
        ShopEntry entry = GetEntry(index);
        int taken = entry != null ? entry.TakeStock(amount) : 0;
        if (taken > 0)
        {
            Changed?.Invoke();
        }

        return taken;
    }

    public void AddStock(int index, int amount)
    {
        ShopEntry entry = GetEntry(index);
        if (entry == null || amount <= 0)
        {
            return;
        }

        entry.AddStock(amount);
        Changed?.Invoke();
    }
}
