[README.md](https://github.com/user-attachments/files/34303024/README.md)
# Space Invaders 3D — Assignment 1

**Video Game Design and Development, Fall 2026**
Author: Jessica Chen
Starter code: [Oren-Steinberg/Space-Invaders-3D](https://github.com/Oren-Steinberg/Space-Invaders-3D)
Engine: Unity 6 (6000.6.0f1), Universal Render Pipeline

This project builds on the in-class Space Invaders starter project. It adds a start screen, two difficulty levels, shields, a win/lose screen, a new background, aliens that speed up, and fixes to several problems in the original code.

---

## How to Play

1. Open the project in Unity and load `Assets/Scenes/SampleScene.unity`.
2. Press **Play**. The start screen appears.
3. Choose a level:
   - **1** or click **EASY** — 5 shields, slower alien fire.
   - **2** or click **HARD** — no shield (one hit ends the game), faster alien fire, bigger speed-up.
4. Controls:
   - **← / →** — move the dart gun
   - **Space** — shoot
   - **R** — return to the start screen after winning or losing

---

## Required Features

### 1. Remaining aliens speed up as others are destroyed
**File:** `AlienFleet.cs` → `Update()`

Each frame the fleet compares how many aliens are left with how many it started with (`initialAlienCount`). It uses `Mathf.Lerp` to raise the speed from 1× (full fleet) up to `GameManager.MaxSpeedMultiplier` (2.5× on Easy, 3× on Hard) as the fleet shrinks. The current direction is kept with `Mathf.Sign(speed)`, so the speed-up never flips the fleet's direction by mistake.

### 2. Detect when the aliens can no longer be stopped
**File:** `AlienFleet.cs` → `FixedUpdate()`

When the fleet steps down, each alien's height is checked against `lowerBound = -6.83f`, which is the spaceship's Y position. If an alien reaches the ship's level, the player can no longer win, so `GameManager.GameOver()` is called and the game-over screen appears.

`AlienFleet` also detects the opposite case. When `aliens.Count` reaches 0, it calls `GameManager.Win()`.

### 3. Spaceship cannot move offscreen
**File:** `Spaceship.cs` → `Update()`

After the arrow-key input is applied, the ship's X position goes through `Mathf.Clamp(position.x, minX, maxX)`, with bounds of −16 to 16 to match the aliens' play area. The bounds are public, so they can be adjusted in the Inspector.

### 4. Problems fixed in the original code

**Issue 1: The alien counter rebuilt its UI text every frame.**
`AlienCounter.Update()` set `uiText.text` on every frame, and `AlienFleet.Update()` wrote the count into it every frame, even when nothing had changed. This rebuilds a string and a TextMeshPro mesh dozens of times a second for no reason.
**Fix:** `AlienCounter` now has a `SetCount(int)` method, and `AlienFleet` calls it only when the number of aliens changes (it tracks `displayedCount`). The text component is cached in `Awake()` so it is ready before the fleet first calls it.

---

**Issue 2: Collisions destroyed whatever they touched.**
`Alien.OnCollisionEnter` and `AlienBullet.OnCollisionEnter` called `Destroy(coll.gameObject)` with no check of what was hit. An alien bullet could destroy another alien, and the spaceship would simply disappear with no game-over handling. The alien also called `fleet.GetComponent<AlienFleet>()` even though `fleet` was already an `AlienFleet`.
**Fix:** Each script now checks what it hit using `GetComponentInParent<Dart>()`, `<Spaceship>()` or `<AlienBullet>()`. Aliens die only to darts. Alien bullets damage only the ship (through `GameManager.PlayerHit()`) or cancel out a dart. Alien bullets ignore alien colliders so they pass through the fleet. The redundant `GetComponent` call was removed, and dead aliens are cleaned out of the fleet list in one place with `aliens.RemoveAll(alien => alien == null)`.

---

**Issue 3: Alien firing and bullet movement were hard-coded and hard to tune.**
Each alien rolled `Random.value < 0.002` on every `FixedUpdate` and checked a shared `static` cooldown. `AlienBullet` moved a hard-coded `0.1` units per physics tick from a `static` speed. Neither could be adjusted in the Inspector or scaled by difficulty.
**Fix:** Each alien now picks its next fire time with `Random.Range(fireRateMin, fireRateMax)`, divided by `GameManager.FireRateScale` so Easy and Hard differ. `AlienBullet` moves with `transform.Translate(Vector3.down * speed * Time.deltaTime)`, so its speed is in units per second. All of these values are public fields.

---

**Issue 4: The fleet's edge detection started from the wrong values.**
In `AlienFleet.FixedUpdate`, `left` and `right` started at `0`. If all remaining aliens were on one side of the origin, the opposite edge would be measured as `0` instead of the real edge. Destroyed aliens were also still looped over.
**Fix:** `left` and `right` now start at `float.MaxValue` and `float.MinValue`, and null (destroyed) aliens are skipped.

---

**Issue 5: Repeated code and empty Unity methods.**
`Spaceship.Update()` read and wrote `transform.position` separately for each arrow key. `Alien`, `AlienBullet`, `AlienCounter` and `Spaceship` had empty `Start()`/`Update()` methods, which Unity still calls every frame.
**Fix:** The ship now reads its position once, applies both inputs, clamps it, and writes it back once. The empty callbacks were removed from those scripts.

---

## Additional Features

### Starting and ending screen
**File:** `GameManager.cs`

`GameManager` creates itself when the scene loads (`[RuntimeInitializeOnLoadMethod]`), so it needs no scene setup. It runs a simple state machine (`Start → Playing → GameOver / Won`):
- **Start screen:** the game title, a short controls line, and EASY/HARD buttons drawn over a custom background image (`Resources/StartBackground.jpg`). The game is paused (`Time.timeScale = 0`) until a level is chosen.
- **End screen:** a dark overlay showing **YOU WIN!** (green) or **GAME OVER** (red), with a prompt to press **R** to reload the scene and return to the start screen.
- Ship input is disabled whenever the game is not in the `Playing` state.

### Additional levels (with modifications)
**File:** `GameManager.cs`

There are two playable levels, chosen from the start screen:

| Setting | Easy | Hard |
|---|---|---|
| Shields | 5 | 1 (one hit = game over) |
| Alien fire rate | 0.5× | 1.5× (three times Easy's rate) |
| Max fleet speed-up | 2.5× | 3× |
| Ship knock-back from hits | Ship stays upright | Physics can knock the ship around |

The values are exposed as `GameManager.BaseAlienSpeed`, `MaxSpeedMultiplier`, and `FireRateScale`, and `AlienFleet` and `Alien` read them.

### Shields (with on-screen display)
**Files:** `GameManager.cs`, `AlienBullet.cs`

The ship has a shield counter. Each alien bullet that hits the ship calls `GameManager.PlayerHit()`, which removes one shield, and the game ends when the shields run out. The current value is shown in the top-right corner of the screen (`Shield: 5` in cyan on Easy, `HARD` in red on Hard). The player's darts can also shoot down incoming alien bullets (`Dart.cs`), which works as a second, active form of defense.

### Replaced the background
**Files:** `SpaceBackground-3.jpg`, `Background.mat`, `BackgroundQuad` in `SampleScene.unity`

The plain blue camera background was replaced with a space image on a quad behind the play field. The start screen has its own background image as well.

### Balanced the game
The original game was too easy: aliens rarely fired, their bullets could destroy their own fleet, and nothing happened when the aliens reached the bottom. Now:
- Reaching the ship's level ends the game.
- The fleet speeds up as it shrinks (up to 3× on Hard).
- Alien fire rate is tuned per level.
- Hard mode allows a single hit.

Easy remains beatable for new players. Hard is a real challenge but can still be cleared by shooting down incoming bullets and thinning the fleet carefully.

---

## Project Structure

```
Assets/
├── Alien.cs           – alien firing and death on dart hit
├── AlienBullet.cs     – alien bullet movement and hit handling
├── AlienCounter.cs    – "Remaining Aliens" UI (updates only on change)
├── AlienFleet.cs      – fleet movement, speed-up, win/lose detection
├── Dart.cs            – player dart; can cancel alien bullets
├── GameManager.cs     – game states, start/end screens, levels, shields, HUD
├── Spaceship.cs       – movement (clamped), shooting, Easy-mode stability
├── Resources/StartBackground.jpg
├── SpaceBackground-3.jpg / Background.mat
└── Scenes/SampleScene.unity
```

---

## Credits

- **Starter project:** Oren Steinberg, [Space-Invaders-3D](https://github.com/Oren-Steinberg/Space-Invaders-3D), built together in class.
- **AI assistance:** This project was developed with help from **Claude** (Anthropic), used as an AI coding assistant. Claude helped with:
  - finding inefficient and non-idiomatic code in the starter project and suggesting fixes
  - designing and writing the `GameManager` (game states, start/end screens, levels, shields, HUD)
  - the alien speed-up, game-over detection, and screen-boundary logic
  - debugging collision handling between darts, alien bullets, aliens, and the ship
  - writing this README

  As the course policy allows, all AI-assisted work came from my own prompts. I reviewed, tested, and integrated it in Unity myself.
