using UnityEngine;
using Gamejam.Core;

namespace Gamejam.Character
{
    /// <summary>
    /// Handles dash mechanic with stamina consumption, cooldown, and noise generation.
    /// Allows players to perform quick bursts of speed with high noise generation.
    /// </summary>
    public class DashSystem : MonoBehaviour
    {
        private PlayerModel playerModel;
        private PlayerInputState playerInputState;
        private StaminaSystem staminaSystem;
        private Vector3 dashDirection = Vector3.zero;

        public void Initialize(PlayerModel model, PlayerInputState inputState, StaminaSystem stamina)
        {
            playerModel = model;
            playerInputState = inputState;
            staminaSystem = stamina;
            Debug.Log("[DashSystem] Initialized with model: " + (model != null) + ", stamina: " + (stamina != null));
        }

        private void FixedUpdate()
        {
            if (playerModel == null)
                return;

            if (playerInputState.DashPressed)
            {
                Debug.Log("[DashSystem FixedUpdate] DashPressed is TRUE! Processing...");
            }

            UpdateDashState();
            HandleDashInput();
        }

        /// <summary>
        /// Updates dash timer and cooldown timers each frame.
        /// </summary>
        private void UpdateDashState()
        {
            // Update dash duration timer
            if (playerModel.IsDashing)
            {
                playerModel.DashTimeRemaining -= Time.fixedDeltaTime;
                if (playerModel.DashTimeRemaining <= 0f)
                {
                    playerModel.IsDashing = false;
                }
            }

            // Update dash cooldown
            if (playerModel.DashCooldownRemaining > 0f)
            {
                playerModel.DashCooldownRemaining -= Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// Handles dash input and initiates dash if conditions are met.
        /// </summary>
        private void HandleDashInput()
        {
            if (!playerInputState.DashPressed)
                return;

            Debug.Log("[DashSystem] Dash pressed! IsDashing: " + playerModel.IsDashing + ", Cooldown: " + playerModel.DashCooldownRemaining + ", Stamina: " + playerModel.Stamina);

            // Check if already dashing or in cooldown
            if (playerModel.IsDashing || playerModel.DashCooldownRemaining > 0f)
            {
                Debug.Log("[DashSystem] Cannot dash: already dashing or in cooldown");
                return;
            }

            // Check stamina threshold
            if (!staminaSystem.HasEnoughStamina(GameConfig.PlayerDashStaminaThreshold))
            {
                Debug.Log("[DashSystem] Cannot dash: insufficient stamina. Current: " + playerModel.Stamina + " Required: " + GameConfig.PlayerDashStaminaThreshold);
                return;
            }

            // Try to consume stamina
            if (!staminaSystem.TryConsumeStamina(GameConfig.PlayerDashStaminaCost))
            {
                Debug.Log("[DashSystem] Cannot consume stamina");
                return;
            }

            // Initiate dash
            StartDash();
        }

        /// <summary>
        /// Starts a dash action with direction based on current movement or player facing.
        /// </summary>
        private void StartDash()
        {
            Vector2 moveDir = playerInputState.MovementDirection;
            
            // If no movement input, dash in facing direction
            if (moveDir.magnitude < 0.1f)
            {
                dashDirection = playerModel.Transform.forward;
            }
            else
            {
                dashDirection = new Vector3(moveDir.x, 0f, moveDir.y).normalized;
            }

            playerModel.IsDashing = true;
            playerModel.DashTimeRemaining = GameConfig.PlayerDashDuration;
            playerModel.DashCooldownRemaining = GameConfig.PlayerDashCooldown;

            Debug.Log("[DashSystem] DASH ACTIVATED! Direction: " + dashDirection + ", Stamina remaining: " + playerModel.Stamina);

            // Generate high-priority noise event
            NoiseGenerator.GenerateNoise(
                playerModel.Transform.position,
                GameConfig.PlayerDashNoisePriority,
                "Dash"
            );
        }

        /// <summary>
        /// Get the direction of current dash, or zero if not dashing.
        /// </summary>
        public Vector3 GetDashDirection()
        {
            return playerModel.IsDashing ? dashDirection : Vector3.zero;
        }

        /// <summary>
        /// Get dash progress as normalized value (0-1, where 1 is just started and 0 is finished).
        /// </summary>
        public float GetDashProgress()
        {
            if (!playerModel.IsDashing || GameConfig.PlayerDashDuration <= 0)
                return 0f;

            return playerModel.DashTimeRemaining / GameConfig.PlayerDashDuration;
        }

        /// <summary>
        /// Check if player can perform a dash right now.
        /// </summary>
        public bool CanDash()
        {
            return !playerModel.IsDashing &&
                   playerModel.DashCooldownRemaining <= 0f &&
                   staminaSystem.HasEnoughStamina(GameConfig.PlayerDashStaminaThreshold);
        }
    }
}
