using UnityEngine;

namespace CDB.Player.Data
{
    /// <summary>
    /// ScriptableObject с параметрами выносливости игрока.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStaminaData", menuName = "CDB/Player/Stamina Data")]
    public class PlayerStaminaData : ScriptableObject
    {
        [Header("Выносливость")]
        [Tooltip("Максимальная выносливость")]
        public float maxStamina = 100f;
        
        [Tooltip("Скорость восстановления выносливости в секунду")]
        public float regenRate = 15f;
    }
}
