using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Система навигации к сундуку (BankZone).
/// Стрелка появляется, когда у игрока есть очки в животе.
/// Указывает на сундук и остаётся на краю экрана, если тот за кадром.
/// С поддержкой спрайт-листа для анимации.
/// </summary>
public class Waypoint : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Объект, на который нужно указывать (BankZone/Сундук)")]
    public Transform target;
    
    [Tooltip("UI Image со стрелкой")]
    public Image arrowImage;
    
    [Tooltip("UI Image с иконкой сундука (опционально, показывается когда сундук на экране)")]
    public Image iconImage;

    [Header("Анимация")]
    [Tooltip("Спрайты для анимации стрелки (из спрайт-листа). Заполни массив спрайтами")]
    public Sprite[] arrowAnimationFrames;
    
    [Tooltip("Скорость анимации (кадров в секунду)")]
    public float animationSpeed = 10f;
    
    [Tooltip("Включить автоматическую анимацию")]
    public bool enableAnimation = true;

    [Header("Настройки")]
    [Tooltip("На каком расстоянии от края экрана будет стрелка")]
    public float screenBorderPadding = 50f;
    
    [Tooltip("Проверять наличие очков и скрывать стрелку если их нет")]
    public bool hideWhenNoScore = true;

    private Camera mainCamera;
    private RectTransform arrowRectTransform;
    private RectTransform iconRectTransform;
    
    // Анимация
    private float animationTimer = 0f;
    private int currentFrameIndex = 0;

    void Start()
    {
        mainCamera = Camera.main;
        
        if (arrowImage)
        {
            arrowRectTransform = arrowImage.GetComponent<RectTransform>();
            arrowImage.gameObject.SetActive(false);
        }
        
        if (iconImage)
        {
            iconRectTransform = iconImage.GetComponent<RectTransform>();
            iconImage.gameObject.SetActive(false);
        }

        if (target == null)
        {
            Debug.LogError("[Waypoint] Target не назначен!");
        }

        if (enableAnimation && (arrowAnimationFrames == null || arrowAnimationFrames.Length == 0))
        {
            Debug.LogWarning("[Waypoint] Анимация включена, но спрайты не загружены!");
        }
    }

    void Update()
    {
        if (target == null || mainCamera == null) return;

        // Обновляем анимацию
        if (enableAnimation && arrowAnimationFrames != null && arrowAnimationFrames.Length > 0)
        {
            UpdateAnimation();
        }

        // Проверяем, есть ли у игрока очки в животе
        bool hasScore = ScoreManager.Instance != null && ScoreManager.Instance.GetBellyScore() > 0;

        if (!hasScore)
        {
            // Если очков нет - скрываем всё и выходим
            if (arrowImage && arrowImage.gameObject.activeSelf) arrowImage.gameObject.SetActive(false);
            if (iconImage && iconImage.gameObject.activeSelf) iconImage.gameObject.SetActive(false);
            return;
        }

        // --- ЛОГИКА ОТОБРАЖЕНИЯ ---
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(target.position);

        // Проверяем, находится ли цель на экране
        bool isOffScreen = targetScreenPos.x < 0 || targetScreenPos.x > Screen.width ||
                           targetScreenPos.y < 0 || targetScreenPos.y > Screen.height ||
                           targetScreenPos.z < 0; // z < 0 значит, что цель за спиной

        // DEBUG
        Debug.Log($"[Waypoint] Target World: {target.position}, Screen: ({targetScreenPos.x:F0}, {targetScreenPos.y:F0}, {targetScreenPos.z:F2}), OffScreen: {isOffScreen}");

        if (isOffScreen)
        {
            // ЦЕЛЬ ЗА ЭКРАНОМ - показываем стрелку
            if (iconImage) iconImage.gameObject.SetActive(false);
            if (arrowImage) arrowImage.gameObject.SetActive(true);

            UpdateArrowPosition(targetScreenPos);
        }
        else
        {
            // ЦЕЛЬ НА ЭКРАНЕ - показываем иконку (если есть)
            if (arrowImage) arrowImage.gameObject.SetActive(false);
            if (iconImage)
            {
                iconImage.gameObject.SetActive(true);
                iconRectTransform.position = targetScreenPos;
            }
        }
    }

    /// <summary>
    /// Обновляет текущий кадр анимации
    /// </summary>
    void UpdateAnimation()
    {
        animationTimer += Time.deltaTime;
        
        // Вычисляем индекс кадра
        float frameDuration = 1f / animationSpeed;
        currentFrameIndex = (int)(animationTimer / frameDuration) % arrowAnimationFrames.Length;
        
        // Применяем спрайт
        if (arrowImage != null)
        {
            arrowImage.sprite = arrowAnimationFrames[currentFrameIndex];
        }
    }

    /// <summary>
    /// Обновляет позицию и поворот стрелки на краю экрана
    /// </summary>
    void UpdateArrowPosition(Vector3 targetScreenPos)
    {
        // Находим центр экрана
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Вычисляем направление к цели
        Vector3 direction = targetScreenPos - screenCenter;
        
        // Если цель за спиной (z < 0), отражаем направление вперед через центр
        if (targetScreenPos.z < 0)
        {
            direction = -direction;
        }

        // Нормализуем направление для углового расчета
        Vector3 normalizedDirection = direction.normalized;

        // Угол поворота стрелки - конвертируем из направления вектора
        // Atan2(y, x) дает угол в градусах, где 0° = вправо, 90° = вверх
        // Спрайт стрелки по умолчанию смотрит вверх, поэтому вычитаем 90°
        float arrowAngle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg - 90f;
        
        // Устанавливаем угол напрямую, без интерполяции
        arrowRectTransform.localEulerAngles = new Vector3(0, 0, arrowAngle);

        // Вычисляем точку пересечения с границей экрана
        float extentX = Screen.width / 2f - screenBorderPadding;
        float extentY = Screen.height / 2f - screenBorderPadding;
        
        Vector3 arrowPos = screenCenter;

        // Обрабатываем оба диагональных направления
        if (Mathf.Abs(normalizedDirection.x) > 0.01f && Mathf.Abs(normalizedDirection.y) > 0.01f)
        {
            float slope = normalizedDirection.y / normalizedDirection.x;
            
            // Попробуем пересечение с горизонтальными границами (верх/низ)
            float intersectY = extentY * Mathf.Sign(normalizedDirection.y);
            float intersectX = intersectY / slope;
            
            if (Mathf.Abs(intersectX) <= extentX)
            {
                // Пересечение с верхом/низом
                arrowPos.x = screenCenter.x + intersectX;
                arrowPos.y = screenCenter.y + intersectY;
            }
            else
            {
                // Пересечение с левой/правой стороной
                intersectX = extentX * Mathf.Sign(normalizedDirection.x);
                intersectY = intersectX * slope;
                
                arrowPos.x = screenCenter.x + intersectX;
                arrowPos.y = screenCenter.y + intersectY;
            }
        }
        else if (Mathf.Abs(normalizedDirection.x) > 0.01f)
        {
            // Только горизонтальное направление
            arrowPos.x = screenCenter.x + extentX * Mathf.Sign(normalizedDirection.x);
            arrowPos.y = screenCenter.y;
        }
        else if (Mathf.Abs(normalizedDirection.y) > 0.01f)
        {
            // Только вертикальное направление
            arrowPos.x = screenCenter.x;
            arrowPos.y = screenCenter.y + extentY * Mathf.Sign(normalizedDirection.y);
        }

        arrowRectTransform.position = arrowPos;
    }
}
