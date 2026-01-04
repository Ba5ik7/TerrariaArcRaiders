# Feature Specification: Raid Entry & Debug Controls

**Feature Branch**: `002-raid-entry-and-debug`
**Created**: 2026-01-04
**Status**: Draft
**Input**: User description: "Raid Entry & Debug Controls: Add a player-facing in-world way to start/end a raid session (hub -> raid -> extract loop) and a dev-only control to force enter/exit for testing, wiring to existing RaidPlayer.TryEnterRaid() and RaidSystem.TryInteractPortal() while keeping glue in adapters, being world-safe, negligible overhead, and testable."

**Problem statement**: The current codebase contains APIs for entering/exiting raids (for example on `RaidPlayer` and `RaidSystem`), but no in-game interaction triggers them, so players cannot start a raid session via normal play and raid-only behavior (such as ARC Drone spawning) never occurs.

**Scope (this feature)**: Provide the missing in-game triggers for entering/exiting a raid session and a dev-only toggle for testing. This feature does not define new raid rules, loot rules, or NPC behavior; it only connects player interactions to existing raid/session logic.

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.

  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - Start and End a Raid via the Hub (Priority: P1)

As a normal player in the hub, I can interact with an obvious, world-safe in-world device (e.g., a portal or console near spawn) to start a raid session, and later use an obvious extraction/exit interaction to end the raid session and return to the hub.

**Why this priority**: This unblocks the core hub -> raid -> extract loop by providing the first real player-facing entry/exit path, and it enables all downstream raid behaviors (raid-only NPC spawning, raid inventory rules, extraction outcomes).

**Independent Test**: Can be fully tested by starting a new world, interacting with the hub device to enter a raid (verifying “in raid” state), then using the extraction/exit interaction to end the raid (verifying “not in raid” state) without needing any other raid features.

**Acceptance Scenarios**:

1. **Given** the player is in the hub and not currently in a raid, **When** they interact with the raid entry device, **Then** a raid session is started and the player is placed into the raid context.
2. **Given** the player is currently in an active raid, **When** they interact with the extraction/exit interaction, **Then** the raid session ends successfully and the player returns to the hub context.
3. **Given** the player tries to use the entry device while already in a raid, **When** they interact, **Then** no new session is created and the player receives clear feedback.
4. **Given** multiple players are on the same server, **When** one player enters or exits a raid, **Then** only that player’s raid session state changes and other players are not affected.

---

### User Story 2 - Dev Toggle for Debugging (Priority: P2)

As a developer/tester, I can trigger a dev-only control (chat command or dev-only item) to force entering or exiting a raid session so I can quickly validate raid behaviors without relying on the full entry UX.

**Why this priority**: Speeds iteration and makes debugging repeatable, which supports the constitution’s “spec-driven delivery and testability” principle while keeping player-facing UX clean.

**Independent Test**: Can be tested by enabling the dev-only control, running the toggle to enter a raid, confirming raid state and raid-only behaviors are enabled, then toggling back to exit and confirming state is cleared.

**Acceptance Scenarios**:

1. **Given** dev-only controls are enabled, **When** the tester triggers the dev enter control, **Then** the player enters a raid session as if they used the normal entry path.
2. **Given** dev-only controls are enabled and the player is in a raid, **When** the tester triggers the dev exit control, **Then** the player exits the raid session as if they used the normal extraction/exit path.
3. **Given** dev-only controls are disabled for normal gameplay, **When** a player attempts to use them, **Then** the controls are not available (or safely refuse with clear feedback) and do not alter raid state.

---

### User Story 3 - World-Safe Entry Device (Priority: P3)

As a player, the raid entry device does not permanently damage my world or block loading if the mod is disabled or removed.

**Why this priority**: Preserves vanilla world integrity and ensures the feature adheres to the constitution’s “world safety and fail-safe operation” principle.

**Independent Test**: Can be tested by placing/using the entry device, then loading the world with the mod disabled and confirming the world remains loadable and no vanilla content is corrupted.

**Acceptance Scenarios**:

1. **Given** a world that has had the raid entry device available, **When** the mod is disabled and the world is loaded, **Then** the world loads successfully and any mod-specific objects do not crash the game.
2. **Given** the mod is re-enabled later, **When** the world is loaded, **Then** the raid entry device becomes available again without requiring manual repair.

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right edge cases.
-->

- Interaction spam: repeated interact attempts do not create duplicate sessions or corrupt raid state.
- Invalid context: attempts to enter a raid during disallowed states (dead/respawning, mid-transition) are safely refused.
- Multiplayer authority: state transitions occur reliably in multiplayer and do not desync clients.
- Player disconnect/reconnect: if a player disconnects while in a raid, the session ends safely (no duplication and no stuck “in raid” state on rejoin).
- Mod disable safety: worlds remain loadable and no irreversible worldgen changes are required for the entry device.

