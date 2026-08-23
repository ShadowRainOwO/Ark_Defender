using UnityEngine;

/// <summary>
/// 挂在宝箱、尸体等场景容器上的交互入口。
/// 被交互后将同物体上的 ContainerData 交给 ContainerManager/UIManager 打开。
/// </summary>
[RequireComponent(typeof(ContainerData))]
public class ContainerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string containerName = "容器";
    [SerializeField] private ContainerData containerData;

    private void Reset()
    {
        // 添加组件或点击 Inspector 的 Reset 时自动查找同物体上的数据组件。
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
            // 优先经过 ContainerManager，以便其他系统知道当前打开的是哪个容器。
            ContainerManager.Instance.Open(containerData);
        }
        else
        {
            // 场景尚未配置 ContainerManager 时仍允许直接打开界面。
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
