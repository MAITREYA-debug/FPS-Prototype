# FPS Movement & Weapon Feel Prototype

A first-person shooter prototype built in Unity, focused on responsive character movement, procedural weapon sway, aim-down-sights alignment, and animation-driven gun feedback inside a greybox test environment.

---

## Overview

This project is a portfolio-oriented FPS sandbox for iterating on core first-person mechanics before building out full game systems. The playable experience lives in `SampleScene`, where a first-person character navigates the **Prototype Map** greybox level with a Desert Eagle-style weapon.

The architecture is component-based and intentionally lightweight. **`InputManager`** reads player and weapon actions from the Unity Input System and exposes movement, look, and action state to the rest of the codebase. **`Player_Controller`** handles locomotion, camera pitch, crouch height adjustment, and ground-state events. **`Weapon_Controller`** layers procedural weapon feel on top — look/move sway, idle breathing, ADS positioning, fire-rate-limited spawning, and weapon animator parameters driven by player state.

Rendering uses the **Universal Render Pipeline (URP)** with separate PC and mobile renderer assets. Hand placement on the weapon is supported in-scene through **Animation Rigging** (two-bone IK constraints and a `RigBuilder` on the player hierarchy). The project is early-stage by design: several input actions and scene objects are wired for future systems that are not yet implemented in code.

---

## Features

### Player Controller
- **CharacterController-based movement** with configurable base speed and state-based speed multipliers
- **Walk, sprint, crouch, and aim movement states** managed by an internal speed-state enum
- **Jumping** with custom gravity and vertical velocity handling
- **Fall detection** with a short air-time threshold before entering a fall movement state
- **Sprint cancellation** when not moving forward
- **Crouch** via smooth `CharacterController` height and center interpolation, with camera height following the controller center

### Camera
- **First-person mouse look** with horizontal body rotation and pitch-clamped vertical camera rotation on a child `CameraHolder`
- **ADS alignment reference** using an `InSight` transform on the weapon as the scope/camera alignment point

### Input System
- **Centralized input** through a singleton `InputManager` backed by `InputActions.inputactions`
- **Separate Player and Weapon action maps** for locomotion vs. fire/aim inputs
- **Keyboard + mouse bindings** for all active gameplay actions
- **Partial gamepad support** — movement is bound to `<Joystick>/stick`; look is mouse-only in the current input asset

### Weapons
- **Desert Eagle 3D model** with PBR textures and in-scene weapon hierarchy (`Weapon Pivot`, `WeaponSway`, `WeaponAnimation`)
- **Procedural weapon sway** driven by look and movement input, with reduced intensity while aiming
- **Idle weapon breathing** using a Lissajous curve offset
- **Aim-down-sights (ADS)** with `SmoothDamp` positioning toward the camera/scope alignment target
- **Fire-rate-limited shooting** that instantiates a bullet prefab at a muzzle spawn point

### Shooting
- **Left-click fire** with configurable fire rate (`fireRate` on `Weapon_Controller`)
- **Bullet prefab spawning** at `BulletSpawnPoint`
- **Automatic bullet cleanup** after a configurable lifetime via `Bullet_script`
- ⚠️ **Partial implementation** — bullets do not use physics, raycasts, or movement logic; they spawn in place and are destroyed after 1 second. There is no damage or hit detection.

### Animation
- **Gun Animator controller** with idle, walk, run, jump start, falling, and landing states
- **Animator parameters** driven from code: `Speed`, `Sprinting`, `isGrounded`
- **Animator triggers** for jump, fall, and land synced from `Player_Controller` ground-state events
- **Animation Rigging** in the scene for left/right hand IK targeting on the weapon rig
- ⚠️ **Partial implementation** — `Player_Controller` references a player body `Animator`, but it is not assigned in the scene; body locomotion animation is not currently driven by code

### Physics
- **CharacterController** collision and grounding for the player
- **MeshCollider** on the imported Prototype Map environment geometry

### Audio
- **Gunshot audio clip** on the bullet prefab's `AudioSource`, set to play on spawn
- ⚠️ **Partial implementation** — firing sound is tied to bullet instantiation, not to a dedicated weapon audio system; no footstep, reload, or ambient audio logic in scripts

### UI
- **Canvas scaffold** present in the scene (Screen Space Overlay with `CanvasScaler` and `GraphicRaycaster`)
- ⚠️ **Partial implementation** — contains a small placeholder `Image` element only; no HUD, crosshair logic, or gameplay UI scripts

### AI
- Not implemented

### Saving
- Not implemented

