// STAGE 2: ADVANCED MOTOR SKILLS (DASH AND STAMINA) - QUICK REFERENCE
//
// ============================================================================
// NEW SYSTEMS ADDED
// ============================================================================
//
// 1. STAMINA SYSTEM (StaminaSystem.cs)
//    - Manages player stamina pool (0-100 by default)
//    - Auto-regenerates when not sprinting or dashing
//    - Regeneration rate: 20 per second (configurable)
//    - Public methods:
//      * TryConsumeStamina(amount) - Consume stamina if available
//      * HasEnoughStamina(required) - Check if can perform action
//      * GetStaminaNormalized() - Get stamina as 0-1 value
//
// 2. DASH SYSTEM (DashSystem.cs)
//    - Performs dash action with stamina cost and cooldown
//    - Generates high-priority noise event (priority 10)
//    - Ignores drag/slow effects during dash
//    - Public methods:
//      * GetDashDirection() - Direction of current dash
//      * GetDashProgress() - Normalized progress (0-1)
//      * CanDash() - Check if dash is available now
//
// 3. NOISE GENERATOR (NoiseGenerator.cs)
//    - Global event system for noise events
//    - Allows enemies to detect player actions
//    - Dash generates noise with position, priority, and type
//    - Usage: NoiseGenerator.GenerateNoise(position, priority, type)
//
// ============================================================================
// STAMINA MECHANICS
// ============================================================================
//
// Max Stamina: 100
// Regen Rate: 20/sec (takes 5 seconds to fully recover)
// Sprint Cost: None (but prevents stamina regen while sprinting)
// Dash Cost: 30 stamina
// Dash Threshold: Must have minimum 30 stamina to attempt dash
//
// Stamina regenerates automatically when:
//   - Not currently dashing
//   - Not currently sprinting
//   - Capped at max value
//
// ============================================================================
// DASH MECHANICS
// ============================================================================
//
// Activation: Press Sprint button (Shift/LB) while not sprinting
// Duration: 0.3 seconds (very quick burst)
// Speed: 3x base movement speed
// Direction: Towards movement input or forward if no input
// Cooldown: 0.5 seconds after dash ends
// Cost: 30 stamina
// Noise: High priority (10) to alert enemies
//
// During dash:
//   - Drag modifiers are IGNORED (no slowdown from surfaces)
//   - High speed burst in chosen direction
//   - High noise event generated
//   - Cannot perform another dash until cooldown expires
//
// ============================================================================
// NEW CONFIGURATION VALUES (GameConfig.cs)
// ============================================================================
//
// STAMINA:
//   PlayerMaxStamina = 100f
//   PlayerStaminaRegenRate = 20f (per second)
//   PlayerDashStaminaCost = 30f
//   PlayerDashStaminaThreshold = 30f (minimum required)
//
// DASH:
//   PlayerDashDuration = 0.3f (seconds)
//   PlayerDashSpeedMultiplier = 3f (applied to base speed)
//   PlayerDashCooldown = 0.5f (seconds)
//   PlayerDashNoisePriority = 10 (high priority for enemies)
//
// ============================================================================
// ARCHITECTURE
// ============================================================================
//
// File Structure:
//   Core/
//     - GameConfig.cs (extended with dash/stamina constants)
//     - GameManager.cs (updated to initialize all systems)
//     - NoiseGenerator.cs (NEW - global noise event system)
//
//   Character/
//     - PlayerModel.cs (extended with stamina and dash state)
//     - PlayerInputState.cs (extended with DashPressed property)
//     - PlayerMovement.cs (updated to apply dash boost)
//     - StaminaSystem.cs (NEW - manages stamina)
//     - DashSystem.cs (NEW - handles dash mechanics)
//
//   Input/
//     - PlayerInputHandler.cs (updated to read dash input)
//
// Update Flow:
//   1. PlayerInputHandler (Update) reads Shift key → sets DashPressed
//   2. StaminaSystem (Update) regenerates stamina when idle
//   3. DashSystem (FixedUpdate) checks input and activates dash
//   4. PlayerMovement (FixedUpdate) applies dash velocity boost
//   5. NoiseGenerator invoked during dash start
//
// ============================================================================
// INPUT MAPPING
// ============================================================================
//
// Dash is triggered by the SAME input as Sprint (Shift/LB)
// Differentiation logic:
//   - If already sprinting: Cannot dash (sprint takes priority)
//   - If in dash cooldown: Cannot dash
//   - If insufficient stamina: Cannot dash
//   - Otherwise: Initiates dash when button pressed
//
// The DashSystem checks these conditions before starting dash
//
// ============================================================================
// NOISE SYSTEM
// ============================================================================
//
// NoiseEvent contains:
//   - Position (where noise occurred)
//   - Priority (1-10+, higher = more important)
//   - NoiseType (string, e.g., "Dash", "Land", "Run")
//   - DetectionRadius (how far enemies can hear it)
//
// Subscribe to noise events:
//   NoiseGenerator.OnNoiseGenerated += MyNoiseHandler;
//
// Enemies can listen for high-priority dash noises and react
//
// ============================================================================
// TESTING CHECKLIST
// ============================================================================
//
// Stamina System:
//   [] Player starts with 100 stamina
//   [] Stamina regenerates when standing still (20/sec)
//   [] Stamina does not regenerate while sprinting
//   [] Stamina display shows correct values
//
// Dash Mechanics:
//   [] Cannot dash with insufficient stamina (< 30)
//   [] Dash costs exactly 30 stamina
//   [] Dash lasts exactly 0.3 seconds
//   [] Dash moves at 3x base speed (15 units/sec)
//   [] Dash moves in movement direction or forward
//   [] Cannot dash again for 0.5 seconds after dash ends
//   [] Dash ignores ground friction/slowdown effects
//
// Input:
//   [] Pressing Sprint while sprinting = normal sprint (no dash)
//   [] Pressing Sprint while idle + enough stamina = dash
//   [] Can dash in any direction (forward, sideways, backward)
//
// Noise Generation:
//   [] Dash generates noise event with priority 10
//   [] Noise event contains correct position
//   [] Noise event type is "Dash"
//
// ============================================================================
// KNOWN LIMITATIONS & FUTURE IMPROVEMENTS
// ============================================================================
//
// Current:
//   - Dash only works on ground (no air dashing)
//   - Sprint and dash share same input button
//   - No visual feedback for stamina in basic implementation
//
// Future (Stage 3+):
//   - Air dash mechanics
//   - Dash direction prediction for enemies
//   - Stamina UI display
//   - Different noise types (footsteps, landing, etc.)
//   - Noise propagation system
//
// ============================================================================
