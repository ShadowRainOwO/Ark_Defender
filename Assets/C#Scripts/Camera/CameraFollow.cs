using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标(玩家)")]
    public Transform target;
    [Header("基础偏移")]
    public Vector3 offset = new Vector3(0, 4, -6);
    [Header("位置平滑速度")]
    public float moveSmoothSpeed = 5f;
    [Header("相机旋转设置")]
    public float rotateAngle = 45f;
    public float rotateSmoothSpeed = 8f;
    [Header("固定俯视角度")]
    public float pitchAngle = 22f;

    private float currentYaw;
    private Vector3 currentOffset;
    private GameInput gameInput;

    void Awake()
    {
        gameInput = new GameInput();
    }

    void Start()
    {
        currentYaw = transform.eulerAngles.y;
        currentOffset = offset;
    }

    void OnEnable()
    {
        gameInput.Player.Enable();
        //按键按下事件
        gameInput.Player.RotateCamLeft.performed += _ => currentYaw -= rotateAngle;
        gameInput.Player.RotateCamRight.performed += _ => currentYaw += rotateAngle;
    }

    void OnDisable()
    {
        gameInput.Player.Disable();
    }

    void LateUpdate()
    {
        currentYaw %= 360f;
        if (currentYaw < 0) currentYaw += 360f;

        Quaternion rot = Quaternion.Euler(0, currentYaw, 0);
        Vector3 targetOffset = rot * offset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, rotateSmoothSpeed * Time.deltaTime);

        //平滑相机位置
        Vector3 desiredCameraPos = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredCameraPos, moveSmoothSpeed * Time.deltaTime);

        //改良LookAt：固定俯仰，消除回弹摆动
        Vector3 dirToPlayer = target.position - transform.position;
        dirToPlayer.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
        targetRot = Quaternion.Euler(pitchAngle, targetRot.eulerAngles.y, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSmoothSpeed * Time.deltaTime);
    }
}