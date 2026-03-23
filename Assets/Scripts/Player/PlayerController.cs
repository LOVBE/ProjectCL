using UnityEngine;
using System;

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

    [Header("Potion Effects")]
    [SerializeField] private float silentStepDuration = 20f;
    [SerializeField] private float heatResistDuration = 20f;

    private float currentStamina;
    private float currentOxygen;
    private float silentStepTimer;
    private float heatResistTimer;
    private CharacterController controller;
    private Vector3 velocity;

    public float CurrentStamina => currentStamina;
    public float CurrentOxygen => currentOxygen;
    public float MaxStamina => maxStamina;
    public float MaxOxygen => maxOxygen;
    public bool HasSilentStepMode => silentStepTimer > 0f;
    public bool HasHeatResistMode => heatResistTimer > 0f;

    public Action<float, float> OnVitalsChanged;
    public Action OnStatusEffectsChanged;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        ResolveCameraReference();
    }

    private void Start()
    {
        ResetVitals();
    }

    public void ResetVitals()
    {
        currentStamina = maxStamina;
        currentOxygen = maxOxygen;
        NotifyVitalsChanged();
    }

    private void Update()
    {
        UpdatePotionTimers();

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
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDir = transform.right * x + transform.forward * z;

        if (isSprinting && moveDir.magnitude > 0)
        {
            currentStamina -= sprintStaminaCostRate * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        velocity.y += gravity * Time.deltaTime;
        
        Vector3 finalMove = moveDir * currentSpeed;
        finalMove.y = velocity.y;
        
        controller.Move(finalMove * Time.deltaTime);

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        NotifyVitalsChanged();
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            ResolveCameraReference();
            if (playerCamera == null)
            {
                return;
            }
        }

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
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        NotifyVitalsChanged();

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
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        NotifyVitalsChanged();

        if(currentStamina <= 0)
        {
            GameManager.Instance?.OnPlayerFainted("Kiệt sức do bị tấn công");
            ResetVitals();
        }
    }

    public bool ConsumePotion(ItemID potionID)
    {
        switch (potionID)
        {
            case ItemID.SilentStepPotion:
                if (PlayerInventory.Instance == null || !PlayerInventory.Instance.RemoveItem(potionID))
                {
                    return false;
                }

                silentStepTimer = silentStepDuration;
                OnStatusEffectsChanged?.Invoke();
                return true;
            case ItemID.HeatResistPotion:
                if (PlayerInventory.Instance == null || !PlayerInventory.Instance.RemoveItem(potionID))
                {
                    return false;
                }

                heatResistTimer = heatResistDuration;
                OnStatusEffectsChanged?.Invoke();
                return true;
            default:
                return false;
        }
    }

    private void UpdatePotionTimers()
    {
        bool statusChanged = false;

        if (silentStepTimer > 0f)
        {
            silentStepTimer = Mathf.Max(0f, silentStepTimer - Time.deltaTime);
            statusChanged = true;
        }

        if (heatResistTimer > 0f)
        {
            heatResistTimer = Mathf.Max(0f, heatResistTimer - Time.deltaTime);
            statusChanged = true;
        }

        if (statusChanged)
        {
            OnStatusEffectsChanged?.Invoke();
        }
    }

    private void NotifyVitalsChanged()
    {
        OnVitalsChanged?.Invoke(currentStamina, currentOxygen);
    }

    private void ResolveCameraReference()
    {
        if (playerCamera != null)
        {
            return;
        }

        Camera childCamera = GetComponentInChildren<Camera>();
        if (childCamera != null)
        {
            playerCamera = childCamera.transform;
            return;
        }

        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }
}
