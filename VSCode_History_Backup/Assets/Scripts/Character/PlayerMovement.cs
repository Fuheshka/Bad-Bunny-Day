using UnityEngine;
using Gamejam.Core;
using Gamejam.Character;

namespace Gamejam.Character
{
    /// <summary>
    /// Handles player movement physics and rotation.
    /// Reads input from PlayerInputState and applies movement to the Rigidbody.
    /// Ensures movement is restricted to the ground plane (X-Z axes).
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerModel playerModel;
        private PlayerInputState playerInputState;
        private DashSystem dashSystem;

        public void Initialize(PlayerModel model, PlayerInputState inputState, DashSystem dash = null)
        {
            playerModel = model;
            playerInputState = inputState;
            dashSystem = dash;
        }

        private void FixedUpdate()
        {
            if (playerModel?.RigidBody == null || playerInputState == null)
                return;

            ApplyMovement();
            ApplyRotation();
        }

        /// <summary>
        /// Applies horizontal movement based on input while preserving Y-axis velocity (gravity).
        /// During dash, ignores drag modifiers and applies dash speed multiplier.
        /// </summary>
        private void ApplyMovement()
        {
            Vector2 moveInput = playerInputState.MovementDirection;
            
            // Calculate target velocity on horizontal plane (X-Z)
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            float currentSpeed = playerInputState.IsSprinting 
                ? GameConfig.PlayerBaseSpeed * GameConfig.PlayerSprintMultiplier 
                : GameConfig.PlayerBaseSpeed;

            // Apply dash speed multiplier if currently dashing
            Vector3 dashBoost = Vector3.zero;
            if (playerModel.IsDashing && dashSystem != null)
            {
                Vector3 dashDir = dashSystem.GetDashDirection();
                dashBoost = dashDir * GameConfig.PlayerBaseSpeed * GameConfig.PlayerDashSpeedMultiplier;
            }

            Vector3 targetVelocity = moveDirection * currentSpeed + dashBoost;

            // Get current velocity and preserve Y component (gravity/jumping)
            Vector3 currentVelocity = playerModel.RigidBody.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            // Apply drag/damping to horizontal movement
            // During dash, ignore drag modifiers to maintain speed
            float dragMultiplier = playerModel.IsDashing ? 0f : GameConfig.PlayerMovementDrag;
            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity, 
                targetVelocity, 
                dragMultiplier * Time.fixedDeltaTime
            );

            // Combine horizontal movement with preserved Y velocity
            Vector3 newVelocity = new Vector3(
                horizontalVelocity.x,
                currentVelocity.y,
                horizontalVelocity.z
            );

            playerModel.RigidBody.linearVelocity = newVelocity;
            playerModel.Velocity = newVelocity;
        }

        /// <summary>
        /// Smoothly rotates the player towards the movement direction.
        /// </summary>
        private void ApplyRotation()
        {
            Vector2 moveInput = playerInputState.MovementDirection;
            
            // Only rotate if there's significant input
            if (moveInput.magnitude < 0.1f)
                return;

            // Calculate target direction on horizontal plane
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Smoothly interpolate towards target rotation
            playerModel.RigidBody.rotation = Quaternion.Lerp(
                playerModel.RigidBody.rotation,
                targetRotation,
                GameConfig.PlayerRotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}
