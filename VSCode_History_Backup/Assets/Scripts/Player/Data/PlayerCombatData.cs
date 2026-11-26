using UnityEngine;

namespace CDB.Player.Data
{
    /// <summary>
    /// ScriptableObject с параметрами здоровья и боя игрока.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerCombatData", menuName = "CDB/Player/Combat Data")]
    public class PlayerCombatData : ScriptableObject
    {
        [Header("Здоровье")]
        [Tooltip("Максимальное здоровье")]
        public int maxHealth = 3;
        
        [Tooltip("Длительность неуязвимости после урона")]
        public float invincibilityDuration = 1.5f;
        
        [Tooltip("Сила отталкивания при получении урона")]
        public float knockbackForce = 10f;

        [Header("Жировой щит")]
        [Tooltip("Порог насыщения для жирового щита (0-1)")]
        [Range(0f, 1f)] public float fatShieldThreshold = 0.5f;
        
        [Tooltip("Множитель скорости после сброса жира")]
        public float fatShieldSpeedBoost = 1.5f;
        
        [Tooltip("Длительность бонуса скорости")]
        public float fatShieldBoostDuration = 1f;
        
        [Tooltip("Какой процент очков из 'живота' теряется при получении урона (0.0 - 1.0)")]
        [Range(0f, 1f)] public float fatShieldScoreLossPercent = 0.5f;
    }
}
