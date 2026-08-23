using System;
using UnityEngine;

/// <summary>
/// 一个格子中的运行时物品数据，由“物品配置 + 当前数量”组成。
/// ItemData 保存静态配置，ItemStack 只保存会在游戏过程中变化的数量。
/// </summary>
[Serializable]
public class ItemStack
{
    [Tooltip("该格子中的物品配置。")]
    [SerializeField] private ItemData item;
    [Min(0)]
    [Tooltip("当前堆叠数量；为 0 时格子会被清空。")]
    [SerializeField] private int amount;

    public ItemData Item => item;
    public int Amount => amount;
    public bool IsEmpty => item == null || amount <= 0;

    public ItemStack()
    {
    }

    public ItemStack(ItemData item, int amount)
    {
        this.item = item;
        this.amount = Mathf.Max(0, amount);
        ClearIfEmpty();
    }

    public ItemStack Clone()
    {
        return new ItemStack(item, amount);
    }

    internal void Set(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = Mathf.Max(0, newAmount);
        ClearIfEmpty();
    }

    internal void ChangeAmount(int delta)
    {
        amount = Mathf.Max(0, amount + delta);
        ClearIfEmpty();
    }

    private void ClearIfEmpty()
    {
        if (amount <= 0)
        {
            item = null;
            amount = 0;
        }
    }
}
