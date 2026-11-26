using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoldInteractionUI : MonoBehaviour
{
    // Ссылка на сам Image, который будет заполняться
    private Image indicatorImage;
    
    // Ссылка на цель, за которой нужно следовать (наш Заяц)
    private Transform targetToFollow;
    
    // Высота, на которой будет висеть индикатор
    private float heightOffset;

    void Awake()
    {
        indicatorImage = GetComponent<Image>();
    }

    void Update()
    {
        // Если есть цель - обновляем позицию на экране
        if (targetToFollow != null)
        {
            // Берем 3D позицию над головой
            Vector3 worldPos = targetToFollow.position + Vector3.up * heightOffset;
            
            // Превращаем в 2D-координаты
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            // Устанавливаем позицию
            transform.position = screenPos;
        }
    }

    /// <summary>
    /// Начать отображение индикатора
    /// </summary>
    /// <param name="target">За кем следовать</param>
    /// <param name="offset">Высота над целью</param>
    public void StartTracking(Transform target, float offset)
    {
        targetToFollow = target;
        heightOffset = offset;
        indicatorImage.fillAmount = 0;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Обновить прогресс заполнения
    /// </summary>
    /// <param name="progress">Значение от 0.0 до 1.0</param>
    public void UpdateProgress(float progress)
    {
        if (gameObject.activeSelf)
        {
            indicatorImage.fillAmount = progress;
        }
    }

    /// <summary>
    /// Скрыть индикатор
    /// </summary>
    public void Hide()
    {
        targetToFollow = null;
        indicatorImage.fillAmount = 0;
        gameObject.SetActive(false);
    }
}