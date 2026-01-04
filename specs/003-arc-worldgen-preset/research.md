# Research: Arc Raider Worldgen Preset

**Feature**: [spec.md](spec.md)
**Date**: 2026-01-04

This document resolves planning unknowns and records key technical decisions with rationales and alternatives.

## Decision 1: How the player selects the Arc Raider world preset

- **Decision**: Use a seed prefix as the first implementation of the “world preset” selection.
  - Proposed prefix: `arc:` (e.g., seed `arc:my-seed`)
- **Rationale**:
  - tModLoader does not provide a stable, officially supported API to add a new first-class toggle/preset to the vanilla Create World UI.
  - Seed text is an existing, per-world input that flows into worldgen and is shareable.
  - Meets the spec’s “or whatever the tModLoader hook equivalent is” requirement while avoiding fragile UI detours.
- **Alternatives considered**:
  - **UI injection / detours into Terraria UI classes**: closest to a real toggle, but fragile across tML/Terraria updates and more maintenance-heavy.
  - **ModConfig toggle**: easy, but not inherently per-world (unless carefully scoped) and less aligned with “world selection.”
  - **World name prefix**: simpler than seed, but renameable and less intentional.
  - **Post-create conversion command/NPC**: robust, but not selected during creation.

## Decision 2: How we tag and detect “this is an Arc Raider world”

- **Decision**: Persist an `IsArcWorld` flag and minimal Arc metadata using tModLoader world storage.
  - Use `ModSystem.SaveWorldData` / `LoadWorldData` for full runtime metadata.
  - Use `ModSystem.SaveWorldHeader` for a tiny header marker (world-list-visible without loading).
- **Rationale**:
  - World data tagging is the most reliable way to detect Arc worlds at runtime and keep vanilla worlds untouched.
  - Header tagging enables future UX improvements (world list marker) without needing to load the world.
- **Alternatives considered**:
  - **Infer from seed on every load**: works, but breaks if seed parsing changes and makes existing worlds ambiguous.
  - **Infer from world name**: renameable.

## Decision 3: Worldgen hook and stage organization

- **Decision**: Implement Arc worldgen as a pipeline of named stages executed via `ModSystem.ModifyWorldGenTasks`.
  - For Arc worlds, replace or heavily override the vanilla worldgen task list with Arc-only tasks.
  - For non-Arc worlds, do not alter the vanilla task list.
- **Rationale**:
  - `ModifyWorldGenTasks(List<GenPass> tasks, ...)` is the supported customization point for worldgen.
  - A single pipeline with explicit, stable stage boundaries matches the spec and supports future expansion.
- **Alternatives considered**:
  - **Only insert passes after known vanilla stages (e.g., after “Micro Biomes”)**: simpler, but does not produce a “completely custom world” and risks fighting vanilla layout.

## Decision 4: Determinism and performance discipline

- **Decision**: Treat each worldgen stage as deterministic for a given seed and use worldgen RNG consistently.
  - Use `WorldGen.genRand` (or deterministic derivations from it) for randomness.
  - Avoid full-world scans when not needed; operate on planned region rectangles.
- **Rationale**:
  - Determinism is required by the spec and improves debugging and testability.
  - Worldgen performance is primarily impacted by large tile loops and per-tile allocations.
- **Alternatives considered**:
  - **System.Random / time-based randomness**: non-deterministic and harder to reproduce.
  - **Frequent per-tile framing and helper calls**: simpler but typically much slower.

## Decision 5: Keeping logic modular and testable

- **Decision**: Split Arc worldgen into:
  - **Core planning** (tModLoader-free): compute region plans, constraints, and anchors.
  - **Adapter execution** (tModLoader-coupled): apply those plans to the real Terraria tile world during worldgen.
- **Rationale**:
  - Aligns with the constitution’s “Decoupled Game Logic and Glue.”
  - Enables unit tests for deterministic planning without requiring the game runtime.
- **Alternatives considered**:
  - **All logic in ModSystem/GenPasses**: simplest, but hard to unit test and tends to grow into an unstructured monolith.
