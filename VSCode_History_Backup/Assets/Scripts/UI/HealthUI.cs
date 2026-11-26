using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI компонент для отображения здоровья игрока в виде сердец/иконок.
/// Подписывается на событие изменения здоровья PlayerController.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private PlayerController player;
    
    [Header("UI Элементы")]
    [Tooltip("Массив Image компонентов для отображения сердец (от первого к последнему)")]
    [SerializeField] private Image[] heartImages;
    
    [Header("Спрайты")]
    [Tooltip("Спрайт полного сердца")]
    [SerializeField] private Sprite fullHeartSprite;
    [Tooltip("Спрайт пустого сердца")]
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Настройки")]
    [Tooltip("Скрывать пустые сердца вместо показа пустого спрайта")]
    public bool hideEmptyHearts = false;

    void Start()
    {
        // Автоматически находим игрока, если не назначен
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (player != null)
        {
            // Подписываемся на изменение здоровья
            player.onHealthChanged.AddListener(UpdateHealthDisplay);
            
            // Инициализируем отображение
            UpdateHealthDisplay(player.GetCurrentHealth(), player.maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthUI: PlayerController не найден!");
        }
    }

    /// <summary>
    /// Обновляет отображение сердец
    /// </summary>
    /// <param name="currentHealth">Текущее здоровье</param>
    /// <param name="maxHealth">Максимальное здоровье</param>
    void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            // Определяем, должно ли сердце быть активным
            bool isWithinMax = i < maxHealth;
            bool isFilled = i < currentHealth;

            if (hideEmptyHearts)
            {
                // Режим скрытия: показываем только заполненные сердца
                heartImages[i].gameObject.SetActive(isFilled);
            }
            else
            {
                // Режим спрайтов: показываем все сердца до maxHealth
                heartImages[i].gameObject.SetActive(isWithinMax);
                
                if (isWithinMax)
                {
                    // Меняем спрайт в зависимости от заполненности
                    if (isFilled && fullHeartSprite != null)
                    {
                        heartImages[i].sprite = fullHeartSprite;
                    }
                    else if (!isFilled && emptyHeartSprite != null)
                    {
                        heartImages[i].sprite = emptyHeartSprite;
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // Отписываемся от события
        if (player != null)
        {
            player.onHealthChanged.RemoveListener(UpdateHealthDisplay);
        }
    }
}
