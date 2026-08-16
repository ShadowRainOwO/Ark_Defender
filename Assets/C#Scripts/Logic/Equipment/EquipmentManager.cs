using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private ItemData equippedWeapon;
    [SerializeField] private ItemData equippedArmor;

    public ItemData EquippedWeapon => equippedWeapon;
    public ItemData EquippedArmor => equippedArmor;
    public event Action EquipmentChanged;

    public bool TryEquip(ItemData item, out ItemData replacedItem)
    {
        replacedItem = null;
        if (item == null) return false;

        switch (item.Category)
        {
            case ItemCategory.Weapon:
                replacedItem = equippedWeapon;
                equippedWeapon = item;
                break;
            case ItemCategory.Armor:
                replacedItem = equippedArmor;
                equippedArmor = item;
                break;
            default:
                return false;
        }

        EquipmentChanged?.Invoke();
        return true;
    }
}
