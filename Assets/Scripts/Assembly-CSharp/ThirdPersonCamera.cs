using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;       // Игрок

    [Header("Настройки Вращения")]
    [Tooltip("Базовая чувствительность (умножается на значение из настроек)")]
    public float baseSensitivity = 1f; 
    public Vector2 verticalLimits = new Vector2(-20, 85);
    
    // Кэшируем значения из SettingsManager
    private float _sensitivityMultiplier = 0.5f;
    private bool _invertY = false; 

    [Header("Сглаживание")]
    public float rotationSmoothTime = 0.12f;

    [Header("Смерть")]
    public float deathZoomOutDistance = 12f;
    public float deathZoomSpeed = 3f;
    public float deathPitchAngle = 45f;
    
    // Внутренние переменные
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;
    private float yaw;   
    private float pitch; 
    
    // Текущая фиксированная дистанция (высчитывается при старте)
    private float distanceFromTarget; 
    
    // Смещение точки фокуса (высчитывается при старте)
    private Vector3 pivotOffset; 

    private GameControls controls;
    private bool isDeathMode = false;
    private float currentDeathDistance;
    private float targetDeathPitch;

    void Awake()
    {
        controls = new GameControls();
    }

    void OnEnable() 
    {
        controls.Gameplay.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Подписка на настройки
        SettingsManager.OnSensitivityChanged += OnSensitivityChanged;
        SettingsManager.OnInvertYChanged += OnInvertYChanged;
        
        // Загрузить текущие значения
        if (SettingsManager.Instance != null)
        {
            _sensitivityMultiplier = SettingsManager.Instance.Sensitivity;
            _invertY = SettingsManager.Instance.InvertY;
        }
    }

    void OnDisable() 
    {
        controls.Gameplay.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Отписка от настроек
        SettingsManager.OnSensitivityChanged -= OnSensitivityChanged;
        SettingsManager.OnInvertYChanged -= OnInvertYChanged;
    }
    
    private void OnSensitivityChanged(float value)
    {
        _sensitivityMultiplier = value;
    }
    
    private void OnInvertYChanged(bool value)
    {
        _invertY = value;
    }

    void Start()
    {
        if (target == null) return;

        // 1. Считываем углы поворота из сцены
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        if (pitch > 180) pitch -= 360;

        currentRotation = new Vector3(pitch, yaw);

        // 2. Считываем и ФИКСИРУЕМ дистанцию
        distanceFromTarget = Vector3.Distance(transform.position, target.position);

        // 3. Вычисляем смещение (Pivot Offset), чтобы сохранить твой кадр
        Vector3 focusPoint = transform.position + transform.forward * distanceFromTarget;
        pivotOffset = focusPoint - target.position;
        
        // Оставляем только высоту (Y)
        pivotOffset = new Vector3(0, pivotOffset.y, 0); 
    }

    public void OnPlayerDeath()
    {
        isDeathMode = true;
        currentDeathDistance = distanceFromTarget;
        targetDeathPitch = deathPitchAngle;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (isDeathMode)
        {
            UpdateDeathCamera();
        }
        else
        {
            UpdateNormalCamera();
        }
    }

    void UpdateNormalCamera()
    {
        // Вращение
        Vector2 mouseDelta = controls.Gameplay.Look.ReadValue<Vector2>();
        float effectiveSensitivity = baseSensitivity * _sensitivityMultiplier;
        
        yaw += mouseDelta.x * effectiveSensitivity;
        
        // Инверсия оси Y
        float yInput = _invertY ? mouseDelta.y : -mouseDelta.y;
        pitch += yInput * effectiveSensitivity;
        pitch = Mathf.Clamp(pitch, verticalLimits.x, verticalLimits.y);

        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        // Позиция (без зума, используем фиксированную distanceFromTarget)
        Vector3 focusPoint = target.position + pivotOffset; 
        transform.position = focusPoint - transform.forward * distanceFromTarget;
    }

    void UpdateDeathCamera()
    {
        currentDeathDistance = Mathf.Lerp(currentDeathDistance, deathZoomOutDistance, deathZoomSpeed * Time.deltaTime);
        pitch = Mathf.Lerp(pitch, targetDeathPitch, deathZoomSpeed * Time.deltaTime);
        yaw += Time.deltaTime * 10f;
        
        currentRotation = new Vector3(pitch, yaw, 0f);
        transform.eulerAngles = currentRotation;
        
        Vector3 focusPoint = target.position + pivotOffset;
        transform.position = focusPoint - transform.forward * currentDeathDistance;
        transform.LookAt(focusPoint);
    }
}