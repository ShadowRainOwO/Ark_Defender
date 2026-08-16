using System;
using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    public static ContainerManager Instance { get; private set; }

    public ContainerData ActiveContainer { get; private set; }
    public event Action<ContainerData> ActiveContainerChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Open(ContainerData container)
    {
        if (container == null) return;
        ActiveContainer = container;
        ActiveContainerChanged?.Invoke(container);
        InventoryUIManager.Instance?.OpenContainer(container);
    }

    public void Close()
    {
        ActiveContainer = null;
        ActiveContainerChanged?.Invoke(null);
    }
}
