using UnityEngine;

public enum ItemCategory
{
    Weapon,         //武器
    Armor,          //护甲
    Consumable,     //消耗品
    Material,       //材料
    Quest,          //任务
    Miscellaneous   //杂项
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Ark Defender/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本信息")]
    [SerializeField] private string itemId;
    [SerializeField] private ItemCategory category;

    [Header("数值")]
    [Min(0f)]
    [SerializeField] private float weight;

    [Min(0)]
    [SerializeField] private int baseMoneyValue;

    [Header("物品简介")]
    [TextArea(3, 8)]
    [SerializeField] private string description;

    [Header("UI 显示")]
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public ItemCategory Category => category;
    public float Weight => weight;
    public int BaseMoneyValue => baseMoneyValue;
    public string Description => description;
    public Sprite Icon => icon;
}
