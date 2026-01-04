# Quickstart & Validation Notes

## Test Results (2026-01-03)
- Command: `dotnet test TerrariaArcRaiders.csproj -v minimal`
- Outcome: Passed (15 tests, 0 failed, 0 skipped)
- Duration: ~55 ms
- Notes: MSTest emitted a discovery warning about missing `tModLoader` assembly; tests still executed because they are headless service/unit tests. No gameplay hooks were invoked.

## Next Steps
- If adding integration tests that reference tModLoader assemblies, ensure the test runner has access to those dependencies or add shims/mocks.
- Rerun `dotnet test TerrariaArcRaiders.csproj -v minimal` after future changes.
