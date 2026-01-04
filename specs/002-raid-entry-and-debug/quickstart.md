# Quickstart: Raid Entry & Debug Controls

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

## Goal

Verify that raids can be entered/exited in-game via a hub interaction, and that developers can force enter/exit via a dev-only control.

## Prereqs

- Build and run the mod in tModLoader.
- Start a new world (recommended for clean validation).

## Manual Validation Checklist

### 1) Player-facing entry/exit

- Create a character and enter the world.
- Locate the hub console near spawn.
- Interact with it:
  - Choose “Enter Raid”.
  - Confirm you receive a message indicating raid entry and that raid-only behavior can occur (e.g., ARC Drone spawning).
- Interact again while in raid:
  - Choose “Extract / Exit Raid”.
  - Confirm you receive a message indicating extraction and you are in hub context again.

### 2) Refusal cases

- Try to “Enter Raid” while already in a raid: confirm refusal + clear feedback.
- Try to “Extract / Exit Raid” while not in a raid: confirm refusal + clear feedback.

### 3) Dev-only toggle

- Enable dev tools (per the chosen config gate).
- Run `/arcraid toggle`:
  - Confirm it enters the raid if you are not in one.
  - Confirm it exits the raid if you are in one.
- Disable dev tools and confirm the command refuses.

### 4) Multiplayer smoke

- Host a server and connect two players.
- Have Player A enter a raid; confirm Player B is unaffected.
- Have Player A exit; confirm Player B is unaffected.

### 5) World safety

- Use the hub console at least once.
- Disable the mod and load the same world:
  - Confirm the world loads without crashing.

## Notes

This feature intentionally treats spawn as the hub and does not add worldgen or persistent tiles.
