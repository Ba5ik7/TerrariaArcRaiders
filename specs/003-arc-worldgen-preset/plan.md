# Implementation Plan: Arc Raider Worldgen Preset

**Branch**: `003-arc-worldgen-preset` | **Date**: 2026-01-04 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Introduce an Arc Raider “world preset” that can be chosen at world creation time (via a tModLoader-compatible selection mechanism), tags worlds as Arc Raider for reliable detection, and runs a modular Arc-only worldgen pipeline with stable named stages and placeholder biome/structure slots. Vanilla worlds remain unaffected.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# (.NET 8 via tModLoader build)
**Primary Dependencies**: tModLoader/Terraria APIs, Microsoft.Xna.Framework (runtime), MSTest (headless Core tests)
**Storage**: tModLoader world/player storage (`TagCompound` + world header tag) and in-memory state
**Testing**: MSTest (`Microsoft.NET.Test.Sdk`, `MSTest.TestFramework`, `MSTest.TestAdapter`)
**Target Platform**: Windows/macOS/Linux (tModLoader)
**Project Type**: Single mod project with Core (testable) + Adapters (tModLoader glue)
**Performance Goals**: Arc world creation typically <30s; no measurable runtime FPS regression vs vanilla
**Constraints**: Vanilla world safety (no changes to non-Arc worlds); deterministic generation; safe failure on corrupted/missing metadata
**Scale/Scope**: Foundation only (named stages + placeholder slots + metadata); no full biome/content set

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Loop fit: Each feature must state how it reinforces the hub -> raid -> extract or die -> stash
  loop (entry/exit, extraction risk, stash impact).
- Separation: Core rules live in plain C# services; Terraria hooks remain thin adapters with explicit
  boundaries to keep logic testable headless.
- World safety: Plans describe how worlds stay loadable if the mod is disabled and how reversible
  changes or migrations are handled.
- Performance/cross-platform: Identify budgets and expensive paths; avoid per-tick heavy work and
  ensure fallback for platform-specific logic.
- Testability: List required unit tests for core logic and integration tests for hook boundaries and
  persistence.

**Gate Result (Pre-Research)**: PASS

- Loop fit: The Safe Hub region and reserved raid-terminal sites explicitly support the hub -> raid -> extract loop by providing a predictable “hub anchor” for future raid entry/exit surfaces.
- Separation: Region planning and constraints live in Core (tModLoader-free); adapters apply the plan via tModLoader hooks.
- World safety: Arc tagging is additive and isolated to Arc worlds; corrupted metadata fails safe; disabling the mod must not crash worlds.
- Performance/cross-platform: Plan avoids per-tick work; worldgen uses bounded region edits and deterministic RNG.
- Testability: Deterministic region planning is unit-testable headless; hook boundaries validated via integration smoke and manual quickstart.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
Core/
├── Models/
├── Services/
└── (new) WorldGen/           # planning-only, tModLoader-free

Adapters/
├── Systems/
└── (new) WorldGen/           # tModLoader GenPasses / task wiring

Tests/
└── Unit/
```

**Structure Decision**: Keep Arc worldgen planning logic in `Core/` for headless tests and keep all Terraria/tModLoader hooks and tile-writing code in `Adapters/`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |

No constitution violations are required for this feature.

## Phase 0: Outline & Research

**Output**: [research.md](research.md)

Key resolved items:

- World preset selection mechanism (tModLoader-compatible): seed prefix approach.
- World tagging/persistence strategy: world header + world data.
- Worldgen stages organization: custom pipeline via `ModifyWorldGenTasks` and named stage order.
- Determinism/performance guidelines for worldgen.

## Phase 1: Design & Contracts

**Outputs**:

- Data model: [data-model.md](data-model.md)
- Interaction contracts: [contracts/interaction-contracts.md](contracts/interaction-contracts.md)
- Quickstart: [quickstart.md](quickstart.md)

### World Type Integration Design

- Selection mechanism: treat a seed prefix (e.g., `arc:`) as the “Arc Raider preset.”
- World tagging:
  - Persist `IsArcWorld` + `DataVersion` in world header (tiny marker).
  - Persist full Arc metadata (hub region + reserved sites + optional region map) in world data.
- Vanilla safety: worldgen tasks and runtime behavior branch on `IsArcWorld` and are no-ops for non-Arc worlds.

### Worldgen Architecture Design

- Core planning service produces an `ArcWorldPlan` (regions + anchors) deterministically from seed + world size.
- Adapter stage pipeline maps the spec’s stable stage boundaries to concrete GenPasses:
  - Stage A: set world flags / clear Arc state
  - Stage B: create base terrain for Arc worlds
  - Stage C: compute region rectangles (hub + 1+ slots)
  - Stage D: paint placeholder biomes using vanilla tiles
  - Stage E: compute reserved structure sites
  - Stage F: place placeholder structures (optional)
  - Stage G: create raid-related reserved anchors in/near hub
  - Stage H: validate constraints and record final metadata

### Extensibility for Future Specs

- New biomes and structures are added as new stages or sub-stages under a stable stage boundary (e.g., a new “Structure Placement” pass).
- Raid-related structures are anchored via reserved sites so future features can place terminals/beacons without reworking early worldgen.

## Phase 2: Implementation Planning (No Tasks File)

This plan intentionally stops before creating `tasks.md`.

Implementation outline (high level):

1. Add Arc-world selection parsing (seed prefix) and store selection for worldgen.
2. Add world metadata + header marker for `IsArcWorld`, `DataVersion`, hub region, reserved sites.
3. Implement Core region planning models and a deterministic plan service with unit tests.
4. Implement adapter worldgen pipeline as named GenPasses, executed only for Arc worlds.
5. Expose hub region + reserved raid sites for future raid systems.
6. Add minimal smoke validation hooks/logging to confirm stage order in Arc worlds only.

Re-run constitution check after design: PASS (no new violations introduced).
