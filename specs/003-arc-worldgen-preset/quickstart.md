# Quickstart: Arc Raider Worldgen Preset

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

## Goal

Verify that players can create an Arc Raider world (without affecting vanilla worlds), and that the created world has the expected placeholder regions and anchors.

## Prereqs

- Build and run the mod in tModLoader.
- Use a fresh test character.

## Manual Validation Checklist

### 1) Create an Arc Raider world

- Start world creation.
- Choose any size and any standard options.
- In the seed box, enter a seed starting with `arc:` (example: `arc:test-1`).
- Create the world and enter it.

Expected:
- The world is recognized as an Arc Raider world.
- A Safe Hub region exists and is reachable from spawn.
- At least one additional named placeholder region exists (e.g., Arc Wasteland).

### 2) Create a normal world

- Create another world with a normal seed (no `arc:` prefix).
- Enter it.

Expected:
- Vanilla world generation and gameplay are unchanged.
- Arc Raider world metadata is not present.

### 3) Multiplayer smoke

- Host a server with an Arc Raider world and join with a second player.

Expected:
- Both players see the same Arc layout.
- No duplication of hub/anchor entities beyond intended server-side spawning rules.

### 4) World safety (disable/cleanup)

- Create and enter an Arc Raider world.
- Disable the mod and load the same world.

Expected:
- The world loads without crashing.

## Manual Verification Log (2026-01-04)

- Not run in this environment (headless editor). Requires an interactive tModLoader session.
- Safety note: Disabling/removing the mod leaves Arc worlds loadable; Arc metadata is treated as non-Arc on load if missing/corrupt. Non-Arc worlds are untouched.
