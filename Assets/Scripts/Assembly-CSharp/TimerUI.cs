using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI компонент для отображения таймера уровня.
/// Формат: "03:00" (минуты:секунды)
/// Меняет цвет при низком времени.
/// </summary>
public class TimerUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("TextMeshPro текст для таймера")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Tooltip("Альтернатива: обычный UI Text")]
    [SerializeField] private Text timerTextLegacy;
    
    [Tooltip("Иконка часов (опционально)")]
    [SerializeField] private Image clockIcon;

    [Header("Цвета")]
    [Tooltip("Обычный цвет таймера")]
    public Color normalColor = Color.white;
    
    [Tooltip("Цвет при предупреждении (мало времени)")]
    public Color warningColor = new Color(1f, 0.8f, 0.2f); // Жёлтый
    
    [Tooltip("Цвет когда время почти вышло")]
    public Color criticalColor = new Color(1f, 0.3f, 0.3f); // Красный
    
    [Tooltip("Порог предупреждения (секунды)")]
    public float warningThreshold = 60f;
    
    [Tooltip("Критический порог (секунды)")]
    public float criticalThreshold = 15f;

    [Header("Анимация")]
    [Tooltip("Пульсировать при критическом времени")]
    public bool pulseOnCritical = true;
    
    [Tooltip("Скорость пульсации")]
    public float pulseSpeed = 2f;
    
    [Tooltip("Масштаб пульсации")]
    public float pulseScale = 1.08f;
    
    [Tooltip("Трясти при критическом времени")]
    public bool shakeOnCritical = false;
    
    [Tooltip("Сила тряски")]
    public float shakeIntensity = 1f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isWarning = false;
    private bool isCritical = false;

    void Start()
    {
        // Сохраняем начальные позиции для анимаций
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // Подписываемся на LevelTimer
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimerUpdated.AddListener(UpdateTimerDisplay);
            LevelTimer.Instance.onWarningStarted.AddListener(OnWarningStarted);
            LevelTimer.Instance.onTimeExpired.AddListener(OnTimeExpired);
            
            // Начальное обновление
            UpdateTimerDisplay(LevelTimer.Instance.GetTimeRemaining());
        }
        else
        {
            Debug.LogWarning("TimerUI: LevelTimer.Instance не найден! Подождите инициализации.");
            StartCoroutine(WaitForLevelTimer());
        }

        SetTextColor(normalColor);
    }

    IEnumerator WaitForLevelTimer()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimerUpdated.AddListener(UpdateTimerDisplay);
            LevelTimer.Instance.onWarningStarted.AddListener(OnWarningStarted);
            LevelTimer.Instance.onTimeExpired.AddListener(OnTimeExpired);
            UpdateTimerDisplay(LevelTimer.Instance.GetTimeRemaining());
        }
    }

    void Update()
    {
        // Анимации при критическом времени
        if (isCritical)
        {
            // Пульсация
            if (pulseOnCritical)
            {
                float pulse = 1f + Mathf.PingPong(Time.time * pulseSpeed, pulseScale - 1f);
                transform.localScale = originalScale * pulse;
            }

            // Тряска
            if (shakeOnCritical)
            {
                Vector3 shake = new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity, shakeIntensity),
                    0f
                );
                transform.localPosition = originalPosition + shake;
            }
        }
        else if (isWarning && pulseOnCritical)
        {
            // Лёгкая пульсация при предупреждении
            float pulse = 1f + Mathf.PingPong(Time.time * pulseSpeed * 0.5f, (pulseScale - 1f) * 0.3f);
            transform.localScale = originalScale * pulse;
        }
        else
        {
            // Сброс к нормальному состоянию
            transform.localScale = originalScale;
            transform.localPosition = originalPosition;
        }
    }

    /// <summary>
    /// Обновление отображения таймера
    /// </summary>
    void UpdateTimerDisplay(float timeRemaining)
    {
        // Форматируем время
        string formattedTime = FormatTime(timeRemaining);
        
        if (timerText != null)
        {
            timerText.text = formattedTime;
        }
        if (timerTextLegacy != null)
        {
            timerTextLegacy.text = formattedTime;
        }

        // Обновляем цвет в зависимости от оставшегося времени
        UpdateTimerColor(timeRemaining);
    }

    void UpdateTimerColor(float timeRemaining)
    {
        Color targetColor;

        if (timeRemaining <= criticalThreshold)
        {
            targetColor = criticalColor;
            
            if (!isCritical)
            {
                isCritical = true;
                isWarning = true;
            }
        }
        else if (timeRemaining <= warningThreshold)
        {
            targetColor = warningColor;
            isCritical = false;
            
            if (!isWarning)
            {
                isWarning = true;
            }
        }
        else
        {
            targetColor = normalColor;
            isWarning = false;
            isCritical = false;
        }

        SetTextColor(targetColor);
    }

    void SetTextColor(Color color)
    {
        if (timerText != null)
        {
            timerText.color = color;
        }
        if (timerTextLegacy != null)
        {
            timerTextLegacy.color = color;
        }
        if (clockIcon != null)
        {
            clockIcon.color = color;
        }
    }

    /// <summary>
    /// Форматирование времени в MM:SS
    /// </summary>
    string FormatTime(float seconds)
    {
        seconds = Mathf.Max(0f, seconds);
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }

    void OnWarningStarted()
    {
        Debug.Log("[TimerUI] Предупреждение активировано!");
    }

    void OnTimeExpired()
    {
        Debug.Log("[TimerUI] Время вышло!");
        
        // Финальное отображение
        if (timerText != null)
        {
            timerText.text = "00:00";
        }
        if (timerTextLegacy != null)
        {
            timerTextLegacy.text = "00:00";
        }
        
        SetTextColor(criticalColor);
        
        // Останавливаем анимации
        isCritical = false;
        isWarning = false;
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
    }

    void OnDestroy()
    {
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimerUpdated.RemoveListener(UpdateTimerDisplay);
            LevelTimer.Instance.onWarningStarted.RemoveListener(OnWarningStarted);
            LevelTimer.Instance.onTimeExpired.RemoveListener(OnTimeExpired);
        }
    }
}
