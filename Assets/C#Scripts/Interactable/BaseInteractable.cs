using UnityEngine;

// 交互类型枚举
public enum InteractType
{
    NPC,        // 商人、NPC人物
    Container,  // 箱子、容器
    SceneAction // 场景互动：修桥、开关、机关
}

public class BaseInteractable : MonoBehaviour
{
    [Header("通用交互设置")]
    public InteractType interactType;
    [Tooltip("UI上显示的交互文字，例如【F 交谈】【F 开启】【F 修复】")]
    public string interactTipText = "【F 交互】";
    [Tooltip("是否已经完成交互（一次性交互：开完箱子不能再开）")]
    public bool isInteracted = false;

    /// <summary>
    /// 交互核心方法，由子类重写实现各自逻辑
    /// </summary>
    public virtual void DoInteract()
    {
        Debug.Log($"基础交互：{gameObject.name}");
    }
}