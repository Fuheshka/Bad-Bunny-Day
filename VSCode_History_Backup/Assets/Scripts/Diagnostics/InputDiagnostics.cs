using UnityEngine;
using UnityEngine.InputSystem;

namespace Gamejam.Diagnostics
{
    /// <summary>
    /// Diagnostic script to verify input system is working correctly.
    /// Add this to Player GameObject to debug input issues.
    /// </summary>
    public class InputDiagnostics : MonoBehaviour
    {
        private PlayerInput playerInput;
        private InputAction sprintAction;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            
            if (playerInput == null)
            {
                Debug.LogError("[InputDiagnostics] PlayerInput component not found on " + gameObject.name);
                return;
            }

            Debug.Log("[InputDiagnostics] PlayerInput found. Actions asset: " + (playerInput.actions != null ? "YES" : "NO"));
            
            if (playerInput.actions != null)
            {
                sprintAction = playerInput.actions.FindAction("Sprint");
                Debug.Log("[InputDiagnostics] Sprint action: " + (sprintAction != null ? "FOUND" : "NOT FOUND"));
                
                if (sprintAction != null)
                {
                    Debug.Log("[InputDiagnostics] Sprint bindings: " + sprintAction.bindings.Count);
                    foreach (var binding in sprintAction.bindings)
                    {
                        Debug.Log("  - Path: " + binding.path + " (Groups: " + binding.groups + ")");
                    }
                }
            }
        }

        private void Update()
        {
            if (playerInput == null || sprintAction == null)
                return;

            bool sprintPressed = sprintAction.IsPressed();
            bool sprintPressedThisFrame = sprintAction.WasPressedThisFrame();
            
            if (sprintPressedThisFrame)
            {
                Debug.Log("[InputDiagnostics] SPRINT PRESSED THIS FRAME");
            }
            
            if (sprintPressed)
            {
                Debug.Log("[InputDiagnostics] SPRINT IS HELD");
            }
        }
    }
}
