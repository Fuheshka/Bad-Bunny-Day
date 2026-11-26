using UnityEngine;
using Gamejam.Character;
using Gamejam.Input;

namespace Gamejam.Core
{
    /// <summary>
    /// Main game manager that initializes and wires up all player systems.
    /// This is the entry point for the game logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private string playerTag = "Player";

        private PlayerModel playerModel;
        private PlayerInputState playerInputState;
        private PlayerInputHandler playerInputHandler;
        private PlayerMovement playerMovement;
        private StaminaSystem staminaSystem;
        private DashSystem dashSystem;

        private void Awake()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            // Find or create the player GameObject
            GameObject playerObject = GameObject.FindWithTag(playerTag);
            
            if (playerObject == null)
            {
                Debug.LogError($"[GameManager] Could not find player GameObject with tag '{playerTag}'");
                return;
            }

            // Initialize player data structures
            playerInputState = new PlayerInputState();
            playerModel = new PlayerModel
            {
                RigidBody = playerObject.GetComponent<Rigidbody>(),
                Transform = playerObject.transform,
                Velocity = Vector3.zero
            };

            if (playerModel.RigidBody == null)
            {
                Debug.LogError("[GameManager] Player GameObject must have a Rigidbody component");
                return;
            }

            // Get or add input handler
            playerInputHandler = playerObject.GetComponent<PlayerInputHandler>();
            if (playerInputHandler == null)
            {
                playerInputHandler = playerObject.AddComponent<PlayerInputHandler>();
                Debug.Log("[GameManager] Added PlayerInputHandler component");
            }

            // Get or add movement handler
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                playerMovement = playerObject.AddComponent<PlayerMovement>();
                Debug.Log("[GameManager] Added PlayerMovement component");
            }

            // Get or add stamina system
            staminaSystem = playerObject.GetComponent<StaminaSystem>();
            if (staminaSystem == null)
            {
                staminaSystem = playerObject.AddComponent<StaminaSystem>();
                Debug.Log("[GameManager] Added StaminaSystem component");
            }

            // Get or add dash system
            dashSystem = playerObject.GetComponent<DashSystem>();
            if (dashSystem == null)
            {
                dashSystem = playerObject.AddComponent<DashSystem>();
                Debug.Log("[GameManager] Added DashSystem component");
            }

            // Initialize systems
            playerInputHandler.Initialize(playerInputState);
            staminaSystem.Initialize(playerModel, playerInputState);
            dashSystem.Initialize(playerModel, playerInputState, staminaSystem);
            playerMovement.Initialize(playerModel, playerInputState, dashSystem);

            Debug.Log("[GameManager] Player systems initialized successfully");
        }
    }
}
