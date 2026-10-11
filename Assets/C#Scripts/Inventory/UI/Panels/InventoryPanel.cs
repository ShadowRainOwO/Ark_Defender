/// <summary>
/// 玩家背包的具体面板类型。
/// 当前复用 ItemContainerPanel 的全部行为，独立类型便于 UIManager 定位和后续扩展。
/// </summary>
public class InventoryPanel : ItemContainerPanel
{
    protected override string DefaultTitle => "背包";
}
