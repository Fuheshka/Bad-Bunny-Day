using UnityEngine;
using CDB.Core;

namespace CDB.Food
{
    /// <summary>
    /// Базовый класс для съедобных объектов.
    /// Реализует интерфейсы взаимодействия и подсветки.
    /// </summary>
    public class EdibleObject : MonoBehaviour, IInteractable, IHighlightable
    {
        [Header("Параметры еды")]
        [Tooltip("Насколько увеличится насыщение (0.0 - 1.0)")]
        [SerializeField] private float _nutritionValue = 0.1f;
        
        [Tooltip("Сколько секунд удерживать кнопку взаимодействия")]
        [SerializeField] private float _eatTime = 1.5f;

        [Tooltip("Очки за съедение")]
        [SerializeField] private int _scoreValue = 100;

        [Header("Лечение")]
        [Tooltip("Сколько HP восстановит еда (0 = не лечит)")]
        [SerializeField] private int _healAmount = 0;

        // Публичные свойства (для обратной совместимости)
        public float nutritionValue => _nutritionValue;
        public float eatTime => _eatTime;
        public int scoreValue => _scoreValue;
        public int healAmount => _healAmount;

        // IInteractable
        public float InteractionTime => _eatTime;
        public bool CanInteract => true;

        // IHighlightable
        public bool IsHighlighted => _isHighlighted;
        
        // Слои для подсветки
        private int _defaultLayer;
        private int _interactableLayer;
        private bool _isHighlighted;

        private void Awake()
        {
            _defaultLayer = LayerMask.NameToLayer("Default");
            _interactableLayer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(gameObject, _defaultLayer);
        }

        // IInteractable методы
        public void OnInteractStart() { }
        public void OnInteractEnd() { }
        public void OnInteractComplete() { }

        // IHighlightable методы
        public void Highlight()
        {
            if (_isHighlighted) return;
            SetLayerRecursively(gameObject, _interactableLayer);
            _isHighlighted = true;
        }

        public void Unhighlight()
        {
            if (!_isHighlighted) return;
            SetLayerRecursively(gameObject, _defaultLayer);
            _isHighlighted = false;
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                if (child == null) continue;
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}