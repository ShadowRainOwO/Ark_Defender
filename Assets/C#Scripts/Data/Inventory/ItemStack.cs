using System;
using UnityEngine;

[Serializable]
public class ItemStack
{
    [SerializeField] private ItemData item;
    [Min(0)]
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
