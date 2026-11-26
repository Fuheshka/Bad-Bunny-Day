using UnityEngine;
using UnityEngine.Events;
using CDB.Core;

namespace CDB.Player.Components
{
    /// <summary>
    /// Компонент здоровья игрока.
    /// Отвечает за получение урона, лечение, неуязвимость и смерть.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
    {
        [Header("Данные")]
        [SerializeField] private Data.PlayerCombatData _combatData;
        
        [Header("Ссылки")]
        [SerializeField] private Renderer[] _renderers;

        [Header("События")]
        public UnityEvent OnDamage;
        public UnityEvent OnDeath;
        public UnityEvent<int, int> OnHealthChanged; // current, max

        // Состояния
        private int _currentHealth;
        private bool _isDead;
        private bool _isInvincible;
        
        // Интерфейс IDamageable
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _combatData != null ? _combatData.maxHealth : 3;
        public bool IsDead => _isDead;
        
        // Интерфейс IHealable
        public bool CanHeal => !_isDead && _currentHealth < MaxHealth;

        private void Start()
        {
            _currentHealth = MaxHealth;
            
            if (_renderers == null || _renderers.Length == 0)
            {
                _renderers = GetComponentsInChildren<Renderer>();
            }
            
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }

        public bool IsAlive() => !_isDead;

        public void TakeDamage(int damage, Vector3? knockbackSource = null)
        {
            if (_isDead || _isInvincible || damage <= 0) return;

            _currentHealth -= damage;
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityRoutine());
                OnDamage?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (!CanHeal || amount <= 0) return;
            
            _currentHealth = Mathf.Min(_currentHealth + amount, MaxHealth);
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }

        public void SetInvincible(bool value)
        {
            _isInvincible = value;
        }

        private void Die()
        {
            _isDead = true;
            OnDeath?.Invoke();
        }

        private System.Collections.IEnumerator InvincibilityRoutine()
        {
            if (_combatData == null) yield break;
            
            _isInvincible = true;
            float elapsed = 0f;
            float blinkInterval = 0.1f;
            bool visible = true;

            while (elapsed < _combatData.invincibilityDuration)
            {
                visible = !visible;
                SetRenderersVisible(visible);
                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            SetRenderersVisible(true);
            _isInvincible = false;
        }

        private void SetRenderersVisible(bool visible)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }
        }

        /// <summary>
        /// Сбросить здоровье (для перезапуска)
        /// </summary>
        public void Reset()
        {
            _currentHealth = MaxHealth;
            _isDead = false;
            _isInvincible = false;
            SetRenderersVisible(true);
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }
    }
}
