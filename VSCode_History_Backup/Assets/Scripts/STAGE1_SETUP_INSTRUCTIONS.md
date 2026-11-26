# Stage 1 Scene Setup Instructions

## Prerequisites
- Open SampleScene.unity in Unity Editor
- Ensure Input System is properly configured
- Regenerate InputSystem C# code if needed (InputSystem_Actions.inputactions → right-click → Generate C# Code)

## Steps to Configure Scene for Stage 1

### 1. Setup Ground/Greybox
1. **Create Ground Plane:**
   - Right-click in Hierarchy → 3D Object → Plane
   - Name it "Ground"
   - Set Position to (0, 0, 0)
   - Set Scale to (10, 1, 10)
   - Add BoxCollider if not present

2. **Create Obstacles (Optional for testing):**
   - Create 3-5 Cubes as obstacles
   - Name them "Obstacle_1", "Obstacle_2", etc.
   - Place them around the scene at different positions
   - Ensure they have BoxColliders for collision

### 2. Setup Player (Rabbit)
1. **Find the Rabbit in the scene:**
   - It should already be in SampleScene
   - Select it in Hierarchy (look for "Rabbit" or similar)

2. **Add/Configure Rigidbody:**
   - Add Component → Physics → Rigidbody
   - Settings:
     - Mass: 1
     - Drag: 0.3
     - Angular Drag: 0.05
     - Freeze Rotation X: ✓ (check)
     - Freeze Rotation Z: ✓ (check)
     - Use Gravity: ✓ (check)

3. **Add/Configure Capsule Collider:**
   - Add Component → Physics → Capsule Collider
   - Settings:
     - Center: (0, 1, 0)
     - Radius: 0.5
     - Height: 2
     - Direction: Y-Axis

### 3. Setup Camera
1. **Adjust Main Camera position:**
   - Select "Main Camera" in Hierarchy
   - Position: (-3, 2, -1) relative to player spawn
   - Or adjust to (0, 0, 0) and let CameraFollower handle it
   - Set Near Clip: 0.1
   - Set Far Clip: 1000

### 4. Add Zenject Context to Scene
1. **Create Zenject Context:**
   - Right-click in Hierarchy → 3D Object → Scene Context
   - This auto-initializes Zenject

2. **Add GameInstaller:**
   - Right-click on Scene Context → Add Child
   - Add Component → GameInstaller (from Installers folder)
   - Attach the script to Scene Context

3. **Verify PlayerInput Component:**
   - Ensure there's a PlayerInput component in the scene
   - If not: Create empty GameObject → Add Component → PlayerInput
   - Set Input Actions to InputSystem_Actions
   - Uncheck "Submit" and "Cancel" interactions if present

### 5. Add GameManager (Optional)
1. **Create GameManager GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it "GameManager"
   - Add Component → GameManager (from Managers folder)

### 6. Verify Scene Structure
Final Hierarchy should look like:
```
Hierarchy:
├── Main Camera
├── Ground (Plane with BoxCollider)
├── Obstacle_1 (Cube with BoxCollider)
├── Obstacle_2 (Cube with BoxCollider)
├── Rabbit (with Rigidbody + Capsule Collider)
├── DirectionalLight
├── GlobalVolume
├── Scene Context (with GameInstaller)
└── PlayerInput (if needed)
```

### 7. Ready to Test!
- Press Play in Unity
- Use WASD to move the rabbit
- Hold Shift to dash (2x speed, 0.5s cooldown)
- Camera should follow from back-left angle
- Rabbit should collide with obstacles and ground

## Troubleshooting

**Rabbit doesn't move:**
- Check PlayerInput exists and has InputSystem_Actions assigned
- Check Input System C# code was generated
- Check Rigidbody is on the rabbit

**Camera doesn't follow:**
- Check CameraFollower script found the rabbit (check logs)
- Ensure main camera is tagged as "MainCamera"

**Rabbit falls through floor:**
- Check Ground has BoxCollider enabled
- Check Rigidbody UseGravity is enabled

## Input Mapping
- **Move**: WASD or Arrow Keys or Left Gamepad Stick
- **Dash**: Shift or Gamepad LB/RB
