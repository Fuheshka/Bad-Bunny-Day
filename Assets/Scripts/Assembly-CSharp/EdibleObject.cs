using UnityEngine;

public class EdibleObject : MonoBehaviour
{
    [Header("Параметры еды")]
    [Tooltip("Насколько увеличится насыщение (0.0 - 1.0)")]
    public float nutritionValue = 0.1f;
    
    [Tooltip("Сколько секунд удерживать кнопку E")]
    public float eatTime = 1.5f;

    [Tooltip("Очки за съедение")]
    public int scoreValue = 100;

    [Header("Лечение")]
    [Tooltip("Сколько HP восстановит еда (0 = не лечит)")]
    public int healAmount = 0;

    // --- НОВЫЙ КОД ДЛЯ ОБВОДКИ ---
    
    private int defaultLayer;      // Слой по умолчанию
    private int interactableLayer; // Слой для обводки
    private bool isHighlighted = false;

    void Awake()
    {
        // Запоминаем ID слоев (это быстрее, чем искать по имени каждый раз)
        defaultLayer = LayerMask.NameToLayer("Default");
        interactableLayer = LayerMask.NameToLayer("Interactable");

        // Убеждаемся, что при старте обводки нет
        SetLayerRecursively(gameObject, defaultLayer);
    }

    /// <summary>
    /// Включает обводку для этого объекта и всех его дочерних частей
    /// </summary>
    public void Highlight()
    {
        if (isHighlighted) return; // Уже подсвечен
        SetLayerRecursively(gameObject, interactableLayer);
        isHighlighted = true;
    }

    /// <summary>
    /// Выключает обводку, возвращая объект в слой по умолчанию
    /// </summary>
    public void Unhighlight()
    {
        if (!isHighlighted) return; // Уже не подсвечен
        SetLayerRecursively(gameObject, defaultLayer);
        isHighlighted = false;
    }

    /// <summary>
    /// Вспомогательный метод для рекурсивного изменения слоя
    /// </summary>
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}