using UnityEngine;

[RequireComponent(typeof(ContainerData))]
public class ContainerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string containerName = "容器";
    [SerializeField] private ContainerData containerData;

    private void Reset()
    {
        containerData = GetComponent<ContainerData>();
    }

    public string GetInteractText()
    {
        return "打开 " + containerName;
    }

    public void Interact()
    {
        if (containerData == null)
        {
            Debug.LogWarning("ContainerInteract 没有绑定 ContainerData。", this);
            return;
        }

        if (ContainerManager.Instance != null)
        {
            ContainerManager.Instance.Open(containerData);
        }
        else
        {
            InventoryUIManager.Instance?.OpenContainer(containerData);
        }
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}
