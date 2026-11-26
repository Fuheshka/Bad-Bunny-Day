using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Менеджер очков - реализует механику "Risk vs Reward".
/// Очки в животе (BellyScore) - временные, теряются при смерти.
/// Общий счёт (TotalScore) - сохраняется при банкинге в норе.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Текущие очки")]
    [SerializeField] private int totalScore = 0;
    [SerializeField] private int bellyScore = 0;

    [Header("События")]
    [Tooltip("Вызывается при изменении очков: (total, belly)")]
    public UnityEvent<int, int> onScoreChanged;
    
    [Tooltip("Вызывается при банкинге очков: (bankedAmount)")]
    public UnityEvent<int> onScoreBanked;
    
    [Tooltip("Вызывается при потере очков из живота")]
    public UnityEvent<int> onBellyScoreLost;

    [Header("Ссылки")]
    [SerializeField] private PlayerController player;

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
        // Автоматически находим игрока, если не назначен
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        // Подписываемся на смерть игрока - теряем belly score
        if (player != null)
        {
            player.onDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            Debug.LogWarning("ScoreManager: PlayerController не найден!");
        }

        // Начальное обновление UI
        onScoreChanged?.Invoke(totalScore, bellyScore);
    }

    /// <summary>
    /// Добавить очки в живот (временные очки).
    /// Вызывается при поедании еды.
    /// </summary>
    public void AddBellyScore(int amount)
    {
        if (amount <= 0) return;
        
        bellyScore += amount;
        Debug.Log($"[ScoreManager] +{amount} в живот. Belly: {bellyScore}, Total: {totalScore}");
        UpdateBellyScoreUI();
    }

    /// <summary>
    /// Перевести очки из живота в общий счёт (банкинг).
    /// Вызывается при входе в нору.
    /// </summary>
    public void BankBellyScore()
    {
        if (bellyScore <= 0) return;

        int bankedAmount = bellyScore;
        totalScore += bellyScore;
        bellyScore = 0;
        
        Debug.Log($"[ScoreManager] БАНКИНГ! +{bankedAmount}. Total: {totalScore}");
        onScoreBanked?.Invoke(bankedAmount);
        onScoreChanged?.Invoke(totalScore, bellyScore);
    }

    /// <summary>
    /// Потерять все очки в животе (при смерти).
    /// </summary>
    public void LoseBellyScore()
    {

        if (bellyScore <= 0) return;

        int lostAmount = bellyScore;
        bellyScore = 0;
        
        Debug.Log($"[ScoreManager] ПОТЕРЯ! -{lostAmount}. Total: {totalScore}");
        onBellyScoreLost?.Invoke(lostAmount);
        onScoreChanged?.Invoke(totalScore, bellyScore);
    }

    /// <summary>
    /// Отнимает очки из живота (при попадании в жировой щит).
    /// </summary>
    public void RemoveBellyScore(int amount)
    {
        if (amount <= 0) return;

        int lostAmount = Mathf.Min(bellyScore, amount);
        if (lostAmount <= 0) return;

        bellyScore -= lostAmount;
        Debug.Log($"[ScoreManager] Жир потерян! -{lostAmount}. Belly: {bellyScore}");
        onBellyScoreLost?.Invoke(lostAmount);
        UpdateBellyScoreUI();
    }

    /// <summary>
    /// Вызывается при смерти игрока.
    /// </summary>
    void OnPlayerDeath()
    {
        LoseBellyScore();
    }

    void UpdateBellyScoreUI()
    {
        onScoreChanged?.Invoke(totalScore, bellyScore);
    }

    // === ГЕТТЕРЫ ===
    
    public int GetTotalScore() => totalScore;
    public int GetBellyScore() => bellyScore;
    public int GetCombinedScore() => totalScore + bellyScore;
    public bool HasBellyScore() => bellyScore > 0;

    void OnDestroy()
    {
        if (player != null)
        {
            player.onDeath.RemoveListener(OnPlayerDeath);
        }
    }
}
