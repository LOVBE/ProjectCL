using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Tuning Di Chuyển")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Tuning Camera")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;

    [Header("Tuning Sinh Tồn")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float oxygenDepletionRate = 1f; // Tụt Dưỡng khí thụ động
    [SerializeField] private float sprintStaminaCostRate = 10f; // Tụt Thể lực khi chạy
    [SerializeField] private float staminaRecoveryRate = 5f;

    private float currentStamina;
    private float currentOxygen;
    private CharacterController controller;
    private Vector3 velocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        ResetVitals();
    }

    public void ResetVitals()
    {
        currentStamina = maxStamina;
        currentOxygen = maxOxygen;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Exploration)
        {
            return; // Chỉ cho phép điều khiển khi đang Thám hiểm
        }

        HandleMouseLook();
        HandleMovement();
        HandleSurvivalVitals();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Xử lý Thể lực khi chạy
        if (isSprinting && move.magnitude > 0)
        {
            currentStamina -= sprintStaminaCostRate * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        // Apply Rơi/Trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleSurvivalVitals()
    {
        currentOxygen -= oxygenDepletionRate * Time.deltaTime;

        if (currentOxygen <= 0)
        {
            GameManager.Instance?.OnPlayerFainted("Hết dưỡng khí");
            ResetVitals();
        }
        else if (currentStamina <= 0 && currentOxygen <= 10f) // Just a harsh logic demo
        {
             // Có thể xử lý hệ quả kiệt sức
        }
    }

    public void ApplyDamage(float staminaDamage)
    {
        // Khi bị quái vật húc trúng
        currentStamina -= staminaDamage;
        if(currentStamina <= 0)
        {
            GameManager.Instance?.OnPlayerFainted("Kiệt sức do bị tấn công");
            ResetVitals();
        }
    }
}
