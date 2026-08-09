using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 9f;
    [Header("人物转向平滑速度")]
    public float turnSpeed = 12f;

    private CharacterController controller;
    private Vector3 velocity;
    public float gravity = -9.81f;

    private GameInput gameInput;
    private Vector2 moveInput;
    private Vector2 mouseScreenPos;
    private bool isSprinting;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        gameInput = new GameInput();
    }

    void OnEnable()
    {
        gameInput.Player.Enable();

        // WASD移动
        gameInput.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        gameInput.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Input System 获取鼠标屏幕坐标
        gameInput.Player.MousePos.performed += ctx => mouseScreenPos = ctx.ReadValue<Vector2>();
        gameInput.Player.MousePos.canceled += ctx => mouseScreenPos = Vector2.zero;

        // Shift疾跑
        gameInput.Player.Sprint.performed += _ => isSprinting = true;
        gameInput.Player.Sprint.canceled += _ => isSprinting = false;
    }

    void OnDisable()
    {
        gameInput.Player.Disable();
    }

    void Update()
    {
        Camera mainCam = Camera.main;

        #region WASD移动逻辑（跟随相机方向）
        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        if (moveDir.magnitude > 0.1f)
        {
            moveDir.Normalize();
            // 判断是否疾跑，选择速度
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }
        #endregion

        #region 水平面计算鼠标朝向（不需要地面碰撞）
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = mainCam.ScreenPointToRay(mouseScreenPos);

        if (playerPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 lookDir = targetPoint - transform.position;
            lookDir.y = 0;
            lookDir.Normalize();

            if (lookDir.magnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region 重力
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        #endregion
    }
}