using UnityEngine;

namespace GameJam.Input
{
    /// <summary>
    /// Data model for player input state
    /// </summary>
    public class PlayerInputState
    {
        /// <summary>
        /// Movement direction from WASD/Gamepad (normalized)
        /// </summary>
        public Vector2 MoveDirection { get; set; } = Vector2.zero;

        /// <summary>
        /// Is player requesting dash/sprint
        /// </summary>
        public bool IsDashing { get; set; }

        public void Reset()
        {
            MoveDirection = Vector2.zero;
            IsDashing = false;
        }
    }
}
