using System;

/// <summary>
/// 普通物品容器面板的通用基类。
/// SlotPanelBase 负责标题和格子生成，本类只增加 IItemContainer 数据绑定与点击转发。
/// </summary>
public class ItemContainerPanel : SlotPanelBase
{
    private IItemContainer container;

    /// <summary>当前面板绑定的数据容器。</summary>
    public IItemContainer Container => container;

    /// <summary>向更上层转发格子单击事件。</summary>
    public event Action<IItemContainer, int> SlotClicked;

    /// <summary>向更上层转发格子双击事件。</summary>
    public event Action<IItemContainer, int> SlotDoubleClicked;

    /// <summary>没有具体子类时使用的备用标题。</summary>
    protected override string DefaultTitle => "物品";

    /// <summary>绑定新数据源，并订阅其变化事件。</summary>
    public virtual void Bind(IItemContainer target)
    {
        if (ReferenceEquals(container, target))
        {
            Refresh();
            return;
        }

        Unbind();
        container = target;

        if (container != null)
        {
            container.Changed += Refresh;
        }

        Refresh();
    }

    /// <summary>解除当前数据源及事件订阅。</summary>
    public virtual void Unbind()
    {
        if (container != null)
        {
            container.Changed -= Refresh;
        }

        container = null;
    }

    /// <summary>刷新容器页面。没有绑定数据时仍生成基础数量的空格子。</summary>
    public void Refresh()
    {
        RefreshPanel();
    }

    protected override int GetRequiredSlotCount()
    {
        // 绑定数据后以真实容器容量为准；未绑定时使用 Inspector 的基础数量。
        return container != null ? container.SlotCount : DefaultSlotCount;
    }

    protected override void RefreshSlot(ItemSlot slot, int slotIndex)
    {
        slot.Bind(container, slotIndex);
    }

    protected override void RegisterSlot(ItemSlot slot)
    {
        // 先取消再订阅，避免刷新时累计相同监听。
        slot.Clicked -= OnSlotClicked;
        slot.DoubleClicked -= OnSlotDoubleClicked;
        slot.Clicked += OnSlotClicked;
        slot.DoubleClicked += OnSlotDoubleClicked;
    }

    protected virtual void OnDestroy()
    {
        Unbind();
    }

    private void OnSlotClicked(ItemSlot slot)
    {
        SlotClicked?.Invoke(slot.Owner, slot.SlotIndex);
    }

    private void OnSlotDoubleClicked(ItemSlot slot)
    {
        SlotDoubleClicked?.Invoke(slot.Owner, slot.SlotIndex);
    }
}
