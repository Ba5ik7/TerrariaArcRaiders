---

description: "Tasks for implementing In-World Visual Indicators"
---

# Tasks: In-World Visual Indicators

**Input**: Design documents from `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\`
**Prerequisites**: `plan.md` (required), `spec.md` (required), `research.md`, `data-model.md`, `contracts/*`, `quickstart.md`

**Tests**: Not requested in the feature spec; tasks focus on implementation + in-game verification.

**Constitution Alignment**: Vanilla-world-safe (no custom tiles), bounded work (no full-world scans), minimal performance impact, core layout logic in `Core/` and Terraria glue in `Adapters/`.

## Format: `- [ ] T### [P?] [US#?] Description with file path`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[US#]**: User story label (US1/US2/US3) for story phases only
- All tasks include absolute paths

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm baseline build + establish file locations for the feature

- [x] T001 Confirm baseline build succeeds (project: `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\TerrariaArcRaiders.csproj`)
- [x] T002 [P] Create feature folder for headless indicator logic in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\`
- [x] T003 [P] Create feature folder for Terraria placement glue in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Indicators\`

Notes:
- Baseline build command: `dotnet build TerrariaArcRaiders.csproj`
- Folders created: `Core/WorldGen/Indicators/`, `Adapters/WorldGen/Indicators/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add the opt-in switch and core (headless) models/services used by all stories

- [x] T004 Add `WorldGenVisualIndicatorsEnabled` config toggle (default false) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\Systems\ArcRaidersConfig.cs`
- [x] T005 Add stage completion run-state (resettable, no tModLoader types) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorRunState.cs`
- [x] T006 [P] Add stage legend metadata (stage -> label/name) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorLegend.cs`
- [x] T007 [P] Add placement model (stage + tile coordinates + label) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorPlacement.cs`
- [x] T008 Implement deterministic layout service (hub rect + stage list -> placements) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorLayoutService.cs`
- [x] T009 Document the indicator legend (human-readable mapping) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\contracts\indicator-legend.md`

Notes:
- Headless types live under `Core/WorldGen/Indicators/` and only depend on `Core/WorldGen/*` primitives (`IntRect`, `ArcWorldGenStage`).
- Layout output is deterministic and bounded to `SafeHubRegion` (no world-size inputs required yet).

**Checkpoint**: Config + headless indicator planning exists; Terraria placement can now be implemented.

---

## Phase 3: User Story 1 - See Arc Worldgen Progress In-World (Priority: P1) — MVP

**Goal**: Place visible in-world indicators per Arc stage so devs can confirm stage execution without logs.

**Independent Test**: Create a new Arc world with indicators enabled; enter the world; locate the indicator “board” and confirm one marker per stage A–H (and that each marker maps to its stage name/order).

### Implementation (US1)

- [x] T010 [P] [US1] Add safe, bounded Terraria placer for a stage "board" (platform + sign/torch placement) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Indicators\ArcWorldGenIndicatorPlacer.cs`
- [x] T011 [US1] Add final worldgen pass to place indicators (runs after vanilla worldgen tasks) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`
- [x] T012 [US1] Append the final indicator pass at the end of worldgen tasks (only when Arc + toggle enabled) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\Systems\ArcWorldGenSystem.cs`
- [x] T013 [US1] Reset indicator run-state at start of worldgen (so repeated generations don't leak state) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\Systems\ArcWorldSystem.cs`

Notes:
- Indicator placement uses vanilla tiles only and is clamped to `SafeHubRegion` (no full-world scans).
- `ArcStageI_VisualIndicators` falls back to placing markers for all stages until per-stage completion marking (T014–T022) is implemented.

### Stage completion recording (US1)

- [x] T014 [P] [US1] Mark Stage A completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageA_Setup.cs`
- [x] T015 [P] [US1] Mark Stage B completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageB_BaseTerrain.cs`
- [x] T016 [P] [US1] Mark Stage C completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageC_RegionPlanning.cs`
- [x] T017 [P] [US1] Mark Stage D completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageD_BiomePainting.cs`
- [x] T018 [P] [US1] Mark Stage E completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageE_StructureReservation.cs`
- [x] T019 [P] [US1] Mark Stage F completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageF_StructurePlacement.cs`
- [x] T020 [P] [US1] Mark Stage G completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageG_RaidAnchors.cs`
- [x] T021 [P] [US1] Mark Stage H completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageH_FinalValidation.cs`
- [x] T022 [P] [US1] If DEBUG marker stage is compiled, mark its completion in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageZ_TestMarker.cs`

Notes:
- Stage completion is recorded via `ArcWorldSystem.WorldGenIndicatorRunState.MarkCompleted(...)` in each pass.
- Debug-only completion uses `ArcWorldGenStage.StageZ_TestMarker` (does not affect main stage ordering).

**Checkpoint**: An Arc world generated with indicators enabled contains visible per-stage markers.

---

## Phase 4: User Story 2 - Keep Visual Indicators Opt-In (Priority: P2)

**Goal**: Ensure indicators never appear unless explicitly enabled, and never appear in non-Arc worlds.

**Independent Test**: Generate (1) Arc world with toggle off, (2) Arc world with toggle on, (3) non-Arc world; verify indicators appear only in case (2).

- [x] T023 [US2] Ensure the indicator pass hard-exits when toggle disabled in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`
- [x] T024 [US2] Ensure the indicator pass hard-exits for non-Arc worlds (even if toggle enabled) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`
- [x] T025 [US2] Update quickstart to include the toggle + expected behaviors in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\quickstart.md`

Notes:
- `ArcStageI_VisualIndicators` hard-exits early for non-Arc worlds and when the config toggle is disabled.

**Checkpoint**: Toggle semantics match FR-001/002/003 and SC-002.

---

## Phase 5: User Story 3 - Quickly Locate Planned Arc Regions (Priority: P3)

**Goal**: Make indicator placement predictable and tied to planned Arc regions (hub/regions/reserved sites).

**Independent Test**: Generate the same Arc seed multiple times; indicator “board” appears in consistent location relative to planned hub region; dev can find it quickly.

- [x] T026 [US3] Anchor layout coordinates to `ArcWorldData.SafeHubRegion` (clamped within world bounds) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorLayoutService.cs`
- [x] T027 [US3] Use the planned hub placement from the layout service when placing the indicator board in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`
- [x] T028 [US3] Place an additional marker at/near the planned reserved site (if present) to validate reserved-site planning in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`

Notes:
- Layout now accepts optional world bounds (`Main.maxTilesX/maxTilesY` passed from the worldgen pass) and clamps outputs accordingly.
- Reserved-site marker uses a bounded, deterministic local search and only places tiles into empty space (no clearing).

**Checkpoint**: Placement is deterministic for seed+size and discoverable near planned regions (FR-006/007, SC-003).

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Harden safety/perf and validate end-to-end

- [x] T029 [P] Keep marker placement bounded and avoid any full-world loops in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Indicators\ArcWorldGenIndicatorPlacer.cs`
- [x] T030 Ensure failures to place markers never fail worldgen (skip placement safely) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageI_VisualIndicators.cs`
- [ ] T031 Run in-game validation steps from `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\quickstart.md`

Notes:
- T031 requires a manual in-game run; `specs/004-in-world-visual-indicators/quickstart.md` now includes a "Validation Log" section to record results.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies
- **Phase 2 (Foundational)**: Depends on Phase 1; blocks all user stories
- **Phase 3 (US1)**: Depends on Phase 2
- **Phase 4 (US2)**: Depends on Phase 3 (uses the implemented pass for behavioral checks)
- **Phase 5 (US3)**: Depends on Phase 3 (refines placement/layout)
- **Phase 6 (Polish)**: Depends on Phases 3–5 as applicable

### User Story Dependencies

- **US1 (P1)**: Requires Phase 2 only
- **US2 (P2)**: Builds on US1 to enforce/validate gating and documentation
- **US3 (P3)**: Builds on US1 to make placement predictable and tied to planned regions

---

## Parallel Execution Examples

### Parallel Example: Foundational (Phase 2)

You can run these in parallel once T004 is done:

- Task: T006 (legend) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorLegend.cs`
- Task: T007 (placement model) in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Core\WorldGen\Indicators\ArcWorldGenIndicatorPlacement.cs`

### Parallel Example: US1 stage completion marking (Phase 3)

These can be done in parallel (different files):

- Task: T014 in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageA_Setup.cs`
- Task: T016 in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageC_RegionPlanning.cs`
- Task: T021 in `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\Adapters\WorldGen\Passes\ArcStageH_FinalValidation.cs`

---

## Implementation Strategy

### MVP First (US1)

1. Complete Phase 1 and Phase 2
2. Complete Phase 3 (US1)
3. Generate an Arc world with indicators enabled and confirm markers are visible and mapped to stage names

### Incremental Delivery

1. US1: visible per-stage indicators (MVP)
2. US2: tighten opt-in and non-Arc safeguards + quickstart docs
3. US3: deterministic placement tied to planned hub/regions
