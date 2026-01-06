# Research: In-World Visual Indicators

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05  
**Source Spec**: `C:\Users\dusel\OneDrive\Documents\My Games\Terraria\tModLoader\ModSources\TerrariaArcRaiders\specs\004-in-world-visual-indicators\spec.md`

## Decisions

### Decision 1: Use vanilla-world-safe indicators (no custom tiles)

**Decision**: Indicators will be built exclusively from vanilla tiles/objects (e.g., blocks, torches, signs), not from modded tile types.

**Rationale**: If the mod is disabled/removed, worlds must remain loadable. Vanilla tile IDs remain valid; modded tile IDs do not.

**Alternatives considered**
- Custom “ArcDebugMarker” tile: rejected because it risks breaking world loading when the mod is missing.
- Pure UI overlay (no world content): rejected because the spec requires in-world indicators.

### Decision 2: Place indicators after vanilla worldgen completes

**Decision**: Place all indicators in a final worldgen pass that runs at the end of Terraria’s worldgen task list.

**Rationale**: The current Arc pipeline is inserted before vanilla tasks; any early tile/object placement could be overwritten by later vanilla worldgen. A final pass avoids that.

**Alternatives considered**
- Place markers during each stage pass: rejected because vanilla worldgen may overwrite them.
- Move all Arc passes to the end: rejected because Arc stages include planning/validation that should occur early and set world metadata.

### Decision 3: Deterministic layout tied to planned hub region

**Decision**: Indicator placement is derived from `ArcWorldData.SafeHubRegion` (planned hub rectangle) and stage order, producing deterministic coordinates for a given seed and world size.

**Rationale**: The hub region is already deterministic and bounded. Using it avoids full-world scanning and makes indicators easier to find.

**Alternatives considered**
- Place near spawn: rejected because spawn location may shift and is less tied to Arc planning outputs.
- Random placement: rejected because it breaks reproducibility.

### Decision 4: Stage completion tracking is in-memory only

**Decision**: Track stage completion in memory during worldgen and render indicators from that state; avoid persisting extra “debug state” into world save unless needed later.

**Rationale**: Keeps world save format stable and minimizes migration risk. The visible world markers already serve as the persistent artifact.

**Alternatives considered**
- Persist an “indicators placed” record in `ArcWorldData`: deferred; only needed if we add cleanup/removal workflows.

## Open Questions (resolved)

- “How do we keep worlds loadable if the mod is removed?” → Only place vanilla tiles/objects and avoid custom tile IDs.
- “How do we ensure markers are not overwritten?” → Place indicators in a final worldgen pass after vanilla tasks.
- “How do we keep it deterministic?” → Derive from planned hub region + stage index.
