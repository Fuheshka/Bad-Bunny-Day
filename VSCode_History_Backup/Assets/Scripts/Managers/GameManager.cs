using Unity.Android.Gradle.Manifest;
using UnityEngine;
using Zenject;

namespace GameJam.Managers
{
    /// <summary>
    /// Main game manager that initializes game systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.Debug.Log("[GameManager] Game initialized");
        }
    }
}
