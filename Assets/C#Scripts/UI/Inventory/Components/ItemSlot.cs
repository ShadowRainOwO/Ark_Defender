using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private GameObject selectedFrame;

    public IItemContainer Owner { get; private set; }
    public int SlotIndex { get; private set; }
    public event Action<ItemSlot> Clicked;
    public event Action<ItemSlot> DoubleClicked;

    public void Bind(IItemContainer owner, int slotIndex)
    {
        Owner = owner;
        SlotIndex = slotIndex;
        Refresh();
    }

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
        if (eventData.clickCount >= 2) DoubleClicked?.Invoke(this);
        else Clicked?.Invoke(this);
    }
}
