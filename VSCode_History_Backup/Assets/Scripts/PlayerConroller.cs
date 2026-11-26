using UnityEngine;
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

    [Header("Система веса (Насыщение)")]
    [Range(0, 1)] public float saturation = 0f; // 0 - худой, 1 - максимум
    public float maxScaleMultiplier = 2.5f;     // Во сколько раз увеличится игрок
    public float minSpeedMultiplier = 0.3f;     // Минимальная скорость при полном весе
    public float massMultiplier = 5f;           // Увеличение массы (инерция)

    // Приватные переменные
    private Rigidbody rb;
    private float currentStamina;
    private bool isDashing = false;
    private bool isEating = false;
    private Vector3 moveInput;

    // Взаимодействие
    private EdibleObject currentFoodTarget;
    private float eatTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
        UpdatePhysicalStats(); // Применить стартовые параметры
    }

    void Update()
    {
        // Регенерация стамины
        if (!isDashing)
        {
            currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
        }

        // Блокировка управления при поедании
        if (isEating)
        {
            HandleEatingProcess();
            return; 
        }

        // Ввод движения
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(x, 0, z).normalized;

        // Ввод рывка
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentStamina >= dashCost && moveInput.magnitude > 0)
        {
            StartCoroutine(DashRoutine());
        }

        // Ввод взаимодействия (начало)
        if (Input.GetKeyDown(KeyCode.E) && currentFoodTarget != null)
        {
            StartEating();
        }
    }

    void FixedUpdate()
    {
        if (isEating) return;

        MoveLogic();
    }

    // --- ЛОГИКА ДВИЖЕНИЯ ---
    void MoveLogic()
    {
        if (isDashing) return; // Во время рывка физика управляется импульсом

        // Расчет текущей скорости в зависимости от веса
        // Чем больше saturation, тем меньше скорость (Lerp от Base до Min)
        float currentMaxSpeed = Mathf.Lerp(baseSpeed, baseSpeed * minSpeedMultiplier, saturation);
        
        // Применяем движение через Velocity для контроля
        Vector3 targetVelocity = moveInput * currentMaxSpeed;
        
        // Сохраняем вертикальную скорость (гравитацию)
        targetVelocity.y = rb.linearVelocity.y; // В Unity 6 использовать linearVelocity, в старых - velocity

        rb.linearVelocity = targetVelocity; 

        // Поворот персонажа в сторону движения
        if (moveInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        currentStamina -= dashCost;

        // Импульс рывка
        // Здесь можно добавить "GenerateNoise()" для врага
        rb.AddForce(moveInput * dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f); // Длительность рывка

        // Торможение после рывка, чтобы не улетать бесконечно
        rb.linearVelocity = Vector3.zero; 
        isDashing = false;
    }

    // --- ЛОГИКА ВЗАИМОДЕЙСТВИЯ ---
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            currentFoodTarget = other.GetComponent<EdibleObject>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food") && currentFoodTarget == other.GetComponent<EdibleObject>())
        {
            StopEating(); // Прервать, если отошел или объект исчез
            currentFoodTarget = null;
        }
    }

    void StartEating()
    {
        isEating = true;
        eatTimer = 0f;
        rb.linearVelocity = Vector3.zero; // Полная остановка
    }

    void HandleEatingProcess()
    {
        // Проверка удержания кнопки
        if (Input.GetKey(KeyCode.E))
        {
            eatTimer += Time.deltaTime;

            // Визуализация процесса (можно добавить UI слайдер здесь)
            // Debug.Log($"Eating... {eatTimer}/{currentFoodTarget.eatTime}");

            if (eatTimer >= currentFoodTarget.eatTime)
            {
                ConsumeFood();
            }
        }
        else
        {
            StopEating(); // Отпустил кнопку - прервал
        }
    }

    void StopEating()
    {
        isEating = false;
        eatTimer = 0f;
    }

    void ConsumeFood()
    {
        // Применяем эффекты еды
        AddSaturation(currentFoodTarget.nutritionValue);
        
        // Удаляем объект
        Destroy(currentFoodTarget.gameObject);
        currentFoodTarget = null;
        
        StopEating();
    }

    // --- СИСТЕМА ВЕСА ---

    public void AddSaturation(float amount)
    {
        saturation = Mathf.Clamp01(saturation + amount);
        UpdatePhysicalStats();
    }

    void UpdatePhysicalStats()
    {
        // 1. Изменение размера
        // Интерполяция от 1 (норма) до maxScaleMultiplier
        float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, saturation);
        transform.localScale = Vector3.one * targetScale;

        // 2. Изменение массы (для инерции при столкновениях)
        rb.mass = Mathf.Lerp(1f, massMultiplier, saturation);
    }
}