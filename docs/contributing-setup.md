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

## Live API keys

Do not put API keys in the repo. CI uses recorded fixtures only. Optional live checks must be local and opt-in.

To smoke-test the SDK against the live UK API, run the [products console sample](../samples/README.md):

```bash
dotnet run --project samples/ProductsConsole
```

Set `OCTOPUS_ENERGY_API_KEY` and `OCTOPUS_ENERGY_ACCOUNT_NUMBER` to exercise authenticated account detail. Consumption is not in the SDK yet.

Read [CONTRIBUTING.md](../CONTRIBUTING.md), [CONVENTIONS.md](../CONVENTIONS.md), and [AGENTS.md](../AGENTS.md).
