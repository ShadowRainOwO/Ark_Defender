using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 单个物品格子的视图组件。
/// 它只显示绑定格子的内容并上报点击事件，不直接修改背包或仓库数据。
/// </summary>
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("显示组件")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject selectedFrame;

    /// <summary>该格子当前所属的数据容器。</summary>
    public IItemContainer Owner { get; private set; }

    /// <summary>该格子在所属容器中的索引。</summary>
    public int SlotIndex { get; private set; }

    /// <summary>用户单击格子时触发。</summary>
    public event Action<ItemSlot> Clicked;

    /// <summary>用户双击格子时触发，通常用于快速转移。</summary>
    public event Action<ItemSlot> DoubleClicked;

    /// <summary>将视图绑定到指定容器的指定格子。</summary>
    public void Bind(IItemContainer owner, int slotIndex)
    {
        Owner = owner;
        SlotIndex = slotIndex;
        Refresh();
    }

    /// <summary>从绑定的数据中重新读取图标与数量。</summary>
    public void Refresh()
    {
        ItemStack stack = Owner?.GetItem(SlotIndex);
        bool hasItem = stack != null && !stack.IsEmpty;
        if (icon != null)
        {
            icon.enabled = hasItem;
            icon.sprite = hasItem ? stack.Item.Icon : null;
        }
        if (amountText != null)
        {
            amountText.text = hasItem && stack.Amount > 1 ? stack.Amount.ToString() : string.Empty;
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedFrame != null) selectedFrame.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ItemSlot 只上报意图，真正执行何种操作由上层面板或业务逻辑决定。
        if (eventData.clickCount >= 2) DoubleClicked?.Invoke(this);
        else Clicked?.Invoke(this);
    }
}
