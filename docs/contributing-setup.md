# Contributor setup

This guide is for developers **working on this repository**, not for NuGet consumers.

## Prerequisites

Install **both** .NET SDKs:

| SDK | Why |
|-----|-----|
| **.NET 10.0** | Primary focus; `global.json` orchestrator |
| **.NET 8.0** | SDK package multi-targets `net8.0` |

CI installs `8.0.x` and `10.0.x`.

```bash
dotnet --list-sdks
```

[global.json](../global.json) pins SDK `10.0.300` with `rollForward: latestFeature`.

## Clone and build

```bash
git clone https://github.com/markheydon/octopus-energy-dotnet.git
cd octopus-energy-dotnet
dotnet clean OctopusEnergy.slnx && \
dotnet restore OctopusEnergy.slnx && \
dotnet build OctopusEnergy.slnx --no-restore --configuration Release -warnaserror && \
dotnet test OctopusEnergy.slnx --no-build --configuration Release
```

Warnings fail the build.

```bash
dotnet format OctopusEnergy.slnx --verify-no-changes
```

## Recorded HTTP fixtures

Default `dotnet test` and CI use **recorded JSON fixtures** only. Tests queue
responses with `QueuedHttpMessageHandler` and `FixtureFile.Read(...)`; they never
call `api.octopus.energy` unless you opt in.

To add a fixture:

1. Capture a representative response locally and redact secrets and real account identifiers.
2. Save it under `tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/`.
3. Load it in tests with `FixtureFile.Read("your-fixture.json")`.

See [Fixtures/README.md](../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/README.md) for naming and pagination notes.

## Live API keys

Do not put API keys in the repo. CI uses recorded fixtures only. Optional live checks must be local and opt-in.

Set `OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1` to run live xUnit smoke tests under
`tests/OctopusEnergy.Client.Tests/Live/`. They remain skipped in CI.

To regenerate the v1.2 GraphQL customer allow-list matrix (issue #21), run:

```bash
export OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1
export OCTOPUS_ENERGY_API_KEY='…'
dotnet test tests/OctopusEnergy.Client.Tests/OctopusEnergy.Client.Tests.csproj \
  -c Release --filter "FullyQualifiedName~LiveGraphQlAllowListProbeTests" \
  --output detailed
```

Results belong in [docs/planning/graphql-customer-allow-list.md](planning/graphql-customer-allow-list.md) (redacted). See `AllowListProbeQueries` for probe documents.

To smoke-test the SDK against the live UK API without xUnit, run the [products console sample](../samples/README.md):

```bash
dotnet run --project samples/ProductsConsole
```

Set `OCTOPUS_ENERGY_API_KEY` and `OCTOPUS_ENERGY_ACCOUNT_NUMBER` for authenticated smoke (account, consumption, viewer, rate limits, meter readings, account billing, devices, telemetry, and Octoplus when the account has the right data). Public catalogue, tariff rates, and industry lookups run without a key. See [samples/README.md](../samples/README.md) for the full checklist and environment variables.

Read [CONTRIBUTING.md](../CONTRIBUTING.md), [CONVENTIONS.md](../CONVENTIONS.md), and [AGENTS.md](../AGENTS.md).
