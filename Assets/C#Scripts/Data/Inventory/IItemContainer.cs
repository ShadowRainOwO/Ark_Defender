using System;

public interface IItemContainer
{
    int SlotCount { get; }
    event Action Changed;

    ItemStack GetItem(int slotIndex);
    int GetAddableAmount(ItemData item);
    int Add(ItemData item, int amount);
    int RemoveAt(int slotIndex, int amount);
}
