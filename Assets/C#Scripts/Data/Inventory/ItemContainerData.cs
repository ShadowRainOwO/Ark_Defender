using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainerData : MonoBehaviour, IItemContainer
{
    [Min(1)]
    [SerializeField] private int slotCount = 24;
    [SerializeField] private List<ItemStack> slots = new List<ItemStack>();

    public int SlotCount => slotCount;
    public event Action Changed;

    protected virtual void Awake()
    {
        EnsureSlotCount();
    }

    protected virtual void OnValidate()
    {
        slotCount = Mathf.Max(1, slotCount);
        EnsureSlotCount();
    }

    public ItemStack GetItem(int slotIndex)
    {
        EnsureSlotCount();
        return IsValidSlot(slotIndex) ? slots[slotIndex] : null;
    }

    public int GetAddableAmount(ItemData item)
    {
        if (item == null)
        {
            return 0;
        }

        EnsureSlotCount();
        int capacity = 0;

        foreach (ItemStack stack in slots)
        {
            if (stack == null || stack.IsEmpty)
            {
                capacity += item.MaxStack;
            }
            else if (stack.Item == item)
            {
                capacity += Mathf.Max(0, item.MaxStack - stack.Amount);
            }
        }

        return capacity;
    }

    public int Add(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return 0;
        }

        EnsureSlotCount();
        int remaining = amount;

        foreach (ItemStack stack in slots)
        {
            if (remaining <= 0)
            {
                break;
            }

            if (stack == null || stack.IsEmpty || stack.Item != item)
            {
                continue;
            }

            int added = Mathf.Min(remaining, item.MaxStack - stack.Amount);
            stack.ChangeAmount(added);
            remaining -= added;
        }

        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i] != null && !slots[i].IsEmpty)
            {
                continue;
            }

            int added = Mathf.Min(remaining, item.MaxStack);
            slots[i] = new ItemStack(item, added);
            remaining -= added;
        }

        int totalAdded = amount - remaining;
        if (totalAdded > 0)
        {
            Changed?.Invoke();
        }

        return totalAdded;
    }

    public int RemoveAt(int slotIndex, int amount)
    {
        if (!IsValidSlot(slotIndex) || amount <= 0)
        {
            return 0;
        }

        ItemStack stack = slots[slotIndex];
        if (stack == null || stack.IsEmpty)
        {
            return 0;
        }

        int removed = Mathf.Min(amount, stack.Amount);
        stack.ChangeAmount(-removed);
        Changed?.Invoke();
        return removed;
    }

    public void NotifyChanged()
    {
        Changed?.Invoke();
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slotCount;
    }

    private void EnsureSlotCount()
    {
        if (slots == null)
        {
            slots = new List<ItemStack>();
        }

        while (slots.Count < slotCount)
        {
            slots.Add(new ItemStack());
        }

        if (slots.Count > slotCount)
        {
            slots.RemoveRange(slotCount, slots.Count - slotCount);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = new ItemStack();
            }
        }
    }
}
