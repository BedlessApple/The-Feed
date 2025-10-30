using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
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

    private CharacterController controller;
    private Vector3 velocity;
    private bool isCrouching;
    private float currentSprintTime;
    private float rechargeTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSprintTime = maxSprintTime;
        if (sprintBar) sprintBar.maxValue = maxSprintTime;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float speed = walkSpeed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentSprintTime > 0 && !isCrouching;

        if (isSprinting)
        {
            speed = sprintSpeed;
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

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : normalHeight;
        }

        if (isCrouching) speed = crouchSpeed;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (sprintBar) sprintBar.value = currentSprintTime;
    }
}
