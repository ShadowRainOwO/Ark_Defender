using UnityEngine;

/// <summary>
/// 玩家的运行时属性。
/// 从 PlayerBaseData 初始化基础数值，并保存装备、天赋或升级产生的运行时加成。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("基础数值配置")]
    [SerializeField] private PlayerBaseData baseData;

    [Header("运行时生命值")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;

    [Header("运行时体力值")]
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxStamina;

    [Header("运行时属性")]
    [SerializeField] private float armor;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float sprintMultiplier = 1f;

    [Header("运行时背包属性")]
    [Min(0)]
    [Tooltip("装备、天赋或升级额外提供的背包格子数。运行时请通过 PlayerInventory 修改。")]
    [SerializeField] private int extraInventorySlots;

    private bool isSprinting;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float Armor => armor;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float SprintMultiplier => sprintMultiplier;
    public float CurrentMoveSpeed => baseMoveSpeed * (isSprinting ? sprintMultiplier : 1f);
    public int DefaultInventorySlots => baseData != null ? baseData.DefaultInventorySlots : 0;
    public int ExtraInventorySlots => Mathf.Max(0, extraInventorySlots);
    public int TotalInventorySlots => DefaultInventorySlots + ExtraInventorySlots;

    private void Awake()
    {
        InitializeFromBaseData();
    }

    public void InitializeFromBaseData()
    {
        if (baseData == null)
        {
            Debug.LogError("PlayerStats 没有指定 PlayerBaseData。", this);
            return;
        }

        maxHealth = baseData.MaxHealth;
        currentHealth = maxHealth;

        maxStamina = baseData.MaxStamina;
        currentStamina = maxStamina;

        armor = baseData.Armor;
        baseMoveSpeed = baseData.BaseMoveSpeed;
        sprintMultiplier = baseData.SprintMultiplier;
        extraInventorySlots = Mathf.Max(0, extraInventorySlots);
        isSprinting = false;
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);
    }

    public void SetCurrentStamina(float value)
    {
        currentStamina = Mathf.Clamp(value, 0f, maxStamina);
    }

    /// <summary>
    /// 写入额外背包格子数。
    /// 只允许 PlayerInventory 在确认容量可以安全变化后调用。
    /// </summary>
    internal void SetExtraInventorySlots(int value)
    {
        extraInventorySlots = Mathf.Max(0, value);
    }
}
