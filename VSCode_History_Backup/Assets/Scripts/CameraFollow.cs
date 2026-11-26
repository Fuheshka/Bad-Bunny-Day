using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    public Transform target; // Сюда перетащить игрока

    [Header("Настройки")]
    [Tooltip("Скорость сглаживания (чем меньше, тем плавнее и медленнее)")]
    public float smoothSpeed = 5f;

    // Вектор смещения камеры относительно игрока
    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Не назначена цель (Target) для камеры!");
            return;
        }

        // Вычисляем смещение автоматически на старте.
        // Камера запомнит, как она стояла относительно игрока в редакторе.
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Целевая позиция = позиция игрока + начальное смещение
        Vector3 desiredPosition = target.position + offset;

        // Плавно перемещаем камеру (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // Камера всегда смотрит на игрока (опционально, если нужно вращение)
        // transform.LookAt(target); 
    }
}