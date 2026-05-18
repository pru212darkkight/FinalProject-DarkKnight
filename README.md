# 🔥 Fire Warrior in the Demon World

> A 2D Action Platformer built with Unity and C# — battle through 5 demon-infested maps, defeat bosses, collect loot, and grow stronger with an RPG equipment system.

---

## 📖 About the Game

**Fire Warrior in the Demon World** is a 2D action platformer developed by a team of 5 as a final project for **PRU212 — Game Development with Unity** at FPT University. Each team member designed and implemented their own map, complete with unique enemies, traps, and a boss encounter. The game features a full RPG progression system including an inventory, equipment shop, coin economy, and a secret gift code mechanic.

---

## 🗺️ Maps & Team

| Scene | Map | Developer |
|---|---|---|
| Home Village | Hub world — NPC shop, gift code NPC, tutorial | Shared |
| Map 1 - Tin | Forest / dungeon — melee & ranged enemies, Boss Wolf | Tin |
| Map 2 - Han | Lava cavern — Beholder, Reaper, falling rocks, lava zones | Han |
| Map 3 - Tuyen | Underwater — underwater enemies, mines, water boss | Tuyen |
| Map 4 - Phuong | Ruins — Meduxa boss, wolves, rats, poison zones, moving platforms | Phuong |
| Map 5 - Hau | Demon fortress — multiple mini-bosses, final boss, saw traps, toxic zones, moving platforms, ladders | Hau |

---

## ✨ Features

### Player Combat System
- **3-hit melee combo** — Attack 1 (1× damage), Attack 2 (1.5× damage), Attack 3 (3× damage), each with independent cooldowns
- **2 magic spells** — Spell 1: fires a fireball projectile (costs 20 mana); Spell 2: area spell (costs 40 mana)
- **Spell 3 (Transform)** — costs 50 mana, transforms the player for 5 seconds with enhanced abilities (10-second cooldown)
- **Defend** — hold to block; drains stamina while active; stops movement
- **Dash** — quick directional dash; costs 30 stamina; 1-second cooldown
- **Hurt system** — knockback + flash effect + stun on taking damage; movement reduced to 50% during stun

### Player Stats & Regeneration
| Stat | Description |
|---|---|
| Health | Regenerates after 5 seconds without taking damage |
| Stamina | Regenerates after 0.5s; consumed by defend and dash |
| Mana | Regenerates after 0.5s; consumed by spells |
| Strength | Scales all melee attack damage |
| Armor | Physical damage reduction |
| Magic Resist | Magic damage reduction |
| Speed / Jump | Movement and jump force |

- All stats are boosted by equipped items from the shop
- Base stats (max health, strength) persist across scenes via PlayerPrefs

### RPG Inventory & Equipment System
- **Inventory** — items owned and equipped are saved via PlayerPrefs and persist across all scenes
- **Equipment slots** — equipping an item of the same type automatically replaces the previous one
- **Stat application** — equipping/unequipping recalculates all player stats instantly
- Items loaded from `Resources/Items/` as ScriptableObjects (`ItemData`)
- Each item can boost: Health, Stamina, Mana, Strength, Armor, Magic Resist, Health Regen, Stamina Regen, Mana Regen, Speed, Jump

### Shop System (Home Village)
- Browse all available items with icon grid
- Detail panel shows full stat bonuses and price
- Purchase with coins; owned items shown as "Owned" (grayed out)
- Coin balance updates in real time

### Coin Economy
- Enemies drop coins on death (`CoinDrop`)
- Coins persist across scenes via PlayerPrefs
- Session coins tracked separately per level (shown on victory/defeat panel)

### Gift Code System (NPC in Home Village)
| Code | Effect |
|---|---|
| `hauhero` | Set max health to 10,000 and strength to 100 |
| `tintientai` | Add 9,999 gold |
| `tuyentutung` | Reset health to 100, strength to 10, and lose all gold |
| `hanhaihuoc` | Wipe all owned and equipped items |

