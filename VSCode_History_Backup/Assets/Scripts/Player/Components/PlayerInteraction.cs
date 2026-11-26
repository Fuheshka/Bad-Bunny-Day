using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CDB.Food;

namespace CDB.Player.Components
{
    /// <summary>
    /// Компонент взаимодействия игрока с едой.
    /// Отвечает за поедание и связь с системой очков.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("UI")]
        [Tooltip("Ссылка на UI Image с Fill Method = Radial 360")]
        [SerializeField] private Image _eatIndicatorUI;

        [Header("События")]
        public UnityEvent OnEatStart;
        public UnityEvent OnEatEnd;
        public UnityEvent<EdibleObject> OnFoodConsumed;
        public UnityEvent<Vector3> OnNoise;

        // Состояния
        private EdibleObject _currentFoodTarget;
        private float _eatTimer;
        private bool _isEating;
        private bool _interactPressed;

        // Ссылки
        private Rigidbody _rb;
        private PlayerWeight _weight;

        public bool IsEating => _isEating;
        public EdibleObject CurrentFood => _currentFoodTarget;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _weight = GetComponent<PlayerWeight>();
        }

        private void Start()
        {
            if (_eatIndicatorUI != null)
            {
                _eatIndicatorUI.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            HandleInteraction();
        }

        /// <summary>
        /// Установить состояние кнопки взаимодействия
        /// </summary>
        public void SetInteractPressed(bool pressed)
        {
            _interactPressed = pressed;
        }

        private void HandleInteraction()
        {
            if (_currentFoodTarget == null)
            {
                if (_eatIndicatorUI != null && _eatIndicatorUI.gameObject.activeSelf)
                    _eatIndicatorUI.gameObject.SetActive(false);
                return;
            }

            if (_eatIndicatorUI != null && !_eatIndicatorUI.gameObject.activeSelf)
                _eatIndicatorUI.gameObject.SetActive(true);

            if (_interactPressed)
            {
                if (!_isEating && IsGrounded())
                {
                    StartEating();
                }

                _eatTimer += Time.deltaTime;

                if (_eatIndicatorUI != null)
                {
                    _eatIndicatorUI.fillAmount = _eatTimer / _currentFoodTarget.eatTime;
                }

                if (_eatTimer >= _currentFoodTarget.eatTime)
                {
                    ConsumeFood();
                }
            }
            else
            {
                if (_isEating)
                {
                    StopEating();
                }

                if (_eatIndicatorUI != null)
                {
                    _eatIndicatorUI.fillAmount = 0;
                }
            }
        }

        private void StartEating()
        {
            _isEating = true;
            _eatTimer = 0f;
            
            if (_rb != null)
                _rb.linearVelocity = Vector3.zero;

            if (_eatIndicatorUI != null)
                _eatIndicatorUI.fillAmount = 0;

            OnEatStart?.Invoke();
        }

        private void StopEating()
        {
            _isEating = false;
            _eatTimer = 0f;
            OnEatEnd?.Invoke();
        }

        private void ConsumeFood()
        {
            if (_currentFoodTarget == null) return;

            // Добавляем сытость
            if (_weight != null)
            {
                _weight.AddSaturation(_currentFoodTarget.nutritionValue);
            }

            // Добавляем очки (через ScoreManager)
            if (ScoreManager.Instance != null && _currentFoodTarget.scoreValue > 0)
            {
                ScoreManager.Instance.AddBellyScore(_currentFoodTarget.scoreValue);
            }

            // Лечение
            var health = GetComponent<PlayerHealth>();
            if (health != null && _currentFoodTarget.healAmount > 0)
            {
                health.Heal(_currentFoodTarget.healAmount);
            }

            OnFoodConsumed?.Invoke(_currentFoodTarget);
            OnNoise?.Invoke(transform.position);

            if (_eatIndicatorUI != null)
                _eatIndicatorUI.gameObject.SetActive(false);

            Destroy(_currentFoodTarget.gameObject);
            _currentFoodTarget = null;
            StopEating();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Food")) return;
            
            var food = other.GetComponent<EdibleObject>();
            if (food == null) return;

            if (_currentFoodTarget != null && _currentFoodTarget != food)
            {
                _currentFoodTarget.Unhighlight();
            }

            _currentFoodTarget = food;
            _currentFoodTarget.Highlight();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Food")) return;
            
            if (_currentFoodTarget != null && _currentFoodTarget.gameObject == other.gameObject)
            {
                _currentFoodTarget.Unhighlight();
                _currentFoodTarget = null;
                StopEating();
                
                if (_eatIndicatorUI != null)
                    _eatIndicatorUI.gameObject.SetActive(false);
            }
        }

        private bool IsGrounded()
        {
            var movement = GetComponent<PlayerMovement>();
            return movement != null && movement.IsGrounded;
        }
    }
}
