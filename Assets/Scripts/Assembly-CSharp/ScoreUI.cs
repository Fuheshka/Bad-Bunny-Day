using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// UI компонент для отображения очков.
/// Показывает общий счёт и временные очки в животе.
/// Формат: "1500 (+300)"
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    [Tooltip("Текст для общего счёта (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    
    [Tooltip("Текст для временных очков в животе (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI bellyScoreText;
    
    [Tooltip("Альтернатива: обычный UI Text для общего счёта")]
    [SerializeField] private Text totalScoreTextLegacy;
    
    [Tooltip("Альтернатива: обычный UI Text для временных очков")]
    [SerializeField] private Text bellyScoreTextLegacy;

    [Header("Настройки отображения")]
    [Tooltip("Префикс перед временными очками")]
    public string bellyPrefix = "(+";
    
    [Tooltip("Суффикс после временных очков")]
    public string bellySuffix = ")";
    
    [Tooltip("Скрывать belly score когда = 0")]
    public bool hideBellyWhenEmpty = true;

    [Header("Анимация")]
    [Tooltip("Анимировать изменение очков")]
    public bool animateChanges = true;
    
    [Tooltip("Длительность анимации изменения")]
    public float animationDuration = 0.3f;
    
    [Tooltip("Масштаб при анимации")]
    public float animationScale = 1.3f;

    [Header("Цвета")]
    [Tooltip("Цвет временных очков")]
    public Color bellyScoreColor = new Color(0.2f, 1f, 0.4f); // Зелёный
    
    [Tooltip("Цвет при потере очков")]
    public Color lostScoreColor = new Color(1f, 0.3f, 0.3f); // Красный

    private int lastTotalScore = 0;
    private int lastBellyScore = 0;
    private Coroutine bellyAnimationCoroutine;

    void Start()
    {
        // Подписываемся на события ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.AddListener(UpdateScoreDisplay);
            ScoreManager.Instance.onScoreBanked.AddListener(OnScoreBanked);
            ScoreManager.Instance.onBellyScoreLost.AddListener(OnBellyScoreLost);
            
            // Начальное обновление
            UpdateScoreDisplay(ScoreManager.Instance.GetTotalScore(), ScoreManager.Instance.GetBellyScore());
        }
        else
        {
            Debug.LogWarning("ScoreUI: ScoreManager.Instance не найден! Подождите инициализации.");
            StartCoroutine(WaitForScoreManager());
        }

        // Устанавливаем цвет belly текста
        SetBellyTextColor(bellyScoreColor);
    }

    IEnumerator WaitForScoreManager()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.AddListener(UpdateScoreDisplay);
            ScoreManager.Instance.onScoreBanked.AddListener(OnScoreBanked);
            ScoreManager.Instance.onBellyScoreLost.AddListener(OnBellyScoreLost);
            UpdateScoreDisplay(ScoreManager.Instance.GetTotalScore(), ScoreManager.Instance.GetBellyScore());
        }
    }

    /// <summary>
    /// Обновление отображения счёта
    /// </summary>
    void UpdateScoreDisplay(int totalScore, int bellyScore)
    {
        // === ОБЩИЙ СЧЁТ ===
        string totalText = totalScore.ToString();
        
        if (totalScoreText != null)
        {
            totalScoreText.text = totalText;
        }
        if (totalScoreTextLegacy != null)
        {
            totalScoreTextLegacy.text = totalText;
        }

        // Анимация если счёт изменился
        if (animateChanges && totalScore != lastTotalScore)
        {
            AnimateTotalScore();
        }
        lastTotalScore = totalScore;

        // === ВРЕМЕННЫЕ ОЧКИ (BELLY) ===
        bool showBelly = bellyScore > 0 || !hideBellyWhenEmpty;
        string bellyText = showBelly ? $"{bellyPrefix}{bellyScore}{bellySuffix}" : "";
        
        if (bellyScoreText != null)
        {
            bellyScoreText.text = bellyText;
            bellyScoreText.gameObject.SetActive(showBelly);
        }
        if (bellyScoreTextLegacy != null)
        {
            bellyScoreTextLegacy.text = bellyText;
            bellyScoreTextLegacy.gameObject.SetActive(showBelly);
        }

        // Анимация belly если изменилось
        if (animateChanges && bellyScore != lastBellyScore && bellyScore > lastBellyScore)
        {
            AnimateBellyScore();
        }
        lastBellyScore = bellyScore;
    }

    /// <summary>
    /// Вызывается при успешном банкинге
    /// </summary>
    void OnScoreBanked(int amount)
    {
        // Можно добавить специальную анимацию банкинга
        AnimateTotalScore();
        Debug.Log($"[ScoreUI] Забанкено: {amount}");
    }

    /// <summary>
    /// Вызывается при потере очков (смерть)
    /// </summary>
    void OnBellyScoreLost(int amount)
    {
        if (amount <= 0) return;
        
        // Показываем потерянные очки красным
        SetBellyTextColor(lostScoreColor);
        
        // Возвращаем нормальный цвет через секунду
        StartCoroutine(ResetBellyColor());
    }

    IEnumerator ResetBellyColor()
    {
        yield return new WaitForSeconds(1f);
        SetBellyTextColor(bellyScoreColor);
    }

    void SetBellyTextColor(Color color)
    {
        if (bellyScoreText != null)
        {
            bellyScoreText.color = color;
        }
        if (bellyScoreTextLegacy != null)
        {
            bellyScoreTextLegacy.color = color;
        }
    }

    // === АНИМАЦИИ ===

    void AnimateTotalScore()
    {
        if (totalScoreText != null)
        {
            StartCoroutine(PunchScale(totalScoreText.transform));
        }
        else if (totalScoreTextLegacy != null)
        {
            StartCoroutine(PunchScale(totalScoreTextLegacy.transform));
        }
    }

    void AnimateBellyScore()
    {
        if (bellyAnimationCoroutine != null)
        {
            StopCoroutine(bellyAnimationCoroutine);
        }

        Transform target = null;
        if (bellyScoreText != null) target = bellyScoreText.transform;
        else if (bellyScoreTextLegacy != null) target = bellyScoreTextLegacy.transform;

        if (target != null)
        {
            bellyAnimationCoroutine = StartCoroutine(PunchScale(target));
        }
    }

    IEnumerator PunchScale(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = originalScale * animationScale;

        float elapsed = 0f;
        float halfDuration = animationDuration / 2f;

        // Увеличиваем
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            target.localScale = Vector3.Lerp(originalScale, punchScale, t);
            yield return null;
        }

        // Уменьшаем обратно
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            target.localScale = Vector3.Lerp(punchScale, originalScale, t);
            yield return null;
        }

        target.localScale = originalScale;
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.RemoveListener(UpdateScoreDisplay);
            ScoreManager.Instance.onScoreBanked.RemoveListener(OnScoreBanked);
            ScoreManager.Instance.onBellyScoreLost.RemoveListener(OnBellyScoreLost);
        }
    }
}
