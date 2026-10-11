using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店中的一条商品记录，保存物品、库存以及买卖价格。
/// 它不是 ItemStack：商店商品还需要无限库存和独立定价等业务信息。
/// </summary>
[Serializable]
public class ShopEntry
{
    [Tooltip("出售的物品配置。")]
    [SerializeField] private ItemData item;
    [Tooltip("-1 表示无限库存")]
    [SerializeField] private int stock = -1;
    [Min(0)]
    [Tooltip("玩家购买一个该物品需要支付的价格；0 表示使用 ItemData 的基础价值。")]
    [SerializeField] private int buyPrice;
    [Min(0)]
    [Tooltip("商店收购一个该物品时支付的价格；0 表示购买价的一半。")]
    [SerializeField] private int sellPrice;

    public ItemData Item => item;
    public int Stock => stock;
    public int BuyPrice => buyPrice > 0 ? buyPrice : item != null ? item.BaseMoneyValue : 0;
    public int SellPrice => sellPrice > 0 ? sellPrice : Mathf.Max(0, BuyPrice / 2);
    public bool HasInfiniteStock => stock < 0;

    internal int TakeStock(int amount)
    {
        // 无限库存只返回请求数量，不修改 stock 字段。
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

/// <summary>
/// 一个具体商店的商品目录与库存数据。
/// ShopManager 负责交易规则，本类只负责查询和改变商品库存。
/// </summary>
public class ShopData : MonoBehaviour
{
    [Tooltip("该商店当前出售的全部商品。")]
    [SerializeField] private List<ShopEntry> entries = new List<ShopEntry>();

    public IReadOnlyList<ShopEntry> Entries => entries;

    /// <summary>商品库存变化后触发，ShopPanel 监听该事件刷新显示。</summary>
    public event Action Changed;

    /// <summary>根据商品索引取得商品记录；索引无效时返回 null。</summary>
    public ShopEntry GetEntry(int index)
    {
        return index >= 0 && index < entries.Count ? entries[index] : null;
    }

    /// <summary>扣除商品库存并返回实际扣除数量。</summary>
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

    /// <summary>将指定数量退回商品库存，主要用于购买未能完全加入背包时的回滚。</summary>
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
