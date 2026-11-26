using UnityEngine;

namespace CDB.Player.Data
{
    /// <summary>
    /// ScriptableObject с параметрами движения игрока.
    /// Позволяет настраивать параметры без перекомпиляции.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementData", menuName = "CDB/Player/Movement Data")]
    public class PlayerMovementData : ScriptableObject
    {
        [Header("Базовое движение")]
        [Tooltip("Базовая скорость передвижения")]
        public float baseSpeed = 8f;
        
        [Tooltip("Множитель минимальной скорости при максимальной сытости")]
        [Range(0.1f, 1f)] public float minSpeedMultiplier = 0.3f;

        [Header("Прыжок")]
        [Tooltip("Сила прыжка")]
        public float jumpForce = 7f;
        
        [Tooltip("Дистанция проверки земли")]
        public float groundCheckDistance = 1.1f;

        [Header("Рывок (Dash)")]
        [Tooltip("Сила рывка")]
        public float dashForce = 15f;
        
        [Tooltip("Стоимость рывка в выносливости")]
        public float dashCost = 30f;
        
        [Tooltip("Длительность рывка")]
        public float dashDuration = 0.2f;

        [Header("Game Feel")]
        [Tooltip("Время после схода с платформы, когда ещё можно прыгнуть")]
        public float coyoteTime = 0.15f;
        
        [Tooltip("Время буферизации нажатия прыжка")]
        public float jumpBufferTime = 0.12f;
        
        [Tooltip("Время буферизации нажатия рывка")]
        public float dashBufferTime = 0.1f;
        
        [Tooltip("Множитель обрезки прыжка при отпускании кнопки")]
        [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.4f;

        [Header("Corner Sliding")]
        [Tooltip("Дистанция проверки углов для скольжения")]
        public float cornerCheckDistance = 0.5f;
        
        [Tooltip("Сила смещения при скольжении по углам")]
        public float cornerSlideForce = 3f;
    }
}
