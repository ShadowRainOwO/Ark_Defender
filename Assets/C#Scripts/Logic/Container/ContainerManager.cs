using System;
using UnityEngine;

/// <summary>
/// 记录玩家当前正在操作的场景容器，并协调打开对应 UI。
/// 容器中的物品由 ContainerData 保存，容器之间的物品移动由 ItemTransferManager 处理。
/// </summary>
public class ContainerManager : MonoBehaviour
{
    public static ContainerManager Instance { get; private set; }

    /// <summary>当前打开的场景容器；未打开时为 null。</summary>
    public ContainerData ActiveContainer { get; private set; }

    /// <summary>当前容器发生切换时触发，其他系统可据此处理距离检测或存档。</summary>
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

    /// <summary>记录指定容器为当前容器，并打开“左容器、右背包”布局。</summary>
    public void Open(ContainerData container)
    {
        if (container == null) return;
        ActiveContainer = container;
        ActiveContainerChanged?.Invoke(container);
        InventoryUIManager.Instance?.OpenContainer(container);
    }

    /// <summary>清除当前容器引用；UI 的关闭仍由 InventoryUIManager 负责。</summary>
    public void Close()
    {
        ActiveContainer = null;
        ActiveContainerChanged?.Invoke(null);
    }
}
