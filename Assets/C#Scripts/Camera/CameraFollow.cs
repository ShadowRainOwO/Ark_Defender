using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标(玩家)")]
    public Transform target;

    [Header("摄像机距离")]
    public Vector3 offset = new Vector3(0, 5, -8);

    [Header("位置平滑速度")]
    public float moveSmoothSpeed = 8f;

    [Header("旋转设置")]
    public float rotateAngle = 45f;
    public float rotateSmoothSpeed = 8f;

    [Header("看向玩家高度")]
    public float lookHeight = 1.5f;

    private float currentYaw;
    private float targetYaw;

    private GameInput gameInput;
    void Awake()
    {
        gameInput = new GameInput();
    }
    void Start()
    {
        currentYaw = transform.eulerAngles.y;
        targetYaw = currentYaw;
    }
    void OnEnable()
    {
        gameInput.Player.Enable();

        // Q 左转45°
        gameInput.Player.RotateCamLeft.performed += _
        =>
        {
            targetYaw -= rotateAngle;
        };

        // E 右转45°
        gameInput.Player.RotateCamRight.performed += _
        =>
        {
            targetYaw += rotateAngle;
        };
    }

    void OnDisable()
    {
        gameInput.Player.Disable();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // =========================
        // 1. E/Q控制水平旋转
        // =========================

        currentYaw = Mathf.LerpAngle(
            currentYaw,
            targetYaw,
            rotateSmoothSpeed * Time.deltaTime
        );

        Quaternion yawRotation =
            Quaternion.Euler(
                0,
                currentYaw,
                0
            );

        // =========================
        // 2. 计算摄像机位置
        // =========================

        Vector3 desiredPosition = target.position + yawRotation * offset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                moveSmoothSpeed * Time.deltaTime
            );
        // =========================
        // 3. 看向玩家
        // 但是只改变上下角度
        // =========================
        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        Vector3 direction = lookTarget - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        float pitch = lookRotation.eulerAngles.x;

        // 保留E/Q控制的水平角
        transform.rotation =
            Quaternion.Euler(
                pitch,
                currentYaw,
                0
            );
    }
}