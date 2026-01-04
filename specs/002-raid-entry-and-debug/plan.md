# Implementation Plan: Raid Entry & Debug Controls

**Branch**: `002-raid-entry-and-debug` | **Date**: 2026-01-04 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-raid-entry-and-debug/spec.md`

## Summary

Add the missing in-game triggers that start and end raid sessions: a player-facing “hub console” interaction near spawn that delegates to existing raid entry/exit APIs, plus a gated dev-only chat command for fast testing. Keep Terraria hooks thin (adapters only), avoid irreversible world mutations, and ensure the feature has negligible runtime overhead when idle.

## Technical Context

**Language/Version**: C# (tModLoader mod; targets `net8.0` per project artifacts)
**Primary Dependencies**: tModLoader API (`ModSystem`, `ModPlayer`, `ModNPC`, `ModCommand`, `ModConfig`), Terraria base game APIs
**Storage**: TagCompound via tModLoader for mod data (stash + portal metadata already present); no irreversible world edits
**Testing**: MSTest unit tests (headless) under `Tests/Unit`
**Target Platform**: Windows/macOS/Linux (tModLoader)
**Project Type**: Single tModLoader mod project
**Performance Goals**: Vanilla-parity gameplay (60 fps) with negligible overhead; no per-tick heavy allocations in new hooks
**Constraints**:
- Glue stays in adapters; no raid rules inside hooks
- World-safe: avoid permanent worldgen/tiles for this feature; mod disable must not break world loading
- Multiplayer-safe: per-player raid state remains isolated and consistent
**Scale/Scope**: One hub entry/exit surface + one dev-only toggle; no new raid rules, loot rules, or NPC AI changes beyond wiring

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Loop fit: Provides explicit entry/exit flows so raids can start/end through player actions; exit delegates to existing extraction behavior and returns to hub (spawn). ✅
- Separation: New code is adapter glue (NPC interaction + command) that calls existing `RaidPlayer`/`RaidSystem` entry/exit methods; no raid rules added to hooks. ✅
- World safety: No new permanent world tiles/structures required; hub device uses an NPC, so disabling the mod leaves the world intact. ✅
- Performance/cross-platform: No custom per-tick polling required; only normal NPC behavior and command parsing on demand. ✅
- Testability: Core session transitions already covered by unit tests; feature adds a manual checklist and keeps new logic small and auditable. ✅

## Project Structure

### Documentation (this feature)

```text
specs/002-raid-entry-and-debug/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── interaction-contracts.md
└── tasks.md             # Phase 2 output (/speckit.tasks) - not created here
```

```text
Adapters/
├── Players/            # `RaidPlayer` (already exists)
├── Systems/            # `RaidSystem` (already exists)
├── NPCs/               # add hub console NPC (planned)
└── (planned) Commands/ # add dev chat command

Core/
├── Models/             # `RaidSession`, `RaidInventory`, `Stash`
└── Services/           # `RaidSessionService`, `StashService`

Tests/
└── Unit/               # MSTest unit tests (core logic)
```

**Structure Decision**: Keep a single-project mod with a strict Core (headless logic) vs Adapters (tModLoader glue) split. Entry UX and dev command live in Adapters and delegate to existing `RaidPlayer`/`RaidSystem` methods.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

None. The plan stays within existing patterns and avoids new infrastructure.

## Phase 0: Research (complete)

- Output: [research.md](research.md)
- Key result: prefer an NPC-based hub console to avoid permanent world mutations; provide dev toggle via gated chat command.

## Phase 1: Design & Contracts (complete)

- Output: [data-model.md](data-model.md)
- Output: [contracts/interaction-contracts.md](contracts/interaction-contracts.md)
- Output: [quickstart.md](quickstart.md)

## Phase 2: Implementation Plan (small slices)

### Slice 1: Player-facing hub console (NPC)

- Add a new adapter NPC that spawns/anchors near `Main.spawnTileX/Y` and provides interaction text/options.
- On interaction:
  - When player is not in raid: delegate to `RaidSystem.TryInteractPortal(player)`.
  - When player is in raid: delegate to `RaidSystem.TryInteractExit(player)`.
- Provide clear, non-spammy feedback based on return values.
- Multiplayer: validate behavior is per-player (uses `player.GetModPlayer<RaidPlayer>()`).

### Slice 2: Dev-only chat command (gated)

- Add a new `ModCommand` with `enter|exit|toggle`.
- Gate behind a config flag (default off) so it is not accessible in normal gameplay.
- Delegate to the same `RaidSystem.TryInteractPortal/TryInteractExit` methods.

### Slice 3: Minimal tests + validation

- Unit tests: no new core rules are introduced; ensure existing unit tests remain green.
- Add a small manual validation checklist (already captured in [quickstart.md](quickstart.md)).

## Risks & Mitigations

- **World safety**: Prefer NPC console (no persistent tiles). If a later feature adds a real portal tile, it must prove safe-disable behavior explicitly.
- **Interaction spam**: Ensure refusal messages are short and avoid repeated spam (cooldown or only message on state change).
- **Server/admin expectations**: Keep dev command gated by config and optionally server-side permission checks.
