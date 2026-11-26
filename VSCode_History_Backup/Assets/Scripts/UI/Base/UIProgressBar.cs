using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CDB.UI
{
    /// <summary>
    /// Базовый класс для UI элементов со слайдером/прогрессом.
    /// </summary>
    public abstract class UIProgressBar : MonoBehaviour
    {
        [Header("UI Элементы")]
        [SerializeField] protected Slider _slider;
        [SerializeField] protected Image _fillImage;
        [SerializeField] protected TextMeshProUGUI _valueText;

        [Header("Цвета")]
        [SerializeField] protected Color _fullColor = Color.green;
        [SerializeField] protected Color _lowColor = Color.yellow;
        [SerializeField] protected Color _emptyColor = Color.red;
        [SerializeField] protected float _lowThreshold = 0.3f;

        [Header("Анимация")]
        [SerializeField] protected bool _smoothTransition = true;
        [SerializeField] protected float _smoothSpeed = 5f;

        protected float _targetValue = 1f;
        protected float _currentValue = 1f;

        protected virtual void Awake()
        {
            InitializeComponents();
        }

        protected virtual void Update()
        {
            if (_smoothTransition)
            {
                _currentValue = Mathf.Lerp(_currentValue, _targetValue, _smoothSpeed * Time.deltaTime);
                ApplyValue(_currentValue);
            }
        }

        protected virtual void InitializeComponents()
        {
            if (_slider != null)
            {
                _slider.minValue = 0f;
                _slider.maxValue = 1f;
            }
        }

        /// <summary>
        /// Установить значение (0-1)
        /// </summary>
        public virtual void SetValue(float normalized)
        {
            _targetValue = Mathf.Clamp01(normalized);
            
            if (!_smoothTransition)
            {
                _currentValue = _targetValue;
                ApplyValue(_currentValue);
            }
        }

        /// <summary>
        /// Установить значение на основе текущего и максимального
        /// </summary>
        public virtual void SetValue(float current, float max)
        {
            SetValue(max > 0 ? current / max : 0);
        }

        protected virtual void ApplyValue(float value)
        {
            if (_slider != null)
                _slider.value = value;
                
            if (_fillImage != null)
                _fillImage.fillAmount = value;

            ApplyColor(value);
            UpdateText(value);
        }

        protected virtual void ApplyColor(float value)
        {
            Color targetColor;
            
            if (value <= _lowThreshold)
                targetColor = Color.Lerp(_emptyColor, _lowColor, value / _lowThreshold);
            else
                targetColor = Color.Lerp(_lowColor, _fullColor, (value - _lowThreshold) / (1f - _lowThreshold));

            if (_fillImage != null)
                _fillImage.color = targetColor;
                
            if (_slider != null && _slider.fillRect != null)
            {
                var fillImage = _slider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = targetColor;
            }
        }

        protected virtual void UpdateText(float value)
        {
            if (_valueText != null)
                _valueText.text = $"{(value * 100):F0}%";
        }
    }
}
