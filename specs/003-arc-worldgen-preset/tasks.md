---

description: "Task list for implementing Arc Raider world preset + worldgen foundation"
---

# Tasks: Arc Raider Worldgen Preset

**Input**: Design documents in `specs/003-arc-worldgen-preset/`
- Spec: [spec.md](spec.md)
- Plan: [plan.md](plan.md)
- Research: [research.md](research.md)
- Data model: [data-model.md](data-model.md)
- Contracts: [contracts/interaction-contracts.md](contracts/interaction-contracts.md)
- Quickstart: [quickstart.md](quickstart.md)

**Constitution alignment (must-haves)**
- Loop integrity: Safe Hub + reserved raid sites anchor the hub → raid → extract loop (see spec FR-015/FR-016).
- Decoupled logic: Planning logic in `Core/` and hooks/tile mutation in `Adapters/`.
- World safety: Non-Arc worlds unchanged; Arc metadata fails safe; mod disable does not break world load.
- Performance: Avoid full-world scans where possible; bounded rectangle edits; deterministic RNG.
- Testability: Deterministic planning is unit-testable headless; hook boundaries validated via quickstart.

## Format

Every task MUST follow this checklist format:

- [ ] T### [P?] [US#?] Description with file path

- **[P]** = can run in parallel (different files, no dependencies)
- **[US#]** = user story mapping (only in user story phases)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish feature-scoped structure and shared utilities for Arc worldgen without affecting vanilla worlds.

**Scope notes**
- Links: plan “Project Structure” + “Worldgen Architecture Design”, spec FR-008..FR-011.

- [X] T001 Create Arc worldgen folder structure `Core/WorldGen/` and `Adapters/WorldGen/` (folders added)
- [X] T002 [P] Add feature docs index notes to `README.md` (link to `specs/003-arc-worldgen-preset/`) (docs index updated)
- [X] T003 [P] Add a minimal debug logging helper for worldgen stages in `Adapters/WorldGen/ArcWorldGenLog.cs` (logging helper added)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core primitives required by all stories (selection parsing, data structures, safe persistence, deterministic planning scaffolding).

**Constraints from constitution**
- Keep planning logic tModLoader-free; adapters remain thin.
- World safety: defaults must be “treat as non-Arc” if metadata is missing/corrupt.

**Links**
- Plan: “World Type Integration Design”, “Worldgen Architecture Design”
- Spec: FR-004..FR-005, FR-017..FR-019; data-model entities

- [X] T004 Define `ArcRegionId` and `IntRect` primitives in `Core/WorldGen/ArcRegionId.cs` and `Core/WorldGen/IntRect.cs` (primitives added)
- [X] T005 Define `ArcReservedSite` and `ArcWorldData` DTOs in `Core/WorldGen/ArcWorldData.cs` (DTOs added)
- [X] T006 Implement seed-prefix parsing `ArcWorldSelection` in `Core/WorldGen/ArcWorldSelection.cs` (seed parsing implemented)
- [X] T007 [P] Add unit tests for seed parsing in `Tests/Unit/ArcWorldSelectionTests.cs` (tests added)
- [X] T008 Implement world metadata bridge (TagCompound <-> DTO) in `Adapters/Systems/ArcWorldDataBridge.cs` (bridge added)
- [X] T009 [P] Add unit tests for world metadata bridge in `Tests/Unit/ArcWorldDataBridgeTests.cs` (tests added)

**Checkpoint**: Core primitives exist; Arc selection and persistence can be implemented safely.

---

## Phase 3: User Story 1 - Create an Arc Raider world (Priority: P1) 🎯 MVP

**Goal**: Player can create an Arc Raider world using a tModLoader-compatible “preset selection” mechanism and the world is tagged for reliable detection; non-Arc worlds are unaffected.

**Independent Test**
- From [spec.md](spec.md): “User Story 1” + AC-001.
- From [quickstart.md](quickstart.md): “Create an Arc Raider world” + “Create a normal world”.

**Architecture notes**
- Use a dedicated `ModSystem` as the integration point:
  - Reads selection before worldgen.
  - Writes/reads header + world data.
  - Exposes `IsArcWorld` + anchors for other systems.
- Keep core selection + data types in `Core/WorldGen/`.

- [ ] T010 [US1] Implement Arc world flag + reset defaults in `Adapters/Systems/ArcWorldSystem.cs`
- [ ] T011 [US1] Persist `IsArcWorld` + `DataVersion` in `Adapters/Systems/ArcWorldSystem.SaveWorldHeader()`
- [ ] T012 [US1] Persist full Arc metadata in `Adapters/Systems/ArcWorldSystem.SaveWorldData()`
- [ ] T013 [US1] Load Arc metadata with safe-fail behavior in `Adapters/Systems/ArcWorldSystem.LoadWorldData()`
- [ ] T014 [US1] Detect Arc preset selection in `Adapters/Systems/ArcWorldSystem.PreWorldGen()` using `Core/WorldGen/ArcWorldSelection.cs`
- [ ] T015 [US1] Ensure non-Arc worlds keep vanilla behavior by gating on `ArcWorldSystem.IsArcWorld` in `Adapters/Systems/ArcWorldSystem.cs`
- [ ] T016 [P] [US1] Add a minimal “Arc world detected” debug log line on load in `Adapters/Systems/ArcWorldSystem.cs`

**Checkpoint**: Player can create Arc vs non-Arc worlds; Arc tag persists; safe default is non-Arc.

---

## Phase 4: User Story 2 - Explore a clearly distinct layout (Priority: P2)

**Goal**: Arc worlds generate a visibly different layout with at least a Safe Hub region and one additional named placeholder region.

**Independent Test**
- From [spec.md](spec.md): “User Story 2” + AC-002 and AC-003.
- Manual validation steps in [quickstart.md](quickstart.md).

**Architecture notes**
- Worldgen pipeline lives in `Adapters/WorldGen/` but consumes a deterministic plan from `Core/WorldGen/`.
- Insert Arc worldgen tasks via `ModSystem.ModifyWorldGenTasks` and execute them only when `IsArcWorld=true`.

**Constraints**
- Performance: avoid repeated full-world scans; prefer bounded region rectangles.
- World safety: do not mutate non-Arc worlds; Arc generation must complete even if optional slots are stubbed.

- [ ] T017 [US2] Define `ArcWorldPlan` (regions + reserved sites) in `Core/WorldGen/ArcWorldPlan.cs`
- [ ] T018 [US2] Implement deterministic plan builder `ArcWorldPlanService` in `Core/WorldGen/ArcWorldPlanService.cs`
- [ ] T019 [P] [US2] Add determinism unit tests for plan builder in `Tests/Unit/ArcWorldPlanServiceTests.cs`
- [ ] T020 [US2] Add worldgen task wiring in `Adapters/Systems/ArcWorldGenSystem.cs` (new `ModSystem`)
- [ ] T021 [US2] Implement Stage A “World Tagging & Setup” pass in `Adapters/WorldGen/Passes/ArcStageA_Setup.cs`
- [ ] T022 [US2] Implement Stage B “Base Terrain Layout” placeholder pass in `Adapters/WorldGen/Passes/ArcStageB_BaseTerrain.cs`
- [ ] T023 [US2] Implement Stage C “Region Planning” pass that writes `ArcWorldData.Regions` in `Adapters/WorldGen/Passes/ArcStageC_RegionPlanning.cs`
- [ ] T024 [US2] Implement Stage D “Biome Painting (placeholder)” pass in `Adapters/WorldGen/Passes/ArcStageD_BiomePainting.cs`
- [ ] T025 [US2] Implement Stage E “Structure Reservation (sites only)” pass in `Adapters/WorldGen/Passes/ArcStageE_StructureReservation.cs`
- [ ] T026 [US2] Implement Stage F “Structure Placement (placeholder)” pass in `Adapters/WorldGen/Passes/ArcStageF_StructurePlacement.cs`
- [ ] T027 [US2] Implement Stage G “Raid-Related Anchors (reserved only)” pass in `Adapters/WorldGen/Passes/ArcStageG_RaidAnchors.cs`
- [ ] T028 [US2] Implement Stage H “Final Polish & Validation” pass (validate hub + 1 slot) in `Adapters/WorldGen/Passes/ArcStageH_FinalValidation.cs`
- [ ] T029 [P] [US2] Add stage-order logging for Arc worlds in `Adapters/WorldGen/ArcWorldGenLog.cs`
- [ ] T030 [US2] Store computed hub region + at least one extra slot into `ArcWorldSystem` world data in `Adapters/Systems/ArcWorldSystem.cs`

**Checkpoint**: Arc world has Safe Hub + one additional named region; world layout is visibly non-vanilla.

---

## Phase 5: User Story 3 - Extend Arc Raider worldgen without rewrites (Priority: P3)

**Goal**: A mod developer can add a new worldgen stage by implementing a small, well-defined interface/class and inserting it at a named stage boundary.

**Independent Test**
- From [spec.md](spec.md): “User Story 3” + AC-003.

**Architecture notes**
- Implement a simple stage registry to map “Stage A..H” boundaries to pass lists.
- Ensure adding/removing a pass does not require editing existing pass implementations.

- [ ] T031 [US3] Define a stage boundary enum/type in `Core/WorldGen/ArcWorldGenStage.cs`
- [ ] T032 [US3] Define an adapter interface for passes in `Adapters/WorldGen/IArcWorldGenPass.cs`
- [ ] T033 [US3] Implement stage registry in `Adapters/WorldGen/ArcWorldGenPipeline.cs`
- [ ] T034 [US3] Refactor stage pass wiring to use the pipeline registry in `Adapters/Systems/ArcWorldGenSystem.cs`
- [ ] T035 [P] [US3] Add a sample “test stage” pass in `Adapters/WorldGen/Passes/ArcStageZ_TestMarker.cs` (debug-only)
- [ ] T036 [US3] Document “how to add a new biome/structure pass” in `specs/003-arc-worldgen-preset/research.md`

**Checkpoint**: Adding a new pass is a small, localized change; stage order remains stable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Hardening, performance checks, and constitution-related guardrails.

- [ ] T037 [P] Add world safety notes for disabling/removing the mod in `specs/003-arc-worldgen-preset/quickstart.md`
- [ ] T038 Add safe-failure handling for corrupted Arc world metadata in `Adapters/Systems/ArcWorldSystem.cs`
- [ ] T039 [P] Add minimal performance guardrails (avoid full-world scan) in `Adapters/WorldGen/Passes/*.cs`
- [ ] T040 [P] Ensure raid systems can query hub region and reserved sites via `Adapters/Systems/ArcWorldSystem.cs`
- [ ] T041 Run quickstart validation steps and record results in `specs/003-arc-worldgen-preset/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)** → **Phase 2 (Foundational)** → **US1** → **US2** → **US3** → **Polish**

### User Story Dependencies

- **US1** depends on Phase 1–2.
- **US2** depends on US1 (must be able to select/tag Arc worlds before Arc worldgen can run).
- **US3** can start after US2 begins but is best after US2’s initial stage set exists.

### Parallel Opportunities

- Setup: T002, T003
- Foundational: T007 and T009 can run in parallel once their corresponding files exist.
- US2: T021–T028 are sequential by stage order, but stage logging (T029) can be parallel.
- US3: T031–T034 can be parallelized with doc work (T036) after stage boundaries are defined.

---

## Parallel Example: User Story 1

- Task: T011 Persist `IsArcWorld` + `DataVersion` in `Adapters/Systems/ArcWorldSystem.SaveWorldHeader()`
- Task: T012 Persist full Arc metadata in `Adapters/Systems/ArcWorldSystem.SaveWorldData()`
- Task: T014 Detect Arc preset selection in `Adapters/Systems/ArcWorldSystem.PreWorldGen()`

---

## Parallel Example: User Story 2

- Task: T017 Define `ArcWorldPlan` in `Core/WorldGen/ArcWorldPlan.cs`
- Task: T018 Implement `ArcWorldPlanService` in `Core/WorldGen/ArcWorldPlanService.cs`
- Task: T019 Unit tests in `Tests/Unit/ArcWorldPlanServiceTests.cs`

---

## Parallel Example: User Story 3

- Task: T031 Define stage boundary enum/type in `Core/WorldGen/ArcWorldGenStage.cs`
- Task: T032 Define adapter interface for passes in `Adapters/WorldGen/IArcWorldGenPass.cs`
- Task: T033 Implement stage registry in `Adapters/WorldGen/ArcWorldGenPipeline.cs`

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1–2
2. Implement US1 tasks T010–T016
3. Stop and validate AC-001 via [quickstart.md](quickstart.md)

### Incremental Delivery

- Add US2 worldgen pipeline to deliver visible Arc layout (AC-002/AC-003)
- Add US3 pipeline registry to enable future specs to plug in passes cleanly

---

## Format Validation

All tasks above are formatted as:

- [ ] T### [P?] [US#?] Description with file path
