using UnityEngine;
using Zenject;
using GameJam.Input;
using GameJam.Player;
using GameJam.Camera;

namespace GameJam.Installers
{
    /// <summary>
    /// Zenject installer for binding all game systems
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _playerPrefab;

        public override void InstallBindings()
        {
            UnityEngine.Debug.Log("[GameInstaller] InstallBindings called");

            try
            {
                // Bind input state
                var inputState = new PlayerInputState();
                Container.Bind<PlayerInputState>().FromInstance(inputState).AsSingle();
                UnityEngine.Debug.Log("[GameInstaller] Input state bound");

                // Collect all ITickable services
                var tickables = new System.Collections.Generic.List<ITickable>();

                // Create input handler as ITickable
                var inputHandler = new PlayerInputHandler(inputState);
                tickables.Add(inputHandler);
                UnityEngine.Debug.Log("[GameInstaller] Input handler created");

                // Find and bind player movement
                var rabbit = FindObjectOfType<Animator>();
                if (rabbit != null)
                {
                    UnityEngine.Debug.Log($"[GameInstaller] Found rabbit: {rabbit.gameObject.name}");
                    var rigidbody = rabbit.GetComponent<Rigidbody>();
                    var transform = rabbit.transform;

                    if (rigidbody != null)
                    {
                        var playerModel = new PlayerModel(rigidbody, transform);
                        var playerMovement = new PlayerMovement(playerModel, inputState);
                        tickables.Add(playerMovement);
                        UnityEngine.Debug.Log("[GameInstaller] Player movement created");
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("[GameInstaller] Rabbit has no Rigidbody!");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("[GameInstaller] Rabbit not found!");
                }

                // Create camera follower as ITickable
                var mainCamera = UnityEngine.Camera.main;
                if (mainCamera != null)
                {
                    var cameraFollower = new CameraFollower(mainCamera);
                    tickables.Add(cameraFollower);
                    UnityEngine.Debug.Log("[GameInstaller] Camera follower created");
                }
                else
                {
                    UnityEngine.Debug.LogError("[GameInstaller] Main camera not found!");
                }

                // Bind all tickables as a collection
                foreach (var tickable in tickables)
                {
                    Container.Bind<ITickable>().FromInstance(tickable);
                }
                UnityEngine.Debug.Log("[GameInstaller] All tickables bound to container");
                UnityEngine.Debug.Log("[GameInstaller] All systems initialized");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[GameInstaller] CRITICAL ERROR: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
