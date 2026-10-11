using System;
using UnityEngine;

/// <summary>
/// 当前装备状态的基础管理器，只区分武器和护甲两类槽位。
/// 本类负责替换装备引用，但不自动从背包扣除物品；背包联动应由更上层操作流程负责。
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    [Header("当前装备")]
    [SerializeField] private ItemData equippedWeapon;
    [SerializeField] private ItemData equippedArmor;

    public ItemData EquippedWeapon => equippedWeapon;
    public ItemData EquippedArmor => equippedArmor;
    public event Action EquipmentChanged;

    /// <summary>
    /// 尝试装备物品，并通过 replacedItem 返回被替换下来的旧装备。
    /// 非武器、非护甲物品会返回 false。
    /// </summary>
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
