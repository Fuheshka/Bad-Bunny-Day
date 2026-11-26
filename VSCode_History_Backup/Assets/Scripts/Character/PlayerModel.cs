using UnityEngine;
using Gamejam.Core;

namespace Gamejam.Character
{
    /// <summary>
    /// Data container for the player character.
    /// Holds references to the physical components and current state.
    /// </summary>
    public class PlayerModel
    {
        /// <summary>The Rigidbody component controlling physics</summary>
        public Rigidbody RigidBody { get; set; }

        /// <summary>The Transform component for position and rotation</summary>
        public Transform Transform { get; set; }

        /// <summary>Current velocity of the player</summary>
        public Vector3 Velocity { get; set; }

        /// <summary>Current stamina value</summary>
        public float Stamina { get; set; }

        /// <summary>Whether the player is currently dashing</summary>
        public bool IsDashing { get; set; }

        /// <summary>Time remaining for current dash</summary>
        public float DashTimeRemaining { get; set; }

        /// <summary>Cooldown time remaining before next dash can be performed</summary>
        public float DashCooldownRemaining { get; set; }

        /// <summary>Initialize stamina to max on creation</summary>
        public PlayerModel()
        {
            Stamina = GameConfig.PlayerMaxStamina;
            IsDashing = false;
            DashTimeRemaining = 0f;
            DashCooldownRemaining = 0f;
        }
    }
}
