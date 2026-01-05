# Contract: Indicator Legend

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Purpose

Defines how a developer can interpret the in-world markers and map them back to Arc worldgen stages without requiring log access.

## Contract

- Each Arc worldgen stage has a unique, distinguishable marker.
- There is a documented legend describing:
  - Stage identifier (A–H)
  - Stage name (human readable)
  - Marker appearance (what to look for)
  - Marker placement rule (where it will appear relative to the planned hub region)

## Acceptance Notes

- A developer can identify the stage associated with each marker by using this legend alone.

## Stage Legend (Current)

This feature uses the following stage labels:

| Stage | Label |
|-------|-------|
| A | Stage A: Setup |
| B | Stage B: Base Terrain |
| C | Stage C: Region Planning |
| D | Stage D: Biome Painting |
| E | Stage E: Structure Reservation |
| F | Stage F: Structure Placement |
| G | Stage G: Raid Anchors |
| H | Stage H: Final Validation |

## Placement Rule (Current)

- Indicators are laid out as an ordered list (A → H) in a compact “board” placed within the planned hub region.
- The exact in-world marker appearance is defined during the later tile/object placement implementation; it must remain unique per stage and readable without logs.
