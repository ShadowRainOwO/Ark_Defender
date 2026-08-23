/// <summary>
/// 场景中所有可被玩家交互的对象接口。
/// InteractionManager 只依赖此接口，因此不需要知道目标是门、NPC、仓库还是容器。
/// </summary>
public interface IInteractable
{
    /// <summary>返回显示给玩家的交互提示文本。</summary>
    string GetInteractText();

    /// <summary>玩家按下交互键时执行。</summary>
    void Interact();

    /// <summary>该对象成为当前最近交互目标时调用。</summary>
    void OnFocus();

    /// <summary>该对象不再是当前交互目标时调用。</summary>
    void OnLoseFocus();
}
