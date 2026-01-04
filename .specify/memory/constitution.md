<!-- Sync Impact Report:
- Version change: N/A -> 1.0.0
- Modified principles: (new) Hub -> Raid -> Extract Loop Integrity; Decoupled Game Logic and Glue; World Safety and Fail-Safe Operation; Performance and Cross-Platform Discipline; Spec-Driven Delivery and Testability
- Added sections: Additional Constraints; Development Workflow
- Removed sections: none
- Templates requiring updates: [updated] .specify/templates/plan-template.md; [updated] .specify/templates/spec-template.md; [updated] .specify/templates/tasks-template.md
- Follow-up TODOs: none
-->

# TerrariaArcRaiders Constitution

## Core Principles

### Hub -> Raid -> Extract Loop Integrity
Every feature must preserve the hub -> raid -> extract or die -> stash loop. Raids start and end
through explicit entry/exit flows; extraction carries risk and reward tension; stashes persist
progress without bypassing raid outcomes. Any new system must state how it fits this loop.

### Decoupled Game Logic and Glue
Game rules live in plain C# modules with minimal tModLoader coupling. Terraria hooks act as thin
adapters that delegate to testable services. Core logic must run headless for unit tests, with
boundaries documented so swapping or disabling hooks does not break rule execution.

### World Safety and Fail-Safe Operation
Mod data must be isolated from vanilla world integrity: no irreversible world mutations, reversible
tile/NPC changes where possible, and explicit migration/rollback plans. If the mod is removed or
disabled, worlds remain loadable and player inventories are cleaned or safely ignored without
crashes. Failing subsystems must degrade gracefully rather than blocking gameplay.

### Performance and Cross-Platform Discipline
Targets: negligible load-time impact and 60 fps gameplay parity with vanilla on Windows/macOS/Linux.
Avoid per-tick heavy allocations, reflection, or blocking I/O in hooks. Any platform-specific logic
requires a fallback path and must be guarded by capability checks.

### Spec-Driven Delivery and Testability
Work begins with a written spec and plan that map to the loop and constraints above. Core logic
requires unit tests; integration tests cover hook boundaries and persistence. Features ship only
when specs, tests, and gameplay validation demonstrate the intended raid loop behavior.

## Additional Constraints
Technology stack: C#, tModLoader, cross-platform target (Windows/macOS/Linux). Architecture must
keep performance overhead minimal and avoid breaking vanilla saves. Persistence belongs in mod
storage, not in-world irreversible changes. Features must expose safe defaults when configuration is
missing or invalid.

## Development Workflow
Start with specification and research, then design data and contracts that respect the hub -> raid
-> extract loop. Implement core logic in isolated services, wrap with Terraria hooks last, and keep
tests green before gameplay validation. Any new feature must document world-safety assurances,
performance expectations, and disable/cleanup behavior.

## Governance
This constitution is the authoritative guardrail for the mod. Amendments require documenting the
change, rationale, migration/rollback steps, and updating dependent templates. Versioning follows
Semantic Versioning for governance text. Compliance reviews occur at plan and PR time: reviewers
must check loop alignment, separation of logic, world safety, performance budgets, and tests.

**Version**: 1.0.0 | **Ratified**: 2026-01-03 | **Last Amended**: 2026-01-03

