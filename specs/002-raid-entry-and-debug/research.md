# Research: Raid Entry & Debug Controls

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

## Goal

Select a world-safe, low-overhead, player-facing way to trigger existing raid entry/exit APIs in-game, plus a dev-only toggle that accelerates testing.

## Decisions

### Decision 1: Use a single stationary “Raid Console” NPC near spawn as the player-facing entry/exit device

- **Decision**: Implement the hub entry/exit UX as an in-world NPC placed/spawned near world spawn (acts like a console/terminal). Right-click interaction presents “Enter Raid” when not in raid and “Extract / Exit Raid” when in raid.
- **Rationale**:
  - Avoids irreversible world mutations: no tiles/objects must be placed into the world save.
  - Natural Terraria interaction model: NPC chat interaction is familiar and discoverable.
  - Keeps hooks thin: NPC interaction delegates directly to `RaidSystem.TryInteractPortal` / `RaidSystem.TryInteractExit`.
  - Performance-friendly: no custom per-tick polling required to detect interaction; only normal NPC update overhead.
- **Alternatives considered**:
  - **Place a ModTile/TileEntity “portal/console” at spawn**: better visual “device”, but writes tiles into the world and must be proven safe on mod disable.
  - **Give the player an item that toggles raid**: world-safe and simple, but less aligned with “hub device near spawn” and can clutter inventories.
  - **Per-tick proximity + input detection at a saved coordinate**: minimal code, but requires continuous polling (even if light) and is easier to get wrong for multiplayer.

### Decision 2: Dev-only control is a chat command gated by config (default off)

- **Decision**: Provide a `ModCommand` (e.g., `/arcraid enter|exit|toggle`) and gate it behind a boolean mod config flag (default `false`).
- **Rationale**:
  - Dev velocity: fastest path to toggle raid state for debugging.
  - Separation: command is adapter glue; it calls the existing entry/exit APIs.
  - Safety: config gate prevents accidental use in normal gameplay.
- **Alternatives considered**:
  - **Compile-time `#if DEBUG`**: safe, but makes it harder to test dev tools in release builds and on servers.
  - **Dev-only item**: works, but adds content and raises additional disable/cleanup considerations.

### Decision 3: Use existing “hub” semantics (teleport to spawn) and avoid new worldgen

- **Decision**: Treat spawn as the hub and keep the current “teleport to spawn” behavior for exit; do not add worldgen or permanent structures for this feature.
- **Rationale**: Fits the prototype stage and respects world safety and “no raid rules in hooks” constraint.
- **Alternatives considered**:
  - **Generate a portal structure**: stronger theming, but is world mutation and out of scope for this “wiring” feature.

## Open Questions

None required to proceed. Any future improvements (real portal tile, structure, or dedicated raid zone teleport) can be a follow-on feature.
