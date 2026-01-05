# TerrariaArcRaiders Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-01-04

## Active Technologies
- C# (tModLoader mod; targets `net8.0` per project artifacts) + tModLoader API (`ModSystem`, `ModPlayer`, `ModNPC`, `ModCommand`, `ModConfig`), Terraria base game APIs (002-raid-entry-and-debug)
- TagCompound via tModLoader for mod data (stash + portal metadata already present); no irreversible world edits (002-raid-entry-and-debug)
- [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION] + [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION] (003-arc-worldgen-preset)
- [if applicable, e.g., PostgreSQL, CoreData, files or N/A] (003-arc-worldgen-preset)
- C# (.NET 8 via tModLoader build) + tModLoader/Terraria APIs, Microsoft.Xna.Framework (runtime), MSTest (headless Core tests) (003-arc-worldgen-preset)
- tModLoader world/player storage (`TagCompound` + world header tag) and in-memory state (003-arc-worldgen-preset)

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
- 003-arc-worldgen-preset: Added C# (.NET 8 via tModLoader build) + tModLoader/Terraria APIs, Microsoft.Xna.Framework (runtime), MSTest (headless Core tests)
- 003-arc-worldgen-preset: Added [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION] + [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION]
- 003-arc-worldgen-preset: Added [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION] + [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION]

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
