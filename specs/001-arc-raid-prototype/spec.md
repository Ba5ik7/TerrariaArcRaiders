# Feature Specification: Prototype ARC Raid Zone v0.1

**Feature Branch**: `001-arc-raid-prototype`
**Status**: Draft
**Input**: Create a spec for the first vertical slice of TerrariaArcRaiders called Prototype ARC Raid Zone v0.1. Context: Use the existing TerrariaArcRaiders constitution in this repo as the governing document. This feature must clearly map to the hub -> raid -> extract loop, respect world safety, and keep game logic decoupled from tModLoader glue. Goals: - Add a single, clearly defined "ARC Raid Zone" experience reachable from a simple in-world entry point (e.g., a special portal or structure). - Inside the zone, spawn ARC Drone enemies (the existing test NPC can be reused or extended) and drop a new resource item "ARC Scrap". - Allow the player to extract from the zone through a defined exit (elevator, portal, etc.), which:   - Takes them back to a safe "hub" context.   - Moves ARC Scrap from "raid inventory" to a persistent stash. - On player death in the raid zone, un-stashed ARC Scrap is lost (according to a simple, clearly defined rule). Constraints: - Implement core raid/stash logic in plain C# services that can be unit tested without Terraria running. - Terraria hooks (ModSystem, ModPlayer, ModItem, ModNPC, etc.) act as thin adapters that delegate to those services. - Persistence must not corrupt or lock vanilla worlds; if the mod is disabled, worlds should still load and ignore the mod’s data without crashing. - Performance impact must be negligible: no per-tick heavy allocations in AI or hooks. Deliverables: - A structured spec document in the location Spec Kit expects (using its spec template), including:   - Problem statement and user stories.   - Scope for v0.1 (what is in vs out).   - Architecture and data model: how raid state, stash, and ARC Scrap are represented; how they survive between sessions.   - Hook points: which tModLoader types and events will be used and how they delegate to core logic.   - World-safety considerations and failure/disable behavior.   - Test strategy: unit tests for logic and basic integration tests. Make sure the spec directly references the constitution’s principles (loop integrity, decoupled logic, world safety, performance, and spec-driven delivery) so reviewers can verify alignment.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Enter, Raid, Extract With Scrap (Priority: P1)

The player uses a clear in-world entry point to enter the ARC Raid Zone, fights ARC Drones, loots ARC Scrap, and extracts via a defined exit that returns them to the hub and moves loot to a persistent stash.

**Why this priority**: Delivers the core hub -> raid -> extract loop and demonstrates risk/reward plus stash handoff, validating the constitution’s loop integrity principle.

**Independent Test**: Spawn the entry point, enter the zone, defeat or kite drones to obtain ARC Scrap, extract via the exit, and verify stash contains the scrap while raid inventory is cleared.

**Acceptance Scenarios**:

1. **Given** the player interacts with the raid entry point, **When** they confirm entry, **Then** they load into the ARC Raid Zone with raid inventory empty and ARC Drones active.
2. **Given** the player holds ARC Scrap in raid inventory, **When** they use the designated extraction exit, **Then** they return to the hub, raid inventory is cleared, and the same amount of ARC Scrap appears in stash storage.
3. **Given** the player collects no ARC Scrap, **When** they extract, **Then** stash remains unchanged and no errors occur.

---

### User Story 2 - Death Penalty In Raid (Priority: P2)

When the player dies inside the ARC Raid Zone, un-stashed ARC Scrap is lost according to a clear rule, preserving the risk element of the loop without harming the vanilla world.

**Why this priority**: Enforces loss-on-failure, a core tension for raids, while exercising world-safety and cleanup expectations.

**Independent Test**: Enter raid, acquire ARC Scrap, die to a drone; on respawn (hub), verify stash unchanged and raid inventory cleared of ARC Scrap.

**Acceptance Scenarios**:

1. **Given** the player holds ARC Scrap in raid inventory, **When** they die in the raid zone, **Then** ARC Scrap is removed from raid inventory and not added to stash.
2. **Given** the player dies with no ARC Scrap, **When** they respawn, **Then** no stash changes occur and no errors are thrown.

---

### User Story 3 - Stash Persistence and Safe Disable (Priority: P3)

ARC Scrap stash persists across sessions and remains harmless if the mod is disabled; worlds and saves load without crashes and ignore mod data gracefully.

**Why this priority**: Validates world safety and persistence boundaries; ensures the feature does not corrupt vanilla worlds and aligns with the constitution’s world-safety principle.

**Independent Test**: Acquire ARC Scrap, extract, save and quit, reload the world to confirm stash remains; disable the mod and reload the world to confirm no crashes and that inventories are clean or safely ignored.

**Acceptance Scenarios**:

