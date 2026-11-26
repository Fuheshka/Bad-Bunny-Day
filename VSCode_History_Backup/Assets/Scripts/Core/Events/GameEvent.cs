using UnityEngine;

namespace CDB.Core.Events
{
    /// <summary>
    /// ScriptableObject для игровых событий (паттерн Observer через ScriptableObject)
    /// </summary>
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "CDB/Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private System.Collections.Generic.List<IGameEventListener> _listeners = 
            new System.Collections.Generic.List<IGameEventListener>();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(IGameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }

    /// <summary>
    /// Интерфейс слушателя игрового события
    /// </summary>
    public interface IGameEventListener
    {
        void OnEventRaised();
    }
}
