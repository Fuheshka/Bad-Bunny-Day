using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
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
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.1f; // Длина луча вниз

    [Header("Game Feel")]
    [Tooltip("Время после схода с платформы, когда ещё можно прыгнуть")]
    public float coyoteTime = 0.15f;
    [Tooltip("Время буферизации нажатия прыжка")]
    public float jumpBufferTime = 0.12f;
    [Tooltip("Время буферизации нажатия рывка")]
    public float dashBufferTime = 0.1f;
    [Tooltip("Множитель обрезки прыжка при отпускании кнопки")]
    public float jumpCutMultiplier = 0.4f;
    [Tooltip("Дистанция проверки углов для скольжения")]
    public float cornerCheckDistance = 0.5f;
    [Tooltip("Слой препятствий для Corner Sliding")]
    public LayerMask obstacleLayer;
    [Tooltip("Сила смещения при скольжении по углам")]
    public float cornerSlideForce = 3f;

    [Header("Система веса")]
    [Range(0, 1)] public float saturation = 0f;
    public float maxScaleMultiplier = 1.5f;
    public float minSpeedMultiplier = 0.3f;
    public float massMultiplier = 5f;

    [Header("Здоровье")]
    public int maxHealth = 3;
    [Tooltip("Длительность неуязвимости после урона")]
    public float invincibilityDuration = 1.5f;
    [Tooltip("Сила отталкивания при получении урона")]
    public float knockbackForce = 10f;
    [Tooltip("Порог насыщения для жирового щита (0-1)")]
    public float fatShieldThreshold = 0.5f;
    [Tooltip("Множитель скорости после сброса жира")]
    public float fatShieldSpeedBoost = 1.5f;
    [Tooltip("Длительность бонуса скорости")]
    public float fatShieldBoostDuration = 1f;
    [Tooltip("Какой процент очков из 'живота' теряется при получении урона (0.0 - 1.0)")]
    [Range(0f, 1f)] public float fatShieldScoreLossPercent = 0.5f;

    [Header("Анимация")]
    public Animator animator;
    public ProceduralAnimator proceduralAnimator;
