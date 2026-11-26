# Stage 1 Setup Guide: Scene Configuration

## Overview
Scripts have been created (`PlayerController.cs` and `CameraFollower.cs`). Now configure your Unity scene manually.

## Step-by-Step Scene Setup

### 1. Ground Plane
- In SampleScene, create a new GameObject: **Create > 3D Object > Plane**
- Name it: `Ground`
- Set Scale: X=10, Y=1, Z=20 (long garden bed shape)
- Position: (0, 0, 0)
- Add Material for visibility (light gray or green)

### 2. Player Character (Capsule)
- Create a new GameObject: **Create > 3D Object > Capsule**
- Name it: `Player`
- Position: (0, 1, 0)
- Set Height: 2, Radius: 0.5

**Add Physics:**
- Add Component > Rigidbody
- Mass: 1
- Drag: 0.5
- Angular Drag: 0.5
- Freeze Rotation: X, Z (to prevent tipping)
- Constraints should show: Freeze Rot X, Freeze Rot Z

**Add Scripts:**
- Add Component > PlayerController
- Add Component > CapsuleCollider (should already exist)
- Set Collider Height: 2, Radius: 0.5

### 3. Camera Setup
- Select `Main Camera` in hierarchy
- Position: (0, 3, -3)
- Rotation: (30, 0, 0) — tilted down for Diablo-like view

**Add Follow Script:**
- Add Component > CameraFollower
- Drag `Player` GameObject into the Target field
- Smooth Speed: 5
- Offset: (0, 3, -3)
- Look At Target: ✓ (checked)

### 4. Obstacles (Cubes)
Create 5-6 obstacles scattered on the map:
- Create: **Create > 3D Object > Cube**
- Name: `Obstacle_1`, `Obstacle_2`, etc.
- Position them around: (3, 0.5, 5), (-3, 0.5, -5), (5, 0.5, 0), etc.
- Set Scale: 1, 1, 1 (or vary sizes for visual interest)
- Add Material (brown or gray)

**Each Cube needs:**
- Box Collider (should auto-exist)
- Rigidbody (if you want physics): 
  - Body Type: Dynamic OR Static (Static = simpler for obstacles)
  - If Static: no need to configure further

### 5. Verify Scene Hierarchy
```
SampleScene
├── Main Camera
│   └── (CameraFollower script attached)
├── Directional Light
├── Player
│   ├── Rigidbody
│   ├── CapsuleCollider
│   └── PlayerController script
├── Ground
│   └── Box Collider
├── Obstacle_1
├── Obstacle_2
├── ...etc
```

## Input System Note
The new Input System is already configured in `InputSystem_Actions.inputactions`:
- **Move**: WASD or Joystick
- **Sprint**: Shift or Gamepad Button North

No additional setup needed—the scripts read these actions automatically.

## Testing Checklist
- [ ] Player capsule moves with WASD
- [ ] Player doesn't fall through ground
- [ ] Player collides with obstacle cubes
- [ ] Shift triggers dash (smooth/fast movement in facing direction)
- [ ] Camera follows player smoothly
- [ ] Camera doesn't clip through objects

## Troubleshooting
- **Player falling through ground?** → Ensure Ground has BoxCollider and both have Rigibody/Collider
- **Camera jittering?** → Increase CameraFollower smoothSpeed (e.g., 8-10)
- **Dash not working?** → Check Input System bindings; ensure Sprint action is mapped to Shift
- **Player stuck on obstacle?** → Check collider sizes match visual size

## Next Steps (Stage 2)
- Animation states for run/idle
- Sound effects for dash
- Visual effects (particle trails)
