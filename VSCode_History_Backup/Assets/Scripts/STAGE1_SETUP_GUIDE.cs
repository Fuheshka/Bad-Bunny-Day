// Stage 1: Basic Setup and Movement - SETUP GUIDE
// 
// This document provides step-by-step instructions to complete Stage 1 implementation.
// 
// ============================================================================
// SCENE SETUP - Follow these steps in Unity Editor:
// ============================================================================
//
// 1. CREATE THE PLAYER GAMEOBJECT
//    - In the Scene, create a new 3D GameObject and name it "Player"
//    - Position it at (0, 1, 0) to place it above the ground plane
//
// 2. ADD PLAYER TAG
//    - Select the Player GameObject
//    - In the Inspector, click the "Tag" dropdown
//    - Create a new tag called "Player" if it doesn't exist
//    - Assign the "Player" tag to the Player GameObject
//
// 3. ADD CAPSULE COLLIDER
//    - Select the Player GameObject
//    - In Inspector, click "Add Component"
//    - Search for and add "Capsule Collider"
//    - Set Height to 2
//    - Set Radius to 0.5
//    - Keep Center at (0, 0, 0) by default
//
// 4. ADD RIGIDBODY
//    - Select the Player GameObject
//    - In Inspector, click "Add Component"
//    - Search for and add "Rigidbody"
//    - Configure the following settings:
//      * Mass: 1
//      * Drag: 0
//      * Angular Drag: 0.05
//      * Gravity enabled: TRUE (to allow falling and jumping later)
//      * Collision Detection: Discrete (or Continuous for fast movement)
//      * Constraints:
//         - FREEZE Rotation X, Y, Z (prevents unwanted tilting)
//         - DO NOT freeze Position Y (we need gravity to work)
//
// 5. ADD PLAYER INPUT COMPONENT
//    - Select the Player GameObject
//    - In Inspector, click "Add Component"
//    - Search for and add "Player Input"
//    - Set Actions to: InputSystem_Actions (find it in Assets/)
//    - Keep Default Control Scheme
//
// 6. CREATE A GROUND PLANE (optional but recommended for testing)
//    - Create a new 3D Plane GameObject (Mesh > Plane)
//    - Name it "Ground"
//    - Scale it to (10, 1, 10) to make it larger
//    - Position at (0, -0.5, 0)
//    - Add a BoxCollider component
//    - Optional: Add a material with a visible color for reference
//
// 7. ADD GAME MANAGER
//    - Create an empty GameObject in the Scene
//    - Name it "GameManager"
//    - In Inspector, click "Add Component"
//    - Search for and add the "GameManager" script
//    - Leave all settings at default (it will find the Player by tag)
//
// ============================================================================
// CODE STRUCTURE
// ============================================================================
//
// The implementation is organized as follows:
//
// Assets/Scripts/Core/
//   - GameConfig.cs         : Central constants for movement (speed, drag, etc.)
//   - GameManager.cs        : Initialization and wiring of all systems
//
// Assets/Scripts/Character/
//   - PlayerModel.cs        : Data container (Rigidbody, Transform, Velocity)
//   - PlayerInputState.cs   : Input state data (movement, sprint, jump)
//   - PlayerMovement.cs     : Physics and movement logic
//
// Assets/Scripts/Input/
//   - PlayerInputHandler.cs : Reads from Input System and updates state
//
// ============================================================================
// HOW IT WORKS
// ============================================================================
//
// 1. GameManager initializes on Awake:
//    - Finds the Player GameObject by tag
//    - Creates PlayerModel (references Rigidbody and Transform)
//    - Creates PlayerInputState (holds input data)
//    - Adds PlayerInputHandler and PlayerMovement components if missing
//    - Initializes all systems with cross-references
//
// 2. PlayerInputHandler reads input each Update:
//    - Reads "Move" action → Gets Vector2 from WASD/Gamepad stick
//    - Reads "Sprint" action → Gets button press from Shift/Gamepad button
//    - Reads "Jump" action → Gets button press (for future implementation)
//    - Updates PlayerInputState with this data
//    - Manages sprint cooldown timer
//
// 3. PlayerMovement applies movement each FixedUpdate:
//    - Reads PlayerInputState for current input
//    - Calculates target velocity (base speed or sprint multiplier)
//    - Applies drag to smooth deceleration
//    - Preserves Y-axis velocity for gravity and jumping
//    - Applies rotation towards movement direction
//    - Sets Rigidbody.linearVelocity (preserves gravity)
//
// ============================================================================
// CONFIGURATION VALUES (GameConfig.cs)
// ============================================================================
//
// PlayerBaseSpeed = 5f
//   - The default walking speed in units per second
//   - Increase for faster movement, decrease for slower
//
// PlayerSprintMultiplier = 1.5f
//   - Multiplier applied to base speed when sprinting
//   - 1.5x means sprint is 1.5 times faster than walk
//
// PlayerMovementDrag = 8f
//   - Higher values = faster deceleration when stopping
//   - Lower values = smoother, more slippery feel
//
// PlayerSprintCooldown = 0.5f
//   - Cooldown between sprint actions (in seconds)
//   - Prevents spam input from triggering sprint too frequently
//
// PlayerRotationSpeed = 10f
//   - How quickly the player rotates to face movement direction
//   - Higher = more responsive turning
//   - Lower = more sluggish turning
//
// ============================================================================
// CONTROLS
// ============================================================================
//
// Keyboard:
//   - WASD: Move the character
//   - Shift: Sprint (1.5x speed)
//   - Space: Jump (implementation coming in Stage 2)
//
// Gamepad:
//   - Left Stick: Move the character
//   - Left Shoulder Button (LB/L1): Sprint
//   - A/Cross Button: Jump (implementation coming in Stage 2)
//
// ============================================================================
// TESTING CHECKLIST
// ============================================================================
//
// [] Player GameObject created with "Player" tag
// [] Capsule Collider added and configured
// [] Rigidbody added with correct constraint settings
//    - Rotation frozen on X, Y, Z
//    - Position Y NOT frozen (to allow gravity)
// [] PlayerInput component added and configured
// [] Ground Plane created for visual reference
// [] GameManager GameObject created and active
// [] Press Play in Unity
// [] Test WASD movement - player should move horizontally
// [] Test Shift + WASD - player should move faster
// [] Test on gamepad - left stick should move, LB should sprint
// [] Player rotates smoothly towards movement direction
// [] Player does NOT fall infinitely (gravity works)
// [] Movement stops smoothly when releasing input
// [] Sprinting is limited by cooldown
//
// ============================================================================
// TROUBLESHOOTING
// ============================================================================
//
// Problem: Player doesn't move
// Solution:
//   - Check Player tag is set correctly
//   - Verify GameManager is in the scene and active
//   - Check PlayerInput component is assigned the InputSystem_Actions
//   - Ensure Rigidbody is not kinematic (check "Body Type" = Dynamic)
//
// Problem: Player falls through the ground
// Solution:
//   - Ensure Rigidbody has Gravity enabled
//   - Check Ground Plane has a Collider
//   - Verify colliders are not set to "Is Trigger"
//
// Problem: Player rotates strangely
// Solution:
//   - Verify Rigidbody has rotation frozen on X, Y, Z
//   - Check PlayerRotationSpeed value is reasonable (10 is default)
//   - Ensure player model is oriented correctly (forward = +Z axis)
//
// Problem: Input not working
// Solution:
//   - Check PlayerInput component exists on Player
//   - Verify InputSystem_Actions asset is assigned
//   - Open InputSystem_Actions and verify Move/Sprint actions exist
//   - Check input bindings: WASD for Move, Shift for Sprint
//
// Problem: Sprint doesn't work
// Solution:
//   - Verify PlayerSprintMultiplier > 1.0 in GameConfig
//   - Check Sprint action exists in InputSystem_Actions
//   - Verify Sprint binding includes Shift key
//   - Ensure sprint cooldown isn't too long
//
// ============================================================================
