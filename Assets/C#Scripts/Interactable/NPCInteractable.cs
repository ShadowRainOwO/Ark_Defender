using UnityEngine;

public class NPCInteractable : BaseInteractable
{
    [Header("NPC设置")]
    public string npcName = "商人";
    // 后续可以挂对话组件、商店配置

    public override void DoInteract()
    {
        if (isInteracted) return;
        Debug.Log($"与NPC【{npcName}】交谈/打开商店");

        // 在这里写：打开对话面板、商店UI
    }
}