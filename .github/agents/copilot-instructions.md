# TerrariaArcRaiders Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-01-04

## Active Technologies
- C# (tModLoader mod; targets `net8.0` per project artifacts) + tModLoader API (`ModSystem`, `ModPlayer`, `ModNPC`, `ModCommand`, `ModConfig`), Terraria base game APIs (002-raid-entry-and-debug)
- TagCompound via tModLoader for mod data (stash + portal metadata already present); no irreversible world edits (002-raid-entry-and-debug)

## Project Structure

```text
Adapters/
Core/
Localization/
Properties/
Tests/
specs/
```

## Commands

dotnet test

## Code Style

C#: Match existing repo style; keep adapters thin and core logic testable headless.

## Recent Changes
- 002-raid-entry-and-debug: Added C# (tModLoader mod; targets `net8.0` per project artifacts) + tModLoader API (`ModSystem`, `ModPlayer`, `ModNPC`, `ModCommand`, `ModConfig`), Terraria base game APIs

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
