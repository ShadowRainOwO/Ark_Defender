using UnityEngine;

public class ContainerInteractable : BaseInteractable
{
    [Header("容器设置")]
    public bool oneTimeOpen = true; // 是否只能开启一次

    public override void DoInteract()
    {
        if (isInteracted && oneTimeOpen)
        {
            Debug.Log("箱子已经开启过了");
            return;
        }

        Debug.Log($"打开容器：{gameObject.name}");
        // 逻辑：播放开箱动画、生成掉落道具

        if (oneTimeOpen)
        {
            isInteracted = true;
            interactTipText = "已开启";
        }
    }
}