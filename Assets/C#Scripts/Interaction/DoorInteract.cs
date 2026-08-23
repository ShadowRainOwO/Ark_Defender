using UnityEngine;

/// <summary>
/// 门的最小交互示例，实现 IInteractable 以接入统一检测。
/// 当前只切换状态并输出日志，尚未连接动画和碰撞体。
/// </summary>
public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    public string GetInteractText()
    {
        return isOpen ? "关闭门" : "打开门";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "门已打开" : "门已关闭", this);
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}
