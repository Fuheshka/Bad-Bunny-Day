# Stage 1 Implementation Checklist

## ✅ Scripts Created

### Core
- [x] GameConfig.cs - Constants for speed, dash, camera offsets

### Input System
- [x] PlayerInputState.cs - Data model for move/dash input
- [x] PlayerInputHandler.cs - Reads Input System each frame (ITickable)

### Player
- [x] PlayerModel.cs - Holds Rigidbody reference and velocity state
- [x] PlayerMovement.cs - Velocity-based movement with dash (ITickable)

### Camera
- [x] CameraFollower.cs - Follows player with Diablo-style angle (ITickable)

### Managers & Setup
- [x] GameManager.cs - Main initialization point
- [x] GameInstaller.cs - Zenject DI bindings for all systems
- [x] STAGE1_SETUP_INSTRUCTIONS.md - Scene configuration guide

## 📋 Manual Scene Setup Required (Open SampleScene.unity)

### Physics Setup
- [ ] Add Rigidbody to Rabbit
  - Mass: 1, Drag: 0.3
  - Freeze Rotation X & Z
- [ ] Add Capsule Collider to Rabbit
  - Center: (0, 1, 0), Radius: 0.5, Height: 2

### Scene Hierarchy
- [ ] Create Ground Plane (scaled 10x10)
- [ ] Create 3-5 Obstacle Cubes for testing
- [ ] Create Scene Context (for Zenject)
- [ ] Add GameInstaller to Scene Context
- [ ] Ensure PlayerInput component exists with InputSystem_Actions

### Verification
- [ ] Input System C# code generated
- [ ] Camera tagged as "MainCamera"
- [ ] Rabbit has Animator (for CameraFollower to find)

## 🎮 Testing
Once scene is configured:
1. Press Play in Unity
2. Use WASD to move
3. Hold Shift to dash
4. Camera should follow from (-3, 2, -1) offset
5. Rabbit should collide with obstacles

## 🔍 Debug Info
- Check Console for initialization logs
- [PlayerInputHandler] should log on frame updates
- [GameManager] logs on game start
- If camera doesn't follow, check if Animator exists on rabbit

## Features Implemented
✅ Velocity-based movement (5 m/s base speed)
✅ Dash mechanic (2x speed, 0.5s cooldown on Shift)
✅ Player rotation towards movement direction
✅ Diablo-style camera follower
✅ Physics integration with Rigidbody
✅ Zenject DI architecture
✅ Input System integration (WASD/Gamepad Move, Shift Sprint)
✅ Friction/damping when not moving
