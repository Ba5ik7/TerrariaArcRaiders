# Quickstart & Validation Notes

## Test Results (2026-01-03)
- Command: `dotnet test TerrariaArcRaiders.csproj -v minimal`
- Outcome: Passed (15 tests, 0 failed, 0 skipped)
- Duration: ~55 ms
- Notes: MSTest emitted a discovery warning about missing `tModLoader` assembly; tests still executed because they are headless service/unit tests. No gameplay hooks were invoked.

## Manual Validation Checklist (T021)

Status: Pending in-game run; steps recorded for execution. Environment here is headless/non-interactive.

- Enter → Loot → Extract: Pending — load world, use entry portal, collect scrap, extract; expect stash increments and raid inventory clears with notification.
- Death Loss: Pending — die in raid with scrap; expect raid inventory clears, stash unchanged, failure message, respawn at spawn.
- Reload Stash Persistence: Pending — save/quit/reload with mod enabled; stash value should persist.
- Disable-Mod Safety: Pending — disable mod and load world; should not crash; raid data ignored safely.
- FPS Sanity: Pending — observe zone with drones; expect vanilla-like performance (no noticeable drops).

## Next Steps
- If adding integration tests that reference tModLoader assemblies, ensure the test runner has access to those dependencies or add shims/mocks.
- Rerun `dotnet test TerrariaArcRaiders.csproj -v minimal` after future changes.
- Execute the above manual checklist in-game and update statuses/results.
