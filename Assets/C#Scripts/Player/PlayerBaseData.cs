using UnityEngine;

/// <summary>
/// 玩家的基础属性配置资源。
/// 保存角色出生时就确定的静态数值，运行时变化不要直接写回该资源。
/// </summary>
[CreateAssetMenu(fileName = "NewPlayerBaseData", menuName = "Ark Defender/Player Base Data")]
public class PlayerBaseData : ScriptableObject
{
    [Header("生命值")]
    [Min(1f)]
    [SerializeField] private float maxHealth = 100f;

    [Header("体力值")]
    [Min(1f)]
    [SerializeField] private float maxStamina = 100f;

    [Header("防御属性")]
    [Min(0f)]
    [SerializeField] private float armor;

    [Header("移动属性")]
    [Min(0f)]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Min(1f)]
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("背包属性")]
    [Min(1)]
    [Tooltip("角色不受装备、天赋等加成时拥有的默认背包格子数。")]
    [SerializeField] private int defaultInventorySlots = 20;

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float Armor => armor;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float SprintMultiplier => sprintMultiplier;
    public int DefaultInventorySlots => Mathf.Max(1, defaultInventorySlots);
}
