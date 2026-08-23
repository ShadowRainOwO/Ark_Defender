using UnityEngine;

/// <summary>
/// NPC 的通用交互入口。
/// 绑定 ShopData 时视为商店 NPC 并打开商店；未绑定时暂时只输出对话日志。
/// </summary>
public class NPCInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName = "NPC";
    [Tooltip("设置后，此 NPC 交互时打开商店。")]
    [SerializeField] private ShopData shopData;

    public string GetInteractText()
    {
        return shopData != null ? "与 " + npcName + " 交易" : "与 " + npcName + " 交谈";
    }

    public void Interact()
    {
        if (shopData != null)
        {
            // 将这个 NPC 自己的商店数据传给右侧 ShopPanel。
            InventoryUIManager.Instance?.OpenShop(shopData);
            return;
        }

        Debug.Log("打开 NPC 对话：" + npcName, this);
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}
