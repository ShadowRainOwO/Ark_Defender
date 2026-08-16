using UnityEngine;

[RequireComponent(typeof(StorageData))]
public class StorageInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string storageName = "仓库";
    [SerializeField] private StorageData storageData;

    private void Reset()
    {
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
