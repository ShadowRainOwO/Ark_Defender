using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包、仓库和场景容器共用的数据基类。
/// 负责维护固定数量的格子，以及物品的堆叠、添加、移除和变化通知。
/// 具体数据类型只负责表达业务身份，不重复实现容器算法。
/// </summary>
public abstract class ItemContainerData : MonoBehaviour, IItemContainer
{
    [Header("容器容量")]
    [Min(1)]
    [Tooltip("该容器拥有的固定格子数量。")]
    [SerializeField] private int slotCount = 24;

    [Header("运行时物品")]
    [Tooltip("每个元素对应一个格子；通常由容器方法维护，不要在运行时直接修改。")]
    [SerializeField] private List<ItemStack> slots = new List<ItemStack>();

    public int SlotCount => slotCount;
    public event Action Changed;

    protected virtual void Awake()
    {
        // 保证反序列化后的列表长度与 Inspector 中配置的格子数量一致。
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

        // 空格可以容纳完整的一组，同类物品格只计算剩余堆叠空间。
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

        // 第一步：优先补满已有的同类物品堆，避免浪费空格。
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

        // 第二步：仍有剩余时再占用空格。
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

    /// <summary>
    /// 尝试修改容器容量。
    /// 扩容会创建空格；缩容时如果将被移除的尾部格子中还有物品，则拒绝操作。
    /// </summary>
    public bool TrySetSlotCount(int newSlotCount)
    {
        newSlotCount = Mathf.Max(1, newSlotCount);
        EnsureSlotCount();

        if (newSlotCount == slotCount)
        {
            return true;
        }

        if (newSlotCount < slotCount)
        {
            for (int i = newSlotCount; i < slots.Count; i++)
            {
                ItemStack stack = slots[i];
                if (stack != null && !stack.IsEmpty)
                {
                    return false;
                }
            }
        }

        slotCount = newSlotCount;
        EnsureSlotCount();
        Changed?.Invoke();
        return true;
    }

    public void NotifyChanged()
    {
        // 提供给存档加载等批量修改场景，通知所有观察者刷新。
        Changed?.Invoke();
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slotCount;
    }

    private void EnsureSlotCount()
    {
        // Unity 反序列化或 Inspector 改容量时，列表可能为空或长度不一致。
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
