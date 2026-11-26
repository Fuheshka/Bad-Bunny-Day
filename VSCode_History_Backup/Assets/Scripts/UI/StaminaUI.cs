using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI компонент для отображения выносливости (stamina).
/// Может работать как World Space UI (над головой) или Screen Space (в углу экрана).
/// </summary>
public class StaminaUI : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Ссылка на игрока")]
    [SerializeField] private PlayerController player;
    
    [Tooltip("Slider для отображения выносливости")]
    [SerializeField] private Slider staminaSlider;
    
    [Tooltip("Image для заливки (Fill) - альтернатива слайдеру")]
    [SerializeField] private Image staminaFill;

    [Header("Режим отображения")]
    [Tooltip("Следовать за игроком (World Space)")]
    public bool followPlayer = false;
    
    [Tooltip("Отступ над головой игрока")]
    public Vector3 followOffset = new Vector3(0f, 2.5f, 0f);

    [Header("Авто-скрытие")]
    [Tooltip("Скрывать когда выносливость полная")]
    public bool hideWhenFull = true;
    
    [Tooltip("Задержка перед скрытием (секунды)")]
    public float hideDelay = 1.5f;
    
    private float hideTimer = 0f;
    private bool isHidden = false;

    [Header("Визуализация")]
    [Tooltip("Цвет при полной выносливости")]
    public Color fullColor = new Color(0.2f, 0.8f, 0.2f); // Зелёный
    
    [Tooltip("Цвет при низкой выносливости")]
    public Color lowColor = new Color(1f, 0.8f, 0.2f); // Жёлтый
    
    [Tooltip("Цвет при пустой выносливости")]
    public Color emptyColor = new Color(1f, 0.3f, 0.3f); // Красный
    
    [Tooltip("Порог низкой выносливости (0-1)")]
    public float lowThreshold = 0.3f;

    [Header("Анимация")]
    [Tooltip("Плавное изменение значения")]
    public bool smoothTransition = true;
    
    [Tooltip("Скорость плавного изменения")]
    public float smoothSpeed = 5f;
    
    [Tooltip("Мигать при пустой выносливости")]
    public bool blinkWhenEmpty = true;
    
    [Tooltip("Скорость мигания")]
    public float blinkSpeed = 4f;

    private float targetValue = 1f;
    private float currentValue = 1f;
    private CanvasGroup canvasGroup;
    private Camera mainCamera;

    void Awake()
    {
        // Сразу устанавливаем полную полоску до Start()
        targetValue = 1f;
        currentValue = 1f;
        
        // Инициализируем слайдер сразу
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = 1f;
        }
        
        if (staminaFill != null)
        {
            staminaFill.fillAmount = 1f;
        }
    }

    void Start()
    {
        // Автоматически находим игрока
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (player != null)
        {
            player.onStaminaChanged.AddListener(UpdateStaminaDisplay);
            
            // Начальное обновление - сразу полная выносливость
            float current = player.GetCurrentStamina();
            float max = player.GetMaxStamina();
            
            // Если игрок уже инициализирован - берём его значения
            if (max > 0 && current > 0)
            {
                targetValue = current / max;
                currentValue = targetValue;
            }
            else
            {
                // Иначе ставим полную полоску
                targetValue = 1f;
                currentValue = 1f;
            }
        }
        else
        {
            Debug.LogWarning("StaminaUI: PlayerController не найден!");
            // Если игрок не найден - всё равно показываем полную полоску
            targetValue = 1f;
            currentValue = 1f;
        }

        // Получаем CanvasGroup для плавного скрытия
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && hideWhenFull)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        mainCamera = Camera.main;

        // Принудительно применяем начальное состояние
        ForceUpdateDisplay();
    }
    
    /// <summary>
    /// Принудительно обновляет отображение (без анимации)
    /// </summary>
    void ForceUpdateDisplay()
    {
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = currentValue;
        }
        
        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentValue;
        }
        
        UpdateColor(currentValue);
    }

    void Update()
    {
        // === СЛЕДОВАНИЕ ЗА ИГРОКОМ ===
        if (followPlayer && player != null)
        {
            transform.position = player.transform.position + followOffset;
            
            // Billboarding - смотрим на камеру
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.forward);
            }
        }

        // === ПЛАВНОЕ ИЗМЕНЕНИЕ ===
        if (smoothTransition && Mathf.Abs(currentValue - targetValue) > 0.001f)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, smoothSpeed * Time.deltaTime);
            SetDisplayValue(currentValue);
            UpdateColor(currentValue);
        }

        // === АВТО-СКРЫТИЕ ===
        if (hideWhenFull && canvasGroup != null)
        {
            if (currentValue >= 0.99f)
            {
                hideTimer += Time.deltaTime;
                if (hideTimer >= hideDelay && !isHidden)
                {
                    StartCoroutine(FadeCanvasGroup(0f));
                    isHidden = true;
                }
            }
            else
            {
                hideTimer = 0f;
                if (isHidden)
                {
                    StartCoroutine(FadeCanvasGroup(1f));
                    isHidden = false;
                }
            }
        }

        // === МИГАНИЕ ПРИ ПУСТОЙ ===
        if (blinkWhenEmpty && currentValue < 0.01f)
        {
            float blink = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            if (staminaFill != null)
            {
                Color c = emptyColor;
                c.a = Mathf.Lerp(0.3f, 1f, blink);
                staminaFill.color = c;
            }
        }
    }

    /// <summary>
    /// Обновление отображения выносливости
    /// </summary>
    void UpdateStaminaDisplay(float current, float max)
    {
        targetValue = max > 0 ? current / max : 0f;

        if (!smoothTransition)
        {
            currentValue = targetValue;
            SetDisplayValue(currentValue);
            UpdateColor(currentValue);
        }
    }

    void SetDisplayValue(float normalizedValue)
    {
        if (staminaSlider != null)
        {
            // Убедимся что slider настроен правильно
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = normalizedValue;
        }
        
        if (staminaFill != null)
        {
            staminaFill.fillAmount = normalizedValue;
        }
    }

    void UpdateColor(float normalizedValue)
    {
        Color targetColor;
        
        if (normalizedValue < 0.01f)
        {
            targetColor = emptyColor;
        }
        else if (normalizedValue < lowThreshold)
        {
            // Интерполяция между empty и low
            float t = normalizedValue / lowThreshold;
            targetColor = Color.Lerp(emptyColor, lowColor, t);
        }
        else
        {
            // Интерполяция между low и full
            float t = (normalizedValue - lowThreshold) / (1f - lowThreshold);
            targetColor = Color.Lerp(lowColor, fullColor, t);
        }

        // Применяем цвет
        if (staminaFill != null)
        {
            staminaFill.color = targetColor;
        }
        
        if (staminaSlider != null && staminaSlider.fillRect != null)
        {
            Image fillImage = staminaSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = targetColor;
            }
        }
    }

    IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.onStaminaChanged.RemoveListener(UpdateStaminaDisplay);
        }
    }
}
