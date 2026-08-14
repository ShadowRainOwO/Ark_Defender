using UnityEngine;

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

    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float Armor => armor;
    public float BaseMoveSpeed => baseMoveSpeed;
    public float SprintMultiplier => sprintMultiplier;
}
