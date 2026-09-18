# Octopus Energy .NET Client

Unofficial .NET client for the [Octopus Energy](https://octopus.energy/) **customer** APIs. Not affiliated with Octopus Energy Limited.

[![CI](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/OctopusEnergy.Client.svg)](https://www.nuget.org/packages/OctopusEnergy.Client/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/OctopusEnergy.Client.svg?label=nuget%20prerelease)](https://www.nuget.org/packages/OctopusEnergy.Client/)

> **Prerelease software.** HTTP transport, pagination, errors, API-key authentication, the products catalogue, and account detail are implemented. Further resource methods are being added. Public APIs will change. See [VERSIONING.md](VERSIONING.md).

**Documentation:** [Consumer docs](https://markheydon.me.uk/octopus-energy-dotnet/) (GitHub Pages) - source in [`docs/`](docs/). Implementer notes: [coding notes](docs/planning/coding-notes.md).

## Goals

Typed, discoverable SDK so callers do not reconstruct REST URLs or GraphQL documents from the Octopus docs. Same intent as [freeagent-dotnet](https://github.com/markheydon/freeagent-dotnet). Scope is **customer dashboard API keys**, not Kraken partner enrolment or quoting. See [GOALS.md](GOALS.md) and [SCOPE.md](SCOPE.md).

## Status

Prerelease. `OctopusEnergyClient` provides REST HTTP transport, pagination, typed errors, dashboard API-key authentication, the products catalogue, account detail, and tariff/GSP helpers (`TariffCode`, `GridSupplyPoint`). Further resource services are being added.

Planned:

- **v1** - REST: products, tariffs, unit rates, standing charges, GSP, account, smart-meter consumption
- **v2** - GraphQL: viewer, bills, devices, Home Mini telemetry, Octoplus, meter readings, product switch

## Installation

```bash
dotnet add package OctopusEnergy.Client
```

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

See [getting started](https://markheydon.me.uk/octopus-energy-dotnet/tutorial/getting-started/), [authentication](https://markheydon.me.uk/octopus-energy-dotnet/how-to/authentication/), and [units, VAT, and time](https://markheydon.me.uk/octopus-energy-dotnet/explanation/units-vat-and-time/) for kWh, VAT fields, BST, and Agile 16:00 behaviour.

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
