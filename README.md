# FPS Movement & Weapon Feel Prototype

A Unity 6 first-person shooter prototype focused on responsive movement, procedural weapon sway, aim-down-sights alignment, raycast shooting, and animation-driven gun feedback inside a greybox test environment.

Unity
URP
C#
Platform

---

## Overview

This project is a portfolio-oriented FPS sandbox for iterating on core first-person mechanics before building out full game systems. The playable experience lives in `SampleScene`, where a first-person character navigates the **Prototype Map** greybox level with a Desert Eagle-style weapon.

The architecture is component-based and intentionally lightweight:


| Component           | Responsibility                                                         |
| ------------------- | ---------------------------------------------------------------------- |
| `InputManager`      | Singleton input hub backed by the Unity Input System                   |
| `Player_Controller` | Locomotion, camera pitch, crouch, jump/fall/land events                |
| `Weapon_Controller` | Procedural sway, idle breathing, ADS positioning, weapon animator sync |
| `Weapon_Shooting`   | Raycast fire, ammo, reload, muzzle flash, hit VFX                      |
| `Bullet_script`     | Visual projectile travel from muzzle to impact point                   |


Rendering uses the **Universal Render Pipeline (URP)**. Hand placement is supported through **Animation Rigging** (two-bone IK constraints on the player hierarchy).

---



## Features



### Player Controller

- **CharacterController-based movement** with configurable base speed and state-based multipliers
- **Walk, sprint, crouch, aim, and fall** movement states
- **Jumping** with custom gravity and vertical velocity
- **Fall detection** with a short air-time threshold before entering fall state
- **Sprint only while moving forward** — sprint is ignored when strafing or moving backward
- **Smooth crouch** via interpolated `CharacterController` height and camera offset



### Camera

- **First-person mouse look** with pitch-clamped vertical rotation on `CameraHolder`
- **ADS alignment** using an `InSight` transform as the scope/camera reference point



### Input System

- **Centralized input** through `InputManager` and `InputActions.inputactions`
- **Separate Player and Weapon action maps**
- **Keyboard + mouse bindings** for all active gameplay actions
- **Partial gamepad support** — movement on left stick; look is mouse-only today



### Weapons & Shooting

- **Desert Eagle 3D model** with PBR textures and weapon hierarchy (`Weapon Pivot` → `WeaponSway` → `WeaponAnimation`)
- **Procedural weapon sway** driven by look and movement input, reduced while aiming
- **Idle weapon breathing** using a Lissajous curve offset
- **Aim-down-sights (ADS)** with `SmoothDamp` positioning toward the scope alignment target
- **Raycast-based shooting** from screen center with optional movement spread
- **Fire-rate limiting**, magazine ammo, and reload on `R`
- **Muzzle flash**, bullet trail travel, and surface hit VFX
- ⚠️ **No damage system** — hits spawn visual feedback only



### Animation

- **Gun Animator** with idle, walk, run, jump, fall, and land states
- **Animator parameters** from code: `Speed`, `Sprinting`, `isGrounded`
- **Animator triggers** for jump, fall, and land synced from player ground events
- **Animation Rigging** for left/right hand IK on the weapon rig



### UI

- **Ammo counter** (TextMeshPro) updated on fire and reload
- ⚠️ **No crosshair, health bar, or interaction prompts yet**



### Not Yet Implemented

- Enemy AI / NavMesh gameplay
- Damage, health, and destructible targets
- Save/load
- Footstep, reload, and dedicated weapon audio systems
- Full gamepad look bindings

---



## Technologies


| Technology                          | Role                           |
| ----------------------------------- | ------------------------------ |
| **Unity 6000.3.16f1**               | Game engine                    |
| **C#**                              | Gameplay logic                 |
| **Universal Render Pipeline (URP)** | Rendering                      |
| **Unity Input System**              | Player and weapon input        |
| **Unity Animator**                  | Weapon animation state machine |
| **Animation Rigging**               | Hand IK in scene               |
| **CharacterController**             | Player movement and grounding  |
| **TextMeshPro + uGUI**              | Ammo HUD                       |


**Installed but unused in gameplay:** Cinemachine, AI Navigation, Timeline, Visual Scripting, Test Framework.

---



## Project Structure

