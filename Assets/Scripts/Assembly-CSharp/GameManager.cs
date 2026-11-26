using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Менеджер игры - управляет состоянием игры, смертью игрока и перезапуском.
/// Синглтон для удобного доступа из других скриптов.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Ссылки")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ThirdPersonCamera playerCamera;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;
    
    [Header("Главный HUD")]
    [Tooltip("Панель с очками/таймером/выносливостью (скрывается при Game Over)")]
    [SerializeField] private GameObject mainHUD;

    [Header("UI финального счёта")]
    [Tooltip("Текст для отображения финального счёта (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [Tooltip("Текст финального счёта для экрана победы (может ссылаться на тот же объект)")]
    [SerializeField] private TextMeshProUGUI finalScoreTextLevelComplete;

    [Header("Настройки")]
    [Tooltip("Задержка перед показом Game Over экрана")]
    public float gameOverDelay = 2.5f;
    
    [Tooltip("Задержка перед показом экрана завершения уровня")]
    public float levelCompleteDelay = 1f;

    private bool isGameOver = false;
    private bool isLevelComplete = false;

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
        // Скрываем панели при старте
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        // Автоматически находим игрока, если не назначен
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        // Автоматически находим камеру, если не назначена
        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<ThirdPersonCamera>();
        }

        // Подписываемся на событие смерти
        if (player != null)
        {
            player.onDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            Debug.LogWarning("GameManager: PlayerController не найден!");
        }

        // Подписываемся на таймер уровня
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimeExpired.AddListener(OnLevelComplete);
        }
        else
        {
            // Попробуем подписаться позже
            StartCoroutine(WaitForLevelTimer());
        }
    }

    IEnumerator WaitForLevelTimer()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimeExpired.AddListener(OnLevelComplete);
        }
    }

    /// <summary>
    /// Вызывается при смерти игрока
    /// </summary>
    void OnPlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Останавливаем таймер при смерти
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.PauseTimer();
        }

        // Активируем режим смерти камеры
        if (playerCamera != null)
        {
            playerCamera.OnPlayerDeath();
        }

        StartCoroutine(GameOverRoutine());
    }

    /// <summary>
    /// Корутина показа Game Over экрана с задержкой
    /// </summary>
    IEnumerator GameOverRoutine()
    {
        // Ждём для анимации смерти и отлёта камеры
        yield return new WaitForSeconds(gameOverDelay);

        // Скрываем главный HUD
        if (mainHUD != null)
        {
            mainHUD.SetActive(false);
        }

        // Показываем панель Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Показываем финальный счёт
        UpdateFinalScoreDisplay();

        // Опционально: остановить время
        // Time.timeScale = 0f;
    }

    /// <summary>
    /// Вызывается когда время уровня истекло
    /// </summary>
    void OnLevelComplete()
    {
        if (isGameOver || isLevelComplete) return;
        isLevelComplete = true;

        Debug.Log("[GameManager] Уровень завершён!");
        
        // Блокируем управление игроком и делаем его неуязвимым
        if (player != null)
        {
            player.SetLevelComplete();
        }
        
        // Активируем режим камеры как при смерти
        if (playerCamera != null)
        {
            playerCamera.OnPlayerDeath();
        }
        
        StartCoroutine(LevelCompleteRoutine());
    }

    /// <summary>
    /// Корутина показа экрана завершения уровня
    /// </summary>
    IEnumerator LevelCompleteRoutine()
    {
        yield return new WaitForSeconds(levelCompleteDelay);

        // Скрываем главный HUD
        if (mainHUD != null)
        {
            mainHUD.SetActive(false);
        }

        // Показываем панель завершения уровня (или Game Over панель если нет отдельной)
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        else if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Показываем финальный счёт
        UpdateFinalScoreDisplay();
    }

    /// <summary>
    /// Обновляет UI с финальным счётом
    /// </summary>
    void UpdateFinalScoreDisplay()
    {
        if (ScoreManager.Instance == null) return;

        int finalScore = ScoreManager.Instance.GetTotalScore();
        string scoreText = $"Счёт: {finalScore}";
        ApplyFinalScoreText(finalScoreText, scoreText);
        ApplyFinalScoreText(finalScoreTextLevelComplete, scoreText);

        Debug.Log($"[GameManager] Финальный счёт: {finalScore}");
    }

    void ApplyFinalScoreText(TextMeshProUGUI textMesh, string value)
    {
        if (textMesh != null)
        {
            textMesh.text = value;
        }
    }

    /// <summary>
    /// Перезапуск текущего уровня (вызывается из UI кнопки)
    /// </summary>
    public void RestartLevel()
    {
        Debug.Log("[GameManager] RestartLevel вызван!");
        
        // Восстанавливаем время если было остановлено
        Time.timeScale = 1f;
        
        // Сбрасываем флаги
        isGameOver = false;
        isLevelComplete = false;
        
        // Перезагружаем сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Загрузка главного меню (вызывается из UI кнопки)
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Предполагается что главное меню - сцена 0
    }

    /// <summary>
    /// Вызывается из кнопок "Выйти" на панелях Game Over и Победы
    /// </summary>
    public void ExitToMainMenu()
    {
        LoadMainMenu();
    }

    /// <summary>
    /// Проверка состояния игры
    /// </summary>
    public bool IsGameOver() => isGameOver;
    
    /// <summary>
    /// Проверка завершения уровня
    /// </summary>
    public bool IsLevelComplete() => isLevelComplete;

    void OnDestroy()
    {
        // Отписываемся от событий при уничтожении
        if (player != null)
        {
            player.onDeath.RemoveListener(OnPlayerDeath);
        }
        
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.onTimeExpired.RemoveListener(OnLevelComplete);
        }
    }
}
