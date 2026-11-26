using UnityEngine;

namespace CDB.Enemy.Data
{
    /// <summary>
    /// ScriptableObject с параметрами патрулирования врага.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyPatrolData", menuName = "CDB/Enemy/Patrol Data")]
    public class EnemyPatrolData : ScriptableObject
    {
        [Header("Patrol Rectangle Area")]
        public Vector3 patrolRectCenter = Vector3.zero;
        public Vector2 patrolRectHalfSize = new Vector2(20f, 15f);

        [Header("Patrol Circle Area")]
        public Vector3 patrolAreaCenter = Vector3.zero;
        public float patrolAreaRadius = 45f;

        [Header("Patrol Movement")]
        public float patrolSpeed = 3f;
        public float patrolPause = 2f;

        [Header("Search")]
        public float searchDuration = 5f;
        public float searchRadius = 6f;
        public float searchMoveSpeed = 4f;
        public float searchPointInterval = 1.2f;
    }
}