### Utilities
- **Event-based jump input** — `InputManager.OnJump` notifies `Player_Controller`
- **Interact and reload events declared** on `InputManager` (`OnInteract`, `OnReload`) but not consumed by any gameplay scripts
- ⚠️ **Scene note** — the player object references a missing `PlayerShoot` script; this component should be removed or reimplemented

---

## Technologies Used

| Technology | Role in Project |
|---|---|
| **Unity 6000.3.16f1** | Game engine and editor |
| **C#** | All custom gameplay logic |
| **Universal Render Pipeline (URP)** | Rendering pipeline (`PC_RPAsset`, `Mobile_RPAsset`, volume profiles) |
| **Unity Input System** | Player/weapon input via `InputActions.inputactions` |
| **Unity Animator** | Weapon animation state machine |
| **Animation Rigging** | Hand IK (`RigBuilder`, `TwoBoneIKConstraint`) in `SampleScene` |
| **CharacterController** | Player movement and grounding |
| **Unity UI (uGUI)** | Canvas scaffold in scene (minimal) |

**Installed packages not currently used in gameplay code or scene setup:** Cinemachine, AI Navigation (no baked NavMesh data), Timeline, Visual Scripting, Test Framework.

---

## Project Structure

```
Prototyping/
├── Assets/
│   ├── fps/                          # Core FPS content and scripts
│   │   ├── Animation/                # Gun animator controller and clips
│   │   │   └── Gun/                  # idle, walk, run, jump, fall, land animations
│   │   ├── Material/                 # Character and weapon materials
│   │   ├── Models/3D/
│   │   │   ├── Humunoid/Soldier/     # Hand/arm mesh for first-person rig
│   │   │   └── Object/               # Desert Eagle model, bullet prefab, audio
│   │   ├── Scripts/
│   │   │   ├── InputManager.cs       # Input System singleton
│   │   │   ├── Player/
│   │   │   │   └── Player_Controller.cs
│   │   │   └── Weapons/
│   │   │       ├── Weapon_Controller.cs
│   │   │       └── Bullet_script.cs
│   │   └── Texture/Desert Eagle/     # PBR texture maps
│   ├── AngeloMaN87/Prototype Map/    # Third-party greybox environment
│   │   ├── Models/                   # FBX level geometry
│   │   ├── Materials/                # Prototype map materials
│   │   ├── Textures/                 # Color-coded prototype textures
│   │   └── Prefabs/PrototypeMap.prefab
│   ├── Scenes/
│   │   └── SampleScene.unity         # Main playable scene
│   ├── Settings/                     # URP assets and volume profiles
│   ├── InputActions.inputactions     # Active Input System asset
│   └── InputActions.cs               # Generated C# wrapper for input
├── Packages/                         # Unity package manifest and lock file
├── ProjectSettings/                  # Editor, physics, and pipeline settings
└── README.md
```

**Other assets in the repo (not part of active gameplay flow):**
- `Assets/InputSystem_Actions.*` — default Unity Input System template (unused)
- `Assets/Player.inputactions` — alternate input asset (unused by `InputManager`)
- `Assets/TutorialInfo/` — Unity template readme/editor utilities

---

## Controls

Bindings are defined in `Assets/InputActions.inputactions` and read by `InputManager`.

| Action | Keyboard / Mouse | Gamepad |
|---|---|---|
| Move | `W` `A` `S` `D` | Left stick |
| Look | Mouse delta | — *(not bound)* |
| Sprint | `Shift` (hold) | — *(not bound)* |
| Jump | `Space` | — *(not bound)* |
| Crouch | `Ctrl` (hold) | — *(not bound)* |
| Interact | `E` | — *(not bound)* |
| Fire | Left mouse button | — *(not bound)* |
| Aim (ADS) | Right mouse button | — *(not bound)* |

> **Note:** `Interact` raises an input event but has no gameplay handler subscribed. `Reload` is declared in code but not bound in the active input asset.

---

## Architecture

