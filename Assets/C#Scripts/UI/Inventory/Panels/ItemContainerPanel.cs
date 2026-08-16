using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerPanel : MonoBehaviour
{
    [SerializeField] private ItemSlot slotPrefab;
    [SerializeField] private Transform slotRoot;

    private readonly List<ItemSlot> slots = new List<ItemSlot>();
    private IItemContainer container;

    public IItemContainer Container => container;
    public event Action<IItemContainer, int> SlotClicked;
    public event Action<IItemContainer, int> SlotDoubleClicked;

    public virtual void Bind(IItemContainer target)
    {
        if (ReferenceEquals(container, target))
        {
            Refresh();
            return;
        }

        Unbind();
        container = target;
        if (container != null) container.Changed += Refresh;
        Refresh();
    }

    public virtual void Unbind()
    {
        if (container != null) container.Changed -= Refresh;
        container = null;
    }

    public void Refresh()
    {
        int requiredSlots = container != null ? container.SlotCount : 0;
        EnsureSlotViews(requiredSlots);
        for (int i = 0; i < slots.Count; i++)
        {
            bool visible = i < requiredSlots;
            slots[i].gameObject.SetActive(visible);
            if (visible) slots[i].Bind(container, i);
        }
    }

    protected virtual void OnDestroy()
    {
        Unbind();
    }

    private void EnsureSlotViews(int count)
    {
        if (slotPrefab == null || slotRoot == null) return;
        while (slots.Count < count)
        {
            ItemSlot slot = Instantiate(slotPrefab, slotRoot);
            slot.Clicked += OnSlotClicked;
            slot.DoubleClicked += OnSlotDoubleClicked;
            slots.Add(slot);
        }
    }

    private void OnSlotClicked(ItemSlot slot)
    {
        SlotClicked?.Invoke(slot.Owner, slot.SlotIndex);
    }

    private void OnSlotDoubleClicked(ItemSlot slot)
    {
        SlotDoubleClicked?.Invoke(slot.Owner, slot.SlotIndex);
    }
}
