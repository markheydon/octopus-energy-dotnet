# Recorded HTTP fixtures

CI and default `dotnet test` runs replay JSON from this folder through
`QueuedHttpMessageHandler`. No network calls or API keys are required.

## Adding a fixture

1. Capture a representative API response locally (curl, browser, or the
   [products console sample](../../../samples/README.md)).
2. Redact secrets, real account numbers, MPANs, MPRNs, postcodes, and API keys.
   Use obvious placeholders (`A-TEST0001`, `1000000000001`, `W1 1AA`).
3. Save the body as `kebab-case-name.json` in this directory.
4. Load it in tests with `FixtureFile.Read("kebab-case-name.json")`.
5. Queue the handler response: `handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read(...))`.

Pagination fixtures should include realistic `next` URLs. Error fixtures should
match the API error JSON shape (`detail` field).

## Live checks

Live API smoke tests live under `tests/OctopusEnergy.Client.Tests/Live/` and are
skipped unless `OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1` is set. They are never run
in CI.
