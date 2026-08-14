using UnityEngine;

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

    private bool isSprinting;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float Armor => armor;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float SprintMultiplier => sprintMultiplier;
    public float CurrentMoveSpeed => baseMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

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
}
