using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Базовые параметры")]
    public float baseSpeed = 8f;
    public float dashForce = 15f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float dashCost = 30f;

    [Header("Прыжок")]
    public float jumpForce = 7f;
    public LayerMask groundLayer; // Слой земли для проверки
    public float groundCheckDistance = 1.1f; // Длина луча вниз

    [Header("Система веса")]
    [Range(0, 1)] public float saturation = 0f;
    public float maxScaleMultiplier = 2.5f;
    public float minSpeedMultiplier = 0.3f;
    public float massMultiplier = 5f;

    // Ссылки
    private Rigidbody rb;
    private GameControls controls;
    private Collider playerCollider;

    // Состояния
    private float currentStamina;
    private bool isDashing = false;
    private bool isEating = false;
    
    // Взаимодействие
    private EdibleObject currentFoodTarget;
    private float eatTimer = 0f;

    void Awake()
    {
        controls = new GameControls();
        
        // Подписки на кнопки
        controls.Gameplay.Dash.performed += ctx => AttemptDash();
        controls.Gameplay.Jump.performed += ctx => AttemptJump();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        currentStamina = maxStamina;
        UpdatePhysicalStats();
    }

    void Update()
    {
        if (!isDashing)
        {
            currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
        }

        HandleInteractionInput();
    }

    void FixedUpdate()
    {
        if (isEating) return;
        MoveLogic();
    }

    // --- ЛОГИКА ДВИЖЕНИЯ ---
    void MoveLogic()
    {
        if (isDashing) return;

        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;

        // Расчет скорости
        float currentMaxSpeed = Mathf.Lerp(baseSpeed, baseSpeed * minSpeedMultiplier, saturation);
        
        // Применяем скорость только по горизонтали, Y оставляем под контролем гравитации
        Vector3 targetVelocity = moveDirection * currentMaxSpeed;
        targetVelocity.y = rb.linearVelocity.y; 

        rb.linearVelocity = targetVelocity;

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
    }

    // --- ЛОГИКА ПРЫЖКА ---
    void AttemptJump()
    {
        // Прыгаем только если на земле и не едим
        if (IsGrounded() && !isEating && !isDashing)
        {
            // Сбрасываем вертикальную скорость перед прыжком для стабильности
            Vector3 vel = rb.linearVelocity;
            vel.y = 0;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        // Пускаем луч от центра персонажа вниз
        // Длина луча масштабируется вместе с персонажем (bounds.extents.y)
        float distanceToEdge = playerCollider.bounds.extents.y;
        
        // Добавляем маленький отступ (0.1f), чтобы луч выходил чуть дальше коллайдера
        return Physics.Raycast(transform.position, Vector3.down, distanceToEdge + 0.1f, groundLayer);
    }

    // --- ЛОГИКА РЫВКА ---
    void AttemptDash()
    {
        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();
        
        if (inputVector.magnitude > 0 && currentStamina >= dashCost && !isEating && !isDashing)
        {
            StartCoroutine(DashRoutine(new Vector3(inputVector.x, 0, inputVector.y).normalized));
        }
    }

    IEnumerator DashRoutine(Vector3 direction)
    {
        isDashing = true;
        currentStamina -= dashCost;
        rb.AddForce(direction * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        // Не сбрасываем velocity полностью, чтобы сохранить импульс прыжка, если он был
        isDashing = false;
    }

    // --- ЛОГИКА ВЗАИМОДЕЙСТВИЯ (Без изменений) ---
    void HandleInteractionInput()
    {
        if (currentFoodTarget == null) return;

        if (controls.Gameplay.Interact.IsPressed())
        {
            if (!isEating && IsGrounded()) StartEating(); // Есть можно только на земле
            
            eatTimer += Time.deltaTime;
            if (eatTimer >= currentFoodTarget.eatTime) ConsumeFood();
        }
        else
        {
            if (isEating) StopEating();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food")) currentFoodTarget = other.GetComponent<EdibleObject>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food") && currentFoodTarget == other.GetComponent<EdibleObject>())
        {
            StopEating();
            currentFoodTarget = null;
        }
    }

    void StartEating()
    {
        isEating = true;
        eatTimer = 0f;
        rb.linearVelocity = Vector3.zero;
    }

    void StopEating()
    {
        isEating = false;
        eatTimer = 0f;
    }

    void ConsumeFood()
    {
        AddSaturation(currentFoodTarget.nutritionValue);
        Destroy(currentFoodTarget.gameObject);
        currentFoodTarget = null;
        StopEating();
    }

    public void AddSaturation(float amount)
    {
        saturation = Mathf.Clamp01(saturation + amount);
        UpdatePhysicalStats();
    }

    void UpdatePhysicalStats()
    {
        float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, saturation);
        transform.localScale = Vector3.one * targetScale;
        rb.mass = Mathf.Lerp(1f, massMultiplier, saturation);
    }
}