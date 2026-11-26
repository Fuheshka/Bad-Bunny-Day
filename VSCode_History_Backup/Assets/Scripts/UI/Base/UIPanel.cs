using UnityEngine;

namespace CDB.UI
{
    /// <summary>
    /// Базовый класс для UI панелей.
    /// Управляет показом/скрытием и анимациями.
    /// </summary>
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Настройки панели")]
        [SerializeField] protected GameObject _panelRoot;
        [SerializeField] protected bool _startHidden = true;

        protected virtual void Start()
        {
            if (_panelRoot == null)
                _panelRoot = gameObject;
                
            if (_startHidden)
                Hide();
        }

        /// <summary>
        /// Показать панель
        /// </summary>
        public virtual void Show()
        {
            if (_panelRoot != null)
                _panelRoot.SetActive(true);
                
            OnShow();
        }

        /// <summary>
        /// Скрыть панель
        /// </summary>
        public virtual void Hide()
        {
            if (_panelRoot != null)
                _panelRoot.SetActive(false);
                
            OnHide();
        }

        /// <summary>
        /// Переключить видимость панели
        /// </summary>
        public virtual void Toggle()
        {
            if (_panelRoot != null && _panelRoot.activeSelf)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// Вызывается при показе панели
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// Вызывается при скрытии панели
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// Проверить видимость панели
        /// </summary>
        public bool IsVisible => _panelRoot != null && _panelRoot.activeSelf;
    }
}
