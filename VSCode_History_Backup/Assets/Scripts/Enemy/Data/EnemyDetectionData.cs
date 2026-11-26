using UnityEngine;

namespace CDB.Enemy.Data
{
    /// <summary>
    /// ScriptableObject с параметрами обнаружения врага.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyDetectionData", menuName = "CDB/Enemy/Detection Data")]
    public class EnemyDetectionData : ScriptableObject
    {
        [Header("Detection")]
        [Tooltip("Время для полного обнаружения игрока")]
        public float detectionTimeNeeded = 1.5f;
        
        [Tooltip("Интервал проверки видимости")]
        public float checkInterval = 0.1f;
        
        [Tooltip("Радиус обнаружения")]
        public float detectionRadius = 8f;

        [Header("Tracking")]
        [Tooltip("Скорость преследования")]
        public float trackingSpeed = 9f;
    }
}
