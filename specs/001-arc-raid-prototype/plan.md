# Implementation Plan: Prototype ARC Raid Zone v0.1

**Branch**: `001-arc-raid-prototype` | **Date**: 2026-01-03 | **Spec**: [specs/001-arc-raid-prototype/spec.md](specs/001-arc-raid-prototype/spec.md)
**Input**: Feature specification from `/specs/001-arc-raid-prototype/spec.md`

## Summary

Deliver a playable ARC Raid Zone vertical slice that preserves the hub -> raid -> extract loop: clear entry point, drones that drop ARC Scrap, extraction that moves raid loot into stash, and death that deletes un-stashed scrap. Core rules live in plain C# services; tModLoader hooks stay as thin adapters. Persistence is isolated and safe to ignore when the mod is disabled. Performance remains at vanilla-like fps with no per-tick heavy allocations.

## Technical Context

**Language/Version**: C# (tModLoader, targets net8 per build artifacts)
**Primary Dependencies**: tModLoader API (ModSystem, ModPlayer, ModItem, ModNPC), base game assets
**Storage**: tModLoader mod storage via TagCompound; per-player stash data keyed by player identifier; no irreversible world changes
**Testing**: .NET unit tests for core services (headless); lightweight integration smoke using hook adapters where feasible
**Target Platform**: Windows/macOS/Linux (tModLoader)
**Project Type**: Single tModLoader mod project
**Performance Goals**: Maintain 60 fps parity with vanilla; avoid per-tick allocations in AI/hooks; negligible load-time overhead
**Constraints**: Must keep worlds loadable if mod is disabled; no irreversible world edits; adapters only thin delegation; safe defaults on missing config
**Scale/Scope**: Single zone, single resource (ARC Scrap), single enemy (ARC Drone), single extraction flow for v0.1

## Constitution Check

- Loop fit: Entry portal into raid; extraction exit back to hub moves raid inventory to stash; death clears raid inventory. ✅
- Separation: Core services (`RaidSessionService`, `StashService`, drop helpers) pure C#; hooks delegate only. ✅
- World safety: Persist in mod storage; ignore safely if mod disabled; no irreversible tiles/NPCs; cleanup on session end. ✅
- Performance/cross-platform: O(1) per tick AI/state; no reflection/heavy alloc; capability checks if platform-specific code appears (none planned). ✅
- Testability: Unit tests for services and rules; integration smoke for entry/extract/save-load via adapters. ✅

## Project Structure

### Documentation (this feature)

```text
specs/001-arc-raid-prototype/
├── plan.md
├── spec.md
└── (future) research.md, data-model.md, quickstart.md, contracts/, tasks.md
```

### Source Code (repository root)

```text
TerrariaArcRaiders/
├── Core/
│   ├── Models/            # RaidSession, RaidInventory, Stash, ArcScrap descriptors
│   └── Services/          # RaidSessionService, StashService, DropRules
├── Adapters/
│   ├── Systems/           # ModSystem for world load/save & zone bootstrap
│   ├── Players/           # ModPlayer for entry/extract/death hooks
│   ├── Items/             # ModItem for ArcScrap wrapper
│   └── NPCs/              # ModNPC for ArcDrone wrapper (delegates to helper)
├── Content/               # Sprites/JSON if added later (out of scope for logic)
└── Tests/
    └── Unit/              # Headless tests for services and rules
```

**Structure Decision**: Single-project tModLoader mod with clear Core (logic) vs Adapters (hooks) split to satisfy decoupled-logic principle; unit tests live under Tests/Unit.

## Plan (incremental slices)

1) Foundations & Data Shapes
- Add Core/Models: `RaidSessionStatus` enum (Entered, Active, Extracted, Failed); `RaidSession` (player id, status, raid inventory of ARC Scrap, timestamps); `RaidInventory` (scrap amount, clear/reset helpers); `Stash` (scrap amount, add/remove with bounds); `ArcScrap` descriptor (id, max stack, value).
- Add Core/Services scaffolding: `RaidSessionService` (state transitions: start, record loot, extract, fail/death handling), `StashService` (persisted stash ops, transfer-in on extract), `DropRules` helper (convert drone drops to raid inventory increments). Keep pure C# with no tModLoader types.
- Tests: Unit tests for state transitions (enter, gain scrap, extract, death clears raid inventory), stash transfer, and idempotent resets.

