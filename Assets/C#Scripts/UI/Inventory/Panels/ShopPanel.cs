using System;

/// <summary>
/// 商店页面视图。
/// 复用 SlotPanelBase 的标题和空格子生成能力，但不把商店当作普通 IItemContainer。
/// </summary>
public class ShopPanel : SlotPanelBase
{
    private ShopData shop;

    /// <summary>当前打开的商店数据。</summary>
    public ShopData Shop => shop;

    /// <summary>商店数据变化后触发，后续商品显示组件可监听。</summary>
    public event Action Refreshed;

    protected override string DefaultTitle => "商店";

    /// <summary>切换当前显示的商店并更新事件订阅。</summary>
    public void Bind(ShopData target)
    {
        if (shop != null)
        {
            shop.Changed -= Refresh;
        }

        shop = target;

        if (shop != null)
        {
            shop.Changed += Refresh;
        }

        Refresh();
    }

    public void Refresh()
    {
        RefreshPanel();
        Refreshed?.Invoke();
    }

    protected override int GetRequiredSlotCount()
    {
        // 基础阶段至少显示配置的空格子；商品超过该数量时自动扩充。
        int shopEntryCount = shop != null ? shop.Entries.Count : 0;
        return Math.Max(DefaultSlotCount, shopEntryCount);
    }

    protected virtual void OnDestroy()
    {
        if (shop != null)
        {
            shop.Changed -= Refresh;
        }
    }
}
