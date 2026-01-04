# Feature Specification: Arc Raider Worldgen Preset

**Feature Branch**: `003-arc-worldgen-preset`
**Created**: 2026-01-04
**Status**: Draft
**Input**: User description: "Introduce a selectable Arc Raider world preset that generates a clearly non-vanilla world layout and provides an extensible world generation pipeline with named stages and placeholder biomes/structures, while keeping vanilla worlds unchanged."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create an Arc Raider world (Priority: P1)

As a player creating a new world, I can select an “Arc Raider” world preset alongside standard world creation options so that the new world is generated using Arc Raider rules and a custom layout, without changing how non-Arc worlds are created.

**Why this priority**: This is the feature’s entry point; without it, no one can access or validate the Arc Raider world foundation.

**Independent Test**: Create an Arc Raider world and a non-Arc world; verify only the Arc world is flagged as Arc Raider and that non-Arc worlds behave unchanged.

**Acceptance Scenarios**:

1. **Given** the world creation flow, **When** the player selects the Arc Raider preset and completes world creation, **Then** the world can be entered and is recognized as an Arc Raider world.
2. **Given** the world creation flow, **When** the player does not select the Arc Raider preset, **Then** world creation behaves as before and the world is not recognized as Arc Raider.

---

### User Story 2 - Explore a clearly distinct layout (Priority: P2)

As a player entering an Arc Raider world for the first time, I notice that the world’s layout and biome composition are clearly distinct from vanilla, even if some areas are early placeholders.

**Why this priority**: Validates that Arc worldgen is in control of terrain/biome composition and that the preset delivers visible player value immediately.

**Independent Test**: Enter an Arc Raider world, explore from spawn, and confirm the presence of the Safe Hub and at least one additional named Arc area.

**Acceptance Scenarios**:

1. **Given** an Arc Raider world, **When** the player starts at spawn and explores normally, **Then** the player can locate a Safe Hub area and at least one other named Arc area without relying on advanced gear.
2. **Given** an Arc Raider world, **When** the player travels across the surface and underground, **Then** the world does not primarily follow vanilla biome layout patterns and instead reflects Arc Raider region planning (see Requirements).

---

### User Story 3 - Extend Arc Raider worldgen without rewrites (Priority: P3)

As a mod developer, I can add or adjust an Arc Raider world generation stage (new biome, structure, ore/loot pass later) by targeting a named stage boundary, without rewriting the entire worldgen pipeline.

**Why this priority**: The feature is a foundation for future expansion; stable stage boundaries are the main mechanism for organized growth.

**Independent Test**: Add a minimal new stage that marks a small region or records that it ran; confirm it runs in the intended stage order for Arc worlds only.

**Acceptance Scenarios**:

1. **Given** an Arc Raider world, **When** a new worldgen stage is added to a named stage boundary, **Then** it runs deterministically within that boundary and does not affect non-Arc worlds.
2. **Given** an Arc Raider world, **When** the new stage is removed/disabled, **Then** Arc world creation still completes successfully and the remaining stage order is unchanged.

---

### Edge Cases

- Creating an Arc Raider world with each available world size option.
- Creating an Arc Raider world while standard “evil/corruption” options are present.
- World generation stage failure mid-process (ensuring safe failure behavior).
- Joining an already-generated Arc Raider world in multiplayer.
- Disabling the mod after Arc Raider worlds exist (fail safely; do not break non-Arc worlds).
- Creating multiple Arc Raider worlds back-to-back (no cross-world contamination).

## Requirements *(mandatory)*

Anchor requirements to the constitution: preserve the hub -> raid -> extract loop, keep core logic decoupled
from game integration points, protect vanilla worlds and allow disable/cleanup, state performance and
cross-platform expectations, and specify how behavior is tested.

### Assumptions

- The Arc Raider world preset is additive: it appears as an additional world option and does not replace or modify existing world types.
- Standard world options (e.g., size, seed) remain available; size affects overall world scale.
- Standard “evil/corruption” options remain visible for compatibility but do not determine Arc Raider biome placement.
- This spec defines foundation, organization, and placeholder regions/structures; it does not define final biome content, items, enemies, bosses, or raid progression.

### Functional Requirements

- **FR-001**: The system MUST provide an “Arc Raider” world preset that can be selected during world creation.
- **FR-002**: The system MUST ensure selecting the Arc Raider preset affects only the newly created world.
- **FR-003**: The system MUST ensure non-Arc worlds (existing and newly created) are generated and played exactly as before.

- **FR-004**: The system MUST persist world metadata sufficient to identify a world as an Arc Raider world on future loads.
- **FR-005**: The system MUST ensure Arc Raider metadata is absent from non-Arc worlds.

- **FR-006**: The system MUST generate an Arc Raider world layout that is observably different from vanilla world layout patterns (even if some content is placeholder).
- **FR-007**: The system MUST define Arc Raider world identity guidance that informs current and future generation stages, including:
  - a “frontier scavenging” feel and pacing
  - early safety anchor (hub) plus increasing risk outward
  - biomes/regions planned as readable zones rather than many tiny alternating stripes

