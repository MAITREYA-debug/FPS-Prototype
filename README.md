# FPS Prototype

A first-person shooter prototype built in Unity, focused on responsive player movement, weapon feel, and animation-driven gunplay.

## Overview

This project is a sandbox for experimenting with core FPS mechanics — character movement, camera look, weapon sway, ADS (aim down sights), and procedural idle breathing on the weapon. It uses Unity's new Input System and Universal Render Pipeline (URP).

## Features

- **Player movement** — Walk, sprint, crouch, and jump with a CharacterController-based movement system
- **Speed state machine** — Movement speed adapts to walk, run, crouch, fall, and aiming states
- **Mouse look** — Pitch-clamped first-person camera with smooth rotation
- **Weapon sway** — Look-based and movement-based weapon rotation with reduced sway while aiming
- **Idle weapon breathing** — Lissajous curve-driven subtle sway for a natural held-weapon feel
- **ADS (Aim Down Sights)** — Smooth camera-to-scope alignment when right-clicking
- **Shooting** — Fire-rate-limited projectile spawning from the weapon muzzle
- **Jump / fall / land animations** — Weapon animator triggers synced with player ground state
- **Input System** — Centralized input via `InputManager` with keyboard, mouse, and gamepad support

## Tech Stack

| | |
|---|---|
| **Engine** | Unity 6000.3.16f1 |
| **Render Pipeline** | Universal Render Pipeline (URP) |
| **Input** | Unity Input System |
| **Language** | C# |

## Project Structure

```
Assets/
├── fps/
│   ├── Scripts/
│   │   ├── Player_Controller.cs    # Movement, look, crouch, jump
│   │   ├── InputManager.cs         # Input System wrapper (singleton)
│   │   ├── WeaponClass.cs          # Base weapon data & animation hooks
│   │   ├── DesertEagleScript.cs    # Desert Eagle weapon implementation
│   │   └── Weapons/
│   │       ├── Weapon_Controller.cs  # Sway, ADS, firing
│   │       └── Bullet_script.cs      # Projectile lifetime
│   ├── Animation/                  # Player & gun animator controllers
│   ├── Models/                     # Character & weapon 3D assets
│   ├── Texture/                    # Weapon PBR textures
│   └── prefab/                     # Player & weapon prefabs
├── AngeloMaN87/Prototype Map/      # Greybox level geometry
└── Scenes/
    └── SampleScene.unity           # Main playable scene
```

## Controls

| Action | Key / Input |
|---|---|
| Move | `W` `A` `S` `D` / Left stick |
| Look | Mouse / Right stick |
| Sprint | `Left Shift` |
| Crouch | `Left Ctrl` |
| Jump | `Space` |
| Fire | Left mouse button |
| Aim (ADS) | Right mouse button |
| Interact | `E` |

## Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download) with **Unity 6000.3.16f1** (or compatible 6000.x editor)
- Windows, macOS, or Linux

### Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd Prototyping
   ```
2. Open the project folder in Unity Hub and select **Add project from disk**.
3. Open `Assets/Scenes/SampleScene.unity`.
4. Press **Play** to enter the prototype.

No additional packages need to be installed — dependencies are defined in `Packages/manifest.json` and restored automatically on first open.

## Architecture Notes

- **`InputManager`** — Singleton that reads from generated `InputActions` and exposes movement, look, and action state to other scripts.
- **`Player_Controller`** — Owns movement physics, camera pitch, crouch height lerping, and dispatches jump/fall/land events to the weapon.
- **`Weapon_Controller`** — Handles procedural sway (look + idle), ADS positioning, fire rate, and drives the weapon animator based on player speed and grounded state.

## Third-Party Assets

- **Prototype Map** by [AngeloMaN87](https://assetstore.unity.com/) — Greybox environment used for level blocking and playtesting.

## Status

This is an early prototype. Planned or in-progress areas include reload mechanics, damage system, ammo management, and additional weapons.

## License

Personal / portfolio project. Third-party assets retain their original licenses.