### Level System
- **Victory Panel** — shown when boss is defeated; displays time survived and gold earned; spawns portal to next map
- **Defeat Panel** — shown when player dies; displays time survived and gold earned; options to retry or return to Home Village
- **Level Timer** — tracks time elapsed per level
- **Portal** — spawns after boss defeat; transitions to next scene with fade effect

### Map-Specific Features

**Map 1 (Tin) — Forest Dungeon**
- Melee enemies with patrol and chase AI
- Ranged enemies that fire projectiles
- Boss Wolf with multi-phase behavior
- Boss door that locks when entering boss arena
- Health and mana restore pickups
- Death traps (instant kill zones)

**Map 2 (Han) — Lava Cavern**
- Beholder enemy (floating, ranged)
- Reaper enemy
- Falling rocks triggered by proximity zones
- Lava zones (damage over time)
- Lightning hitbox attacks
- Wall unlock mechanic after clearing enemies

**Map 3 (Tuyen) — Underwater**
- Underwater enemy AI with health bars
- Underwater mines (proximity explosion)
- Damage water zones
- Underwater player attack system
- Water boss with movement and attack phases
- Parallax background scrolling

**Map 4 (Phuong) — Ruins**
- Meduxa boss with spike and close-range attacks
- Wolf and rat enemies
- Poison damage zones
- Moving platforms and bridge movers
- Rope generator mechanic
- Storm damage zones
- Camera shake on boss attacks
- Summoned enemy waves during boss fight

**Map 5 (Hau) — Demon Fortress**
- Multiple mini-boss types: DemonBat, DemonHornRed, EvilWizard, MiniBoss variants
- Final boss encounter
- Saw trap damage zones
- Toxic zones (damage over time)
- Moving platforms
- Waterfall platforms
- Ladder climbing mechanic
- Auto-closing gates

### Home Village (Hub)
- NPC shop for buying equipment
- Gift code NPC
- Tutorial area
- Dedicated background music

### Audio System
- Per-scene background music (Home Village, Map 1–5)
- SFX: footsteps, jump, attack (2 variants), spell cast, enemy hurt, player hurt, death, buy item, equip item, button click, victory fanfare, defeat sound, portal teleport, shield sounds
- AudioManager singleton persists across scenes

### Camera
- Smooth camera follow with configurable offset
- Camera focus manager: pans to portal after boss defeat, then returns to player

---

## 🛠️ Built With

| Technology | Details |
|---|---|
| Engine | Unity (2D) |
| Render Pipeline | Universal Render Pipeline (URP) |
| Language | C# |
| Input | Unity New Input System |
| UI Text | TextMeshPro |
| Animation | Unity Animator + Spriter2UnityDX |
| Persistence | PlayerPrefs (inventory, coins, stats) |
| Version Control | Git |

---

## 🏗️ Project Structure