- **FR-008**: The system MUST implement an Arc Raider world generation pipeline composed of named stages with a stable, documented stage order.
- **FR-009**: The system MUST provide stage boundaries that future specs can reference by name.
- **FR-010**: The system MUST allow future addition/removal of stages without requiring changes to earlier stages.

- **FR-011**: The system MUST define the following stage boundaries (names are stable even if content changes):
  - **Stage A: World Tagging & Setup**
  - **Stage B: Base Terrain Layout**
  - **Stage C: Region Planning (Biome Slotting)**
  - **Stage D: Biome Painting (Placeholder Allowed)**
  - **Stage E: Structure Reservation (Sites Only)**
  - **Stage F: Structure Placement (Placeholder Allowed)**
  - **Stage G: Raid-Related Anchors (Reserved Only)**
  - **Stage H: Final Polish & Validation**

- **FR-012**: The system MUST define an initial set of named biome/structure slots with intent and constraints, including:
  - **Safe Hub (Biome Slot)**: safe, readable anchor area; intended to host future raid terminals and core interactions.
  - **Arc Wasteland (Biome Slot)**: primary “open traversal” region; early-to-mid risk; placeholder-friendly.
  - **Raid Scar (Biome Slot)**: high-risk landmark region; intended as a future gate/telegraph for raid progression.
  - **Drone Factory Ruins (Structure/Biome Slot)**: structure-themed region; intended for later expansion.
  - **Service Tunnels (Biome Slot, stub)**: underground connective region; can start as a minimal placeholder.

- **FR-013**: The system MUST ensure at least the Safe Hub slot is placed in every Arc Raider world.
- **FR-014**: The system MUST ensure at least one additional non-hub slot is placed in every Arc Raider world.

- **FR-015**: The system MUST reserve (even if not populate) at least one raid-related structure site located within or adjacent to the Safe Hub, intended for future raid terminal placement.
- **FR-016**: The system MUST expose/record at least the Safe Hub region and any reserved raid-related sites for future raid-system integration.

- **FR-017**: The system MUST be resilient to placeholder content: missing optional slots/stages must not prevent world creation.
- **FR-018**: The system MUST fail safely if an Arc Raider world generation stage cannot complete: it must not corrupt existing worlds and must not break subsequent world creation.

- **FR-019**: The system MUST be deterministic for a given world seed and selected world options such that repeated generation yields equivalent region layout and stage ordering.
- **FR-020**: The system MUST keep world creation time within reasonable player expectations for each world size (see Success Criteria).

### Requirement Acceptance Criteria

- **AC-001 (World Selection)**: Create a world with the Arc Raider preset and one without it; confirm the Arc world is recognized as Arc Raider and the non-Arc world is not (covers FR-001 to FR-005).
- **AC-002 (Distinct Layout + Slots)**: In an Arc Raider world, confirm the Safe Hub is present and at least one additional named slot area is present; confirm traversal between them is feasible with early-game movement (covers FR-006, FR-012 to FR-014).
- **AC-003 (Named Stage Boundaries)**: Confirm the Arc Raider world generation reports (via logs or other observable indicators) that the named stage boundaries exist and run in the documented order for Arc worlds only (covers FR-008 to FR-011).
- **AC-004 (Raid Anchors Reserved)**: Confirm the Safe Hub region is recorded and at least one raid-related reserved site exists in or adjacent to it (covers FR-015 to FR-016).
- **AC-005 (Resilience + Safe Failure)**: Intentionally disable/remove an optional slot/stage and confirm Arc world creation still completes; confirm a stage failure does not corrupt existing worlds and does not prevent subsequent world creation (covers FR-017 to FR-018).
- **AC-006 (Determinism)**: Generate the same Arc Raider world seed twice and confirm the stage ordering and high-level slot layout are equivalent (covers FR-019).

### Key Entities *(include if feature involves data)*

- **Arc Raider World Preset**: A selectable world preset that determines whether a world uses Arc Raider generation rules.
- **Arc World Metadata**: Persisted world data that identifies the world as Arc Raider and stores anchors (e.g., Safe Hub region, reserved raid sites).
- **Worldgen Stage**: A named, ordered step in the Arc Raider pipeline.
- **Biome Slot**: A planned region concept with a name, intent, and placement constraints.
- **Structure Slot / Reserved Site**: A planned structure concept or reserved placement site with name, intent, and constraints.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A player can create an Arc Raider world by selecting the Arc Raider preset, and can create a non-Arc world without any change in the flow.
- **SC-002**: In at least 95% of Arc Raider world creations, world creation completes successfully without manual intervention.
- **SC-003**: For each world size option, at least 95% of Arc Raider world creations complete within 30 seconds on a typical gaming PC from the last 5 years.
- **SC-004**: In an Arc Raider world, a player can reach a Safe Hub area from initial spawn within 2 minutes of normal traversal.
- **SC-005**: A mod developer can add a new Arc Raider worldgen stage targeting a named stage boundary, and it runs in Arc Raider worlds only without breaking existing stages.
