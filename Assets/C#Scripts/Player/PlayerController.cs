using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("人物转向平滑速度")]
    public float turnSpeed = 12f;

    private CharacterController controller;
    private PlayerStats playerStats;
    private Vector3 velocity;
    public float gravity = -9.81f;

    private GameInput gameInput;
    private Vector2 moveInput;
    private Vector2 mouseScreenPos;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerStats = GetComponent<PlayerStats>();
        gameInput = new GameInput();
    }

    void OnEnable()
    {
        // WASD移动
        gameInput.Player.Move.performed += OnMovePerformed;
        gameInput.Player.Move.canceled += OnMoveCanceled;

        // Input System 获取鼠标屏幕坐标
        gameInput.Player.MousePos.performed += OnMousePosPerformed;
        gameInput.Player.MousePos.canceled += OnMousePosCanceled;

        // Shift疾跑
        gameInput.Player.Sprint.performed += OnSprintPerformed;
        gameInput.Player.Sprint.canceled += OnSprintCanceled;

        gameInput.Player.Enable();
    }

    void OnDisable()
    {
        gameInput.Player.Disable();

        gameInput.Player.Move.performed -= OnMovePerformed;
        gameInput.Player.Move.canceled -= OnMoveCanceled;
        gameInput.Player.MousePos.performed -= OnMousePosPerformed;
        gameInput.Player.MousePos.canceled -= OnMousePosCanceled;
        gameInput.Player.Sprint.performed -= OnSprintPerformed;
        gameInput.Player.Sprint.canceled -= OnSprintCanceled;

        moveInput = Vector2.zero;
        playerStats.SetSprinting(false);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnMousePosPerformed(InputAction.CallbackContext context)
    {
        mouseScreenPos = context.ReadValue<Vector2>();
    }

    private void OnMousePosCanceled(InputAction.CallbackContext context)
    {
        mouseScreenPos = Vector2.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        playerStats.SetSprinting(true);
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        playerStats.SetSprinting(false);
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
            controller.Move(moveDir * playerStats.CurrentMoveSpeed * Time.deltaTime);
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
