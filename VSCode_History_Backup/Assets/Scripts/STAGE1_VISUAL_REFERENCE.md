# STAGE 1 - ВИЗУАЛЬНЫЕ ПРИМЕРЫ КОНФИГУРАЦИИ

## 🖼️ Как должна выглядеть Hierarchy в Unity

```
Hierarchy (SampleScene):
│
├─ 📷 Main Camera
│   └─ Tag: MainCamera
│   └─ Components:
│       ├─ Transform
│       └─ Camera
│
├─ 🌍 Ground (Plane)
│   └─ Position: (0, 0, 0)
│   └─ Scale: (10, 1, 10)
│   └─ Components:
│       ├─ Transform
│       ├─ Mesh Filter (with Plane mesh)
│       ├─ Mesh Renderer
│       └─ Box Collider
│
├─ 🟥 Obstacle_1 (Cube)
│   └─ Position: (3, 1, 0)
│   └─ Components:
│       ├─ Transform
│       ├─ Mesh Filter (with Cube mesh)
│       ├─ Mesh Renderer
│       └─ Box Collider
│
├─ 🟥 Obstacle_2 (Cube)
│   └─ Position: (-3, 1, 2)
│   └─ Components: (same as Obstacle_1)
│
├─ 🐰 Rabbit
│   └─ (модель FBX с Animator)
│   └─ Position: (0, 0, 0)
│   └─ Components:
│       ├─ Transform
│       ├─ Mesh Filter
│       ├─ Mesh Renderer
│       ├─ Animator ✓
│       ├─ Rigidbody ✓
│       └─ Capsule Collider ✓
│
├─ 💡 Directional Light
│   └─ Components:
│       ├─ Transform
│       └─ Light
│
├─ 📦 Global Volume (URP)
│   └─ Components: Volume
│
├─ 🎮 PlayerInput
│   └─ GameObject (пустой или простой)
│   └─ Components:
│       ├─ Transform
│       └─ Player Input ✓
│
└─ ⚙️ Scene Context
    └─ (специальный Zenject объект)
    └─ Components:
        ├─ Transform
        ├─ Scene Context (Zenject)
        └─ Game Installer ✓
```

---

## 🔍 Как выглядит Inspector для Rabbit

```
╔════════════════════════════════════════════════════════════════╗
║ RABBIT INSPECTOR VIEW                                          ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║ Name: Rabbit                                                   ║
║ Tag: Untagged         Layer: Default                           ║
║                                                                ║
║ ▼ Transform                                                    ║
║   Position: (0, 0, 0)                                          ║
║   Rotation: (0, 0, 0)                                          ║
║   Scale: (1, 1, 1)                                             ║
║                                                                ║
║ ▼ Mesh Filter                                                  ║
║   Mesh: Rabbit_mesh                                            ║
║                                                                ║
║ ▼ Mesh Renderer                                                ║
║   Cast Shadows: On                                             ║
║   ...                                                          ║
║                                                                ║
║ ▼ Animator                   ← ДОЛЖЕН БЫТЬ!                   ║
║   Controller: Rabbit_Animator (AnimatorController)            ║
║   ...                                                          ║
║                                                                ║
║ ▼ Rigidbody                  ← ДОЛЖЕН БЫТЬ!                   ║
║   Mass: 1                                                      ║
║   Drag: 0.3                                                    ║
║   Angular Drag: 0.05                                           ║
║   Use Gravity: ✓                                               ║
║   Is Kinematic: ☐                                              ║
║   Collision Detection: Discrete                               ║
║   Constraints:                                                 ║
║     Freeze Position: ☐ ☐ ☐                                    ║
║     Freeze Rotation: ✓ ☐ ✓  (X и Z заморожены!)              ║
║                                                                ║
║ ▼ Capsule Collider           ← ДОЛЖЕН БЫТЬ!                   ║
║   Center: (0, 1, 0)                                            ║
║   Radius: 0.5                                                  ║
║   Height: 2                                                    ║
║   Direction: Y-Axis                                            ║
║   Is Trigger: ☐                                                ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 🔍 Как выглядит Inspector для Main Camera

```
╔════════════════════════════════════════════════════════════════╗
║ MAIN CAMERA INSPECTOR VIEW                                     ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║ Name: Main Camera                                              ║
║ Tag: MainCamera        ← ОБЯЗАТЕЛЬНО!                          ║
║ Layer: Default                                                 ║
║                                                                ║
║ ▼ Transform                                                    ║
║   Position: (0, 2, -3)  ← примерно                            ║
║   Rotation: (0, 0, 0)                                          ║
║   Scale: (1, 1, 1)                                             ║
║                                                                ║
║ ▼ Camera                                                       ║
║   Projection: Perspective                                      ║
║   Field of View: 60                                            ║
║   Near Clip Plane: 0.1                                         ║
║   Far Clip Plane: 1000                                         ║
║   ...                                                          ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 🔍 Как выглядит Inspector для Ground

