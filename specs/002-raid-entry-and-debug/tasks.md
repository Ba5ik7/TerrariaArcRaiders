---

description: "Task list for Raid Entry & Debug Controls"
---

# Tasks: Raid Entry & Debug Controls

**Input**: Design documents from `/specs/002-raid-entry-and-debug/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/interaction-contracts.md](contracts/interaction-contracts.md), [quickstart.md](quickstart.md)

**Constitution Alignment**: Tasks below preserve the hub -> raid -> extract loop, keep core logic separate from Terraria hooks, protect vanilla worlds (safe disable), honor performance budgets (no new per-tick heavy work), and keep behavior testable.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `- [ ] [TaskID] [P?] [Story?] Description with file path`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[US#]**: Which user story this task belongs to (US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Ensure the project has the minimal scaffolding needed to add adapter-level features cleanly.

- [x] T001 Confirm branch builds and tests run locally via `dotnet test` (baseline)
- [x] T002 [P] Add documentation links for this feature in README.md (optional) or keep scope limited to specs only

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Small shared building blocks required before any user story work.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T003 Create dev-tools config flag in TerrariaArcRaiders.cs or a new config file at Adapters/Systems/ArcRaidersConfig.cs
- [x] T004 [P] Define shared user feedback helper for raid entry/exit messages in Adapters/Systems/RaidUiNotifications.cs

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Start and End a Raid via the Hub (Priority: P1) 🎯 MVP

**Goal**: Provide a player-facing, discoverable in-world hub interaction to enter/exit raids.

**Scope**: Add a hub console NPC near spawn that routes interaction to existing `RaidSystem.TryInteractPortal` and `RaidSystem.TryInteractExit`.

**Acceptance Criteria**:

- Entering via the hub console sets raid state such that raid-only behavior can occur (e.g., ARC Drone spawn chance > 0).
- Exiting via the hub console ends the raid and returns the player to hub context.
- Refusal cases are handled with clear, non-spammy feedback.

**Architecture Notes**:

- Adapter-only: implement UX in `Adapters/NPCs/*` and delegate to existing APIs.
- No new raid rules: do not implement raid logic inside the NPC.
- Multiplayer: interactions should act on the invoking player only.

**Constitution Constraints**: Loop integrity, decoupled logic, world safety (no permanent tiles), performance (no polling), testability (manual checklist + existing unit tests).

**Links**: [spec.md](spec.md) User Story 1; [plan.md](plan.md) “Slice 1: Player-facing hub console (NPC)”; [contracts/interaction-contracts.md](contracts/interaction-contracts.md) Contract A

### Implementation for User Story 1

- [x] T005 [P] [US1] Create hub console NPC skeleton in Adapters/NPCs/RaidHubConsoleNPC.cs
- [x] T006 [US1] Implement hub console spawn/anchor near spawn in Adapters/Systems/RaidSystem.cs
- [x] T007 [US1] Implement NPC interaction routing: enter when not in raid; exit when in raid in Adapters/NPCs/RaidHubConsoleNPC.cs
- [x] T008 [US1] Add refusal messaging and anti-spam guard (cooldown or only-on-change) in Adapters/NPCs/RaidHubConsoleNPC.cs
- [x] T009 [US1] Manual verification pass: complete “Player-facing entry/exit” and “Refusal cases” in specs/002-raid-entry-and-debug/quickstart.md

**Checkpoint**: US1 is independently functional and testable

---

## Phase 4: User Story 2 - Dev Toggle for Debugging (Priority: P2)

**Goal**: Provide a dev-only toggle to force raid entry/exit quickly for testing.

**Scope**: Add a `ModCommand` that calls the same entry/exit APIs, gated by a config flag defaulting to off.

**Acceptance Criteria**:

- When dev tools are enabled, `/arcraid toggle` enters if not in raid, else exits.
- When dev tools are disabled, the command refuses safely and prints usage.

**Architecture Notes**:

- Adapter-only command logic in `Adapters/Commands/*`.
- Delegate to `RaidSystem.TryInteractPortal/TryInteractExit`.

**Constitution Constraints**: Separation (no raid rules in hooks), world safety (no world edits), performance (command only on demand), testability (manual + small unit tests where feasible).

**Links**: [spec.md](spec.md) User Story 2; [plan.md](plan.md) “Slice 2: Dev-only chat command (gated)”; [contracts/interaction-contracts.md](contracts/interaction-contracts.md) Contract B

### Implementation for User Story 2

- [x] T010 [P] [US2] Create command folder and command skeleton in Adapters/Commands/ArcRaidCommand.cs
- [x] T011 [US2] Implement config gate check and usage messages in Adapters/Commands/ArcRaidCommand.cs
- [x] T012 [US2] Implement `enter|exit|toggle` actions delegating to RaidSystem in Adapters/Commands/ArcRaidCommand.cs
- [x] T013 [US2] Manual verification pass: complete “Dev-only toggle” section in specs/002-raid-entry-and-debug/quickstart.md

**Checkpoint**: US2 is independently functional and testable

---

## Phase 5: User Story 3 - World-Safe Entry Device (Priority: P3)

**Goal**: Ensure the entry device does not corrupt worlds and safe-disable behavior is preserved.

**Scope**: Validate and harden spawn/anchor behavior and persistence so worlds remain loadable without the mod.

**Acceptance Criteria**:

- Worlds that used the feature remain loadable with the mod disabled.
- No permanent tiles are required for the hub console UX.

**Architecture Notes**:

- Prefer ephemeral entities (NPC) over worldgen.
- Keep any stored metadata minimal and optional.

**Constitution Constraints**: World safety and fail-safe operation is the primary driver for this story.

**Links**: [spec.md](spec.md) User Story 3; [plan.md](plan.md) Constitution Check “World safety”

### Implementation for User Story 3

- [x] T014 [US3] Audit and minimize world data writes for this feature in Adapters/Systems/RaidSystem.cs
- [x] T015 [US3] Ensure safe load behavior when portal metadata is missing/corrupt (no exceptions) in Adapters/Systems/RaidSystem.cs
- [x] T016 [US3] Manual verification pass: complete “World safety” section in specs/002-raid-entry-and-debug/quickstart.md

**Checkpoint**: US3 is independently functional and testable

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Small improvements across stories without changing scope.

- [ ] T017 [P] Tighten player messaging consistency (enter/exit/refusal) in Adapters/Systems/RaidUiNotifications.cs
- [ ] T018 Run full manual quickstart checklist in specs/002-raid-entry-and-debug/quickstart.md and record any deltas

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion
- **User Stories (Phase 3–5)**: Depend on Foundational completion
- **Polish (Phase 6)**: Depends on desired user stories being complete

### User Story Dependencies

- **US1 (P1)**: Depends on Phase 2 only
- **US2 (P2)**: Depends on Phase 2 only
- **US3 (P3)**: Depends on US1 (because US3 hardens/validates the chosen device implementation)

### Parallel Opportunities

- T003 and T004 can be done in parallel.
- Within US1, T005 can be done in parallel with T006 if you stub the spawn/anchor API.
- US2 can be implemented in parallel with US1 after Phase 2.

---

## Parallel Example: User Story 1

```bash
Task: "Create hub console NPC skeleton in Adapters/NPCs/RaidHubConsoleNPC.cs"
Task: "Implement hub console spawn/anchor near spawn in Adapters/Systems/RaidSystem.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 + Phase 2
2. Complete Phase 3 (US1)
3. STOP and validate via specs/002-raid-entry-and-debug/quickstart.md

### Incremental Delivery

1. US1 (player-facing entry/exit)
2. US2 (dev toggle)
3. US3 (world-safety hardening/validation)
4. Polish
