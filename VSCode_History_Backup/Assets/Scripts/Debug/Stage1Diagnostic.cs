using UnityEngine;
using UnityEngine.InputSystem;
using UnityDebug = UnityEngine.Debug;

namespace GameJam.Debug
{
    /// <summary>
    /// Debug utility to quickly diagnose Stage 1 setup issues
    /// Attach to any GameObject to run diagnostics
    /// </summary>
    public class Stage1Diagnostic : MonoBehaviour
    {
        private void Start()
        {
            RunDiagnostics();
        }

        [ContextMenu("Run Diagnostics")]
        public void RunDiagnostics()
        {
            UnityDebug.Log("\n═══════════════════════════════════════════════════════════");
            UnityDebug.Log("           STAGE 1 DIAGNOSTIC REPORT");
            UnityDebug.Log("═══════════════════════════════════════════════════════════\n");

            CheckAnimator();
            CheckRigidbody();
            CheckCapsuleCollider();
            CheckMainCamera();
            CheckPlayerInput();
            CheckInputActions();
            CheckGround();
            CheckGameInstaller();

            UnityDebug.Log("\n═══════════════════════════════════════════════════════════");
            UnityDebug.Log("           END OF DIAGNOSTIC REPORT");
            UnityDebug.Log("═══════════════════════════════════════════════════════════\n");
        }

        private void CheckAnimator()
        {
            UnityDebug.Log("\n[1] CHECKING FOR ANIMATOR (Rabbit)...");
            var animator = FindObjectOfType<Animator>();
            if (animator != null)
            {
                UnityDebug.Log($"   ✓ Found Animator on: {animator.gameObject.name}");
            }
            else
            {
                UnityDebug.LogError("   ✗ NO ANIMATOR FOUND! Add Animator component to Rabbit.");
            }
        }

        private void CheckRigidbody()
        {
            UnityDebug.Log("\n[2] CHECKING FOR RIGIDBODY...");
            var animator = FindObjectOfType<Animator>();
            if (animator == null)
            {
                UnityDebug.LogWarning("   ⚠ Skipping (no Animator found)");
                return;
            }

            var rb = animator.GetComponent<Rigidbody>();
            if (rb != null)
            {
                UnityDebug.Log($"   ✓ Rigidbody found!");
                UnityDebug.Log($"     Mass: {rb.mass}, Drag: {rb.linearDamping}");
                UnityDebug.Log($"     Use Gravity: {rb.useGravity}");
                if (!rb.useGravity)
                    UnityDebug.LogWarning("     ⚠ WARNING: Gravity is DISABLED!");
            }
            else
            {
                UnityDebug.LogError("   ✗ NO RIGIDBODY! Add Rigidbody component.");
            }
        }

        private void CheckCapsuleCollider()
        {
            UnityDebug.Log("\n[3] CHECKING FOR CAPSULE COLLIDER...");
            var animator = FindObjectOfType<Animator>();
            if (animator == null)
            {
                UnityDebug.LogWarning("   ⚠ Skipping (no Animator found)");
                return;
            }

            var collider = animator.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                UnityDebug.Log($"   ✓ Capsule Collider found!");
                UnityDebug.Log($"     Center: {collider.center}, Radius: {collider.radius}, Height: {collider.height}");
            }
            else
            {
                UnityDebug.LogError("   ✗ NO CAPSULE COLLIDER! Add Capsule Collider component.");
            }
        }

        private void CheckMainCamera()
        {
            UnityDebug.Log("\n[4] CHECKING FOR MAIN CAMERA...");
            var camera = UnityEngine.Camera.main;
            if (camera != null)
            {
                UnityDebug.Log($"   ✓ Main Camera found!");
                UnityDebug.Log($"     Position: {camera.transform.position}");
                UnityDebug.Log($"     Tag: {camera.gameObject.tag}");
                if (camera.gameObject.tag != "MainCamera")
                    UnityDebug.LogWarning("     ⚠ WARNING: Tag is not 'MainCamera'!");
            }
            else
            {
                UnityDebug.LogError("   ✗ NO MAIN CAMERA! Check that camera has 'MainCamera' tag.");
            }
        }

        private void CheckPlayerInput()
        {
            UnityDebug.Log("\n[5] CHECKING FOR PLAYER INPUT COMPONENT...");
            var playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput != null)
            {
                UnityDebug.Log($"   ✓ PlayerInput found on: {playerInput.gameObject.name}");
                UnityDebug.Log($"     Actions: {playerInput.actions.name}");
            }
            else
            {
                UnityDebug.LogError("   ✗ NO PLAYER INPUT! Create empty GameObject with PlayerInput component.");
            }
        }

        private void CheckInputActions()
        {
            UnityDebug.Log("\n[6] CHECKING INPUT ACTIONS...");
            try
            {
                var playerInput = FindObjectOfType<PlayerInput>();
                if (playerInput == null)
                {
                    UnityDebug.LogWarning("   ⚠ Skipping (no PlayerInput found)");
                    return;
                }

                var moveAction = playerInput.actions["Move"];
                var sprintAction = playerInput.actions["Sprint"];

                if (moveAction != null)
                    UnityDebug.Log("   ✓ 'Move' action exists");
                else
                    UnityDebug.LogError("   ✗ 'Move' action NOT FOUND!");

                if (sprintAction != null)
                    UnityDebug.Log("   ✓ 'Sprint' action exists");
                else
                    UnityDebug.LogError("   ✗ 'Sprint' action NOT FOUND!");
            }
            catch (System.Exception ex)
            {
                UnityDebug.LogError($"   ✗ Error checking actions: {ex.Message}");
            }
        }

        private void CheckGround()
        {
            UnityDebug.Log("\n[7] CHECKING FOR GROUND PLANE...");
            var planes = FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
            var hasPlane = false;
            foreach (var plane in planes)
            {
                if (plane.mesh.name.Contains("Plane"))
                {
                    UnityDebug.Log($"   ✓ Ground plane found: {plane.gameObject.name}");
                    hasPlane = true;
                    break;
                }
            }
            if (!hasPlane)
                UnityDebug.LogWarning("   ⚠ No ground plane detected (optional for testing)");
        }

        private void CheckGameInstaller()
        {
            UnityDebug.Log("\n[8] CHECKING FOR ZENJECT INSTALLER...");
            try
            {
                var installer = FindObjectOfType<GameJam.Installers.GameInstaller>();
                if (installer != null)
                {
                    UnityDebug.Log($"   ✓ GameInstaller found!");
                }
                else
                {
                    UnityDebug.LogWarning("   ⚠ GameInstaller not found (ensure Scene Context exists with GameInstaller)");
                }
            }
            catch
            {
                UnityDebug.LogWarning("   ⚠ Could not check GameInstaller");
            }
        }
    }
}
