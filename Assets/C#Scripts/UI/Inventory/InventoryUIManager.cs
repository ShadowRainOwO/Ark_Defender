using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包系统的 UI 总控制器。
/// 只负责打开/关闭界面、绑定数据和安排左右面板，不直接增删物品或处理交易。
/// 普通物品转移交给 ItemTransferManager，购买出售交给 ShopManager。
/// </summary>
public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    /// <summary>当前采用的左右布局组合。</summary>
    public enum InventoryUIMode
    {
        Closed,         // 全部关闭
        InventoryOnly, // 左侧为空，背包位于右侧
        Storage,        // 左侧背包，右侧仓库
        ShopInventory, // 左侧背包，右侧商店
        ShopStorage,   // 左侧仓库，右侧商店
        Container      // 左侧场景容器，右侧背包
    }

    [Header("当前 UI 模式")]
    [SerializeField] private InventoryUIMode currentMode = InventoryUIMode.Closed;
    [Header("玩家数据")]
    [Tooltip("玩家随身背包；打开任何含背包的布局时会绑定到 InventoryPanel。")]
    [SerializeField] private PlayerInventory playerInventory;
    [Tooltip("玩家永久仓库；商店切换到仓库来源时使用。")]
    [SerializeField] private StorageData playerStorage;
    [Header("根节点")]
    [SerializeField] private GameObject rootUI;
    [Header("左右布局挂点")]
    [Tooltip("左侧面板的父节点。")]
    [SerializeField] private RectTransform leftSlot;
    [Tooltip("右侧面板的父节点。")]
    [SerializeField] private RectTransform rightSlot;
    [Header("功能面板")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private RectTransform storagePanel;
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform containerPanel;
    [Header("商店来源切换")]
    [SerializeField] private GameObject switchButtonObject;
    [SerializeField] private Button switchButton;
    [SerializeField] private TMP_Text switchButtonText;
    [Header("关闭按钮")]
    [SerializeField] private Button closeButton;

    public InventoryUIMode CurrentMode => currentMode;
    public bool IsOpen => currentMode != InventoryUIMode.Closed;

    private void Awake()
    {
        // 单例用于场景交互脚本快速打开界面；同一场景只允许一个实例。
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        switchButton?.onClick.AddListener(SwitchShopLeftPanel);
        closeButton?.onClick.AddListener(Close);
    }

    private void Start()
    {
        Close();
    }

    private void OnDestroy()
    {
        switchButton?.onClick.RemoveListener(SwitchShopLeftPanel);
        closeButton?.onClick.RemoveListener(Close);
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OpenInventory()
    {
        BindPlayerInventory();
        SetMode(InventoryUIMode.InventoryOnly);
    }

    public void OpenStorage()
    {
        BindPlayerInventory();
        SetMode(InventoryUIMode.Storage);
    }

    public void OpenStorage(StorageData data)
    {
        // 场景中的 StorageInteract 会把本次交互的具体仓库传进来。
        GetPanel<StoragePanel>(storagePanel)?.Bind(data);
        OpenStorage();
    }

    public void OpenShop()
    {
        BindPlayerInventory();
        if (playerStorage != null)
        {
            GetPanel<StoragePanel>(storagePanel)?.Bind(playerStorage);
        }
        SetMode(InventoryUIMode.ShopInventory);
    }

    public void OpenShop(ShopData data)
    {
        // NPCInteract 会把当前 NPC 对应的商店数据传进来。
        GetPanel<ShopPanel>(shopPanel)?.Bind(data);
        OpenShop();
    }

    public void OpenContainer()
    {
        BindPlayerInventory();
        SetMode(InventoryUIMode.Container);
    }

    public void OpenContainer(ContainerData data)
    {
        // ContainerInteract/ContainerManager 会传入当前打开的具体容器。
        GetPanel<ContainerPanel>(containerPanel)?.Bind(data);
        OpenContainer();
    }

    public void Close()
    {
        currentMode = InventoryUIMode.Closed;
        HideAllPanels();
        SetSwitchButton(false);
        if (rootUI != null)
        {
            rootUI.SetActive(false);
        }
    }

    /// <summary>在商店保持开启的情况下，切换左侧的背包或仓库。</summary>
    public void SwitchShopLeftPanel()
    {
        if (currentMode == InventoryUIMode.ShopInventory)
        {
            SetMode(InventoryUIMode.ShopStorage);
        }
        else if (currentMode == InventoryUIMode.ShopStorage)
        {
            SetMode(InventoryUIMode.ShopInventory);
        }
    }

    private void SetMode(InventoryUIMode newMode)
    {
        currentMode = newMode;
        if (rootUI != null)
        {
            rootUI.SetActive(true);
        }
        ApplyLayout();
    }

    private void ApplyLayout()
    {
        // 每次先隐藏所有业务面板，再按模式把需要的面板放入左右挂点。
        HideAllPanels();
        switch (currentMode)
        {
            case InventoryUIMode.Closed:
                SetSwitchButton(false);
                if (rootUI != null) rootUI.SetActive(false);
                break;
            case InventoryUIMode.InventoryOnly:
                ShowPanel(inventoryPanel, rightSlot);
                SetSwitchButton(false);
                break;
            case InventoryUIMode.Storage:
                ShowPanel(inventoryPanel, leftSlot);
                ShowPanel(storagePanel, rightSlot);
                SetSwitchButton(false);
                break;
            case InventoryUIMode.ShopInventory:
                ShowPanel(inventoryPanel, leftSlot);
                ShowPanel(shopPanel, rightSlot);
                SetSwitchButton(true, "切换到仓库");
                break;
            case InventoryUIMode.ShopStorage:
                ShowPanel(storagePanel, leftSlot);
                ShowPanel(shopPanel, rightSlot);
                SetSwitchButton(true, "切换到背包");
                break;
            case InventoryUIMode.Container:
                ShowPanel(containerPanel, leftSlot);
                ShowPanel(inventoryPanel, rightSlot);
                SetSwitchButton(false);
                break;
        }
    }

    private void BindPlayerInventory()
    {
        GetPanel<InventoryPanel>(inventoryPanel)?.Bind(playerInventory);
    }

    private static T GetPanel<T>(RectTransform panel) where T : Component
    {
        return panel != null ? panel.GetComponent<T>() : null;
    }

    private static void ShowPanel(RectTransform panel, RectTransform slot)
    {
        if (panel == null || slot == null) return;

        // 重新挂到布局槽后铺满父节点，从而让同一个面板可在左右两侧复用。
        panel.SetParent(slot, false);
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;
        panel.localScale = Vector3.one;
        panel.anchoredPosition = Vector2.zero;
        panel.gameObject.SetActive(true);
    }

    private void HideAllPanels()
    {
        SetPanelActive(inventoryPanel, false);
        SetPanelActive(storagePanel, false);
        SetPanelActive(shopPanel, false);
        SetPanelActive(containerPanel, false);
    }

    private static void SetPanelActive(RectTransform panel, bool active)
    {
        if (panel != null) panel.gameObject.SetActive(active);
    }

    private void SetSwitchButton(bool visible, string text = "")
    {
        if (switchButtonObject != null) switchButtonObject.SetActive(visible);
        if (visible && switchButtonText != null) switchButtonText.text = text;
    }
}
