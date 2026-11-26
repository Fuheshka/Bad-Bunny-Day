using UnityEngine;

namespace CDB.Player.Data
{
    /// <summary>
    /// ScriptableObject с параметрами системы веса/сытости игрока.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerWeightData", menuName = "CDB/Player/Weight Data")]
    public class PlayerWeightData : ScriptableObject
    {
        [Header("Система веса")]
        [Tooltip("Максимальный множитель размера при полной сытости")]
        public float maxScaleMultiplier = 1.5f;
        
        [Tooltip("Множитель массы при полной сытости")]
        public float massMultiplier = 5f;
        
        [Tooltip("Время плавного увеличения размера")]
        public float scaleSmoothDuration = 0.3f;
    }
}
