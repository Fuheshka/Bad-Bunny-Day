using UnityEngine;
using UnityEngine.InputSystem;
using Gamejam.Core;
using Gamejam.Character;

namespace Gamejam.Input
{
    /// <summary>
    /// Handles player input from keyboard and gamepad.
    /// Reads from the new Input System and updates PlayerInputState.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputState playerInputState;
        private PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction sprintAction;
        private InputAction jumpAction;
        private InputAction dashAction;

        private float sprintCooldownTimer = 0f;

        private void OnEnable()
        {
            if (playerInput != null)
            {
                playerInput.actions.Enable();
            }
        }

        private void OnDisable()
        {
            if (playerInput != null)
            {
                playerInput.actions.Disable();
            }
        }

        public void Initialize(PlayerInputState inputState)
        {
            playerInputState = inputState;
            
            // Lazy-find the PlayerInput component on this GameObject
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            if (playerInput != null)
            {
                // Get references to the input actions
                moveAction = playerInput.actions.FindAction("Move");
                sprintAction = playerInput.actions.FindAction("Sprint");
                jumpAction = playerInput.actions.FindAction("Jump");
                dashAction = playerInput.actions.FindAction("Sprint");  // Dash uses Sprint action key

                playerInput.actions.Enable();
                Debug.Log("[PlayerInputHandler] Input actions initialized. Sprint action: " + (sprintAction != null ? "FOUND" : "NOT FOUND"));
            }
            else
            {
                Debug.LogError("[PlayerInputHandler] PlayerInput component not found on " + gameObject.name);
            }
        }

        private void Update()
        {
            if (playerInputState == null || playerInput == null)
                return;

            // Update sprint cooldown
            if (sprintCooldownTimer > 0f)
            {
                sprintCooldownTimer -= Time.deltaTime;
            }

            // Read movement input
            if (moveAction != null)
            {
                Vector2 moveInput = moveAction.ReadValue<Vector2>();
                playerInputState.MovementDirection = moveInput.normalized;
            }

            // Read sprint input
            if (sprintAction != null)
            {
                bool sprintInput = sprintAction.IsPressed();
                playerInputState.IsSprinting = sprintInput && sprintCooldownTimer <= 0f;
                
                if (sprintInput && sprintCooldownTimer <= 0f)
                {
                    sprintCooldownTimer = GameConfig.PlayerSprintCooldown;
                }
            }
            else
            {
                Debug.LogWarning("[PlayerInputHandler] Sprint action is NULL");
            }

            // Read jump input
            if (jumpAction != null)
            {
                playerInputState.JumpPressed = jumpAction.WasPressedThisFrame();
            }

            // Read dash input (same as sprint but we differentiate in DashSystem)
            if (dashAction != null)
            {
                bool dashPressed = dashAction.WasPressedThisFrame();
                playerInputState.DashPressed = dashPressed;
                
                if (dashPressed)
                {
                    Debug.Log("[PlayerInputHandler] DashPressed set to TRUE - IsSprinting: " + playerInputState.IsSprinting);
                }
            }
            else
            {
                Debug.LogWarning("[PlayerInputHandler] Dash action is NULL");
                playerInputState.DashPressed = false;
            }
        }

        private void OnDestroy()
        {
            if (playerInput != null)
            {
                playerInput.actions.Disable();
            }
        }
    }
}
