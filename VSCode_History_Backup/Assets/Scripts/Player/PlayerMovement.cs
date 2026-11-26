using UnityEngine;
using GameJam.Core;
using GameJam.Input;
using Zenject;

namespace GameJam.Player
{
    /// <summary>
    /// Handles player movement logic with velocity-based physics
    /// Implements ITickable to update every frame
    /// </summary>
    public class PlayerMovement : ITickable
    {
        private readonly PlayerModel _playerModel;
        private readonly PlayerInputState _inputState;

        public PlayerMovement(PlayerModel playerModel, PlayerInputState inputState)
        {
            _playerModel = playerModel;
            _inputState = inputState;
        }

        public void Tick()
        {
            CalculateMovement();
            ApplyPhysics();
        }

        private void CalculateMovement()
        {
            // Get input direction
            Vector3 moveDir = new Vector3(_inputState.MoveDirection.x, 0f, _inputState.MoveDirection.y);

            // Calculate target speed based on dash state
            float targetSpeed = GameConfig.PlayerBaseSpeed;
            if (_inputState.IsDashing)
            {
                targetSpeed *= GameConfig.PlayerDashMultiplier;
            }

            // Normalize and apply speed
            if (moveDir.magnitude > 0.1f)
            {
                moveDir.Normalize();
                _playerModel.CurrentVelocity = moveDir * targetSpeed;

                // Rotate player towards movement direction
                RotateTowardsMovement(moveDir);
            }
            else
            {
                // Apply friction/damping when not moving
                _playerModel.CurrentVelocity = Vector3.Lerp(
                    _playerModel.CurrentVelocity,
                    Vector3.zero,
                    GameConfig.PlayerDrag * Time.deltaTime
                );
            }
        }

        private void ApplyPhysics()
        {
            if (_playerModel.Rigidbody == null)
                return;

            // Get current rigidbody velocity to preserve Y (gravity)
            Vector3 rigidbodyVel = _playerModel.Rigidbody.linearVelocity;

            // Apply calculated horizontal velocity, preserve vertical
            Vector3 finalVelocity = new Vector3(
                _playerModel.CurrentVelocity.x,
                rigidbodyVel.y,  // Keep gravity/jumping velocity
                _playerModel.CurrentVelocity.z
            );

            _playerModel.Rigidbody.linearVelocity = finalVelocity;
        }

        private void RotateTowardsMovement(Vector3 direction)
        {
            if (direction.magnitude < 0.1f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _playerModel.Transform.rotation = Quaternion.Lerp(
                _playerModel.Transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
    }
}