1. **Given** stash holds ARC Scrap, **When** the player saves, exits, and reloads the world with the mod enabled, **Then** stash still holds the same ARC Scrap amount.
2. **Given** stash held ARC Scrap, **When** the mod is disabled and the world loads, **Then** the world loads without error and player inventory/world integrity is preserved (stash data ignored safely).

---

### Edge Cases

- Player attempts extraction while inventory is full: stash transfer still succeeds without dropping or deleting items; excess is refused with a clear message.
- Player disconnects or quits inside the raid: on re-entry, session ends and raid inventory is cleared (no free duplication), ensuring world safety.
- Multiple players enter: each player’s raid inventory and stash are isolated; extraction of one player does not affect another.
- Entry/exit spam: repeated entry/exit does not duplicate stash items or strand the player.
- Performance: ARC Drone AI and hooks avoid per-tick heavy allocations; frame rate remains comparable to vanilla in the zone.

## Requirements *(mandatory)*

Anchor requirements to the constitution: preserve the hub -> raid -> extract loop, keep core logic decoupled from Terraria hooks, protect vanilla worlds and allow disable/cleanup, state performance and cross-platform expectations, and specify how behavior is tested.

### Functional Requirements

- **FR-001 (Loop Entry/Exit)**: Provide a clear in-world entry point in the hub that loads the ARC Raid Zone and an explicit extraction exit that returns the player to the hub, preserving the hub -> raid -> extract loop.
- **FR-002 (Raid Session Service)**: Implement a C# service to manage raid session state (entered, active, extracted, failed) independent of Terraria APIs, enabling headless unit tests.
- **FR-003 (Raid Inventory vs Stash)**: Track ARC Scrap separately as raid inventory during the run; on successful extraction move it to persistent stash, clearing raid inventory; stash persists across sessions.
- **FR-004 (Death Loss Rule)**: On death inside the raid zone, raid inventory ARC Scrap is lost and not added to stash; respawn returns player to hub context safely.
- **FR-005 (ARC Drone Spawn/Behavior)**: Spawn ARC Drones in the raid zone using thin ModNPC adapters delegating to core AI/behavior helpers; drones drop ARC Scrap into raid inventory rules without heavy per-tick allocations.
- **FR-006 (ARC Scrap Item)**: Define ARC Scrap item with thin ModItem adapter; core data model holds its value/stacking; item integrates with raid inventory and stash transfer rules.
- **FR-007 (Persistence & World Safety)**: Store raid and stash data in mod storage isolated from vanilla world data; if the mod is disabled, worlds still load and ignore mod data without crashes; include cleanup/ignore paths.
- **FR-008 (Performance & Cross-Platform)**: Avoid reflection or heavy allocation in hooks/AI; keep per-tick logic O(1) with bounded allocations; support Windows/macOS/Linux with capability checks for any platform-specific code paths.
- **FR-009 (Hook Delegation)**: Terraria hooks (ModSystem for world load/save, ModPlayer for entry/extract/death, ModItem for ARC Scrap, ModNPC for ARC Drone) delegate to core services; hooks contain minimal logic and no game rules.
- **FR-010 (Tests)**: Provide unit tests for raid session service, inventory/stash transfer, and death loss rule; provide basic integration tests covering entry, extraction, and stash persistence using hook adapters where applicable.

### Key Entities *(include if feature involves data)*

- **RaidSession**: State of a run (player id, status entered/active/extracted/failed, raid inventory contents, timestamps), independent of Terraria types.
- **RaidInventory**: Transient container of ARC Scrap during a raid; cleared on extraction or death; not persisted after session end.
- **Stash**: Persistent storage of ARC Scrap tied to player identity; survives reloads; must ignore safely if mod disabled.
- **ArcScrap**: Resource descriptor (id, name, stack size, value) with ModItem adapter for in-world item behavior.
- **ArcDrone**: Enemy descriptor (id, stats, drop table, behaviors) with ModNPC adapter delegating AI helpers that avoid heavy per-tick allocations.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Players can enter the ARC Raid Zone from the hub and extract back within 3 minutes end-to-end under normal conditions (loop integrity).
- **SC-002**: Successful extraction transfers 100% of ARC Scrap from raid inventory to stash with zero duplication or loss; death in-raid results in 100% loss of un-stashed ARC Scrap (risk enforcement).
- **SC-003**: Stash contents remain intact across save/quit/reload with the mod enabled; loading the world with the mod disabled completes without crashes or data corruption (world safety).
- **SC-004**: ARC Drone encounters and hook delegation do not introduce noticeable frame drops; maintain parity with vanilla 60 fps baseline on Windows/macOS/Linux test runs (performance discipline).
- **SC-005**: Core raid/stash logic unit tests and integration smoke tests execute and pass in CI, demonstrating decoupled logic and hook adapter correctness (spec-driven delivery).
