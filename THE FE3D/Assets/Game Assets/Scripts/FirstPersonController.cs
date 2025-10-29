using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 6f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float lookSensitivity = 2f;
    public float maxLookAngle = 85f;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool sprintPressed;
    private bool crouchPressed;
    private bool isCrouching;
    private bool isGrounded;
    private float defaultCameraY;
    private float bobTimer;
    private float xRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera)
            defaultCameraY = playerCamera.localPosition.y;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleCrouch();
        HandleHeadBob();
    }

    // Called automatically by PlayerInput
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value) => jumpPressed = value.isPressed;
    public void OnSprint(InputValue value) => sprintPressed = value.isPressed;
    public void OnCrouch(InputValue value) => crouchPressed = value.isPressed;

    void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float speed = walkSpeed;
        if (sprintPressed && !isCrouching) speed = sprintSpeed;
        else if (isCrouching) speed = crouchSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        if (jumpPressed && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (crouchPressed)
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;
    }

    void HandleHeadBob()
    {
        if (!enableHeadBob || !isGrounded) return;

        bool isMoving = moveInput.magnitude > 0.1f;
        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount;
            Vector3 camPos = playerCamera.localPosition;
            camPos.y = newY;
            playerCamera.localPosition = camPos;
        }
        else
        {
            bobTimer = 0;
            Vector3 camPos = playerCamera.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, defaultCameraY, Time.deltaTime * bobSpeed);
            playerCamera.localPosition = camPos;
        }
    }
}
