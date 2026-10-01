## Archived Session State: 20261002_003600
# Session State — Prototype in progress

**Concept:** Pulverize (`design/gdd/game-concept.md`)
**Skill:** /prototype — Engine path, Unity 2022.3.62f2
**Location:** `prototypes/pulverize-concept/` (standalone Unity project, outside the root code root)
**Current phase:** Phase 6 — Playtest debrief (prototype compiles clean in Unity batch mode; waiting for user to play)

## Hypothesis
If the player moves while weapons auto-fire and every enemy shatters into shards that magnetize back as XP,
the loop feels satisfying enough that the tester starts a second run by choice, and the sim holds >= 30 fps
at 300 enemies + 3000 shards on the dev machine (desktop proxy for mobile budget).

## Riskiest assumption
Shatter-and-collect (hit-stop, shake, impact audio, ASMR pitch-chain collect) is satisfying at scale without
becoming unreadable or too heavy.

## Scope (in)
- WASD / mouse-drag move, auto-fire at nearest enemy
- Enemies die -> shards -> magnet -> XP, chained pitch audio
- Hit-stop, screen shake, procedural audio (no asset files)
- Level-up with 3 random Cores out of 6 (stat-level only, no Fusion)
- 5-minute run, restart with R, FPS/enemy/shard counters, F = spawn +100 enemies stress

## Cut
Props, bosses/Elites, Fusion, meta progression, haptics (desktop cannot test), menus, real art, mobile device test.
---

