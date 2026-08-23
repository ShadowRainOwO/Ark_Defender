using UnityEngine;

/// <summary>
/// 挂在仓库交互物上的入口。
/// 玩家交互时把该物体的 StorageData 传给 InventoryUIManager。
/// </summary>
[RequireComponent(typeof(StorageData))]
public class StorageInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string storageName = "仓库";
    [SerializeField] private StorageData storageData;

    private void Reset()
    {
        // 自动绑定同物体上的仓库数据，减少 Inspector 漏配。
        storageData = GetComponent<StorageData>();
    }

    public string GetInteractText()
    {
        return "打开 " + storageName;
    }

    public void Interact()
    {
        if (storageData == null)
        {
            Debug.LogWarning("StorageInteract 没有绑定 StorageData。", this);
            return;
        }

        InventoryUIManager.Instance?.OpenStorage(storageData);
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}
