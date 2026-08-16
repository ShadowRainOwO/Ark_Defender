using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance { get; private set; }

    public enum InventoryUIMode
    {
        Closed,
        InventoryOnly,
        Storage,
        ShopInventory,
        ShopStorage,
        Container
    }

    [Header("当前 UI 模式")]
    [SerializeField] private InventoryUIMode currentMode = InventoryUIMode.Closed;
    [Header("玩家数据")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private StorageData playerStorage;
    [Header("根节点")]
    [SerializeField] private GameObject rootUI;
    [Header("左右布局挂点")]
    [SerializeField] private RectTransform leftSlot;
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