```
InputActions (Input System asset)
        │
        ▼
  InputManager (singleton)
   │         │         │
   │         │         └── Weapon map: isFiring, isAimingIn
   │         └── Player map: Move, Look, Sprint, Jump, Crouch, Interact
   │
   ├── event: OnJump ──────────────► Player_Controller.jump()
   │
   ▼
Player_Controller
   ├── StateManager()          → speed multipliers (walk/run/crouch/fall/aim)
   ├── HandleMovement()        → CharacterController locomotion + gravity
   ├── HandleLook()            → yaw on body, pitch on CameraHolder
   ├── HandleCrouch()          → height/center lerp + camera offset
   └── JumpEvents()            → ground/fall/land detection
           │
           │  weapon_Controller.onjump() / Falling() / OnLanding()
           ▼
Weapon_Controller
   ├── Sway_Look_Calculation()   → input-driven local rotation
   ├── Sway_Idle_Calculation()   → Lissajous idle offset
   ├── isAiming_Calculation()    → ADS SmoothDamp toward scope alignment
   ├── Shoot()                   → fire-rate gate + bullet Instantiate()
   └── Animator parameters       → Speed, Sprinting, isGrounded
           │
           ▼
     Bullet_script (on spawned prefab)
           └── Destroy after lifetime
```

### Design patterns and conventions
- **Singleton access** — `InputManager.instance` provides global read-only input state
- **Event delegation** — jump uses a static C# event rather than direct input polling inside `Player_Controller`
- **Explicit initialization** — `Weapon_Controller.Initialization(Player_Controller)` resolves player references from the parent hierarchy at runtime
- **State-driven movement** — a private `SpeedState` enum selects movement multipliers each frame
- **Separation of concerns** — player locomotion/camera vs. weapon feel/animation/firing live in separate components

### Scene hierarchy (simplified)
- `InputManager` — input singleton
- Player root — `CharacterController`, `Player_Controller`, Animation Rigging, Desert Eagle prefab instance
  - `CameraHolder` → `Camera`, `Weapon Pivot` → `WeaponSway` → `WeaponAnimation` (gun `Animator`), `InSight`, `BulletSpawnPosition`
- `PrototypeMap` prefab instance — greybox level
- `Canvas` — UI scaffold
- `Global Volume` — URP post-processing volume

---

## Installation

### Prerequisites
- [Unity Hub](https://unity.com/download)
- **Unity 6000.3.16f1** (Unity 6) — version recorded in `ProjectSettings/ProjectVersion.txt`

### Setup
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Prototyping
   ```
2. Open **Unity Hub** → **Add project from disk** → select the `Prototyping` folder.
3. Allow Unity to import assets and restore packages from `Packages/manifest.json`.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play**.

No manual package installation is required beyond opening the project in the correct Unity version.

---

## Screenshots

<!-- Replace placeholders with exported images or GIFs -->

| Gameplay | Description |
|---|---|
| ![Gameplay screenshot 1](docs/screenshots/gameplay-01.png) | First-person view in the Prototype Map greybox level |
| ![Gameplay screenshot 2](docs/screenshots/gameplay-02.png) | Weapon sway and movement in action |
| ![Gameplay GIF](docs/screenshots/ads-demo.gif) | Aim-down-sights alignment demo |
| ![Gameplay GIF](docs/screenshots/jump-anim.gif) | Jump / fall / land weapon animation sync |

> Create a `docs/screenshots/` folder and add PNG or GIF captures from Play Mode. GIFs work well for sway, ADS, and jump animation feedback.

---

## Future Improvements

Realistic next steps based on the current codebase:

- Add **projectile movement** (rigidbody velocity or raycast hitscan) and **impact/damage** handling
- Wire **reload** into the Input System and implement ammo + reload animation states
- Hook up **Interact** to actual world interactions or pickup logic
- Replace or remove the **missing `PlayerShoot`** component reference on the player
- Drive **player body animation** or remove unused animator references
- Build a proper **HUD** (crosshair, ammo counter, interaction prompts)
- Move **weapon audio** to a dedicated firing path instead of only the bullet prefab
- Add **gamepad look** bindings and input device switching
- Introduce **ScriptableObject-based weapon data** if multiple weapons are planned
- Bake **NavMesh** and add AI only if enemy navigation becomes a project goal
- Evaluate **Cinemachine** for camera polish if procedural camera work grows in scope

---

## Third-Party Assets

- **Prototype Map** by AngeloMaN87 — greybox environment (`Assets/AngeloMaN87/Prototype Map/`)
- **Gunshot audio** — `eaglaxle-gun-shot-1-530788.mp3` (referenced on the bullet prefab)

Third-party assets retain their original licenses.

---

## Repository Information

**Suggested GitHub description:**
> Unity 6 FPS prototype focused on CharacterController movement, procedural weapon sway, ADS feel, and animation-driven gun feedback in URP.

**Suggested topics:**
`unity` `unity3d` `fps` `first-person-shooter` `game-development` `csharp` `unity-input-system` `urp` `prototype` `portfolio` `character-controller` `game-feel`

---

## License

Personal / portfolio project. See third-party asset licenses for included store content.
