using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    [Header("检测范围")]
    public float interactDistance = 1f;

    [Header("检测层")]
    public LayerMask interactLayer;

    //当前交互目标
    private IInteractable currentInteractable;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        DetectInteractable();

        //交互按键
        if (Input.GetKeyDown(KeyCode.F))
        {

            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }

        }

    }
    /// <summary>
    /// 检测附近可交互对象
    /// </summary>
    void DetectInteractable()
    {
        Collider[] colliders =Physics.OverlapSphere(transform.position,interactDistance,interactLayer);

        IInteractable nearest = null;

        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();

            if (interactable == null)
                continue;

            float distance =Vector3.Distance(transform.position,col.transform.position );

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
    void ChangeTarget(IInteractable newTarget)
    {
        //目标没有变化
        if (currentInteractable == newTarget)
            return;

        //离开旧目标
        if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();

            Debug.Log("已离开交互目标" );

            HideInteractUI();
        }

        //切换目标
        currentInteractable = newTarget;

        //进入新目标
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
    void ShowInteractUI(string text)
    {

        Debug.Log("[E] " + text);

        //这里以后接UI
        //例如：
        //InteractionUI.Show(text)
    }

    /// <summary>
    /// 隐藏提示
    /// </summary>
    void HideInteractUI()
    {
        Debug.Log("隐藏交互提示" );
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,interactDistance);
    }
}