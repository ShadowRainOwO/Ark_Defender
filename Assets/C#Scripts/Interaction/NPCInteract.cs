using UnityEngine;

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
