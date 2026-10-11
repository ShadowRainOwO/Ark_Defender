using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 所有“标题 + 格子区域”页面的基础视图。
/// 只负责设置标题、统计已有格子，以及在数量不足时生成 ItemSlot Prefab。
/// 不处理物品数据、点击、转移、购买或出售。
/// </summary>
public abstract class SlotPanelBase : MonoBehaviour
{
    [Header("页面标题")]
    [Tooltip("页面顶部的标题文本。不指定时会自动查找名为 Title 的子物体。")]
    [SerializeField] private TMP_Text titleText;

    [Tooltip("留空时使用具体页面提供的默认标题。")]
    [SerializeField] private string titleOverride;

    [Header("格子区域")]
    [Tooltip("基础空格子数量。没有绑定数据时直接使用该值。")]
    [Min(0)]
    [SerializeField] private int defaultSlotCount = 20;

    [Tooltip("格子预制体，Prefab 根节点必须挂载 ItemSlot 脚本。")]
    [SerializeField] private ItemSlot slotPrefab;

    [Tooltip("生成格子的父节点。不指定时会自动查找名为 SlotArea 的子物体。")]
    [SerializeField] private Transform slotRoot;

    private readonly List<ItemSlot> slotViews = new List<ItemSlot>();
    private bool hasCachedExistingSlots;
    private bool hasWarnedMissingPrefab;

    /// <summary>具体页面提供的默认标题。</summary>
    protected abstract string DefaultTitle { get; }

    /// <summary>Inspector 中配置的基础格子数量。</summary>
    protected int DefaultSlotCount => defaultSlotCount;

    protected virtual void Awake()
    {
        FindReferencesIfNeeded();
    }

    protected virtual void OnEnable()
    {
        // 页面每次显示时重新检查，动态修改容量后也能补齐格子。
        RefreshPanel();
    }

    /// <summary>
    /// 刷新标题与格子数量。已有格子会被复用，只补充缺少的部分。
    /// 多出的格子不会删除，而是暂时隐藏，避免反复创建和销毁对象。
    /// </summary>
    public void RefreshPanel()
    {
        FindReferencesIfNeeded();
        RefreshTitle();

        int requiredCount = Mathf.Max(0, GetRequiredSlotCount());
        EnsureSlotCount(requiredCount);

        for (int i = 0; i < slotViews.Count; i++)
        {
            bool shouldShow = i < requiredCount;
            slotViews[i].gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                RefreshSlot(slotViews[i], i);
            }
        }
    }

    /// <summary>返回当前页面需要显示的格子数量。</summary>
    protected virtual int GetRequiredSlotCount()
    {
        return defaultSlotCount;
    }

    /// <summary>刷新一个具体格子；基础页面只把它清空。</summary>
    protected virtual void RefreshSlot(ItemSlot slot, int slotIndex)
    {
        slot.Bind(null, slotIndex);
    }

    /// <summary>发现或创建格子后调用，子类可在这里订阅点击事件。</summary>
    protected virtual void RegisterSlot(ItemSlot slot)
    {
    }

    private void FindReferencesIfNeeded()
    {
        if (slotRoot == null)
        {
            slotRoot = transform.Find("SlotArea");
        }

        if (titleText == null)
        {
            Transform titleTransform = transform.Find("Title");
            if (titleTransform != null)
            {
                titleText = titleTransform.GetComponent<TMP_Text>();
            }
        }
    }

    private void RefreshTitle()
    {
        if (titleText == null)
        {
            return;
        }

        titleText.text = string.IsNullOrWhiteSpace(titleOverride)
            ? DefaultTitle
            : titleOverride;
    }

    private void EnsureSlotCount(int requiredCount)
    {
        CacheExistingSlots();

        if (slotRoot == null)
        {
            Debug.LogWarning($"{name} 没有找到 SlotArea，无法生成格子。", this);
            return;
        }

        while (slotViews.Count < requiredCount)
        {
            if (slotPrefab == null)
            {
                if (!hasWarnedMissingPrefab)
                {
                    Debug.LogWarning($"{name} 没有指定 ItemSlot Prefab，无法补充格子。", this);
                    hasWarnedMissingPrefab = true;
                }

                break;
            }

            ItemSlot newSlot = Instantiate(slotPrefab, slotRoot);
            newSlot.name = $"ItemSlot_{slotViews.Count + 1:00}";
            slotViews.Add(newSlot);
            RegisterSlot(newSlot);
        }
    }

    private void CacheExistingSlots()
    {
        if (hasCachedExistingSlots || slotRoot == null)
        {
            return;
        }

        hasCachedExistingSlots = true;

        // 只统计 SlotArea 的直接子物体，避免把 ItemSlot 内部节点算作格子。
        for (int i = 0; i < slotRoot.childCount; i++)
        {
            ItemSlot existingSlot = slotRoot.GetChild(i).GetComponent<ItemSlot>();
            if (existingSlot == null)
            {
                continue;
            }

            slotViews.Add(existingSlot);
            RegisterSlot(existingSlot);
        }
    }
}
