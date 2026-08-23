using UnityEngine;

/// <summary>
/// 玩家随身背包的数据组件。
/// 最终容量来自 PlayerStats 的“默认格子 + 额外格子”，UI 只读取本类的 SlotCount。
/// </summary>
[RequireComponent(typeof(PlayerStats))]
public class PlayerInventory : ItemContainerData
{
    [Header("人物属性")]
    [Tooltip("提供默认背包格子和额外背包格子。留空时自动查找同物体上的 PlayerStats。")]
    [SerializeField] private PlayerStats playerStats;

    public PlayerStats PlayerStats => playerStats;

    protected override void Awake()
    {
        base.Awake();

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        RefreshCapacityFromStats();
    }

    /// <summary>
    /// 根据人物属性重新同步背包容量。
    /// 适合读取存档后调用；如果缩容会移除仍有物品的格子，则返回 false。
    /// </summary>
    public bool RefreshCapacityFromStats()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerInventory 没有找到 PlayerStats，无法同步背包容量。", this);
            return false;
        }

        return TrySetSlotCount(playerStats.TotalInventorySlots);
    }

    /// <summary>
    /// 安全修改额外背包格子数。
    /// 先验证实际容器能否调整，再把结果写入 PlayerStats，避免属性与容器容量不一致。
    /// </summary>
    public bool TrySetExtraInventorySlots(int newExtraSlots)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerInventory 没有找到 PlayerStats，无法修改额外背包容量。", this);
            return false;
        }

        newExtraSlots = Mathf.Max(0, newExtraSlots);
        int targetSlotCount = playerStats.DefaultInventorySlots + newExtraSlots;

        if (!TrySetSlotCount(targetSlotCount))
        {
            Debug.LogWarning(
                $"背包无法缩小到 {targetSlotCount} 格：将被移除的格子中仍有物品。",
                this);
            return false;
        }

        playerStats.SetExtraInventorySlots(newExtraSlots);
        return true;
    }

    /// <summary>在现有额外容量基础上增加或减少格子。</summary>
    public bool TryAddExtraInventorySlots(int delta)
    {
        if (playerStats == null)
        {
            return false;
        }

        return TrySetExtraInventorySlots(playerStats.ExtraInventorySlots + delta);
    }
}