[Tooltip("Перетащи сюда AudioSource, который играет музыку уровня")]
    [SerializeField] private AudioSource _audioSource; 
    
    [Space(5)]
    [SerializeField] private AudioSource _levelMusic; 
    [Range(0f, 1f)] public float musicVolume = 0.3f;

    [Space(5)]
    [SerializeField] private AudioClip[] _audioRun; 
    [Range(0f, 1f)] public float runVolume = 0.3f;

    [Space(5)]
    [SerializeField] private AudioClip _audioEat;
    [Range(0f, 1f)] public float eatVolume = 0.8f;

    [Space(5)]
    [SerializeField] private AudioClip _audioJump;
    [Range(0f, 1f)] public float jumpVolume = 0.5f;

    [Space(5)]
    [SerializeField] private AudioClip _audioDash;
    [Range(0f, 1f)] public float dashVolume = 0.6f;

    [Space(5)]
    [SerializeField] private AudioClip _audioTakeDamage;
    [Range(0f, 1f)] public float damageVolume = 1f;

    [Space(5)]
    [SerializeField] private AudioClip _audioDeath;
    [Range(0f, 1f)] public float deathVolume = 1f;

    [Space(5)]
    [SerializeField] private AudioClip _audioRabbitGrowth;
    [Range(0f, 1f)] public float rabbitGrowthVolume = 0.85f;

    [Header("UI Взаимодействия")]
    [Tooltip("Ссылка на UI Image с Fill Method = Radial 360")]
    public Image eatIndicatorUI;

    // Хеши параметров анимации (Animator Controller)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
    private static readonly int AnimIsDashing = Animator.StringToHash("IsDashing");
    private static readonly int AnimIsEating = Animator.StringToHash("IsEating");
    private static readonly int AnimDieTrigger = Animator.StringToHash("Die"); // Триггер вместо Bool

    // Ссылки
    private Rigidbody rb;
    private GameControls controls;
    private Collider playerCollider;
    private Transform cameraTransform;
    
    // Начальные размеры коллайдера (для масштабирования)
    private Vector3 initialColliderSize;
    private Vector3 initialColliderCenter;
    private float initialColliderRadius;
    private float initialColliderHeight;

    // Состояния
    private float currentStamina;
    private bool isDashing = false;
    private bool isEating = false;
    private bool wasGrounded = true;
    private float lastVerticalVelocity = 0f;
    
    // HP состояния
    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;
    private Renderer[] playerRenderers;
    
    // Game Feel состояния
    private float lastGroundedTime = float.NegativeInfinity;        // Coyote Time
    private float lastJumpPressTime = float.NegativeInfinity;       // Jump Buffer
    private float lastDashPressTime = float.NegativeInfinity;       // Dash Buffer
    private bool hasJumped = false;        // Уже прыгнул в этом "сеансе" (сбрасывается при касании земли)
    private bool isJumpHeld = false;       // Variable Jump Height - зажата ли кнопка
    private Vector3 bufferedDashDirection; // Сохранённое направление рывка

    // Grounding (строго по контакту с поверхностью, а не по Raycast)
    private readonly HashSet<Collider> groundContactIds = new HashSet<Collider>();
    private bool isGrounded;
    private bool wasGroundedForJump;
    private float ignoreGroundUntilTime = float.NegativeInfinity;
    //[SerializeField] private Farmer fermer;
    public UnityEvent<Vector3> onNoise;
    
    // HP события
    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public UnityEvent<int, int> onHealthChanged; // current, max
    
    // Stamina событие
    public UnityEvent<float, float> onStaminaChanged; // current, max
    
    // Взаимодействие
    private EdibleObject currentFoodTarget;
    private float eatTimer = 0f;
    private bool isReadyToAnimate = false;
    
    void Awake()
    {
        controls = new GameControls();
        
        // Подписки на кнопки (с буферизацией)
        controls.Gameplay.Dash.performed += ctx => OnDashInput();
        controls.Gameplay.Jump.performed += ctx => OnJumpInput();
        controls.Gameplay.Jump.canceled += ctx => OnJumpRelease(); // Variable Jump

        controls.Gameplay.Interact.performed += ctx => ChangeAudio(_audioEat);
        controls.Gameplay.Interact.canceled += ctx => _audioSource.Pause();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        currentStamina = maxStamina;
        
        // Сохраняем начальные размеры коллайдера
        SaveInitialColliderSize();
        
        // Находим главную камеру автоматически
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Не найдена MainCamera!");
        }

        // Автоматически находим Animator, если не назначен
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Автоматически находим ProceduralAnimator, если не назначен
        if (proceduralAnimator == null)
        {
            proceduralAnimator = GetComponentInChildren<ProceduralAnimator>();
        }

        // Инициализация HP
        currentHealth = maxHealth;
        playerRenderers = GetComponentsInChildren<Renderer>();
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        UpdatePhysicalStats();

        if (_levelMusic != null)
        {
            _levelMusic.volume = musicVolume; // Применяем значение с ползунка
            _levelMusic.loop = true;
            // На всякий случай убедимся, что она играет
            
            if (!_levelMusic.isPlaying) _levelMusic.Play();
        }

        if (eatIndicatorUI != null)
        {
            eatIndicatorUI.gameObject.SetActive(false);
        }

        StartCoroutine(StabilizeAtStart());
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null || _audioSource == null) return;
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clip, volume);
    }
    
    /// <summary>
    /// Сохраняет начальные размеры коллайдера для масштабирования
    /// </summary>
    void SaveInitialColliderSize()
    {
        if (playerCollider is CapsuleCollider capsule)
        {
            initialColliderRadius = capsule.radius;
            initialColliderHeight = capsule.height;
            initialColliderCenter = capsule.center;
        }
        else if (playerCollider is BoxCollider box)
        {
            initialColliderSize = box.size;
            initialColliderCenter = box.center;
        }
        else if (playerCollider is SphereCollider sphere)
        {
            initialColliderRadius = sphere.radius;
            initialColliderCenter = sphere.center;
        }
    }
    
    /// <summary>
    /// Обновляет размер коллайдера в соответствии с масштабом персонажа
    /// </summary>
    void UpdateColliderSize(float scaleMultiplier)
    {
        if (playerCollider is CapsuleCollider capsule)
        {
            capsule.radius = initialColliderRadius * scaleMultiplier;
            capsule.height = initialColliderHeight * scaleMultiplier;
            capsule.center = initialColliderCenter * scaleMultiplier;
        }
        else if (playerCollider is BoxCollider box)
        {
            box.size = initialColliderSize * scaleMultiplier;
            box.center = initialColliderCenter * scaleMultiplier;
        }
        else if (playerCollider is SphereCollider sphere)
        {
            sphere.radius = initialColliderRadius * scaleMultiplier;
            sphere.center = initialColliderCenter * scaleMultiplier;
        }
    }

    void Update()
    {
        if (isDead) return;
        
        if (!isDashing)
        {
            float prevStamina = currentStamina;
            currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
            if (Mathf.Abs(currentStamina - prevStamina) > 0.01f)
            {
                onStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }

        // === GAME FEEL TIMERS ===
        UpdateCoyoteTime();
        ProcessJumpBuffer();
        ProcessDashBuffer();

        HandleInteractionInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (isEating) return;
        MoveLogic();
        ApplyCornerSliding();
        ApplyWallSlide(); // Скольжение вниз при контакте со стеной
    }

    // --- ЛОГИКА АНИМАЦИЙ ---
    void UpdateAnimations()
    {
        if (!isReadyToAnimate) return; 

        bool grounded = IsGrounded();
        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();
        float speed = inputVector.magnitude;
        bool isInAir = !grounded;
        bool isJumping = isInAir && rb.linearVelocity.y > 0.1f;
        bool isFalling = isInAir && rb.linearVelocity.y <= 0.1f;

        // === ANIMATOR (основные анимации) ===
        if (animator != null)
        {
            // Speed = 0 если в воздухе, рывке или едим (чтобы не было анимации ходьбы)
            float animSpeed = (isInAir || isDashing || isEating) ? 0f : speed;
            animator.SetFloat(AnimSpeed, animSpeed);
            
            animator.SetBool(AnimIsGrounded, grounded);
            animator.SetBool(AnimIsJumping, isJumping);
            animator.SetBool(AnimIsDashing, isDashing);
            animator.SetBool(AnimIsEating, isEating);
        }

        // === PROCEDURAL ANIMATOR (squash & stretch эффекты) ===
        if (proceduralAnimator == null) return;

        // Проверка приземления
        if (grounded && !wasGrounded)
        {
            proceduralAnimator.AnimateLand(lastVerticalVelocity);
        }
        wasGrounded = grounded;
        lastVerticalVelocity = rb.linearVelocity.y;

        // Приоритет состояний (от высшего к низшему):
        // 1. Еда
        // 2. Рывок
        // 3. Прыжок/Падение
        // 4. Idle

        // Состояние еды (высший приоритет)
        if (isEating)
        {
            proceduralAnimator.AnimateEating();
           //ChangeAudio(_audioEat);
            return;
        }

        // Состояние рывка (управляется в DashRoutine)
        if (isDashing) 
        {
            ChangeAudio(_audioDash);
            return;
        }

        // Состояние в воздухе
        if (!grounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                proceduralAnimator.AnimateJumpAir();
            }
            else
            {
                proceduralAnimator.AnimateFall(rb.linearVelocity.y);
            }
            return;
        }

        // На земле - сброс к нормальному состоянию
        proceduralAnimator.ResetToIdle();
    }

    private void ChangeAudio(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    // --- ЛОГИКА ДВИЖЕНИЯ ---
    void MoveLogic()
    {
        if (isDashing) return;

        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();

        // Если камера существует, рассчитываем движение относительно неё
        Vector3 moveDirection = Vector3.zero;

        if (inputVector.magnitude >= 0.1f)
        {
            // Берем вектора направления камеры
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // "Сплющиваем" их, чтобы игнорировать наклон камеры вверх/вниз
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Формируем итоговый вектор движения:
            // W/S двигает вдоль camForward, A/D вдоль camRight
            moveDirection = (camForward * inputVector.y + camRight * inputVector.x).normalized;
        }

        // Расчет скорости
        float currentMaxSpeed = Mathf.Lerp(baseSpeed, baseSpeed * minSpeedMultiplier, saturation);
        
        Vector3 targetVelocity = moveDirection * currentMaxSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Unity 6 (или .velocity)

        rb.linearVelocity = targetVelocity;

        // Поворот персонажа в сторону движения
        if (moveDirection.magnitude > 0.1f)
        {
            // Увеличил скорость поворота (15f), чтобы персонаж быстрее реагировал на мышь
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime);
        }
    }

    // === GAME FEEL: COYOTE TIME ===
    void UpdateCoyoteTime()
    {
        bool groundedNow = isGrounded;

        if (groundedNow)
        {
            // Пока стоим на земле — обновляем время «последнего контакта с землёй»
            lastGroundedTime = Time.time;

            // Сбрасываем запрет прыжка ТОЛЬКО в момент реального приземления
            if (!wasGroundedForJump)
            {
                hasJumped = false;
            }
        }

        wasGroundedForJump = groundedNow;
    }

    bool CanJumpWithCoyote()
    {
        // НЕЛЬЗЯ прыгать если уже прыгнули в этом "сеансе"
        if (hasJumped) return false;
        
        // Можно прыгать если на земле или недавно были на земле (Coyote Time)
        return Time.time - lastGroundedTime <= coyoteTime;
    }

    // === GAME FEEL: INPUT BUFFERING ===
    void OnJumpInput()
    {
        lastJumpPressTime = Time.time;
        TryExecuteJump();
    }

    void OnDashInput()
    {
        lastDashPressTime = Time.time;
        // Сохраняем направление рывка в момент нажатия
        ChangeAudio(_audioDash);
        bufferedDashDirection = GetDashDirection();
        TryExecuteDash();
    }

    void ProcessJumpBuffer()
    {
        // Защита от срабатывания в первые кадры сцены
        if (Time.time < jumpBufferTime) return;
        
        // Если есть буферизованный прыжок и мы приземлились
        if (Time.time - lastJumpPressTime <= jumpBufferTime)
        {
            TryExecuteJump();
        }
    }

    void ProcessDashBuffer()
    {
        // Защита от срабатывания в первые кадры сцены
        if (Time.time < dashBufferTime) return;
        
        // Если есть буферизованный рывок и мы можем его выполнить
        if (Time.time - lastDashPressTime <= dashBufferTime)
        {
            TryExecuteDash();
        }
    }

    // === GAME FEEL: VARIABLE JUMP HEIGHT ===
    void OnJumpRelease()
    {
        // Если отпустили кнопку прыжка во время подъёма - обрезаем скорость
        if (isJumpHeld && rb.linearVelocity.y > 0)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y *= jumpCutMultiplier;
            rb.linearVelocity = vel;
        }
        isJumpHeld = false;
    }

    // --- ЛОГИКА ПРЫЖКА ---
    void TryExecuteJump()
    {
        // Прыгаем если: есть Coyote Time (и не прыгали), не едим, не в рывке
        if (CanJumpWithCoyote() && !isEating && !isDashing)
        {
            ChangeAudio(_audioJump);
            ExecuteJump();
            // Сбрасываем буфер (на -infinity чтобы гарантированно не сработал повторно)
            lastJumpPressTime = float.NegativeInfinity;
        }
    }

    void ExecuteJump()
    {
        hasJumped = true;     // Отмечаем что прыгнули - больше нельзя до приземления
        isJumpHeld = true;    // Для Variable Jump Height

        // Сразу переводим в «в воздухе», чтобы контакт с землёй в тот же physics-step
        // не считался приземлением и не сбрасывал hasJumped.
        isGrounded = false;
        groundContactIds.Clear();
        ignoreGroundUntilTime = Time.time + 0.05f;
        
        // Сбрасываем вертикальную скорость перед прыжком
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;
        rb.linearVelocity = vel;

        // Запускаем анимацию подготовки к прыжку
        if (proceduralAnimator != null)
        {
            proceduralAnimator.AnimateJumpStart();
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    bool IsGrounded() => isGrounded;

    private void OnCollisionEnter(Collision collision)
    {
        TryRegisterGroundContact(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryRegisterGroundContact(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Убираем контакт, если это земля
        if (((1 << collision.gameObject.layer) & groundLayer.value) == 0) return;

        groundContactIds.Remove(collision.collider);
        isGrounded = groundContactIds.Count > 0;
    }

    private void TryRegisterGroundContact(Collision collision)
    {
        if (Time.time < ignoreGroundUntilTime) return;
        if (((1 << collision.gameObject.layer) & groundLayer.value) == 0) return;

        // Считаем "землёй" только поверхность под ногами (нормаль вверх)
        bool hasUpwardNormal = false;
        var contacts = collision.contacts;
        for (int i = 0; i < contacts.Length; i++)
        {
            if (Vector3.Dot(contacts[i].normal, Vector3.up) > 0.5f)
            {
                hasUpwardNormal = true;
                break;
            }
        }
        if (!hasUpwardNormal) return;

        groundContactIds.Add(collision.collider);
        isGrounded = true;
    }

    // --- ЛОГИКА РЫВКА ---
    Vector3 GetDashDirection()
    {
        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();

        if (inputVector.magnitude > 0.1f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            return (camForward * inputVector.y + camRight * inputVector.x).normalized;
        }
        return transform.forward;
    }

    void TryExecuteDash()
    {
        if (currentStamina >= dashCost && !isEating && !isDashing)
        {
            StartCoroutine(DashRoutine(bufferedDashDirection));
            // Сбрасываем буфер
            lastDashPressTime = 0f;
        }
    }

    IEnumerator DashRoutine(Vector3 direction)
    {
        isDashing = true;
        currentStamina -= dashCost;
        onStaminaChanged?.Invoke(currentStamina, maxStamina);

        // Запускаем анимацию рывка
        if (proceduralAnimator != null)
        {
            proceduralAnimator.AnimateDash(direction);
        }

        rb.AddForce(direction * dashForce, ForceMode.Impulse);
        //fermer.HearNoise(transform.position);
        onNoise.Invoke(transform.position);
        yield return new WaitForSeconds(0.2f);
        
        // Завершаем анимацию рывка
        if (proceduralAnimator != null)
        {
            proceduralAnimator.AnimateDashEnd();
        }
        
        isDashing = false;
    }

    // === GAME FEEL: CORNER SLIDING ===
    void ApplyCornerSliding()
    {
        if (isDashing || isEating) return;
        
        Vector2 inputVector = controls.Gameplay.Move.ReadValue<Vector2>();
        if (inputVector.magnitude < 0.1f) return;

        // Получаем направление движения
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 moveDir = (camForward * inputVector.y + camRight * inputVector.x).normalized;

        // Размер персонажа (масштабируется с сытостью)
        float playerRadius = playerCollider.bounds.extents.x;
        float checkDist = cornerCheckDistance + playerRadius;

        // Центральный луч
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        bool centerHit = Physics.Raycast(origin, moveDir, checkDist, obstacleLayer);

        if (!centerHit) return; // Нет препятствия впереди

        // Боковые лучи (левый и правый под углом 45°)
        Vector3 rightDir = Quaternion.Euler(0, 45, 0) * moveDir;
        Vector3 leftDir = Quaternion.Euler(0, -45, 0) * moveDir;

        bool rightHit = Physics.Raycast(origin, rightDir, checkDist, obstacleLayer);
        bool leftHit = Physics.Raycast(origin, leftDir, checkDist, obstacleLayer);

        Vector3 slideForce = Vector3.zero;

        // Если справа свободно, а слева стена - смещаемся вправо
        if (!rightHit && leftHit)
        {
            slideForce = Vector3.Cross(Vector3.up, moveDir).normalized * cornerSlideForce;
        }
        // Если слева свободно, а справа стена - смещаемся влево
        else if (!leftHit && rightHit)
        {
            slideForce = -Vector3.Cross(Vector3.up, moveDir).normalized * cornerSlideForce;
        }
        // Если обе стороны свободны (узкий угол) - проверяем дополнительно
        else if (!rightHit && !leftHit)
        {
            // Дополнительные лучи по краям персонажа
            Vector3 rightOffset = Vector3.Cross(Vector3.up, moveDir) * playerRadius;
            bool edgeRightHit = Physics.Raycast(origin + rightOffset, moveDir, checkDist, obstacleLayer);
            bool edgeLeftHit = Physics.Raycast(origin - rightOffset, moveDir, checkDist, obstacleLayer);

            if (!edgeRightHit && edgeLeftHit)
            {
                slideForce = Vector3.Cross(Vector3.up, moveDir).normalized * cornerSlideForce;
            }
            else if (edgeRightHit && !edgeLeftHit)
            {
                slideForce = -Vector3.Cross(Vector3.up, moveDir).normalized * cornerSlideForce;
            }
        }

        if (slideForce != Vector3.zero)
        {
            rb.AddForce(slideForce, ForceMode.Acceleration);
        }
    }

    // === WALL SLIDE: Скольжение вниз при контакте со стеной ===
    void ApplyWallSlide()
    {
        // Только в воздухе
        if (IsGrounded()) return;
        
        // Проверяем контакт со стеной
        float playerRadius = playerCollider.bounds.extents.x;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        
        // Проверяем все 4 стороны
        bool touchingWall = 
            Physics.Raycast(origin, transform.forward, playerRadius + 0.1f, obstacleLayer) ||
            Physics.Raycast(origin, -transform.forward, playerRadius + 0.1f, obstacleLayer) ||
            Physics.Raycast(origin, transform.right, playerRadius + 0.1f, obstacleLayer) ||
            Physics.Raycast(origin, -transform.right, playerRadius + 0.1f, obstacleLayer);
        
        if (touchingWall && rb.linearVelocity.y <= 0)
        {
            // Если касаемся стены и падаем/зависаем - убираем горизонтальную скорость
            // и добавляем небольшую силу вниз чтобы соскальзывать
            Vector3 vel = rb.linearVelocity;
            
            // Сбрасываем горизонтальную скорость если она направлена в стену
            vel.x *= 0.9f;
            vel.z *= 0.9f;
            
            // Если почти не падаем - добавляем гравитацию
            if (Mathf.Abs(vel.y) < 0.5f)
            {
                vel.y = -2f; // Принудительно падаем вниз
            }
            
            rb.linearVelocity = vel;
        }
    }

    // --- ЛОГИКА ВЗАИМОДЕЙСТВИЯ (Без изменений) ---
    void HandleInteractionInput()
    {
        if (currentFoodTarget == null)
        {
            if (eatIndicatorUI != null && eatIndicatorUI.gameObject.activeSelf)
                eatIndicatorUI.gameObject.SetActive(false);
            
            return;
        }

        // Если еда есть, показываем индикатор
        if (eatIndicatorUI != null && !eatIndicatorUI.gameObject.activeSelf)
        {
             eatIndicatorUI.gameObject.SetActive(true);
        }

        if (controls.Gameplay.Interact.IsPressed())
        {
            if (!isEating && IsGrounded()) StartEating();
            
            eatTimer += Time.deltaTime;

            // --- ОБНОВЛЕНИЕ ПОЛОСКИ ---
            if (eatIndicatorUI != null)
            {
                // fillAmount - это значение от 0 до 1.
                // Делим текущее время удержания на общее время поедания.
                eatIndicatorUI.fillAmount = eatTimer / currentFoodTarget.eatTime;
            }

            if (eatTimer >= currentFoodTarget.eatTime)
            {
                ConsumeFood();
            }
        }
        else
        {
            if (isEating) StopEating();

            // Если кнопку отпустили - сбрасываем полоску
            if (eatIndicatorUI != null)
            {
                eatIndicatorUI.fillAmount = 0;
            }
        }
    }
// --- ИСПРАВЛЕННЫЕ МЕТОДЫ ТРИГГЕРА (БЕЗ UI ТЕКСТА) ---

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошли в триггер объекта с тегом "Food"
        if (other.CompareTag("Food"))
        {
            EdibleObject food = other.GetComponent<EdibleObject>();
            if (food == null) return;

            // Если у нас уже была выбрана другая еда, гасим её обводку
            if (currentFoodTarget != null && currentFoodTarget != food)
            {
                currentFoodTarget.Unhighlight();
            }

            // Запоминаем новую цель и подсвечиваем её
            currentFoodTarget = food;
            currentFoodTarget.Highlight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Проверяем, что вышли из триггера объекта с тегом "Food"
        if (other.CompareTag("Food"))
        {
            // Убеждаемся, что мы выходим именно из ТОЙ еды, которая у нас сейчас в цели
            if (currentFoodTarget != null && currentFoodTarget.gameObject == other.gameObject)
            {
                // Гасим обводку
                currentFoodTarget.Unhighlight();
                
                // Сбрасываем цель
                currentFoodTarget = null;
                
                // Прерываем процесс поедания, если он шел
                StopEating();
                
                // Также прячем индикатор поедания (круг)
                if (eatIndicatorUI != null) eatIndicatorUI.gameObject.SetActive(false);
            }
        }
    }
    void StartEating()
    {
        isEating = true;
        eatTimer = 0f;
        rb.linearVelocity = Vector3.zero;

        if (eatIndicatorUI != null)
        {
            eatIndicatorUI.fillAmount = 0; // Сбрасываем
        }

        // --- ФИКС ЗВУКА ПОЕДАНИЯ ---
        _audioSource.clip = _audioEat;
        _audioSource.loop = true; // Включаем зацикливание
        _audioSource.Play();
    }

    void StopEating()
    {
        isEating = false;
        eatTimer = 0f;
        
        if (proceduralAnimator != null)
        {
            proceduralAnimator.AnimateEatEnd();
        }

        // --- ФИКС ЗВУКА ПОЕДАНИЯ ---
        _audioSource.loop = false; // Выключаем зацикливание
        _audioSource.Stop();       // Останавливаем звук
    }

    void ConsumeFood()
    {
        AddSaturation(currentFoodTarget.nutritionValue);
        
        // Добавляем очки в живот (ScoreManager)
        if (ScoreManager.Instance != null && currentFoodTarget.scoreValue > 0)
        {
            ScoreManager.Instance.AddBellyScore(currentFoodTarget.scoreValue);
        }
        
        // Лечение, если еда восстанавливает HP
        if (currentFoodTarget.healAmount > 0)
        {
            Heal(currentFoodTarget.healAmount);
        }

        PlaySound(_audioRabbitGrowth, rabbitGrowthVolume);
        
        if (eatIndicatorUI != null)
        {
            eatIndicatorUI.gameObject.SetActive(false);
        }

        Destroy(currentFoodTarget.gameObject);
        currentFoodTarget = null;
        //fermer.HearNoise(cameraTransform.position);
        onNoise.Invoke(transform.position);
        StopEating();
    }

    public void AddSaturation(float amount)
    {
        saturation = Mathf.Clamp01(saturation + amount);
        // Запускаем плавное увеличение размера
        StartCoroutine(SmoothScaleUpdate());
    }

    /// <summary>
    /// Плавное изменение размера при изменении сытости
    /// </summary>
    private Coroutine scaleCoroutine;
    
    IEnumerator SmoothScaleUpdate()
    {
        float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, saturation);
        Vector3 targetScaleVec = Vector3.one * targetScale;
        Vector3 startScale = transform.localScale;
        
        float duration = 0.3f; // Время плавного увеличения
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Используем EaseOutBack для "упругого" эффекта
            t = 1f + 1.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f);
            
            transform.localScale = Vector3.Lerp(startScale, targetScaleVec, Mathf.Clamp01(t));
            
            // Обновляем базовый масштаб для процедурных анимаций
            if (proceduralAnimator != null)
            {
                proceduralAnimator.UpdateBaseScale(transform.localScale);
            }
            
            yield return null;
        }
        
        transform.localScale = targetScaleVec;
        rb.mass = Mathf.Lerp(1f, massMultiplier, saturation);
        
        // Обновляем коллайдер
        UpdateColliderSize(targetScale);
        
        if (proceduralAnimator != null)
        {
            proceduralAnimator.UpdateBaseScale(targetScaleVec);
        }
    }

    void UpdatePhysicalStats()
    {
        float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, saturation);
        transform.localScale = Vector3.one * targetScale;
        rb.mass = Mathf.Lerp(1f, massMultiplier, saturation);
        
        // Обновляем коллайдер
        UpdateColliderSize(targetScale);
        
        // Обновляем базовый масштаб для процедурных анимаций
        if (proceduralAnimator != null)
        {
            proceduralAnimator.UpdateBaseScale(Vector3.one * targetScale);
        }
    }

    // === СИСТЕМА ЗДОРОВЬЯ ===
    
    /// <summary>
    /// Получение урона. Вызывается врагами при попадании.
    /// </summary>
    /// <param name="damage">Количество урона</param>
    /// <param name="knockbackSource">Позиция источника урона для отталкивания</param>
    public void TakeDamage(int damage, Vector3? knockbackSource = null)
    {
        if (isDead || isInvincible) return;
        ChangeAudio(_audioTakeDamage);
        
        // === ПОТЕРЯ ОЧКОВ ПРОПОРЦИОНАЛЬНО СЫТОСТИ ===
        // Чем жирнее, тем больше теряет при любом урона
        if (ScoreManager.Instance != null && saturation > 0f)
        {
            int currentBellyScore = ScoreManager.Instance.GetBellyScore();
            if (currentBellyScore > 0)
            {
                // Отнимаем процент очков в зависимости от сытости
                // Формула: базовый% + дополнительный% от сытости
                // Минимум при saturation = 0.1: 50% * (0.1 + 0.5) = 30%
                // Максимум при saturation = 1.0: 50% * (1.0 + 0.5) = 75%
                float lossMultiplier = fatShieldScoreLossPercent * (saturation + 0.5f);
                int scoreToLose = Mathf.RoundToInt(currentBellyScore * lossMultiplier);
                if (scoreToLose > 0)
                {
                    ScoreManager.Instance.RemoveBellyScore(scoreToLose);
                    Debug.Log($"[TakeDamage] При урона потеряно {scoreToLose} очков (saturation: {saturation:F2}, loss%: {fatShieldScoreLossPercent})");
                }
            }
        }
        
        // === ЖИРОВОЙ ЩИТ ===
        if (saturation >= fatShieldThreshold)
        {
            // Жир полностью поглощает урон
            saturation = 0f;
            UpdatePhysicalStats();
            
            // Бонус скорости для побега
            StartCoroutine(FatShieldBoostRoutine());
            
            // Отталкивание
            if (knockbackSource.HasValue)
            {
                ApplyKnockback(knockbackSource.Value);
            }
            
            // Запуск неуязвимости и эффектов
            StartCoroutine(InvincibilityRoutine());
            proceduralAnimator?.AnimateDamage();
            onDamage?.Invoke();
            
            return;
        }

        // === ОБЫЧНЫЙ УРОН ===
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Отталкивание
        if (knockbackSource.HasValue)
        {
            ApplyKnockback(knockbackSource.Value);
        }

        // Проверка смерти
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Запуск неуязвимости и эффектов
        StartCoroutine(InvincibilityRoutine());
        proceduralAnimator?.AnimateDamage();
        onDamage?.Invoke();
    }

    /// <summary>
    /// Применить отталкивание от источника урона
    /// </summary>
    void ApplyKnockback(Vector3 source)
    {
        Vector3 knockbackDirection = (transform.position - source).normalized;
        knockbackDirection.y = 0.3f; // Немного вверх
        knockbackDirection.Normalize();
        
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Корутина неуязвимости с миганием
    /// </summary>
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;
        float blinkInterval = 0.1f;
        bool visible = true;

        while (elapsed < invincibilityDuration)
        {
            // Переключаем видимость рендереров
            visible = !visible;
            foreach (var renderer in playerRenderers)
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Гарантируем что персонаж видим после окончания
        foreach (var renderer in playerRenderers)
        {
            if (renderer != null)
                renderer.enabled = true;
        }

        isInvincible = false;
    }

    /// <summary>
    /// Бонус скорости после сброса жирового щита
    /// </summary>
    IEnumerator FatShieldBoostRoutine()
    {
        float originalSpeed = baseSpeed;
        baseSpeed *= fatShieldSpeedBoost;
        
        yield return new WaitForSeconds(fatShieldBoostDuration);
        
        baseSpeed = originalSpeed;
    }

    /// <summary>
    /// Смерть персонажа
    /// </summary>
    void Die()
    {
        isDead = true;
        
        Debug.Log("Player DIED! Triggering death animation");
        ChangeAudio(_audioDeath);
        // Отключаем управление
        controls.Gameplay.Disable();
        
        // Останавливаем движение
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        
        if (_levelMusic != null)
        {
            StartCoroutine(FadeOutMusicRoutine(_levelMusic));
        }

        // Запускаем анимацию смерти
        if (animator != null)
        {
            // Сбрасываем все другие состояния
            animator.SetFloat(AnimSpeed, 0f);
            animator.SetBool(AnimIsJumping, false);
            animator.SetBool(AnimIsDashing, false);
            animator.SetBool(AnimIsEating, false);
            animator.SetBool(AnimIsGrounded, true);
            
            // Триггер смерти
            animator.SetTrigger(AnimDieTrigger);
            Debug.Log($"Animator: Die trigger set. Animator enabled: {animator.enabled}");
        }
        else
        {
            Debug.LogError("Animator is NULL! Cannot play death animation.");
        }
        
        // Оповещаем подписчиков (камера, GameManager)
        onDeath?.Invoke();
    }

    IEnumerator FadeOutMusicRoutine(AudioSource musicSource)
    {
        float startVol = musicSource.volume;
        float duration = 0.5f; // Время затухания
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        
        musicSource.Stop();
    }
    /// <summary>
    /// Лечение персонажа
    /// </summary>
    /// <param name="amount">Количество восстанавливаемого здоровья</param>
    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        if (currentHealth >= maxHealth) return;
        
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Получить текущее здоровье (для UI)
    /// </summary>
    public int GetCurrentHealth() => currentHealth;
    
    /// <summary>
    /// Проверка жив ли персонаж
    /// </summary>
    public bool IsAlive() => !isDead;

    // === ГЕТТЕРЫ ДЛЯ UI ===
    
    /// <summary>
    /// Получить текущую выносливость
    /// </summary>
    public float GetCurrentStamina() => currentStamina;
    
    /// <summary>
    /// Получить максимальную выносливость
    /// </summary>
    public float GetMaxStamina() => maxStamina;
    
    /// <summary>
    /// Получить текущую сытость (0-1)
    /// </summary>
    public float GetSaturation() => saturation;

    // === МЕТОДЫ ДЛЯ БАНКИНГА (НОРА) ===
    
    /// <summary>
    /// Сбросить сытость до нуля (при банкинге в норе).
    /// Персонаж становится снова худым и быстрым.
    /// </summary>
    public void ResetSaturation()
    {
        saturation = 0f;
        UpdatePhysicalStats();
        Debug.Log("[PlayerController] Сытость сброшена! Снова худой и быстрый.");
    }

    // === ЗАВЕРШЕНИЕ УРОВНЯ ===
    
    /// <summary>
    /// Вызывается когда уровень завершён (время вышло).
    /// Блокирует управление и делает игрока неуязвимым.
    /// </summary>
    public void SetLevelComplete()
    {
        if (isDead) return;
        
        Debug.Log("[PlayerController] Уровень завершён! Блокируем управление.");

        if (_levelMusic != null)
        {
            StartCoroutine(FadeOutMusicRoutine(_levelMusic));
        }
        
        // Делаем неуязвимым (чтобы фермер не мог убить)
        isInvincible = true;
        
        // Отключаем управление
        controls.Gameplay.Disable();
        
        // Останавливаем движение
        rb.linearVelocity = Vector3.zero;
        
        // Останавливаем еду если ел
        if (isEating)
        {
            StopEating();
        }
    }

    IEnumerator StabilizeAtStart()
    {
        isReadyToAnimate = false;
        
        // 1. Ждем, пока физика "утрясется" (0.5 сек)
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            
            // ПРИНУДИТЕЛЬНО сбрасываем скейл, чтобы не было "желе"
            // Рассчитываем правильный размер от сытости
            float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, saturation);
            transform.localScale = Vector3.one * targetScale;
            
            // Если есть процедурный аниматор - обновляем его базу, но не даем дергаться
            if (proceduralAnimator != null)
            {
                proceduralAnimator.UpdateBaseScale(transform.localScale);
            }

            yield return null;
        }

        // 2. Всё успокоилось, разрешаем анимации
        isReadyToAnimate = true;
        
        // Сбрасываем запомненную скорость, чтобы не сработала анимация приземления
        lastVerticalVelocity = 0f;
        wasGrounded = true; 
    }
}