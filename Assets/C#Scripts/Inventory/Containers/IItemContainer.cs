using System;

/// <summary>
/// 所有“可存放普通物品的容器”都应实现此接口。
/// 玩家背包、玩家仓库和场景容器通过该接口被 UI 与转移逻辑统一处理。
/// 商店不是普通容器，因为购买和出售还需要处理金币与售价，所以不实现此接口。
/// </summary>
public interface IItemContainer
{
    /// <summary>容器拥有的格子总数。</summary>
    int SlotCount { get; }

    /// <summary>容器内容发生变化后触发，UI 通过它刷新格子。</summary>
    event Action Changed;

    /// <summary>获取指定格子的物品堆；索引无效时返回 null。</summary>
    ItemStack GetItem(int slotIndex);

    /// <summary>计算当前容器最多还能加入多少个指定物品。</summary>
    int GetAddableAmount(ItemData item);

    /// <summary>加入物品并返回实际加入的数量，容量不足时可能小于 amount。</summary>
    int Add(ItemData item, int amount);

    /// <summary>从指定格子移除物品并返回实际移除的数量。</summary>
    int RemoveAt(int slotIndex, int amount);
}
