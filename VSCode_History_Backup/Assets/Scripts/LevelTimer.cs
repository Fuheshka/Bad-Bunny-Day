using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Таймер уровня - обратный отсчёт до конца раунда.
/// При истечении времени вызывает событие завершения.
/// </summary>
public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("Настройки")]
    [Tooltip("Время уровня в секундах (180 = 3 минуты)")]
    public float levelDuration = 180f;
    
    [Tooltip("Автоматически запускать таймер при старте")]
    public bool autoStart = true;
    
    [Tooltip("Ставить игру на паузу при истечении времени")]
    public bool pauseOnExpire = false;

    [Header("Предупреждения")]
    [Tooltip("Время когда начинать предупреждать (секунд осталось)")]
    public float warningTime = 30f;
    
    [Tooltip("Звук тиканья при предупреждении")]
    public AudioClip tickSound;
    
    [Tooltip("Интервал тиканья")]
    public float tickInterval = 1f;
    
    private float lastTickTime;
    private AudioSource audioSource;

    [Header("События")]
    [Tooltip("Вызывается каждый кадр с оставшимся временем")]
    public UnityEvent<float> onTimerUpdated;
    
    [Tooltip("Вызывается когда входим в зону предупреждения")]
    public UnityEvent onWarningStarted;
    
    [Tooltip("Вызывается когда время истекло")]
    public UnityEvent onTimeExpired;

    [Header("Состояние (для отладки)")]
    [SerializeField] private float currentTime;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool hasWarned = false;
    [SerializeField] private bool hasExpired = false;

    void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentTime = levelDuration;
        
        // Создаём AudioSource для тиканья
        if (tickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = tickSound;
        }

        if (autoStart)
        {
            StartTimer();
        }

        // Начальное обновление UI
        onTimerUpdated?.Invoke(currentTime);
    }

    void Update()
    {
        if (!isRunning || hasExpired) return;

        // Уменьшаем время
        currentTime -= Time.deltaTime;
        
        // Обновляем UI
        onTimerUpdated?.Invoke(Mathf.Max(0f, currentTime));

        // Проверка предупреждения
        if (!hasWarned && currentTime <= warningTime)
        {
            hasWarned = true;
            onWarningStarted?.Invoke();
            Debug.Log($"[LevelTimer] ПРЕДУПРЕЖДЕНИЕ! Осталось {currentTime:F0} секунд!");
        }

        // Тиканье при предупреждении
        if (hasWarned && tickSound != null && audioSource != null)
        {
            if (Time.time - lastTickTime >= tickInterval)
            {
                audioSource.Play();
                lastTickTime = Time.time;
            }
        }

        // Проверка истечения времени
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            hasExpired = true;
            isRunning = false;

            if (pauseOnExpire)
            {
                Time.timeScale = 0f;
            }

            onTimeExpired?.Invoke();
            Debug.Log("[LevelTimer] ВРЕМЯ ВЫШЛО!");
        }
    }

    // === УПРАВЛЕНИЕ ТАЙМЕРОМ ===

    /// <summary>
    /// Запустить таймер
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        hasExpired = false;
        Debug.Log($"[LevelTimer] Таймер запущен! {levelDuration} секунд.");
    }

    /// <summary>
    /// Поставить таймер на паузу
    /// </summary>
    public void PauseTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// Возобновить таймер
    /// </summary>
    public void ResumeTimer()
    {
        if (!hasExpired)
        {
            isRunning = true;
        }
    }

    /// <summary>
    /// Сбросить таймер к начальному значению
    /// </summary>
    public void ResetTimer()
    {
        currentTime = levelDuration;
        isRunning = false;
        hasWarned = false;
        hasExpired = false;
        onTimerUpdated?.Invoke(currentTime);
    }

    /// <summary>
    /// Добавить время (бонус)
    /// </summary>
    public void AddTime(float seconds)
    {
        currentTime += seconds;
        
        // Если вышли из зоны предупреждения - сбрасываем флаг
        if (currentTime > warningTime)
        {
            hasWarned = false;
        }
        
        onTimerUpdated?.Invoke(currentTime);
        Debug.Log($"[LevelTimer] +{seconds} секунд! Теперь: {currentTime:F0}");
    }

    // === ГЕТТЕРЫ ===

    public float GetTimeRemaining() => currentTime;
    public float GetTimeRemainingNormalized() => Mathf.Clamp01(currentTime / levelDuration);
    public bool IsRunning() => isRunning;
    public bool HasExpired() => hasExpired;
    public bool IsWarning() => hasWarned && !hasExpired;

    /// <summary>
    /// Получить оставшееся время в формате MM:SS
    /// </summary>
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
