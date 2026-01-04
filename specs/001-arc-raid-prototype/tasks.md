# Tasks: Prototype ARC Raid Zone v0.1

**Input**: [specs/001-arc-raid-prototype/spec.md](specs/001-arc-raid-prototype/spec.md), [specs/001-arc-raid-prototype/plan.md](specs/001-arc-raid-prototype/plan.md)
**Constitution Guardrails**: Loop integrity (hub -> raid -> extract), decoupled logic vs hooks, world safety and safe disable, performance discipline, spec-driven testability.

## Phase 1: Setup (Shared Infrastructure)

- [X] T001 Establish feature directories per plan in TerrariaArcRaiders/Core/, TerrariaArcRaiders/Adapters/, TerrariaArcRaiders/Tests/Unit/ and ensure project file includes them
  - Goal/Scope: Prepare folders and csproj includes so subsequent slices compile.
  - Acceptance: Build succeeds; new folders exist; no behavior changes.
  - Architecture: No logic; structure only.
  - Constraints: Avoid unintended item edits; keep setup minimal.
  - Links: [plan structure](specs/001-arc-raid-prototype/plan.md#L23-L57)
  - Notes: Directories created and csproj now includes Core/Adapters/Tests globs; build succeeded.

## Phase 2: Foundational (Blocking Prerequisites)

- [X] T002 Define core models `RaidSessionStatus`, `RaidSession`, `RaidInventory`, `Stash`, `ArcScrap` in TerrariaArcRaiders/Core/Models/
  - Goal/Scope: Data shapes for raid loop; no logic beyond basic helpers.
  - Acceptance: Types compile; covers fields noted in plan; helpers reset/clear safely.
  - Architecture: Pure C#; no tML types; serializable-friendly.
  - Constraints: Decoupled logic; world-safety ready; perf O(1) ops.
  - Links: [plan foundations](specs/001-arc-raid-prototype/plan.md#L59-L70), [spec entities](specs/001-arc-raid-prototype/spec.md#L83-L113)
  - Notes: Added ArcScrap descriptor, inventory/stash helpers, session status/timestamps; all pure C#; build passes.

- [X] T003 Implement core services `RaidSessionService`, `StashService`, `DropRules` in TerrariaArcRaiders/Core/Services/
  - Goal/Scope: State transitions (start, loot, extract, fail/death), stash deposit, drop-to-raid rules.
  - Acceptance: Unit-callable methods enforcing extract clears raid inventory, death clears without stash; handles zero/duplicate sessions gracefully.
  - Architecture: Pure C#; deterministic; no hooks; inject persistence later.
  - Constraints: Loop integrity, decoupled, no allocations per tick.
  - Links: [plan foundations](specs/001-arc-raid-prototype/plan.md#L59-L74), [spec FR-002..FR-004](specs/001-arc-raid-prototype/spec.md#L53-L76)
  - Notes: Added services with safe guards for inactive sessions, death clears stash untouched, extract deposits all scrap then clears; simple drop rule awards 1 scrap; build passes.

- [X] T004 Add unit tests for services/models in TerrariaArcRaiders/Tests/Unit/
  - Goal/Scope: Cover enter->loot->extract path, death loss path, stash transfer bounds, drop helper mapping.
  - Acceptance: Tests fail if rules regress; verify 100% transfer on extract, 0% on death; no duplication.
  - Architecture: Headless; pure C#; no tML.
  - Constraints: Spec-driven testability; perf not critical in tests.
  - Links: [plan foundations tests](specs/001-arc-raid-prototype/plan.md#L72-L74), [spec FR-010](specs/001-arc-raid-prototype/spec.md#L73-L76)
  - Notes: Added MSTest-based unit tests for extract->stash, death loss, inventory reset, and drop rules; build passes.

## Phase 3: User Story 1 - Enter, Raid, Extract With Scrap (Priority: P1) 🎯 MVP

**Goal**: Player enters raid zone, fights drones, extracts, and stash receives ARC Scrap.

- [X] T005 [US1] Implement `IRaidPersistence` interface and default TagCompound serializer in TerrariaArcRaiders/Core/Services/
  - Acceptance: Can serialize stash and optional session snapshot; handles missing/corrupt data with safe defaults.
  - Architecture: DTO-layer only; no hooks.
  - Constraints: World safety; safe-disable readiness.
  - Links: [plan persistence](specs/001-arc-raid-prototype/plan.md#L76-L84), [spec FR-007](specs/001-arc-raid-prototype/spec.md#L68-L70)
  - Notes: Added IRaidPersistence, TagCompoundDto, and RaidPersistence with stash/session snapshot save/load, safe defaults on missing/corrupt data; included unit tests; build clean.

- [X] T006 [US1] Add `Adapters/Systems/RaidSystem` for load/save wiring using `IRaidPersistence`
  - Acceptance: On world load/save, stash persists per player without corrupting vanilla; ignores missing data.
  - Architecture: Thin ModSystem delegating to persistence; no game rules.
  - Constraints: Decoupled hooks; world safety; performance (no heavy alloc on save/load).
  - Links: [plan persistence](specs/001-arc-raid-prototype/plan.md#L76-L84), [spec FR-009](specs/001-arc-raid-prototype/spec.md#L71-L73)
  - Notes: Added RaidSystem with per-player stash registry and TagCompound bridge; delegates to IRaidPersistence; safe on missing/corrupt data; build clean.

- [X] T007 [US1] Create `Adapters/Players/RaidPlayer` handling entry, extract, death delegation
  - Acceptance: Entry starts session; extract triggers service extract + stash deposit; death triggers fail; raid inventory cleared accordingly.
  - Architecture: ModPlayer thin wrapper calling services; stores transient raid inventory state; no rules inline.
  - Constraints: Loop integrity; decoupled; world safety; perf light.
  - Links: [plan entry/exit](specs/001-arc-raid-prototype/plan.md#L86-L96), [spec US1](specs/001-arc-raid-prototype/spec.md#L15-L36), [spec FR-001..FR-005](specs/001-arc-raid-prototype/spec.md#L45-L65)
  - Notes: Added ModPlayer with session lifecycle (enter, scrap award, extract, death fail) delegating to services and shared stash registry; resets on world enter.

- [X] T008 [US1] Implement entry portal/structure placement near spawn and interaction to enter raid
  - Acceptance: Single portal exists per world; interacting calls RaidPlayer entry; no duplication on reload.
  - Architecture: Minimal tile/object placement via ModSystem; uses guards to avoid world mutation issues.
  - Constraints: World safety; avoid irreversible changes; performance neutral.
  - Links: [plan entry/exit](specs/001-arc-raid-prototype/plan.md#L86-L94), [spec FR-001](specs/001-arc-raid-prototype/spec.md#L45-L48)
  - Notes: Added portal state in RaidSystem with persisted spawn-near location, idempotent init, and TryInteractPortal delegating to RaidPlayer entry without world edits.

- [X] T009 [US1] Implement extraction exit inside raid zone that triggers extract flow
  - Acceptance: Works only when session active; returns player to hub; stash gains raid scrap; raid inventory clears.
  - Architecture: Thin adapter calling services; positioning logic minimal.
  - Constraints: Loop integrity; world safety; perf light.
  - Links: [plan entry/exit](specs/001-arc-raid-prototype/plan.md#L86-L96), [spec US1 acceptance 2](specs/001-arc-raid-prototype/spec.md#L23-L36)
  - Notes: Added exit interaction helper in RaidSystem that gates on active session, delegates extraction to RaidPlayer, and teleports player to spawn hub; no world edits.

- [X] T010 [US1] Add integration smoke test or adapter shim for entry -> loot -> extract flow
  - Acceptance: Simulated flow shows stash increases; raid inventory clears; no duplication.
  - Architecture: Uses services with stubbed adapters; minimal hook reliance.
  - Constraints: Testability; performance not critical.
  - Links: [plan tests](specs/001-arc-raid-prototype/plan.md#L125-L129), [spec SC-005](specs/001-arc-raid-prototype/spec.md#L126-L128)
  - Notes: Added MSTest smoke covering enter->loot->extract via services; verifies stash gains scrap, raid inventory clears, and re-extract/extra loot do not duplicate.

## Phase 4: User Story 2 - Death Penalty In Raid (Priority: P2)

**Goal**: Enforce loss of raid inventory on death inside raid, preserving risk without harming world.

- [X] T011 [US2] Wire death handling in RaidPlayer to call fail path and clear raid inventory
  - Acceptance: Death in raid zeros raid inventory; stash unchanged; respawn in hub.
  - Architecture: Thin adapter call; no logic inside hook.
  - Constraints: Loop integrity; world safety; decoupled logic.
  - Links: [plan entry/exit death](specs/001-arc-raid-prototype/plan.md#L86-L96), [spec US2](specs/001-arc-raid-prototype/spec.md#L38-L58), [spec FR-004](specs/001-arc-raid-prototype/spec.md#L60-L62)
  - Notes: RaidPlayer Kill now calls FailRun to clear raid inventory and leaves stash untouched; respawn teleports to spawn hub when a raid death occurred.

- [X] T012 [P] [US2] Extend unit tests for death loss scenarios and no-loot death
  - Acceptance: Tests fail if scrap not cleared on death or stash mutates incorrectly.
  - Architecture: Headless tests on services.
  - Constraints: Testability; performance not critical.
  - Links: [plan tests](specs/001-arc-raid-prototype/plan.md#L125-L129), [spec US2 acceptance](specs/001-arc-raid-prototype/spec.md#L46-L58)
  - Notes: Added service-level death tests verifying scrap cleared, stash unchanged, and death handling idempotent.

## Phase 5: User Story 3 - Stash Persistence and Safe Disable (Priority: P3)

**Goal**: Stash survives reloads and remains harmless when mod disabled.

- [X] T013 [US3] Ensure stash save/load via ModPlayer or ModSystem with safe-ignore on missing data
  - Acceptance: Save->quit->reload preserves stash; missing data loads safely with defaults; no crashes if mod disabled.
  - Architecture: Uses IRaidPersistence with TagCompound; defensive parsing.
  - Constraints: World safety; decoupled persistence; performance on load/save.
  - Links: [plan persistence safety](specs/001-arc-raid-prototype/plan.md#L98-L109), [spec US3](specs/001-arc-raid-prototype/spec.md#L60-L82), [spec FR-007](specs/001-arc-raid-prototype/spec.md#L68-L70)
  - Notes: Added per-player stash SaveData/LoadData with defensive parsing and hardened ModSystem load to ignore corrupt entries, preserving world load safety.

- [X] T014 [P] [US3] Add persistence round-trip tests and disable-mode simulation
  - Acceptance: Round-trip retains stash; simulate absence/corrupt data returns defaults without exception.
  - Architecture: Serializer tests; optionally hook shim.
  - Constraints: Testability; world safety.
  - Links: [plan tests](specs/001-arc-raid-prototype/plan.md#L111-L117), [spec SC-003](specs/001-arc-raid-prototype/spec.md#L118-L122)
  - Notes: Added serializer tests covering round-trip, corrupt status, negative clamps, and missing stash key defaults to ensure safe loads.

## Phase 6: ARC Scrap Item & ARC Drone NPC (Enablers for US1 loot loop)

- [X] T015 Implement ArcScrap ModItem adapter in TerrariaArcRaiders/Adapters/Items/
  - Acceptance: Item exists, stacks per descriptor, uses core data; no game rules inside adapter.
  - Architecture: Thin wrapper delegating to descriptor; created via drop rules only.
  - Constraints: Decoupled; performance neutral.
  - Links: [plan NPC/item](specs/001-arc-raid-prototype/plan.md#L98-L109), [spec FR-006](specs/001-arc-raid-prototype/spec.md#L63-L65)
  - Notes: Added ModItem using ArcScrap descriptor for stack/value; no gameplay rules embedded.

- [X] T016 Implement ArcDrone ModNPC adapter with lightweight AI and drop delegation
  - Acceptance: Spawns only in raid zone; AI avoids per-tick allocations; OnKill awards ARC Scrap via DropRules into active raid session.
  - Architecture: Thin adapter; AI helper in Core if needed; guards for active session.
  - Constraints: Performance discipline; loop integrity; decoupled rules.
  - Links: [plan NPC/item](specs/001-arc-raid-prototype/plan.md#L98-L109), [spec FR-005](specs/001-arc-raid-prototype/spec.md#L61-L63)
  - Notes: Added ArcDroneNPC with raid-only spawn gate and OnKill drop delegation to DropRules when session active; reuses vanilla AI style for lightweight behavior.

- [X] T017 [P] Add unit test for DropRules NPC kill -> raid inventory increment
  - Acceptance: Killing drone increases raid scrap once; no session => no award.
  - Architecture: Service-level test; no tML needed.
  - Constraints: Testability; performance not critical.
  - Links: [plan tests](specs/001-arc-raid-prototype/plan.md#L125-L129), [spec FR-005](specs/001-arc-raid-prototype/spec.md#L61-L63)
  - Notes: Added DropRules tests covering active session increment, finished session ignore, and null session ignore.

## Phase 7: Minimal UX Hooks & Messaging

- [X] T018 Add simple notifications (entry, extract success, death loss, stash updated) via chat/onscreen text
  - Acceptance: Messages appear at each event; no UI beyond text; does not block gameplay.
  - Architecture: Hooks call shared notifier helper; no logic changes.
  - Constraints: Performance light; no world edits.
  - Links: [plan UX](specs/001-arc-raid-prototype/plan.md#L111-L117), [spec edge cases](specs/001-arc-raid-prototype/spec.md#L84-L95)
  - Notes: Added lightweight chat notifications in RaidPlayer for entry, extraction/stash update, and raid failure/death.

- [ ] T019 Guard extraction when player inventory full and still update stash (stash separate)
  - Acceptance: Extraction succeeds; stash increments; clear message if player inventory cannot accept physical items (if any drop attempt happens).
  - Architecture: Adapter-level guard; uses stash transfer regardless of inventory space.
  - Constraints: World safety; loop integrity.
  - Links: [spec edge cases](specs/001-arc-raid-prototype/spec.md#L84-L95)

## Phase 8: Test & QA Pass

- [ ] T020 Run unit test suite and document results in specs/001-arc-raid-prototype/quickstart.md (or note pending creation)
  - Acceptance: Tests pass; failures documented with follow-ups.
  - Architecture: N/A.
  - Constraints: Testability.
  - Links: [plan tests](specs/001-arc-raid-prototype/plan.md#L125-L129)

- [ ] T021 Manual validation checklist (enter/loot/extract, death loss, stash survives reload/disable, fps sanity) recorded in quickstart.md
  - Acceptance: Steps executed and noted; issues captured.
  - Architecture: N/A.
  - Constraints: World safety; performance awareness.
  - Links: [plan manual](specs/001-arc-raid-prototype/plan.md#L125-L129), [spec success criteria](specs/001-arc-raid-prototype/spec.md#L115-L128)

## Dependencies & Execution Order

- Foundational (T002-T004) must precede adapters and items.
- US1 entry/extract (T005-T010) depends on foundations; portal/exit relies on services and persistence wiring.
- Death handling (T011-T012) depends on services and RaidPlayer.
- Persistence safety (T013-T014) depends on persistence implementation T005 and system wiring T006.
- NPC/Item tasks (T015-T017) depend on services; can proceed after foundations in parallel with adapter wiring.
- UX (T018-T019) depends on core flows working.
- QA tasks (T020-T021) last.

## Parallel Opportunities

- Models/services (T002-T004) mostly linear; tests can run in parallel once services stubbed.
- Persistence serializer T005 can proceed alongside T003 once data shapes exist.
- ArcScrap/ArcDrone (T015-T017) can develop in parallel with entry/exit wiring (T007-T009) after services exist.
- UX messaging (T018) can proceed after base hooks exist; inventory guard (T019) after extraction flow ready.

## Implementation Strategy

- MVP first: complete through US1 tasks (T002-T010) to demo entry->loot->extract with stash.
- Incrementally add US2 death penalty, then US3 persistence safety, then NPC/Item polish and UX.
- Keep hooks thin; keep tests green after each slice; small PRs aligned with task IDs.