## Requirements *(mandatory)*

<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

Anchor requirements to the constitution: preserve the hub -> raid -> extract loop, keep core logic
decoupled from Terraria hooks, protect vanilla worlds and allow disable/cleanup, state performance
and cross-platform expectations, and specify how behavior is tested.

### Functional Requirements

- **FR-001 (Player-Facing Entry)**: The mod MUST provide a clear, player-facing in-world interaction in the hub that attempts to start a raid session. *(Acceptance: A new character can discover and use it without external tools.)*
- **FR-002 (Player-Facing Exit)**: The mod MUST provide a clear, player-facing way to end a raid session (successful extraction/exit) and return the player to the hub context. *(Acceptance: Exiting reliably returns the player to hub context and clears “in raid” state.)*
- **FR-003 (Wire to Existing APIs)**: Entry/exit interactions MUST delegate to the existing raid/session APIs (for example `RaidPlayer` and `RaidSystem`) rather than duplicating rules inside Terraria hooks. *(Acceptance: Hooks contain no independent raid rules beyond argument/permission checks and forwarding.)*
- **FR-004 (Hook Points / Glue Location)**: Entry/exit triggers MUST be implemented via thin adapters using appropriate hook points (for example `ModSystem` for safe world-level setup, `ModPlayer` for interaction routing, and `ModCommand` and/or a dev-only `ModItem` for debug toggles). Core raid rules MUST remain in services/models without Terraria hook dependencies. *(Acceptance: Core logic remains unit-testable without a running game.)*
- **FR-005 (Dev-Only Control)**: The mod MUST provide a lightweight, dev-only mechanism (chat command and/or dev-only item) that can force entering/exiting a raid session for testing. *(Acceptance: A tester can toggle enter/exit quickly in a local world.)*
- **FR-006 (Dev Control Gating)**: Dev-only controls MUST be disabled by default for normal gameplay and MUST be gated in a way that prevents accidental use in standard play. *(Acceptance: In default configuration, a normal player cannot access the dev control.)*
- **FR-007 (World Safety)**: The entry/exit UX MUST avoid irreversible world changes. Any placed tiles/objects MUST be minimal and MUST not corrupt or prevent world loading if the mod is disabled. *(Acceptance: Disabling the mod still allows affected worlds to load.)*
- **FR-008 (Performance)**: Entry/exit UX MUST have negligible runtime overhead and MUST NOT require per-tick heavy work to function. *(Acceptance: No continuous polling or per-tick allocations are required just to keep the entry UX available.)*
- **FR-009 (Multiplayer Safety)**: Raid session state changes MUST be authoritative and consistent in multiplayer; one player’s entry/exit MUST NOT affect other players’ session state. *(Acceptance: Two players can be in different raid states without interference.)*
- **FR-010 (User Feedback)**: When entry/exit actions are refused (invalid state, already in raid, dev controls disabled), the player MUST receive clear, non-spammy feedback. *(Acceptance: The player sees a clear message and repeated attempts do not flood chat.)*
- **FR-011 (Tests)**: The feature MUST include unit tests where feasible (core state transitions and safety behavior) plus a simple manual checklist for in-game verification. *(Acceptance: Tests run green; manual checklist steps are enumerated and reproducible.)*

**Dependencies**: This feature depends on the existing raid/session state and entry/exit APIs already present in core services/models and adapter layers.

**Assumptions**: The hub context is the default player spawn area for the current prototype; the raid context already exists as a distinct “in-raid” state even if the exact biome/teleport implementation evolves.

### Key Entities *(include if feature involves data)*

- **RaidSession**: A player’s current raid run state (not in raid, active raid, ended raid), including the reason it ended (extract vs forced exit).
- **Raid Entry Device**: The player-facing hub interaction point that initiates a raid session.
- **Dev Toggle Control**: A developer-facing trigger that can force raid session entry/exit for testing and debugging.

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001 (Playable Entry Path)**: A new player can start and end a raid session using only the in-game hub UX within 60 seconds, without using debug controls.
- **SC-002 (Debug Velocity)**: A tester can enter and exit a raid session using the dev-only control in under 10 seconds.
- **SC-003 (World Safety)**: A world that has used the entry UX loads successfully with the mod disabled, with no crashes and no vanilla content corrupted.
- **SC-004 (Negligible Overhead)**: When idle (no interactions), the feature adds no noticeable gameplay overhead; entering/exiting incurs only the one-time work required for the interaction itself.
- **SC-005 (Testability)**: Unit tests covering key state transitions and refusal cases execute and pass consistently, and the manual checklist can be completed end-to-end without ambiguous steps.
