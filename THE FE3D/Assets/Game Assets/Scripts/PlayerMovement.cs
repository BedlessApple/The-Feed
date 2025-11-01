using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float normalHeight = 2f;

    [Header("Sprint Stamina")]
    public float maxSprintTime = 10f;
    public float rechargeTick = 1.5f;
    public Slider sprintBar;

    [Header("Camera")]
    public GameObject playerCamera;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isCrouching;
    private float currentSprintTime;
    private float rechargeTimer;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentSprintTime = maxSprintTime;

        // Disable camera for non-owners
        if (!IsOwner && playerCamera)
        {
            playerCamera.SetActive(false);
        }

        // Initialize UI for owner
        if (IsOwner && sprintBar)
        {
            sprintBar.maxValue = maxSprintTime;
            sprintBar.value = currentSprintTime;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // Sprint logic
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentSprintTime > 0 && !isCrouching;
        float speed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);

        if (isSprinting)
        {
            currentSprintTime -= Time.deltaTime;
            rechargeTimer = 0f;
        }
        else
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= rechargeTick && currentSprintTime < maxSprintTime)
            {
                currentSprintTime += 1f;
                rechargeTimer = 0f;
            }
        }

        // Crouch toggle
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : normalHeight;
        }

        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Update UI
        if (sprintBar) sprintBar.value = currentSprintTime;

        // Update Animator
        float movementMagnitude = new Vector2(x, z).magnitude;
        animator.SetFloat("Speed", movementMagnitude);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsJumping", !isGrounded);
    }
}
