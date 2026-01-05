# Data Model: In-World Visual Indicators

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Entities

### `WorldgenIndicatorSettings`

Represents whether in-world worldgen indicators are enabled.

- `Enabled`: boolean (default: false)

### `WorldgenStageStatus`

Represents the runtime status of a specific Arc worldgen stage during world generation.

- `Stage`: Arc worldgen stage identifier (A–H; optional debug stages)
- `Completed`: boolean

### `WorldgenIndicatorPlacement`

Represents a single in-world marker tied to an Arc worldgen stage.

- `Stage`: Arc worldgen stage identifier
- `WorldX`: integer tile coordinate
- `WorldY`: integer tile coordinate
- `LegendLabel`: short text label describing the stage (for documentation/legend; may be rendered in-world depending on marker type)

### `WorldgenIndicatorLegend`

Mapping for interpreting markers.

- `Entries`: list of (`Stage`, `LegendLabel`, `MarkerDescription`)

## Relationships

- `WorldgenIndicatorSettings.Enabled` gates creation of `WorldgenIndicatorPlacements`.
- A world generation run produces 0..N `WorldgenStageStatus` entries (one per stage).
- A completed Arc world with indicators enabled produces 1 `WorldgenIndicatorPlacement` per stage (for stages that completed).

## Validation Rules

- Placements must be within world bounds.
- Placements must be derived from bounded inputs (hub region + stage index); no full-world scanning is allowed.
- Placements must be deterministic for the same world seed and size.

## State Transitions

- `WorldgenStageStatus.Completed` transitions from false → true when a stage pass successfully finishes.

## Persistence

- The indicators themselves are persisted as world content (vanilla tiles/objects).
- Stage status and legend are runtime-only unless future cleanup/migration needs require persistence.
