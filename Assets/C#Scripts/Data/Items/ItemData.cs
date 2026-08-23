using UnityEngine;

/// <summary>物品的大类，用于筛选、装备判断和不同业务规则。</summary>
public enum ItemCategory
{
    Weapon,         //武器
    Armor,          //护甲
    Consumable,     //消耗品
    Material,       //材料
    Quest,          //任务
    Miscellaneous   //杂项
}

/// <summary>
/// 单种物品的静态配置资源。
/// 一个 ItemData 资源可被许多 ItemStack 引用，运行时数量不要存放在这里。
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Ark Defender/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本信息")]
    [Tooltip("用于存档或配置表匹配的唯一标识。")]
    [SerializeField] private string itemId;
    [Tooltip("物品所属类别。")]
    [SerializeField] private ItemCategory category;

    [Header("数值")]
    [Min(1)]
    [Tooltip("单个格子允许堆叠的最大数量。")]
    [SerializeField] private int maxStack = 99;

    [Min(0f)]
    [Tooltip("单个物品的重量。")]
    [SerializeField] private float weight;

    [Min(0)]
    [Tooltip("默认价值；商店未填写覆盖价格时会以此为基础。")]
    [SerializeField] private int baseMoneyValue;

    [Header("物品简介")]
    [TextArea(3, 8)]
    [SerializeField] private string description;

    [Header("UI 显示")]
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public ItemCategory Category => category;
    public int MaxStack => Mathf.Max(1, maxStack);
    public float Weight => weight;
    public int BaseMoneyValue => baseMoneyValue;
    public string Description => description;
    public Sprite Icon => icon;
}
