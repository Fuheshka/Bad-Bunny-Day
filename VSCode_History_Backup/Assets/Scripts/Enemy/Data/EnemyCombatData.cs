using UnityEngine;

namespace CDB.Enemy.Data
{
    /// <summary>
    /// ScriptableObject с параметрами атаки врага.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyCombatData", menuName = "CDB/Enemy/Combat Data")]
    public class EnemyCombatData : ScriptableObject
    {
        [Header("Shooting")]
        [Tooltip("Время перезарядки между выстрелами")]
        public float fireCooldown = 2f;
        
        [Tooltip("Время наведения")]
        public float chargeTime = 0.5f;
        
        [Tooltip("Время блокировки цели перед выстрелом")]
        public float lockTime = 0.3f;
        
        [Tooltip("Радиус попадания")]
        public float shotHitRadius = 2.46f;

        [Header("Damage")]
        [Tooltip("Урон от выстрела")]
        public int damage = 1;
    }
}