2) Persistence Strategy
- Implement `IRaidPersistence` interface (save/load stash per player, optional last session snapshot) using TagCompound; default implementation in Core/Services or Adapters with clean boundaries.
- Add `Adapters/Systems/RaidSystem` (ModSystem) to load/save stash data via TagCompound; ensure safe-ignore paths when data missing/corrupt; no world tile edits.
- Tests: Unit test persistence serializer (pure data to TagCompound-like DTO) to ensure forward/backward safety and empty/default handling.

3) Entry/Exit Wiring (Adapters)
- Add `Adapters/Players/RaidPlayer` (ModPlayer): handles entry trigger, calls `RaidSessionService.Start`; holds transient raid inventory state in memory; on player death calls `RaidSessionService.Fail`; on extract calls `RaidSessionService.Extract` then `StashService.Deposit` and clears raid inventory.
- Add entry point mechanism (v0.1 simple): place a world portal/structure near spawn via `RaidSystem.OnWorldLoad` once per world if absent; interaction calls into service; include guard to avoid duplicates.
- Add extraction exit inside zone (paired portal or tile) that triggers extract flow; ensure it only works when session active.
- Tests: Integration smoke (if feasible) or service-level tests simulating entry/extract/death via adapter shims to validate delegation order.

4) ARC Scrap Item & ARC Drone NPC
- Add `Adapters/Items/ArcScrapItem` (ModItem) thin wrapper around `ArcScrap` descriptor; ensure it is only created via drop rules and respects max stack.
- Add `Adapters/NPCs/ArcDroneNPC` (ModNPC) that spawns only inside the raid zone; AI uses lightweight helper (no per-tick alloc); OnKill delegates to `DropRules` to award scrap into current raid session inventory.
- Performance guard: avoid LINQ/alloc in AI; use cached references.
- Tests: Unit tests for drop helper mapping NPC kill -> raid inventory increment; optional perf assertion (allocation-free per tick) via simple benchmark guard.

5) Stash Persistence & Disable Safety
- Ensure stash saved per player id via `ModSystem.SaveWorldData`/`LoadWorldData` or `ModPlayer.SaveData`/`LoadData` (choose per-player scope; prefer ModPlayer for stash).
- Add safe disable path: if mod disabled, stash data ignored; no crashes on load; optional cleanup helper to clear transient raid session on unload.
- Tests: Persistence round-trip tests; simulate disable by deserializing absence of data (expect defaults, no exceptions).

6) Minimal UX Hooks
- Provide basic text prompts/notifications (entry, extraction success, death loss) via Chat/OnScreenText; avoid complex UI for v0.1.
- Guard against inventory-full on extraction: if player inventory full, stash still updated because stash is separate; communicate result.

7) Test & QA Pass
- Run unit tests; add integration smoke (enter -> loot -> extract) via adapter-driven harness if feasible; manual in-game validation checklist for fps sanity.
- Profiling spot-check: ensure drone AI and hooks allocate minimally (review allocations, use pooled structs where possible).

## Risks & Mitigations
- World safety: Use per-player stash via ModPlayer; avoid tile edits; portal placement uses minimal tiles and checks duplicates; provide remove-on-unload if needed.
- Duplication/loot exploits: Gate extraction to active session; clear raid inventory on exit/death; ensure drop helper respects active session only.
- Performance: Avoid per-tick allocations in NPC AI; keep state cached; minimal logic in hooks.
- Multi-player: Keep session keyed by player id; stash per player; avoid shared static state; ensure thread-safety assumptions (tML main thread).

## Test Plan (minimal, incremental)
- Unit: `RaidSessionService` state transitions; death loss; extract transfer; stash bounds; drop helper NPC kill -> raid inventory increment; persistence serializer round-trip.
- Integration (lightweight): Adapter shim simulating entry -> loot -> extract; death scenario clearing raid inventory; save/load stash persistence.
- Manual: In-game run through user stories (US1 enter/loot/extract, US2 death loss, US3 stash survives reload and safe disable), fps sanity, duplicate prevention.

## Deliverable/Commit Slices
- Commit 1: Core models + services + unit tests (headless).
- Commit 2: Persistence interface + serializer + tests.
- Commit 3: Adapters for ModPlayer/ModSystem entry/extract/death wiring; simple portal placement; smoke test harness if possible.
- Commit 4: ArcScrap ModItem + ArcDrone ModNPC + drop helper linkage; perf guard adjustments.
- Commit 5: Persistence hook wiring, safe-disable handling, notifications, cleanup polish.
- Commit 6: Test and manual validation notes.
