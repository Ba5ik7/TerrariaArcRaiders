# Quickstart: In-World Visual Indicators

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Goal

Generate an Arc world that contains visible, in-world markers for each Arc worldgen stage so you can debug the pipeline by inspection.

## Steps

1. Enable the mod and reload tModLoader.
2. Enable the “worldgen visual indicators” setting (debug/opt-in).
3. Create a new world using an Arc seed prefix, for example: `arc:test`.
4. After generation completes, enter the world and locate the planned hub area (roughly upper-mid world). Indicators should be placed in or near the planned hub region.

## What to look for

- One marker per Arc worldgen stage (A–H).
- Markers are deterministic for the same seed and world size (recreating the world should produce the same marker layout).
- Markers should not appear in non-Arc worlds, or when the setting is disabled.
