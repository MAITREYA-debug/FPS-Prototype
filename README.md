# FPS Movement & Weapon Feel Prototype

A Unity 6 first-person shooter prototype focused on responsive movement, procedural weapon sway, aim-down-sights alignment, a multi-weapon inventory system, and animation-driven gun feedback inside a greybox test environment.

![Unity](https://img.shields.io/badge/Unity-6000.3.16f1-black?logo=unity)
![URP](https://img.shields.io/badge/Pipeline-URP-blue)
![C#](https://img.shields.io/badge/Language-C%23-purple)
![Branch](https://img.shields.io/badge/Branch-weapon--system-orange)

---

## Overview

This project is a portfolio-oriented FPS sandbox for iterating on core first-person mechanics before building out full game systems. The playable experience lives in `SampleScene`, where a first-person character navigates the **Prototype Map** greybox level with swappable weapons (pistol, rifle, melee).

The architecture is component-based and split across **movement**, **weapon feel**, **weapon combat**, and **inventory management**:

| Component | Location | Responsibility |
|---|---|---|
| `InputManager` | `Scripts/Game Managers/` | Singleton input hub — reads Input System actions and raises gameplay events |
| `Player_Controller` | `Scripts/Player/` | Locomotion, camera, crouch, jump/fall/land, weapon pickup raycast |
| `Weapon_Controller` | `Scripts/Weapons/` | Procedural sway, idle breathing, ADS positioning, weapon animator sync |
| `Weapon_Manager` | `Scripts/Game Managers/` | Weapon slots, switching, pickup/drop, IK hand targets, animator swapping |
| `Gun` (+ `Pistol`, `Rifle`, `Melee`) | `Scripts/Weapons/` | Per-weapon shooting, reload, and ammo driven by `GunData` assets |
| `PickUpGun_Script` | `Scripts/Weapons/` | World-dropped weapons with physics throw and saved ammo state |
| `Bullet_script` | `Scripts/Weapons/` | Visual projectile travel from muzzle to impact point |
| `Weapon_Shooting` | `Scripts/Player/` | Legacy shooting component — superseded by the `Gun` hierarchy on this branch |
| `PlayerSoundManager` | `Scripts/Game Managers/` | Placeholder — no audio logic implemented yet |

Rendering uses the **Universal Render Pipeline (URP)**. Hand placement is handled through **Animation Rigging** (`RigBuilder`, `TwoBoneIKConstraint`) with per-weapon hand target transforms.

---

## Code Analysis

This section reflects what the scripts **actually do today** on the `weapon-system` branch.

### Input flow — `InputManager`

- Singleton with duplicate-instance protection and cleanup in `OnDestroy`.
- Exposes polled state: `MoveInput`, `LookInput`, `isSprinting`, `isCrouching`, `isAimingIn`, `isFiring`.
- Raises static events consumed by other systems:

| Event | Triggered by | Consumed by |
|---|---|---|
| `OnJump` | `Space` | `Player_Controller` |
| `OnInteract` | `E` (Player map) | *(no handler subscribed)* |
| `OnReload` | `R` | `Pistol`, `Rifle`, `Gun.tryReload()` |
| `OnFire` | Left mouse press | `Pistol`, `Melee` |
| `OnWeaponSelected` | `1` / `2` / `3` | `Weapon_Manager` |
| `onPickup` | `E` (Weapon map) | `Player_Controller.WeaponPickInteraction()` |
| `onDrop` | `F` | `Weapon_Manager.try_DropWeapon()` |

> **Note:** `E` is bound to both `Interact` (Player map) and `WeaponPickUp` (Weapon map), so pressing `E` fires both events. Only `onPickup` has a gameplay handler.

### Player — `Player_Controller`

- Uses `CharacterController` with five speed states: walk, run, crouch, fall, aim.
- Sprint only applies when moving **forward** (`moveInput.y > 0`).
- Jump applies custom gravity; fall state triggers after 0.15 s airborne with downward velocity.
- Crouch smoothly lerps controller height and repositions `cameraHolder`.
- Exposes `weaponAnimation_Speed` (0–1) to drive gun animator `Speed` parameter.
- `WeaponPickInteraction()` raycasts 4 m from viewport center against `InteractionMask`, finds `PickUpGun_Script`, and delegates to `Weapon_Manager.PickUp_Weapon()`.

### Weapon feel — `Weapon_Controller`

- Initialized by `Player_Controller` at runtime via `Initialization()`.
- **Look/move sway:** input-driven local rotation with clamped euler angles; intensity quartered while ADS.
- **Idle breathing:** Lissajous curve offset applied to `WeaponSwayObject` position.
- **ADS:** `SmoothDamp` moves weapon toward alignment target derived from `ScopeCameraPosition` and `AimOffset`.
- Syncs animator: `Speed`, `Sprinting`, `isGrounded`; triggers `OnJump`, `Falling`, `OnLanding`.
- `SetValuesForWeapon()` hot-swaps scope transform and animator when `Weapon_Manager` changes weapons.

### Weapon inventory — `Weapon_Manager`

Central hub for the multi-weapon system:

- **Three slot types:** primary (`1`), secondary (`2`), melee (`3`) — mapped to keyboard number keys.
- **ID ranges** used for slot assignment when picking up weapons:
  - Primary: `100 < gunId < 200` (e.g. AKM = `101`)
  - Secondary: `0 < gunId < 10` (e.g. Desert Eagle = `1`)
- Maintains `holdWeapons` dictionary (equipped prefabs) and `throwWeapons` dictionary (drop templates).
- On weapon switch: deactivates current gun, activates selected gun, updates `Weapon_Controller` scope/animator, swaps player animator controller, rebuilds IK rig.
- **Drop:** instantiates throw prefab, copies ammo via `saveGunData()`, applies physics impulse via `PickUpGun_Script.throwWeapon()`. Melee cannot be dropped.
- **Pickup:** assigns weapon to correct slot; auto-drops previous weapon in that slot if occupied.
- Updates left/right IK target transforms every frame to match active weapon hand positions.

### Combat — `Gun` hierarchy

Abstract base class shared by all weapons, configured via **`GunData` ScriptableObject** assets:

| Asset | gunId | Role |
|---|---|---|
| `Pistol.asset` | 1 | Secondary — Desert Eagle |
| `Akm.asset` | 101 | Primary — assault rifle |
| `Bren.asset` | — | Primary rifle variant |
| `shortGun.asset` | — | Short gun variant |
| `melee.asset` | 11 | Melee — knife (short-range raycast, no bullet) |

**`GunData` fields:** `gunId`, `gunName`, `shootlayer`, `magazine`, `reloadTime`, `FireRate`, `ErrorRange`, `MaxTravelDistance`, sway settings.

| Class | Fire model | Reload | VFX |
|---|---|---|---|
| `Pistol` | `OnFire` event + `tryShoot()` tick | Coroutine with `reloadTime` delay | Bullet trail only |
| `Rifle` | Continuous `tryShoot()` while holding fire | Coroutine reload | Muzzle flash + bullet + hit VFX |
| `Melee` | `OnFire` event, short-range raycast | N/A | Hit VFX only |

Shared shooting logic in `Gun`:
- Viewport-center raycast with optional movement spread (`ErrorRange` as pixel offset).
- Spawns `Bullet_script` for hitscan-visual projectile travel.
- `UpdateAmmoDisplay()` writes to TextMeshPro ammo text.

### World weapons — `PickUpGun_Script`

- Attached to droppable weapon prefabs with a `Rigidbody`.
- Stores `GunData` reference and remaining `bulletCount`.
- `throwWeapon()` applies forward + upward impulse and spin when dropped.
- `saveGunData()` copies state from the held weapon before drop.

### Projectile — `Bullet_script`

- Coroutine moves transform toward raycast hit point at configurable speed.
- Clears trail renderer on spawn; destroys on arrival or after `lifeTime` timeout.

### Partial / legacy / not yet implemented

| Area | Status |
|---|---|
| `Weapon_Shooting` | Still in project — older monolithic shooter; `Gun` classes are the active system |
| `PlayerSoundManager` | Empty stub — no footstep or weapon audio |
| `OnInteract` | Event fires on `E` but no script subscribes |
| Damage / health | Hits spawn VFX only — no `IDamageable` or health system |
| Enemy AI | Not implemented |
| Save/load | Not implemented |
| Gamepad look | Not bound |
| Reload animation | Reload is timer-based only — no animator trigger |

---

## Features

### Player Controller
- CharacterController movement with walk, sprint, crouch, aim, and fall states
- Jump with custom gravity; fall detection after brief air time
- Sprint only while moving forward
- Smooth crouch height and camera offset interpolation

### Camera & ADS
- First-person mouse look with pitch clamp on `CameraHolder`
- ADS alignment via `InSight` / `ScopePosition` transform on each weapon

### Weapon System
- Three weapon categories: primary, secondary, melee
- Hot-swap with `1` / `2` / `3`
- Pick up world weapons with `E` (4 m interaction raycast)
- Drop current weapon with `F` (melee excluded)
- Per-weapon `GunData` ScriptableObjects for stats and tuning
- Automatic slot assignment by `gunId` range
- Animation Rigging hand IK updated per active weapon

### Shooting
- Raycast from viewport center with movement spread
- Fire-rate gating via `GunData.FireRate`
- Magazine ammo with timed reload (`GunData.reloadTime`)
- Muzzle flash, bullet trail, and surface hit VFX (weapon-dependent)
- No damage system yet

### Animation
- Per-weapon gun Animator (idle, walk, run, jump, fall, land)
- Player body animator controller swapped on weapon change
- Parameters: `Speed`, `Sprinting`, `isGrounded`
- Triggers: `OnJump`, `Falling`, `OnLanding`, `fire`

### UI
- TextMeshPro ammo counter updated per active weapon

---

## Technologies

| Technology | Role |
|---|---|
| **Unity 6000.3.16f1** | Game engine |
| **C#** | Gameplay logic |
| **Universal Render Pipeline (URP)** | Rendering |
| **Unity Input System** | Player and weapon input |
| **ScriptableObjects** | `GunData` weapon configuration |
| **Unity Animator** | Weapon and player animation |
| **Animation Rigging** | Two-bone hand IK |
| **CharacterController** | Player movement |
| **TextMeshPro + uGUI** | Ammo HUD |

**Installed but unused in gameplay:** Cinemachine, AI Navigation, Timeline, Visual Scripting, Test Framework.

---

## Project Structure

```
Prototyping/
├── Assets/
│   ├── fps/
│   │   ├── Animation/Gun/
│   │   ├── Material/
│   │   ├── Models/3D/
│   │   ├── Scriptable Objects/       # GunData assets (Pistol, Akm, Bren, etc.)
│   │   │   └── Gun Data.cs
│   │   ├── Scripts/
│   │   │   ├── Game Managers/
│   │   │   │   ├── InputManager.cs
│   │   │   │   ├── Weapon_Manager.cs
│   │   │   │   └── PlayerSoundManager.cs
│   │   │   ├── Player/
│   │   │   │   ├── Player_Controller.cs
│   │   │   │   └── Weapon_Shooting.cs   # legacy
│   │   │   └── Weapons/
│   │   │       ├── Gun.cs
│   │   │       ├── Pistol.cs
│   │   │       ├── Rifle.cs
│   │   │       ├── Melee.cs
│   │   │       ├── Weapon_Controller.cs
│   │   │       ├── PickUpGun_Script.cs
│   │   │       └── Bullet_script.cs
│   │   └── Texture/
│   ├── AngeloMaN87/Prototype Map/
│   ├── Scenes/SampleScene.unity
│   ├── Settings/
│   └── InputActions.inputactions
├── docs/
│   ├── screenshots/                  # PNG / GIF captures (see below)
│   └── videos/                       # MP4 demos (see Videos section)
├── Packages/
├── ProjectSettings/
├── README.md
└── LINKEDIN_POST.md
```

---

## Controls

| Action | Keyboard / Mouse | Gamepad |
|---|---|---|
| Move | `W` `A` `S` `D` | Left stick |
| Look | Mouse | — |
| Sprint | `Shift` (hold, forward only) | — |
| Jump | `Space` | — |
| Crouch | `Ctrl` (hold) | — |
| Pick up weapon | `E` | — |
| Drop weapon | `F` | — |
| Primary weapon | `1` | — |
| Secondary weapon | `2` | — |
| Melee weapon | `3` | — |
| Fire | Left mouse (hold for rifle) | — |
| Aim (ADS) | Right mouse | — |
| Reload | `R` | — |

---

## Architecture

```
InputActions
      │
      ▼
InputManager (singleton)
 ├── Player map  → Move, Look, Sprint, Jump, Crouch, Interact
 ├── Weapon map  → Fire, Aim, Reload, Slot select, Pickup, Drop
 └── Events → OnJump, OnFire, OnReload, OnWeaponSelected, onPickup, onDrop
      │
      ├─────────────────────────┬──────────────────────────┐
      ▼                         ▼                          ▼
Player_Controller        Weapon_Manager              Gun (Pistol/Rifle/Melee)
 ├── movement/look       ├── slot switching (1/2/3)   ├── tryShoot / shoot
 ├── jump/fall/land       ├── pickup / drop            ├── reload coroutine
 └── pickup raycast       ├── IK hand targets          ├── GunData stats
      │                   └── animator swap            └── raycast + VFX
      ▼
Weapon_Controller
 ├── procedural sway + idle breathing
 ├── ADS SmoothDamp alignment
 └── weapon Animator sync
      │
      ▼
Bullet_script → visual travel to impact point

PickUpGun_Script → world weapon with Rigidbody throw physics
GunData (ScriptableObject) → per-weapon configuration
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
   git checkout weapon-system
   ```
2. Open Unity Hub → **Add project from disk** → select the `Prototyping` folder.
3. Wait for package restore and asset import.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play**.

No extra package installation is required beyond the correct Unity version.

---

## Screenshots

Add static captures to `docs/screenshots/` for quick GitHub preview:

| File | Suggested content |
|---|---|
| `gameplay-01.png` | First-person view in the greybox map |
| `gameplay-02.png` | Weapon sway while moving |
| `weapon-switch.png` | Primary / secondary / melee in hand |
| `pickup-drop.png` | Dropped weapon on the ground |
| `ads-demo.gif` | ADS alignment (GIF for motion) |

### How to add screenshots

1. Create the folder (if it does not exist):
   ```bash
   mkdir -p docs/screenshots
   ```
2. In Unity, enter **Play Mode** and capture frames with **Win + Shift + S** (Windows Snipping Tool), **OBS**, or the **Unity Recorder** package.
3. Save as `.png` (still images) or `.gif` (short loops — use ScreenToGif, OBS, or ezgif.com).
4. Copy files into `docs/screenshots/`.
5. Embed in this README:
   ```markdown
   ![Description](docs/screenshots/gameplay-01.png)
   ```
6. Commit and push:
   ```bash
   git add docs/screenshots/
   git commit -m "Add gameplay screenshots"
   git push
   ```

---

## Videos

Gameplay videos give the best sense of movement feel, weapon switching, and pickup/drop mechanics. Store them in `docs/videos/` and link or embed them here.

### Recommended demo clips

| File | Content | Target length |
|---|---|---|
| `movement-demo.mp4` | Walk, sprint, crouch, jump, fall | 15–30 s |
| `weapon-sway-ads.mp4` | Idle sway, movement sway, ADS transition | 15–30 s |
| `shooting-demo.mp4` | Pistol/rifle fire, reload, hit VFX | 20–40 s |
| `weapon-system-demo.mp4` | Switch slots, pick up, drop weapons | 30–60 s |
| `full-gameplay.mp4` | Combined showcase for portfolio / LinkedIn | 1–2 min |

### How to add videos to this repository

#### Step 1 — Create the folder

```bash
mkdir -p docs/videos
```

#### Step 2 — Record gameplay

Choose one of these methods:

| Tool | Notes |
|---|---|
| **OBS Studio** (free) | Best for full-screen capture; export MP4 after recording |
| **Unity Recorder** (`com.unity.recorder`) | Captures Game view directly; cleanest for portfolio clips |
| **Windows Game Bar** (`Win + G`) | Quick clips; re-encode if file size is large |
| **ShareX** (free) | Screen recording with MP4 output |

Recording tips:
- Use **1920×1080** or **1280×720** resolution.
- Hide the Unity Editor UI — record **Game view** only.
- Keep each clip focused on one feature (sway, shooting, weapon swap, etc.).

#### Step 3 — Export settings

For GitHub-hosted files (recommended under **~10 MB** per file):

| Setting | Value |
|---|---|
| Format | `.mp4` |
| Codec | H.264 |
| Frame rate | 30 or 60 fps |
| Bitrate | 5–8 Mbps for 1080p |

If a file exceeds GitHub's comfortable inline size (~10–25 MB), upload to **YouTube** (unlisted) or **Google Drive** and link instead.

#### Step 4 — Add files to the project

```bash
# Copy your exported file into the repo
cp ~/Videos/shooting-demo.mp4 docs/videos/shooting-demo.mp4

git add docs/videos/shooting-demo.mp4
git commit -m "Add shooting demo video"
git push
```

#### Step 5 — Embed in this README

**Option A — Inline MP4** (works for smaller files committed to the repo):

```markdown
## Demo Videos

### Weapon System
<video src="docs/videos/weapon-system-demo.mp4" controls width="720"></video>

### Shooting & Reload
<video src="docs/videos/shooting-demo.mp4" controls width="720"></video>
```

**Option B — Clickable link** (works for any file size):

```markdown
[Watch full gameplay demo](docs/videos/full-gameplay.mp4)
```

**Option C — YouTube / external host** (best for portfolio visibility):

```markdown
[![Gameplay Demo](docs/screenshots/video-thumbnail.png)](https://youtu.be/YOUR_VIDEO_ID)
```

For Option C, also export a thumbnail (`docs/screenshots/video-thumbnail.png`) so the README shows a preview image.

#### Step 6 — Add a demo section below (uncomment when files are ready)

<!-- Uncomment and fill in once videos are added:

## Demo Videos

### Movement & Weapon Feel
<video src="docs/videos/movement-demo.mp4" controls width="720"></video>

### Weapon Switching, Pickup & Drop
<video src="docs/videos/weapon-system-demo.mp4" controls width="720"></video>

### Shooting & Reload
<video src="docs/videos/shooting-demo.mp4" controls width="720"></video>

-->

> **Current status:** No demo videos committed yet. Follow the steps above to add them.

---

## Roadmap

Based on the current `weapon-system` branch:

- **Combat:** `IDamageable` interface, target dummies, headshot multipliers, object pooling
- **Weapons:** Reload animation trigger, empty-mag click sound, unify `Weapon_Shooting` removal
- **Audio:** Implement `PlayerSoundManager` — footsteps, fire, reload, pickup
- **UI:** Crosshair with dynamic spread, interaction prompt, weapon name display
- **Input:** Resolve duplicate `E` binding (`Interact` vs `WeaponPickUp`); add gamepad look
- **Inventory:** Align melee `gunId` values across assets and `Weapon_Manager` defaults
- **AI:** NavMesh bake + basic enemy patrol (if scope expands)
- **Polish:** Recoil kick, camera shake, drop/pickup animation

---

## Third-Party Assets

- **Prototype Map** by AngeloMaN87 — greybox environment
- **Gunshot audio** — `eaglaxle-gun-shot-1-530788.mp3`
- **Unity Particle Pack** — included VFX examples

Third-party assets retain their original licenses.

---

## License

Personal / portfolio project. See third-party asset licenses for included store content.

---

## Connect

- **GitHub:** `https://github.com/<your-username>/Prototyping`
- **LinkedIn:** See `LINKEDIN_POST.md` for a ready-to-publish project announcement
- **Demo video:** Add to `docs/videos/` and embed above once recorded

**Suggested GitHub topics:**  
`unity` `unity3d` `fps` `first-person-shooter` `game-development` `csharp` `unity-input-system` `urp` `prototype` `portfolio` `character-controller` `game-feel` `scriptableobject` `weapon-system`
