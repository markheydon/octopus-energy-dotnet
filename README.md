# Octopus Energy .NET Client

Unofficial .NET client for the [Octopus Energy](https://octopus.energy/) **customer** APIs. Not affiliated with Octopus Energy Limited.

[![CI](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/OctopusEnergy.Client.svg)](https://www.nuget.org/packages/OctopusEnergy.Client/)

> **Stable `1.x`.** The REST customer core shipped in `1.0.0`; `1.1.0` adds developer-experience improvements (hosting, testability, workflow helpers, consumption safety). Public API changes follow SemVer. See [VERSIONING.md](VERSIONING.md).

**Documentation:** [Consumer docs](https://markheydon.me.uk/octopus-energy-dotnet/) (GitHub Pages) - source in [`docs/`](docs/). Implementer notes: [coding notes](docs/planning/coding-notes.md).

## Goals

Typed, discoverable SDK so callers do not reconstruct REST URLs or GraphQL documents from the Octopus docs. Same intent as [freeagent-dotnet](https://github.com/markheydon/freeagent-dotnet). Scope is **customer dashboard API keys**, not Kraken partner enrolment or quoting. See [GOALS.md](GOALS.md) and [SCOPE.md](SCOPE.md).

## Status

Stable on NuGet **1.1.0** (REST customer core and developer experience). `OctopusEnergyClient` provides REST HTTP transport, pagination, typed errors, dashboard API-key authentication, the products catalogue, tariff rates, grid supply points, account detail, electricity/gas consumption, and tariff/GSP helpers (`TariffCode`, `GridSupplyPoint`).

**On `main` (pre-1.2.0 tag):** authenticated clients also expose curated GraphQL-backed resources (viewer, rate limits, meter readings, billing, devices, telemetry, Octoplus, product switch) without caller-supplied JWTs. Those accessors are not on NuGet until **[1.2.x](docs/reference/api-coverage.md)** ships — build from source or wait for the release tag. See [API coverage](docs/reference/api-coverage.md) for the split.

Roadmap (GitHub milestone → NuGet version; all stay on package **1.x** — see [release labels](VERSIONING.md#release-labels-quick-reference)):

- **v1.0** REST core — shipped **`1.0.0`**: products, tariffs, unit rates, standing charges, GSP, account, smart-meter consumption
- **v1.1** REST developer experience — shipped **`1.1.0`**: HttpClientFactory, interfaces, workflow helpers, safer consumption listing, 429 retry, trim-safe REST JSON, consumer how-tos
- **v1.2** GraphQL extras — in progress on **`main`** (**`1.2.x`** NuGet when tagged, not `2.0.0`): **viewer**, **rate limits**, **electricity and gas meter readings** (gas allow-list live probe deferred — no gas-capable dashboard key), **reading consent**, **account billing**, **SmartFlex devices**, **Home Mini telemetry**, **Octoplus** (public-host enrolment/points/wheel; not saving sessions on `api.backend.octopus.energy`), and **product switch**; **`AddOctopusEnergyClient`**, GraphQL JSON source generation, and opt-in meter reading mutations in the Products console sample

## Installation

```bash
dotnet add package OctopusEnergy.Client
```

Latest on [nuget.org](https://www.nuget.org/packages/OctopusEnergy.Client/) is **1.1.0** (REST). Pin that version when you do not need v1.2 GraphQL accessors yet (`dotnet add package OctopusEnergy.Client --version 1.1.0`). GraphQL-backed resources ship in **1.2.x** after the [1.2.0 release checklist](plan/RELEASE.md#120-tag-checklist).

## Quick start

```csharp
using OctopusEnergy.Client;

using var client = new OctopusEnergyClient();

await foreach (var product in client.Products.ListAsync())
{
    Console.WriteLine(product.DisplayName);
}
```

For account and consumption calls, pass your API key from the [Octopus dashboard](https://octopus.energy/dashboard/new/accounts/personal-details/api-access). Treat it as a secret.

See [getting started](https://markheydon.me.uk/octopus-energy-dotnet/tutorial/getting-started/), [authentication](https://markheydon.me.uk/octopus-energy-dotnet/how-to/authentication/), [consumption](https://markheydon.me.uk/octopus-energy-dotnet/how-to/consumption/), [tariff rates](https://markheydon.me.uk/octopus-energy-dotnet/how-to/tariff-rates/), [industry lookups](https://markheydon.me.uk/octopus-energy-dotnet/how-to/industry/), [meter readings](https://markheydon.me.uk/octopus-energy-dotnet/how-to/meter-readings/), [account billing](https://markheydon.me.uk/octopus-energy-dotnet/how-to/account-billing/), and [units, VAT, and time](https://markheydon.me.uk/octopus-energy-dotnet/explanation/units-vat-and-time/) for kWh, VAT fields, BST, and Agile 16:00 behaviour.

## Compatibility

| Host | Supported without extra MSBuild JSON settings |
|------|-----------------------------------------------|
| SDK-style `.csproj` apps | Yes |
| File-based apps (`dotnet run app.cs`, .NET 10+) | Yes — use `#:package OctopusEnergy.Client@<version>` |
| Native AOT publish of apps using this library | REST and GraphQL JSON deserialisation is source-generated; the package is not marketed as fully AOT-compatible end-to-end |

File-based apps enable trim-friendly JSON defaults. You do not need `#:property JsonSerializerIsReflectionEnabledByDefault=true` for this client.

## Building from source

```bash
git clone https://github.com/markheydon/octopus-energy-dotnet.git
cd octopus-energy-dotnet
dotnet clean OctopusEnergy.slnx && \
dotnet restore OctopusEnergy.slnx && \
dotnet build OctopusEnergy.slnx --no-restore --configuration Release -warnaserror && \
dotnet test OctopusEnergy.slnx --no-build --configuration Release
```

Contributor setup: [docs/contributing-setup.md](docs/contributing-setup.md). For a local live API smoke check, see [samples/](samples/).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md). By participating, you agree to [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).

## Support and security

- [SUPPORT.md](SUPPORT.md)
- [SECURITY.md](SECURITY.md)

## Licence

MIT - see [LICENSE](LICENSE).

## Resources

- [Octopus REST and GraphQL docs](https://docs.octopus.energy/)
- [GraphQL IDE](https://api.octopus.energy/v1/graphql/)
- [Kraken public announcements](https://announcements.kraken.tech/announcements/public/)