```
Assets/
├── Scenes/
│   ├── Main Menu.unity
│   ├── Home Village.unity       # Hub world — shop, gift code NPC
│   ├── Map 1 - Tin.unity        # Forest dungeon
│   ├── Map 2 - Han.unity        # Lava cavern
│   ├── Map 3 - Tuyen.unity      # Underwater
│   ├── Map 4 - Phuong.unity     # Ruins
│   └── Map 5 - Hau.unity        # Demon fortress (final map)
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController1.cs     # Full player: movement, combat, spells, dash, defend, stats
│   │   ├── PlayerInteraction.cs     # Interact with NPCs, chests, portals
│   │   ├── PlayerClimbStep.cs       # Ladder climbing
│   │   ├── FireSpellEffect.cs       # Fireball projectile behavior
│   │   ├── PortalController.cs      # Portal transition logic
│   │   └── CameraFollow.cs          # Smooth camera follow
│   ├── Common/
│   │   ├── GameManager.cs           # Victory/defeat panels, timer, music, portal spawn
│   │   ├── Inventory.cs             # Item ownership, equip/unequip, PlayerPrefs persistence
│   │   ├── ShopManager.cs           # Shop UI, item purchase logic
│   │   ├── PlayerMoney.cs           # Coin tracking and persistence
│   │   ├── ItemData.cs              # ScriptableObject: item stats definition
│   │   ├── TreasureChest.cs         # Chest open and item drop
│   │   ├── CoinDrop.cs              # Coin drop on enemy death
│   │   ├── Portal.cs                # Portal scene transition
│   │   ├── Teleporter.cs            # In-map teleporter
│   │   ├── LevelTimer.cs            # Level elapsed time tracker
│   │   ├── PauseGame.cs             # Pause menu
│   │   └── BossChecker.cs           # Checks if all bosses are defeated
│   ├── GiftCode/
│   │   ├── GiftCodeManager.cs       # Code input, validation, reward application
│   │   └── GiftCodeNPC.cs           # NPC interaction to open gift code panel
│   ├── Map1/                        # Enemy, BossWolf, Projectile, traps, health pickups
│   ├── Map 2/                       # Beholder, Reaper, FallingRock, LavaZone, Lightning
│   ├── Map3/                        # Underwater enemies, mines, water boss, parallax
│   ├── Map4/                        # Meduxa boss, wolves, rats, poison, moving platforms
│   ├── Map5/                        # Mini-bosses, final boss, saw traps, toxic zones, ladders
│   ├── UI/                          # HUD, stats panel, item popup
│   ├── Audio/                       # AudioManager singleton
│   ├── Camera/                      # CameraFocusManager
│   ├── MainMenu/                    # Main menu controller
│   ├── NPCMenu/                     # NPC dialog and menu
│   ├── Tutorial/                    # Tutorial triggers
│   └── GameLogData/                 # Defeat log data structures
├── Prefabs/                         # Player, enemies, items, portals, treasure chests, recovery
├── Animations/                      # Player and enemy animation controllers
├── Sprites/                         # Player, maps, backgrounds, GUI sprites
├── Audios/                          # Music and SFX clips
└── FantasyInventory/                # Inventory UI assets
```

---

## 🚀 Getting Started

### Prerequisites
- Unity Hub installed
- Unity Editor (check `ProjectSettings/ProjectVersion.txt` for exact version)
- Git with long path support enabled (required on Windows):
  ```bash
  git config --global core.longpaths true
  ```
  Or enable via Windows Registry (run as Administrator):
  ```powershell
  New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" -Name "LongPathsEnabled" -Value 1 -PropertyType DWORD -Force
  ```

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```

2. Open **Unity Hub** → **Add project from disk** → select the cloned folder

3. Open the project in Unity Editor

4. Open `Assets/Scenes/Main Menu.unity` and press **Play**

### Play Online
🎮 [Play on itch.io](https://hauhuynh2k4.itch.io/fire-warrior-in-the-demon-world)

---

## 🎮 Controls

| Key | Action |
|---|---|
| `A` / `D` | Move left / right |
| `Space` | Jump |
| `J` | Attack 1 (basic) |
| `K` | Attack 2 (1.5× damage) |
| `L` | Attack 3 (3× damage) |
| `Q` | Spell 1 — Fireball (20 mana) |
| `E` | Spell 2 — Area spell (40 mana) |
| `R` | Spell 3 — Transform (50 mana) |
| `Left Shift` (hold) | Defend |
| `Left Ctrl` | Dash |
| `F` | Interact (NPC / chest / portal) |
| `I` | Open Inventory |
| `Esc` | Pause |

> *(Update with actual keybindings from Input System if different)*

---

## 📚 Course Information

- **Course**: PRU212 — Game Development with Unity
- **Semester**: Semester 7
- **University**: FPT University
- **Team size**: 5 members

---

## 📝 License

This project is developed for educational purposes as part of the FPT University curriculum.
