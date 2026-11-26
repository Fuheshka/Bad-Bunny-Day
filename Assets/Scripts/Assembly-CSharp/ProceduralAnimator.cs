using UnityEngine;

/// <summary>
/// Процедурные Squash & Stretch эффекты для прыжка, падения, рывка и еды.
/// Работает ПОВЕРХ обычного Animator - дополняет его, не заменяет.
/// Повесьте этот компонент на дочерний объект с мешем/моделью персонажа!
/// </summary>
public class ProceduralAnimator : MonoBehaviour
{
    [Header("Настройки Squash & Stretch")]
    [Tooltip("Сила эффекта (0 = выкл, 1 = максимум)")]
    [Range(0f, 1f)]
    public float intensity = 0.8f;  // Увеличил по умолчанию
    
    [Tooltip("Скорость возврата к нормальной форме")]
    public float returnSpeed = 8f;  // Чуть медленнее, чтобы эффект был виден дольше

    [Header("Прыжок")]
    public float jumpSquash = 0.5f;      // Сжатие перед прыжком (сильнее)
    public float jumpStretch = 1.5f;     // Растяжение в воздухе (сильнее)

    [Header("Приземление")]
    public float landSquash = 0.4f;      // Сжатие при приземлении (сильнее)

    [Header("Падение")]
    public float fallStretch = 1.4f;     // Растяжение при падении (сильнее)

    [Header("Рывок")]
    public float dashStretch = 1.6f;     // Растяжение вперёд (сильнее)
    public float dashSquash = 0.5f;      // Сужение по бокам (сильнее)

    [Header("Еда")]
    public float eatBobSpeed = 8f;       // Скорость покачивания
    public float eatBobAmount = 0.15f;   // Амплитуда покачивания (больше)
    public float eatPulse = 0.15f;       // Пульсация при жевании (больше)

    // Внутренние переменные
    private Vector3 baseScale;
    private Vector3 targetScale;
    private Vector3 currentScale;
    private Vector3 basePosition;
    private float eatTimer = 0f;

    void Start()
    {
        baseScale = transform.localScale;
        basePosition = transform.localPosition;
        currentScale = baseScale;
        targetScale = baseScale;
    }

    void LateUpdate()
    {
        // Плавная интерполяция к целевому масштабу
        currentScale = Vector3.Lerp(currentScale, targetScale, returnSpeed * Time.deltaTime);
        transform.localScale = currentScale;
    }

    /// <summary>
    /// Сжатие перед прыжком
    /// </summary>
    public void AnimateJumpStart()
    {
        float squash = Mathf.Lerp(1f, jumpSquash, intensity);
        float stretch = Mathf.Lerp(1f, 1f / jumpSquash, intensity);
        
        currentScale = new Vector3(
            baseScale.x * stretch,
            baseScale.y * squash,
            baseScale.z * stretch
        );
        targetScale = baseScale;
    }

    /// <summary>
    /// Растяжение в воздухе (прыжок вверх)
    /// </summary>
    public void AnimateJumpAir()
    {
        float stretch = Mathf.Lerp(1f, jumpStretch, intensity);
        float squash = Mathf.Lerp(1f, 1f / jumpStretch, intensity);
        
        targetScale = new Vector3(
            baseScale.x * squash,
            baseScale.y * stretch,
            baseScale.z * squash
        );
    }

    /// <summary>
    /// Растяжение при падении
    /// </summary>
    public void AnimateFall(float fallSpeed)
    {
        float speedFactor = Mathf.Clamp01(Mathf.Abs(fallSpeed) / 15f);
        float stretch = Mathf.Lerp(1f, fallStretch, intensity * speedFactor);
        float squash = Mathf.Lerp(1f, 1f / fallStretch, intensity * speedFactor);
        
        targetScale = new Vector3(
            baseScale.x * squash,
            baseScale.y * stretch,
            baseScale.z * squash
        );
    }

    /// <summary>
    /// Сжатие при приземлении
    /// </summary>
    public void AnimateLand(float impactSpeed)
    {
        float speedFactor = Mathf.Clamp01(Mathf.Abs(impactSpeed) / 15f);
        float squash = Mathf.Lerp(1f, landSquash, intensity * speedFactor);
        float stretch = Mathf.Lerp(1f, 1f / landSquash, intensity * speedFactor);
        
        currentScale = new Vector3(
            baseScale.x * stretch,
            baseScale.y * squash,
            baseScale.z * stretch
        );
        targetScale = baseScale;
    }

    /// <summary>
    /// Растяжение при рывке
    /// </summary>
    public void AnimateDash(Vector3 direction)
    {
        float stretch = Mathf.Lerp(1f, dashStretch, intensity);
        float squash = Mathf.Lerp(1f, dashSquash, intensity);
        
        currentScale = new Vector3(
            baseScale.x * squash,
            baseScale.y * squash,
            baseScale.z * stretch
        );
        targetScale = currentScale;
    }

    /// <summary>
    /// Окончание рывка
    /// </summary>
    public void AnimateDashEnd()
    {
        targetScale = baseScale;
    }

    /// <summary>
    /// Анимация поедания
    /// </summary>
    public void AnimateEating()
    {
        eatTimer += Time.deltaTime * eatBobSpeed;
        
        float pulse = Mathf.Sin(eatTimer) * eatPulse * intensity;
        
        targetScale = new Vector3(
            baseScale.x * (1f + pulse),
            baseScale.y * (1f - pulse * 0.5f),
            baseScale.z * (1f + pulse)
        );

        float bob = (Mathf.Sin(eatTimer) * 0.5f + 0.5f) * eatBobAmount * intensity;
        transform.localPosition = basePosition + new Vector3(0, -bob, 0);
    }

    /// <summary>
    /// Окончание еды
    /// </summary>
    public void AnimateEatEnd()
    {
        targetScale = baseScale;
        transform.localPosition = basePosition;
        eatTimer = 0f;
    }

    /// <summary>
    /// Сброс к нормальному состоянию
    /// </summary>
    public void ResetToIdle()
    {
        targetScale = baseScale;
        transform.localPosition = basePosition;
    }

    /// <summary>
    /// Обновить базовый масштаб (при изменении размера из-за сытости)
    /// </summary>
    public void UpdateBaseScale(Vector3 newScale)
    {
        baseScale = newScale;
    }

    /// <summary>
    /// Анимация получения урона (тряска + сжатие)
    /// </summary>
    public void AnimateDamage()
    {
        float shake = intensity * 0.3f;
        
        // Случайное смещение для эффекта тряски
        float randomX = Random.Range(-shake, shake);
        float randomZ = Random.Range(-shake, shake);
        
        currentScale = new Vector3(
            baseScale.x * (1f + randomX),
            baseScale.y * (1f - intensity * 0.2f), // Небольшое сжатие
            baseScale.z * (1f + randomZ)
        );
        targetScale = baseScale;
    }
}
