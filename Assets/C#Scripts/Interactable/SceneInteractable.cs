using UnityEngine;

public class SceneInteractable : BaseInteractable
{
    [Header("场景互动设置")]
    public string actionName = "修复桥梁";
    public bool needResource = false;
    //public int costResource = 5; // 需要消耗材料

    public override void DoInteract()
    {
        if (isInteracted) return;

        Debug.Log($"执行场景互动：{actionName}");
        // 逻辑：判断背包材料、播放建造动画、改变场景状态

        isInteracted = true;
        interactTipText = "已完成";
    }
}