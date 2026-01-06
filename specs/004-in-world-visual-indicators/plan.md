# Implementation Plan: In-World Visual Indicators

**Branch**: `004-in-world-visual-indicators` | **Date**: 2026-01-05  
**Spec**: `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\spec.md`  
**Input**: Feature specification from `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\spec.md`

## Summary

Add an opt-in debug feature that places deterministic, in-world visual markers for each Arc worldgen stage so developers can validate the pipeline by inspecting the generated world (not just logs). Markers are safe (vanilla-world compatible), bounded (no full-world scans), deterministic, and only applied to Arc worlds when explicitly enabled.

## Technical Context

**Language/Version**: C# (`net8.0`), tModLoader 2025.x (`TML_2025_11` define)  
**Primary Dependencies**: tModLoader/Terraria APIs, MSTest for headless unit tests  
**Storage**: Vanilla world tiles (for the indicators) + mod world data via `TagCompound` (`ArcWorldData`) where needed  
**Testing**: MSTest; `dotnet build -p:BuildTests=true` for headless compile checks  
**Target Platform**: Windows/macOS/Linux (tModLoader)  
**Project Type**: tModLoader mod (single project)  
**Performance Goals**: No noticeable worldgen slowdown; indicator placement is O(stages) and bounded to a tiny region  
**Constraints**: No full-world scans; indicators must use vanilla world content; must not break world load/save if the mod is disabled/removed; deterministic placement for seed+size  
**Scale/Scope**: ~8 Arc stages today (A–H) plus optional debug marker; small debug-only surface area

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Loop fit: Pass. This improves debugging of Arc world setup (hub/regions), supporting the hub → raid → extract loop indirectly by making worldgen correctness observable.
- Separation: Pass. Keep indicator planning/layout logic in `Core/` (headless service); Terraria hooks/passes call into it.
- World safety: Pass, with a strict constraint: indicators MUST be built from vanilla tiles/objects so worlds remain loadable if the mod is disabled/removed.
- Performance/cross-platform: Pass. Placement is bounded and deterministic, no per-tick work, no I/O in hooks.
- Testability: Pass. Layout and mapping logic is unit-testable headless; integration validated via in-game worldgen.

## Project Structure

### Documentation (this feature)

```text
specs/004-in-world-visual-indicators/
  spec.md
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
```

### Source Code (repository root)

```text
Adapters/
  Systems/
    ArcRaidersConfig.cs
    ArcWorldGenSystem.cs
    ArcWorldSystem.cs
  WorldGen/
    ArcWorldGenPipeline.cs
    Passes/
      ArcStageA_Setup.cs
      ArcStageB_BaseTerrain.cs
      ArcStageC_RegionPlanning.cs
      ArcStageD_BiomePainting.cs
      ArcStageE_StructureReservation.cs
      ArcStageF_StructurePlacement.cs
      ArcStageG_RaidAnchors.cs
      ArcStageH_FinalValidation.cs
      ArcStageZ_TestMarker.cs

Core/
  WorldGen/
    ArcWorldData.cs
    ArcWorldGenStage.cs
    ArcWorldPlanService.cs

Tests/
  Unit/
    ArcWorldPlanServiceTests.cs
```

**Structure Decision**: Single tModLoader mod project. New logic lives in `Core/WorldGen/Indicators/` (headless), wired from `Adapters/WorldGen/Passes/`.

## Phase 0: Outline & Research

**Goals**
- Choose indicator mechanism that is safe for vanilla world loading (no custom tiles).
- Choose where/when to place indicators so they survive vanilla worldgen.
- Define deterministic layout tied to planned Arc regions.

**Deliverable**
- `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\research.md`

## Phase 1: Design & Contracts

**Design decisions (high-level)**
- Add a single opt-in config flag (default off).
- Record stage completion in memory during worldgen; place all indicators at the end of worldgen to avoid vanilla overwrites.
- Place indicators near the planned hub region (`ArcWorldData.SafeHubRegion`) using a bounded, deterministic layout.
- Use only vanilla tiles/objects (e.g., blocks/torches/signs) so worlds remain loadable even if the mod is removed.

**Deliverables**
- `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\data-model.md`
- `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\contracts\`
- `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\quickstart.md`

**Agent context update**
- Run `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\.specify\scripts\powershell\update-agent-context.ps1 -AgentType codex`

**Post-Phase 1 Constitution Re-check (completed)**
- Loop fit: Pass (debug tooling for worldgen correctness).
- Separation: Pass (plan keeps layout logic in `Core/`, adapters in `Adapters/`).
- World safety: Pass (vanilla-only markers; no custom tiles).
- Performance/cross-platform: Pass (bounded placement; no per-tick work).
- Testability: Pass (layout/planning unit tests + in-game validation).

## Phase 2: Implementation Planning (stop after this phase)

**Planned implementation steps**
1. Add config toggle for worldgen indicators (default off).
2. Add headless indicator layout service (inputs: hub rect, stage list; outputs: placements + legend).
3. Add stage-completion tracker used by Arc passes.
4. Add a final “place indicators” worldgen pass inserted after vanilla worldgen tasks.
5. Add unit tests for deterministic layout and legend mapping.
6. Validate in-game: generate Arc world with indicators on/off; verify visibility, determinism, and no impact on non-Arc worlds.
