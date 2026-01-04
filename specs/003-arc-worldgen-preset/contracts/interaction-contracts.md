# Contracts: Arc Raider World Preset & Worldgen Stages

**Feature**: [../spec.md](../spec.md)
**Date**: 2026-01-04

These are *in-game interaction contracts* (not HTTP APIs). They document stable user-facing triggers and their expected behavior.

## Contract A: Selecting the Arc Raider preset during world creation

### Trigger

- Player enters a world seed using the Arc prefix and completes world creation.
  - Example: seed begins with `arc:`

### Behavior

- World generation treats this as an Arc Raider world.
- The created world is tagged as Arc Raider world metadata.
- Non-Arc world creation (seed without the prefix) behaves exactly as before.

### Failure behavior

- If the seed is empty or malformed after the prefix: the system still creates an Arc Raider world using a deterministic fallback seed.

### Non-goals

- No UI injection into the world creation screen for this iteration.

## Contract B: Arc world identification on load

### Trigger

- Player loads/enters an Arc Raider world.

### Behavior

- The system detects `IsArcWorld=true` from persisted world metadata.
- Arc-specific systems may read the Safe Hub region and reserved raid sites.

### Failure behavior

- If metadata is missing/corrupt:
  - The world remains loadable.
  - The system treats the world as non-Arc by default unless a safe recovery mechanism exists.

## Contract C: Named Arc worldgen stages

### Trigger

- Player creates an Arc Raider world.

### Behavior

- World generation runs the Arc worldgen stages in a stable, documented order:
  - Stage A: World Tagging & Setup
  - Stage B: Base Terrain Layout
  - Stage C: Region Planning (Biome Slotting)
  - Stage D: Biome Painting (Placeholder Allowed)
  - Stage E: Structure Reservation (Sites Only)
  - Stage F: Structure Placement (Placeholder Allowed)
  - Stage G: Raid-Related Anchors (Reserved Only)
  - Stage H: Final Polish & Validation

### Notes

- Stages are observable via logs or debug output (implementation-defined) for validation.