```
Prototyping/
├── Assets/
│   ├── fps/
│   │   ├── Animation/Gun/          # Weapon animator + clips
│   │   ├── Material/
│   │   ├── Models/3D/
│   │   │   ├── Humunoid/Soldier/   # First-person arm mesh
│   │   │   └── Object/             # Desert Eagle, bullet, audio
│   │   ├── Scripts/
│   │   │   ├── InputManager.cs
│   │   │   ├── Player/
│   │   │   │   ├── Player_Controller.cs
│   │   │   │   └── Weapon_Shooting.cs
│   │   │   └── Weapons/
│   │   │       ├── Weapon_Controller.cs
│   │   │       └── Bullet_script.cs
│   │   └── Texture/Desert Eagle/
│   ├── AngeloMaN87/Prototype Map/  # Greybox environment
│   ├── Scenes/SampleScene.unity    # Main playable scene
│   ├── Settings/                   # URP assets
│   └── InputActions.inputactions
├── Packages/
├── ProjectSettings/
├── README.md
└── LINKEDIN_POST.md
```

---



## Controls


| Action    | Keyboard / Mouse             | Gamepad                      |
| --------- | ---------------------------- | ---------------------------- |
| Move      | `W` `A` `S` `D`              | Left stick                   |
| Look      | Mouse                        | —                            |
| Sprint    | `Shift` (hold, forward only) | —                            |
| Jump      | `Space`                      | —                            |
| Crouch    | `Ctrl` (hold)                | —                            |
| Interact  | `E`                          | — *(event only, no handler)* |
| Fire      | Left mouse                   | —                            |
| Aim (ADS) | Right mouse                  | —                            |
| Reload    | `R`                          | —                            |


---



## Architecture

```
InputActions
      │
      ▼
InputManager (singleton)
 ├── Player map → Move, Look, Sprint, Jump, Crouch, Interact
 ├── Weapon map → Fire, Aim, Reload
 └── Events: OnJump, OnInteract, OnReload
      │
      ├──────────────────────────────┐
      ▼                              ▼
Player_Controller              Weapon_Shooting
 ├── movement / look / crouch   ├── raycast fire + spread
 ├── jump / fall / land events   ├── ammo + reload
 └── weapon anim speed           └── VFX (flash, hit, bullet)
      │
      ▼
Weapon_Controller
 ├── procedural sway + idle breathing
 ├── ADS SmoothDamp alignment
 └── weapon Animator sync
      │
      ▼
Bullet_script → visual travel to impact point
```

---



## Getting Started



### Prerequisites

- [Unity Hub](https://unity.com/download)
- **Unity 6000.3.16f1** (see `ProjectSettings/ProjectVersion.txt`)



### Setup

1. Clone the repository:
  ```bash
   git clone https://github.com/<your-username>/Prototyping.git
   cd Prototyping
  ```
2. Open Unity Hub → **Add project from disk** → select the `Prototyping` folder.
3. Wait for package restore and asset import.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play**.

No extra package installation is required beyond the correct Unity version.

---



## Screenshots & Demo

Add captures to `docs/screenshots/` for the best GitHub presentation:


| File                | Suggested content                    |
| ------------------- | ------------------------------------ |
| `gameplay-01.png`   | First-person view in the greybox map |
| `gameplay-02.png`   | Weapon sway while moving             |
| `ads-demo.gif`      | ADS alignment                        |
| `shooting-demo.gif` | Fire, muzzle flash, and hit VFX      |


---



## Roadmap

Suggested next steps based on the current codebase:

- **Combat:** damageable targets, health, headshot multipliers, object pooling for bullets/VFX
- **Weapons:** reload animation, empty-mag handling, ScriptableObject weapon data for multiple guns
- **Audio:** dedicated fire/reload/footstep systems decoupled from prefab spawn
- **UI:** crosshair, reload prompt, interaction prompts
- **Input:** gamepad look, aim assist toggle, rebinding UI
- **World:** interactable doors/pickups wired to `OnInteract`
- **AI:** NavMesh bake + basic enemy patrol/chase (if scope expands)
- **Polish:** recoil kick, camera shake, procedural crosshair spread

---



## Third-Party Assets

- **Prototype Map** by AngeloMaN87 — greybox environment
- **Gunshot audio** — `eaglaxle-gun-shot-1-530788.mp3`
- **Unity Particle Pack** — included VFX examples (not core to gameplay flow)

Third-party assets retain their original licenses.

---



## License

Personal / portfolio project. See third-party asset licenses for included store content.

---



## Connect

Replace placeholders with your links when publishing:

- **GitHub:** `https://github.com/<your-username>/Prototyping`
- **LinkedIn:** See `LINKEDIN_POST.md` for a ready-to-publish project announcement
- **Demo video:** *(optional — add a YouTube or itch.io link)*

**Suggested GitHub topics:**  
`unity` `unity3d` `fps` `first-person-shooter` `game-development` `csharp` `unity-input-system` `urp` `prototype` `portfolio` `character-controller` `game-feel`