namespace Gamejam.Core
{
    /// <summary>
    /// Central configuration class for all game constants.
    /// Organize configuration by feature area.
    /// </summary>
    public static class GameConfig
    {
        // ===== PLAYER MOVEMENT =====
        /// <summary>Base walking speed of the player (units per second)</summary>
        public const float PlayerBaseSpeed = 5f;

        /// <summary>Speed multiplier when sprinting/dashing</summary>
        public const float PlayerSprintMultiplier = 1.5f;

        /// <summary>Drag/friction applied to horizontal movement (deceleration)</summary>
        public const float PlayerMovementDrag = 8f;

        /// <summary>Cooldown time between sprint actions (seconds)</summary>
        public const float PlayerSprintCooldown = 0.5f;

        /// <summary>Rotation speed when turning towards movement direction (smoothing factor)</summary>
        public const float PlayerRotationSpeed = 10f;

        // ===== PLAYER PHYSICS =====
        /// <summary>Player mass for physics calculations</summary>
        public const float PlayerMass = 1f;

        /// <summary>Friction coefficient for the player Rigidbody</summary>
        public const float PlayerFriction = 0.4f;

        /// <summary>Angular drag to prevent unwanted rotation</summary>
        public const float PlayerAngularDrag = 0.05f;

        // ===== PLAYER STAMINA =====
        /// <summary>Maximum stamina capacity</summary>
        public const float PlayerMaxStamina = 100f;

        /// <summary>Stamina regeneration per second when not performing actions</summary>
        public const float PlayerStaminaRegenRate = 20f;

        /// <summary>Stamina cost for performing a dash</summary>
        public const float PlayerDashStaminaCost = 30f;

        /// <summary>Minimum stamina required to perform a dash</summary>
        public const float PlayerDashStaminaThreshold = 30f;

        // ===== PLAYER DASH =====
        /// <summary>Duration of the dash action in seconds</summary>
        public const float PlayerDashDuration = 0.3f;

        /// <summary>Speed multiplier during dash (applied to current movement direction)</summary>
        public const float PlayerDashSpeedMultiplier = 3f;

        /// <summary>Cooldown between dash attempts in seconds</summary>
        public const float PlayerDashCooldown = 0.5f;

        /// <summary>Priority level for noise generation during dash (for enemy alerting)</summary>
        public const int PlayerDashNoisePriority = 10;
    }
}
