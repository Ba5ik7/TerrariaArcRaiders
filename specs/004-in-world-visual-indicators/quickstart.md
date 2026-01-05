# Quickstart: In-World Visual Indicators

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Goal

Generate an Arc world that contains visible, in-world markers for each Arc worldgen stage so you can debug the pipeline by inspection.

## Steps

1. Enable the mod and reload tModLoader.
2. Enable the “Arc worldgen visual indicators (debug)” setting (Mod Config → Server Side).
3. Create a new world using an Arc seed prefix, for example: `arc:test`.
4. After generation completes, enter the world and locate the planned hub area (roughly upper-mid world). Indicators are placed within the planned hub region.

## What to look for

- One marker per Arc worldgen stage (A–H).
- Markers are deterministic for the same seed and world size (recreating the world should produce the same marker layout).
- Markers should not appear in non-Arc worlds, or when the setting is disabled.

## Expected Behaviors

- Arc world + toggle OFF: no indicator markers are placed.
- Arc world + toggle ON: indicator markers are placed (one per stage).
- Non-Arc world (any toggle setting): no indicator markers are placed.
