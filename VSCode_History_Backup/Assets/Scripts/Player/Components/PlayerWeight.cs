using UnityEngine;
using UnityEngine.Events;

namespace CDB.Player.Components
{
    /// <summary>
    /// Компонент системы веса/сытости игрока.
    /// Отвечает за масштабирование персонажа и изменение его характеристик.
    /// </summary>
    public class PlayerWeight : MonoBehaviour
    {
        [Header("Данные")]
        [SerializeField] private Data.PlayerWeightData _weightData;

        [Header("Ссылки")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private ProceduralAnimator _proceduralAnimator;

        [Header("События")]
        public UnityEvent<float> OnSaturationChanged; // 0-1

        // Состояния
        [SerializeField, Range(0, 1)] private float _saturation;
        private Collider _collider;
        
        // Начальные размеры коллайдера
        private Vector3 _initialColliderSize;
        private Vector3 _initialColliderCenter;
        private float _initialColliderRadius;
        private float _initialColliderHeight;

        public float Saturation => _saturation;
        public float SpeedMultiplier => 1f - (_saturation * (1f - (_weightData != null ? 0.3f : 0.3f)));

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            
            SaveInitialColliderSize();
        }

        private void Start()
        {
            OnSaturationChanged?.Invoke(_saturation);
        }

        private void SaveInitialColliderSize()
        {
            if (_collider is CapsuleCollider capsule)
            {
                _initialColliderRadius = capsule.radius;
                _initialColliderHeight = capsule.height;
                _initialColliderCenter = capsule.center;
            }
            else if (_collider is BoxCollider box)
            {
                _initialColliderSize = box.size;
                _initialColliderCenter = box.center;
            }
            else if (_collider is SphereCollider sphere)
            {
                _initialColliderRadius = sphere.radius;
                _initialColliderCenter = sphere.center;
            }
        }

        /// <summary>
        /// Добавить сытость
        /// </summary>
        public void AddSaturation(float amount)
        {
            if (amount <= 0) return;
            
            _saturation = Mathf.Clamp01(_saturation + amount);
            StartCoroutine(SmoothScaleUpdate());
            OnSaturationChanged?.Invoke(_saturation);
        }

        /// <summary>
        /// Сбросить сытость
        /// </summary>
        public void ResetSaturation()
        {
            _saturation = 0f;
            UpdatePhysicalStats();
            OnSaturationChanged?.Invoke(_saturation);
        }

        /// <summary>
        /// Немедленно обновить физические параметры
        /// </summary>
        public void UpdatePhysicalStats()
        {
            if (_weightData == null) return;
            
            float targetScale = Mathf.Lerp(1f, _weightData.maxScaleMultiplier, _saturation);
            transform.localScale = Vector3.one * targetScale;
            
            if (_rigidbody != null)
                _rigidbody.mass = Mathf.Lerp(1f, _weightData.massMultiplier, _saturation);
            
            UpdateColliderSize(targetScale);
            
            if (_proceduralAnimator != null)
                _proceduralAnimator.UpdateBaseScale(Vector3.one * targetScale);
        }

        private System.Collections.IEnumerator SmoothScaleUpdate()
        {
            if (_weightData == null) yield break;
            
            float targetScale = Mathf.Lerp(1f, _weightData.maxScaleMultiplier, _saturation);
            Vector3 targetScaleVec = Vector3.one * targetScale;
            Vector3 startScale = transform.localScale;
            
            float elapsed = 0f;
            
            while (elapsed < _weightData.scaleSmoothDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _weightData.scaleSmoothDuration;
                // EaseOutBack для "упругого" эффекта
                t = 1f + 1.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f);
                
                transform.localScale = Vector3.Lerp(startScale, targetScaleVec, Mathf.Clamp01(t));
                
                if (_proceduralAnimator != null)
                    _proceduralAnimator.UpdateBaseScale(transform.localScale);
                
                yield return null;
            }
            
            transform.localScale = targetScaleVec;
            
            if (_rigidbody != null)
                _rigidbody.mass = Mathf.Lerp(1f, _weightData.massMultiplier, _saturation);
            
            UpdateColliderSize(targetScale);
            
            if (_proceduralAnimator != null)
                _proceduralAnimator.UpdateBaseScale(targetScaleVec);
        }

        private void UpdateColliderSize(float scaleMultiplier)
        {
            if (_collider is CapsuleCollider capsule)
            {
                capsule.radius = _initialColliderRadius * scaleMultiplier;
                capsule.height = _initialColliderHeight * scaleMultiplier;
                capsule.center = _initialColliderCenter * scaleMultiplier;
            }
            else if (_collider is BoxCollider box)
            {
                box.size = _initialColliderSize * scaleMultiplier;
                box.center = _initialColliderCenter * scaleMultiplier;
            }
            else if (_collider is SphereCollider sphere)
            {
                sphere.radius = _initialColliderRadius * scaleMultiplier;
                sphere.center = _initialColliderCenter * scaleMultiplier;
            }
        }
    }
}
