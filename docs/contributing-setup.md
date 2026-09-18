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

To smoke-test the SDK against the live UK API without xUnit, run the [products console sample](../samples/README.md):

```bash
dotnet run --project samples/ProductsConsole
```

Set `OCTOPUS_ENERGY_API_KEY` and `OCTOPUS_ENERGY_ACCOUNT_NUMBER` to exercise authenticated account detail and consumption in the [products console sample](../samples/README.md). Public catalogue, tariff rates, and industry lookups run without a key.

Read [CONTRIBUTING.md](../CONTRIBUTING.md), [CONVENTIONS.md](../CONVENTIONS.md), and [AGENTS.md](../AGENTS.md).
