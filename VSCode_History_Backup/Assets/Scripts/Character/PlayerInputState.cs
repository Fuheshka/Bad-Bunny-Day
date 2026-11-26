using UnityEngine;

namespace Gamejam.Character
{
    /// <summary>
    /// Holds the input state for the player character.
    /// This is updated by PlayerInputHandler and read by PlayerMovement.
    /// </summary>
    public class PlayerInputState
    {
        /// <summary>Movement direction from input (X and Z axes, normalized)</summary>
        public Vector2 MovementDirection { get; set; }

        /// <summary>Whether the player is currently sprinting</summary>
        public bool IsSprinting { get; set; }

        /// <summary>Whether the player pressed jump this frame</summary>
        public bool JumpPressed { get; set; }

        /// <summary>Whether the player pressed dash this frame</summary>
        public bool DashPressed { get; set; }
    }
}
