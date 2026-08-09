using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [Header("交互提示UI")]
    public GameObject interactTipUI;
    public RectTransform tipRect;
    public Text tipText; // 新增：文字组件
    public Vector2 uiOffset = new Vector2(30, 30);

    [Header("玩家设置")]
    public Transform playerTrans;
    public float rayStartHeight = 1f;

    [Header("射线遮挡")]
    public LayerMask obstacleLayer;

    private List<BaseInteractable> inRangeObjects = new List<BaseInteractable>();
    public BaseInteractable CurrentInteractable { get; private set; }

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        interactTipUI.SetActive(false);
    }

    void Update()
    {
        RefreshClosestInteractable();

        if (CurrentInteractable != null)
        {
            // 更新UI文字（箱子显示【F开启】商人【F交谈】）
            tipText.text = CurrentInteractable.interactTipText;

            Vector3 worldPos = CurrentInteractable.transform.position;
            Vector2 screenPos = mainCam.WorldToScreenPoint(worldPos);
            tipRect.anchoredPosition = screenPos + uiOffset;
            interactTipUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                CurrentInteractable.DoInteract();
            }
        }
        else
        {
            interactTipUI.SetActive(false);
        }
    }

    void RefreshClosestInteractable()
    {
        inRangeObjects.RemoveAll(item => item == null);

        if (inRangeObjects.Count == 0)
        {
            CurrentInteractable = null;
            return;
        }

        BaseInteractable closestObj = null;
        float minDistance = float.MaxValue;

        foreach (var obj in inRangeObjects)
        {
            // 可选：如果交互已经完成，不进行选中
            // if(obj.isInteracted) continue;

            Vector3 rayOrigin = playerTrans.position + Vector3.up * rayStartHeight;
            Vector3 direction = obj.transform.position - rayOrigin;
            float distance = direction.magnitude;

            bool isBlocked = Physics.Raycast(
                rayOrigin,
                direction.normalized,
                distance,
                obstacleLayer
            );

            if (isBlocked)
                continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                closestObj = obj;
            }
        }

        CurrentInteractable = closestObj;
    }

    public void AddInteractable(BaseInteractable obj)
    {
        if (!inRangeObjects.Contains(obj))
        {
            inRangeObjects.Add(obj);
        }
    }

    public void RemoveInteractable(BaseInteractable obj)
    {
        if (inRangeObjects.Contains(obj))
        {
            inRangeObjects.Remove(obj);
        }
    }

    // UI按钮点击交互
    public void OnClickInteractButton()
    {
        if (CurrentInteractable != null)
        {
            CurrentInteractable.DoInteract();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTrans == null) return;
        Vector3 origin = playerTrans.position + Vector3.up * rayStartHeight;
        foreach (var obj in inRangeObjects)
        {
            if (obj == null) continue;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, obj.transform.position);
        }
    }
}