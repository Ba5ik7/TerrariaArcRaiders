# Data Model: Arc Raider Worldgen Preset

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

This data model describes the conceptual entities and persisted metadata needed to support an Arc Raider world preset.

## Entities

### ArcWorldSelection

Represents how the world preset was selected during world creation.

- **Fields**
  - `Mode`: `None | ArcRaider`
  - `Source`: `SeedPrefix | Other` (future-proofing)
  - `RawSeedText`: string (optional; for debug/logging only)
- **Validation rules**
  - `Mode=ArcRaider` if and only if the selection mechanism indicates Arc Raider (initially `arc:` seed prefix).

### ArcWorldHeader

Tiny, world-list-visible metadata.

- **Fields**
  - `IsArcWorld`: bool
  - `DataVersion`: int
- **Constraints**
  - Keep minimal to avoid slowing world selection UI.

### ArcWorldData

Full runtime metadata persisted into the world.

- **Fields**
  - `IsArcWorld`: bool
  - `DataVersion`: int
  - `SafeHubRegion`: `IntRect` (required in Arc worlds)
  - `Regions`: map of `ArcRegionId -> IntRect` (optional slots can be absent)
  - `ReservedSites`: list of `ArcReservedSite`
- **Validation rules**
  - If `IsArcWorld=true`, `SafeHubRegion` must be present and non-empty.
  - `Regions` must not contain overlapping regions unless explicitly allowed in a future spec.
  - `ReservedSites` must be inside or adjacent to `SafeHubRegion` for the “hub terminal” reserved site.

### IntRect

tModLoader-free rectangle primitive for region bounds.

- **Fields**
  - `X`, `Y`, `Width`, `Height` (all ints)
- **Validation rules**
  - `Width > 0` and `Height > 0`
  - Rectangle must be within world bounds during generation.

### ArcReservedSite

A reserved placement location intended for future structure placement.

- **Fields**
  - `Kind`: `RaidTerminal | Other`
  - `X`, `Y`: int (tile coordinates)
  - `Radius`: int (tile radius; optional)
- **Validation rules**
  - Tile coordinates must be within world bounds.

### ArcRegionId

Named slot identifiers referenced by future specs and stages.

- **Values (initial)**
  - `SafeHub`
  - `ArcWasteland`
  - `RaidScar`
  - `DroneFactoryRuins`
  - `ServiceTunnels`

## State and Transitions

- World creation starts with `ArcWorldSelection` derived from user input (seed prefix).
- World generation computes a region plan and writes `ArcWorldHeader` and `ArcWorldData`.
- Runtime gameplay logic detects `IsArcWorld` and uses anchors (`SafeHubRegion`, `ReservedSites`) for future integrations.
