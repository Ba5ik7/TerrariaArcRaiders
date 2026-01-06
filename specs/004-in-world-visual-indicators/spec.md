# Feature Specification: In-World Visual Indicators

**Feature Branch**: `004-in-world-visual-indicators`
**Created**: 2026-01-05
**Status**: Draft
**Input**: User description: "Add in-world visual indicators for each step of the Arc worldgen pipeline so mod developers can debug Arc world generation by looking at the world itself instead of relying only on logs."

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

### User Story 1 - See Arc Worldgen Progress In-World (Priority: P1)

As a mod developer, I want an Arc world to show clear, in-world indicators that each Arc worldgen stage ran, so I can debug the pipeline by looking at the generated world rather than relying only on logs.

**Why this priority**: This is the core value of the feature: faster debugging and quicker iteration on worldgen changes.

**Independent Test**: Create a new Arc world with visual indicators enabled, enter the world, and verify that each expected stage has a visible indicator in the world that unambiguously maps back to the stage name/order.

**Acceptance Scenarios**:

1. **Given** an Arc world is created with indicators enabled, **When** world generation completes, **Then** the world contains a visible indicator for each Arc worldgen stage.
2. **Given** the world is generated, **When** the player enters the world, **Then** the indicators are discoverable without requiring log access.

---

### User Story 2 - Keep Visual Indicators Opt-In (Priority: P2)

As a mod developer (or server host), I want the in-world indicators to be opt-in, so normal play is unaffected unless the debugging feature is explicitly enabled.

**Why this priority**: Visual indicators are a debugging aid and should not alter the experience for players who do not want them.

**Independent Test**: Create two Arc worlds with identical settings except for the indicator toggle; verify the indicators appear only in the enabled case and do not appear in the disabled case.

**Acceptance Scenarios**:

1. **Given** an Arc world is created with indicators disabled, **When** world generation completes, **Then** no indicator content is added to the world.
2. **Given** a non-Arc world is created, **When** world generation completes, **Then** the feature does not add indicator content regardless of settings.

---

### User Story 3 - Quickly Locate Planned Arc Regions (Priority: P3)

As a mod developer, I want the indicators to be placed in predictable, easy-to-find locations tied to the planned Arc regions, so I can quickly inspect whether planning outputs (like hub/regions/reserved sites) look correct.

**Why this priority**: When planning bugs occur, being able to quickly find the planned areas reduces debugging time and supports faster iteration.

**Independent Test**: Generate an Arc world with indicators enabled and verify the indicators appear within or adjacent to the planned hub/region locations in a way that is consistent across repeated generations of the same seed.

**Acceptance Scenarios**:

1. **Given** an Arc world is created from a fixed seed with indicators enabled, **When** it is generated multiple times, **Then** the indicators appear in consistent locations relative to planned regions.

---

### Edge Cases

- World generation stops early or errors: indicators MUST not cause additional failures or corrupt the generated world.
- Planned regions are missing or invalid: indicators MUST not attempt unsafe placement and the world MUST remain loadable.
- Indicators are enabled on a world where Arc generation is not active: the feature MUST not place anything.
- Multiplayer: indicators MUST not create client/server divergence or desync in world content.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a single opt-in setting that enables or disables in-world visual indicators for Arc world generation.
- **FR-002**: When indicators are disabled, the system MUST NOT add any indicator content during world generation.
- **FR-003**: The system MUST NOT add indicator content to non-Arc worlds.
- **FR-004**: When indicators are enabled for an Arc world, the system MUST create one visible indicator per Arc worldgen stage in the pipeline.
- **FR-005**: Each stage’s indicator MUST be distinguishable from other stages’ indicators and must be mappable back to the stage name/order.
- **FR-006**: Indicators MUST be placed at predictable locations tied to Arc planning outputs (e.g., planned regions or reserved sites), without requiring full-world scanning.
- **FR-007**: Indicator placement MUST be deterministic for a given world seed and world size (to support reproducible debugging).
- **FR-008**: Indicators MUST NOT prevent world creation, loading, or saving; if indicator placement cannot be performed safely, the system MUST skip indicator placement and allow the world to proceed.
- **FR-009**: Indicators MUST NOT materially increase world generation time beyond reasonable developer expectations for debug tooling.
- **FR-010**: The system MUST provide a way for a developer to interpret the indicators (e.g., via a documented legend or in-world labeling) without requiring access to logs.

### Key Entities *(include if feature involves data)*

- **Worldgen Stage**: A discrete step in the Arc worldgen pipeline that can complete successfully or be skipped/failed.
- **Visual Indicator**: A persistent, in-world marker that represents a specific worldgen stage.
- **Indicator Legend**: The mapping from each stage to its indicator appearance/meaning.
- **Indicator Setting**: The user-controlled (developer-controlled) toggle that determines whether indicators are generated.

### Assumptions

- Indicators are intended primarily for mod developers and testers, not for standard player-facing gameplay.
- Arc planning outputs (regions/reserved sites) exist and can be used to choose bounded placement locations.
- A “visible indicator” can be any persistent, discoverable world content a player can find after world creation.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: With indicators enabled, a developer can confirm that all Arc worldgen stages ran (or identify the first missing stage) by inspecting the generated world within 2 minutes of entering the world.
- **SC-002**: With indicators disabled, 0 indicators are present in newly generated Arc worlds.
- **SC-003**: In 10 repeated world generations of the same Arc seed and size (with indicators enabled), indicator placement locations remain consistent across runs.
- **SC-004**: Worlds generated with indicators enabled remain loadable and playable (no indicator-related crashes during generation or on subsequent loads).
