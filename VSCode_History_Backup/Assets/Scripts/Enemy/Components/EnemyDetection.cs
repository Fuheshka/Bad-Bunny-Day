using UnityEngine;
using CDB.Core;

namespace CDB.Enemy.Components
{
    /// <summary>
    /// Компонент обнаружения игрока.
    /// Отвечает за проверку видимости и расстояния до цели.
    /// </summary>
    public class EnemyDetection : MonoBehaviour
    {
        [Header("Данные")]
        [SerializeField] private Data.EnemyDetectionData _detectionData;

        [Header("Ссылки")]
        [SerializeField] private Transform _target;

        [Header("Слои")]
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private LayerMask _playerMask;

        // Состояния
        private float _detectionTimer;
        private bool _canSeeTarget;
        private Vector3 _lastKnownPosition;

        public bool CanSeeTarget => _canSeeTarget;
        public float DetectionProgress => _detectionTimer / (_detectionData != null ? _detectionData.detectionTimeNeeded : 1f);
        public bool IsFullyDetected => _detectionTimer >= (_detectionData != null ? _detectionData.detectionTimeNeeded : 1f);
        public Vector3 LastKnownPosition => _lastKnownPosition;
        public Transform Target => _target;

        private void Start()
        {
            if (_detectionData != null)
            {
                InvokeRepeating(nameof(CheckLineOfSight), 0.05f, _detectionData.checkInterval);
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void CheckLineOfSight()
        {
            if (_target == null || _detectionData == null)
            {
                _canSeeTarget = false;
                return;
            }

            // Проверяем видимость
            Vector3 from = transform.position + Vector3.up * 0.5f;
            Vector3 targetCenter = _target.position + Vector3.up * 0.5f;
            Vector3 dir = (targetCenter - from).normalized;
            float dist = Vector3.Distance(from, targetCenter);

            // Проверяем дистанцию
            if (dist > _detectionData.detectionRadius)
            {
                _canSeeTarget = false;
                return;
            }

            // Проверяем препятствия
            if (Physics.Raycast(from, dir, out RaycastHit hit, dist, _obstacleMask))
            {
                _canSeeTarget = false;
            }
            else
            {
                _canSeeTarget = true;
                _lastKnownPosition = GetTargetGroundPosition();
            }
        }

        /// <summary>
        /// Обновить таймер обнаружения
        /// </summary>
        public void UpdateDetectionTimer(bool seesTarget, float deltaTime)
        {
            if (_detectionData == null) return;
            
            if (seesTarget)
            {
                _detectionTimer += deltaTime;
            }
            else
            {
                _detectionTimer = Mathf.Max(0f, _detectionTimer - deltaTime * 3f);
            }
        }

        /// <summary>
        /// Сбросить таймер обнаружения
        /// </summary>
        public void ResetDetectionTimer()
        {
            _detectionTimer = 0f;
        }

        /// <summary>
        /// Получить позицию цели на земле
        /// </summary>
        public Vector3 GetTargetGroundPosition()
        {
            if (_target == null) return _lastKnownPosition;
            return new Vector3(_target.position.x, 0.05f, _target.position.z);
        }

        /// <summary>
        /// Проверить, жива ли цель (если это игрок)
        /// </summary>
        public bool IsTargetAlive()
        {
            if (_target == null) return false;
            
            var damageable = _target.GetComponent<IDamageable>();
            return damageable == null || damageable.IsAlive();
        }
    }
}
