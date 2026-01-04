# Data Model: Raid Entry & Debug Controls

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

This feature adds no new core raid rules or persistence formats. It introduces new *adapter-level* interaction surfaces that call existing raid/session logic.

## Entities (conceptual)

### Raid Entry Device (Hub Console)

- **Type**: In-world interactive “device” represented as a stationary NPC.
- **Fields**:
  - `WorldSpawnAnchor`: Derived from `Main.spawnTileX/Y`.
  - `Mode`: Derived per-player at interaction time (`Enter` if player is not in raid, `Exit` if player is in raid).
- **Relationships**:
  - Calls into per-player raid state via `RaidPlayer`.

### Dev Toggle Control

- **Type**: Chat command entry point.
- **Fields**:
  - `Enabled`: boolean (from mod config; default off).
  - `Action`: `enter | exit | toggle`.
- **Relationships**:
  - Delegates to the same entry/exit APIs as the player-facing device.

### Existing Entities Used (unchanged)

- `RaidSession` (core): Represents current raid run state.
- `Stash` (core): Stores extracted scrap.
- `RaidPlayer` (adapter): Holds the current session instance and exposes `IsInRaid`.
- `RaidSystem` (adapter): Provides `TryInteractPortal` and `TryInteractExit` and stores world-level stash/portal metadata.

## Validation Rules

- Dev toggle must be refused when dev tools are disabled.
- Entry is refused when already in a raid.
- Exit is refused when not in a raid.
- Multiplayer: one player’s transitions do not affect others.
