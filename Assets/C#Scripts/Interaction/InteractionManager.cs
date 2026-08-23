using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家交互检测总入口。
/// 每帧查找范围内最近的 IInteractable，并在玩家按下交互键时调用目标的 Interact。
/// 它不直接打开背包、商店或门，而是把具体行为交给目标组件。
/// </summary>
public class InteractionManager : MonoBehaviour
{
    /// <summary>当前场景中的交互管理器实例。</summary>
    public static InteractionManager Instance;

    [Header("检测范围")]
    [Tooltip("以玩家为中心搜索可交互对象的半径。")]
    public float interactDistance = 1f;

    [Header("检测层")]
    [Tooltip("只有位于这些 Layer 的碰撞体才会参与交互检测。")]
    public LayerMask interactLayer;

    // 当前距离最近、可以响应交互键的目标。
    private IInteractable currentInteractable;

    // 由 Input System 生成的输入封装类。
    private GameInput gameInput;

    private void Awake()
    {
        Instance = this;
        gameInput = new GameInput();
    }

    private void OnEnable()
    {
        gameInput.Player.Interact.performed += OnInteractPerformed;
        gameInput.Player.Enable();
    }

    private void OnDisable()
    {
        gameInput.Player.Disable();
        gameInput.Player.Interact.performed -= OnInteractPerformed;
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    /// <summary>
    /// 检测附近可交互对象
    /// </summary>
    private void DetectInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactDistance, interactLayer);

        IInteractable nearest = null;

        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector3.Distance(transform.position, col.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        ChangeTarget(nearest);
    }
    /// <summary>
    /// 切换当前交互对象
    /// </summary>
    private void ChangeTarget(IInteractable newTarget)
    {
        // 目标没有变化时无需重复触发进入/离开事件。
        if (currentInteractable == newTarget)
            return;

        // 离开旧目标。
        if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();

            Debug.Log("已离开交互目标");

            HideInteractUI();
        }

        // 切换到新目标。
        currentInteractable = newTarget;

        // 进入新目标。
        if (currentInteractable != null)
        {
            currentInteractable.OnFocus();

            Debug.Log("进入交互范围: " + currentInteractable.GetInteractText());

            ShowInteractUI(currentInteractable.GetInteractText());
        }
    }

    /// <summary>
    /// 显示交互提示
    /// </summary>
    private void ShowInteractUI(string text)
    {

        Debug.Log("[F] " + text);

        // TODO：以后在这里接入正式的 InteractionUI.Show(text)。
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    private void HideInteractUI()
    {
        Debug.Log("隐藏交互提示");
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,interactDistance);
    }
}
