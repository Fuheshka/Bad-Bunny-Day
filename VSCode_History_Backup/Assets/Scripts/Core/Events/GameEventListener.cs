using UnityEngine;
using UnityEngine.Events;

namespace CDB.Core.Events
{
    /// <summary>
    /// Компонент-слушатель игровых событий
    /// </summary>
    public class GameEventListener : MonoBehaviour, IGameEventListener
    {
        [Tooltip("Событие для прослушивания")]
        [SerializeField] private GameEvent _event;
        
        [Tooltip("Ответ на событие")]
        [SerializeField] private UnityEvent _response;

        private void OnEnable()
        {
            if (_event != null)
                _event.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (_event != null)
                _event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            _response?.Invoke();
        }
    }
}
