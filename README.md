# 🎯 3D Aim Trainer (Unity)

A 3D aim trainer game project similar to Aim Lab and Valorant's shooting range. Developed in Unity 3D, the game helps players improve their reflexes, precision, and tracking speed through specialized training exercises.

---

## ✨ Key Features

* **Diegetic UI:** Interact with the game system directly in the 3D environment (shoot the `Start` / `Cancel` buttons to control the game flow) without needing an external mouse cursor.
* **Detailed Hitbox System:** Damage detection based on Bot body parts (Headshot x3, Body x1, Legs x0.6).
* **Smart AI Bots:** Integrated with Mixamo Animations, Bots can stand still (Static) or dodge/move (Moving/Strafing).
* **Seamless Transitions:** Quickly switch between Modes and Scenes using shortcuts (`F2`, `F3`) without going through the Main Menu, retaining mouse sensitivity and weapon configurations.

---

## 🎮 Training Modes

The project includes 2 main Scenes with specific exercises:

### 1. Game_BOT (Realistic Shooting Range Simulation)
Practice reflexes and flick aim with humanoid Bot targets.
* **Mode 1 - Fixed Count (30 Bots):** Eliminate 30 bots in the shortest time possible.
* **Mode 2 - Time Challenge (50 Bots):** Complete the challenge of eliminating 50 bots.
* *Option:* Toggle Bot movement state (Static ↔ Moving).

### 2. Game_DOT (High Precision Training)
Practice with small circular dot targets.
* **Mode 1 - Multi-Dot Mode:** Multiple dots appear randomly; shooting one spawns another.
* **Mode 2 - Single Moving Dot Mode:** A single dot moves along a random trajectory (Tracking practice).

---

## 🔫 Weapon System

The game provides 2 basic weapon types to serve different training purposes:

| Weapon | Damage/Round | Fire Rate | Recoil | Best For | Ammo Capacity |
| :--- | :---: | :---: | :---: | :--- | :---: |
| **Pistol** | 50 | Low | Very Low | Precision, Headshots | 12 rounds/mag |
| **Rifle** | 40 | Fast | Medium | Tracking, Spray Control | 30 rounds/mag |

**Hitbox Multipliers (Game_BOT):**
* **Head:** x3 damage (Max 105 - 150 damage).
* **Body:** x1 damage.
* **Legs:** x0.6 damage.

---

## ⌨️ Controls

* `W` `A` `S` `D`: Movement.
* `Left Mouse Button (LMB)`: Fire.
* `1` / `2`: Switch weapons (Pistol / Rifle).
* `R`: Reload.
* `F2`: Toggle between **Game_BOT** and **Game_DOT** scenes.
* `F3`: Switch training mode in the current Scene (or toggle Bot movement).
* `ESC`: Open Pause Menu.

---

## 🛠️ Development Status (Roadmap)

- [x] FPS Camera & Movement Setup (Starter Assets).
- [x] Raycast Shooting & Weapon Logic.
- [x] Mixamo Bot Model & Animation Integration (Strafe).
- [x] Hitbox Division & Multiplier Damage Calculation.
- [x] Spawner System (Random Bot spawning within a designated area).
- [x] Diegetic UI (Shoot Start / Cancel buttons to manage game flow).
- [X] Real-time Canvas Scoreboard.
- [X] Finalize Game_DOT Scene.
- [X] Highscore Save System.

---

## 🚀 Installation & Getting Started

1. Clone this repository to your local machine.
2. Open the project using **Unity Editor** (Version 6000.2.1f1).
3. Open the `Scenes` folder, select `MainMenu` or `Game_BOT`, enter a name and hit Play.

---
*Developed by Tran Nguyen Ngoc Bao*
