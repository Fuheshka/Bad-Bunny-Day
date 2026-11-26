using UnityEngine;
using UnityEngine.Events;

namespace CDB.Player.Components
{
    /// <summary>
    /// Компонент выносливости игрока.
    /// Отвечает за расход и восстановление стамины.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Данные")]
        [SerializeField] private Data.PlayerStaminaData _staminaData;

        [Header("События")]
        public UnityEvent<float, float> OnStaminaChanged; // current, max

        // Состояния
        private float _currentStamina;
        private bool _isRegenerating = true;

        public float Current => _currentStamina;
        public float Max => _staminaData != null ? _staminaData.maxStamina : 100f;
        public float Normalized => _currentStamina / Max;

        private void Start()
        {
            _currentStamina = Max;
            OnStaminaChanged?.Invoke(_currentStamina, Max);
        }

        private void Update()
        {
            if (_isRegenerating && _staminaData != null)
            {
                Regenerate();
            }
        }

        private void Regenerate()
        {
            if (_currentStamina >= Max) return;
            
            float prevStamina = _currentStamina;
            _currentStamina = Mathf.MoveTowards(_currentStamina, Max, _staminaData.regenRate * Time.deltaTime);
            
            if (Mathf.Abs(_currentStamina - prevStamina) > 0.01f)
            {
                OnStaminaChanged?.Invoke(_currentStamina, Max);
            }
        }

        /// <summary>
        /// Попытаться потратить стамину
        /// </summary>
        /// <param name="amount">Количество</param>
        /// <returns>True если хватило стамины</returns>
        public bool TryConsume(float amount)
        {
            if (_currentStamina < amount) return false;
            
            _currentStamina -= amount;
            OnStaminaChanged?.Invoke(_currentStamina, Max);
            return true;
        }

        /// <summary>
        /// Проверить достаточно ли стамины
        /// </summary>
        public bool HasEnough(float amount)
        {
            return _currentStamina >= amount;
        }

        /// <summary>
        /// Приостановить/возобновить регенерацию
        /// </summary>
        public void SetRegeneration(bool enabled)
        {
            _isRegenerating = enabled;
        }

        /// <summary>
        /// Добавить стамину
        /// </summary>
        public void Add(float amount)
        {
            if (amount <= 0) return;
            _currentStamina = Mathf.Min(_currentStamina + amount, Max);
            OnStaminaChanged?.Invoke(_currentStamina, Max);
        }

        /// <summary>
        /// Сбросить стамину до максимума
        /// </summary>
        public void Reset()
        {
            _currentStamina = Max;
            _isRegenerating = true;
            OnStaminaChanged?.Invoke(_currentStamina, Max);
        }
    }
}
