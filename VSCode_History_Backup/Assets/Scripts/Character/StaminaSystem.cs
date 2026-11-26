using UnityEngine;
using Gamejam.Core;

namespace Gamejam.Character
{
    /// <summary>
    /// Manages player stamina regeneration and consumption.
    /// Handles stamina drain for actions and natural regeneration over time.
    /// </summary>
    public class StaminaSystem : MonoBehaviour
    {
        private PlayerModel playerModel;
        private PlayerInputState playerInputState;

        public void Initialize(PlayerModel model, PlayerInputState inputState)
        {
            playerModel = model;
            playerInputState = inputState;
            Debug.Log("[StaminaSystem] Initialized with stamina: " + model.Stamina + " / " + GameConfig.PlayerMaxStamina);
        }

        private void Update()
        {
            if (playerModel == null)
                return;

            RegenerateStamina();
        }

        /// <summary>
        /// Regenerates stamina over time when player is not performing stamina-draining actions.
        /// </summary>
        private void RegenerateStamina()
        {
            if (!playerModel.IsDashing && !playerInputState.IsSprinting)
            {
                playerModel.Stamina = Mathf.Min(
                    playerModel.Stamina + GameConfig.PlayerStaminaRegenRate * Time.deltaTime,
                    GameConfig.PlayerMaxStamina
                );
            }
        }

        /// <summary>
        /// Attempts to consume stamina for an action.
        /// </summary>
        /// <param name="amount">Amount of stamina to consume</param>
        /// <returns>True if stamina was successfully consumed, false if insufficient stamina</returns>
        public bool TryConsumeStamina(float amount)
        {
            if (playerModel.Stamina >= amount)
            {
                playerModel.Stamina -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get current stamina as a normalized value (0-1)
        /// </summary>
        public float GetStaminaNormalized()
        {
            return playerModel.Stamina / GameConfig.PlayerMaxStamina;
        }

        /// <summary>
        /// Check if player has enough stamina to perform an action
        /// </summary>
        public bool HasEnoughStamina(float requiredAmount)
        {
            return playerModel.Stamina >= requiredAmount;
        }
    }
}
