using UnityEngine;

namespace CDB.Core
{
    /// <summary>
    /// Синглтон, который не уничтожается при смене сцен
    /// </summary>
    /// <typeparam name="T">Тип наследника</typeparam>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            
            if (Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