```
╔════════════════════════════════════════════════════════════════╗
║ GROUND INSPECTOR VIEW                                          ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║ Name: Ground                                                   ║
║ Tag: Untagged         Layer: Default                           ║
║                                                                ║
║ ▼ Transform                                                    ║
║   Position: (0, 0, 0)                                          ║
║   Rotation: (0, 0, 0)                                          ║
║   Scale: (10, 1, 10)   ← важно для размера!                   ║
║                                                                ║
║ ▼ Mesh Filter                                                  ║
║   Mesh: Plane                                                  ║
║                                                                ║
║ ▼ Mesh Renderer                                                ║
║   ...                                                          ║
║                                                                ║
║ ▼ Box Collider         ← ДОЛЖЕН БЫТЬ!                         ║
║   Center: (0, 0, 0)                                            ║
║   Size: (1, 1, 1)                                              ║
║   Is Trigger: ☐                                                ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 🔍 Как выглядит Inspector для PlayerInput

```
╔════════════════════════════════════════════════════════════════╗
║ PLAYERINPUT INSPECTOR VIEW                                     ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║ Name: PlayerInput                                              ║
║ Tag: Untagged         Layer: Default                           ║
║                                                                ║
║ ▼ Transform                                                    ║
║   Position: (0, 0, 0)                                          ║
║                                                                ║
║ ▼ Player Input         ← ДОЛЖЕН БЫТЬ!                         ║
║   Actions: InputSystem_Actions   ← выбери из дропдауна        ║
║   Default Control Scheme: Keyboard                             ║
║   Default Action Map: Player                                   ║
║   Camera: (none)                                               ║
║   UI Input Module: (none)                                      ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 🔍 Как выглядит Inspector для Scene Context

```
╔════════════════════════════════════════════════════════════════╗
║ SCENE CONTEXT INSPECTOR VIEW                                   ║
╠════════════════════════════════════════════════════════════════╣
║                                                                ║
║ Name: Scene Context                                            ║
║ Tag: Untagged         Layer: Default                           ║
║                                                                ║
║ ▼ Transform                                                    ║
║   Position: (0, 0, 0)                                          ║
║                                                                ║
║ ▼ Scene Context (Zenject Component)                            ║
║   (это часть Zenject фреймворка)                               ║
║                                                                ║
║ ▼ Game Installer       ← ДОЛЖЕН БЫТЬ!                         ║
║   Player Prefab: (none) - это поле можно оставить пустым      ║
║   (скрипт найдёт rabbit автоматически)                        ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
```

---

## 📊 InputSystem_Actions как должен выглядеть

### Action Map: Player

```
Player (Action Map)
├─ Move (Action)
│  ├─ Type: Value
│  ├─ Control Type: Vector2
│  ├─ Bindings:
│  │  ├─ Keyboard: WASD → (W:-Y, S:+Y, A:-X, D:+X)
│  │  └─ Gamepad: Left Stick
│
├─ Sprint (Action)
│  ├─ Type: Button
│  ├─ Bindings:
│  │  ├─ Keyboard: Left Shift
│  │  └─ Gamepad: LB / RB
```

---

## 🎮 ПОЛНЫЙ ПРИМЕР СЦЕНЫ В GAME VIEW

```
           ┌─────────────────────┐
           │                     │
           │      GAME VIEW      │
           │                     │
   ┌───────┴─────────────────────┴───────┐
   │                                       │
   │  🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩  │ ← Ground (Plane)
   │  🟩                           🟩  │
   │  🟩           📷              🟩  │ ← Camera (в углу)
   │  🟩                           🟩  │
   │  🟩      🐰 (Rabbit)          🟩  │ ← Player
   │  🟩         (A WASD)           🟩  │
   │  🟩                           🟩  │
   │  🟩  🟥 Obstacle_1            🟩  │ ← Obstacle (куб)
   │  🟩                           🟩  │
   │  🟩       🟥 Obstacle_2       🟩  │ ← Obstacle (куб)
   │  🟩                           🟩  │
   │  🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩 🟩  │
   │                                    │
   └────────────────────────────────────┘

   УПРАВЛЕНИЕ:
   W / ↑ = вперёд
   A / ← = влево
   S / ↓ = назад
   D / → = вправо
   Shift = рывок (2x скорость)
```

---

## ✅ ПОСЛЕДНЯЯ ПРОВЕРКА

**Перед тем как нажать Play, убедись:**

```
□ Rabbit имеет 3 компонента:
  □ Animator
  □ Rigidbody (с нужными настройками)
  □ Capsule Collider (с нужными настройками)

□ Main Camera имеет Tag "MainCamera"

□ Ground существует с BoxCollider

□ PlayerInput компонент в сцене с Actions = InputSystem_Actions

□ Scene Context существует с GameInstaller компонентом

□ Сохранена сцена (Ctrl+S)
```

**Если ВСЁ верно → нажми Play и наслаждайся! 🎉**

