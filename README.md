# Octopus Energy .NET Client

Unofficial .NET client for the [Octopus Energy](https://octopus.energy/) **customer** APIs. Not affiliated with Octopus Energy Limited.

[![CI](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/markheydon/octopus-energy-dotnet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/OctopusEnergy.Client.svg)](https://www.nuget.org/packages/OctopusEnergy.Client/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/OctopusEnergy.Client.svg?label=nuget%20prerelease)](https://www.nuget.org/packages/OctopusEnergy.Client/)

> **Prerelease software.** HTTP transport, pagination, errors, and API-key authentication are implemented. Resource methods are still being added. Public APIs will change. See [VERSIONING.md](VERSIONING.md).

**Documentation:** [docs/](docs/) - consumer guides plus [planning notes](docs/planning/coding-notes.md) for implementers.

## Goals

Typed, discoverable SDK so callers do not reconstruct REST URLs or GraphQL documents from the Octopus docs. Same intent as [freeagent-dotnet](https://github.com/markheydon/freeagent-dotnet). Scope is **customer dashboard API keys**, not Kraken partner enrolment or quoting. See [GOALS.md](GOALS.md) and [SCOPE.md](SCOPE.md).

## Status

Prerelease. `OctopusEnergyClient` provides REST HTTP transport, pagination, typed errors, dashboard API-key authentication, and tariff/GSP helpers (`TariffCode`, `GridSupplyPoint`). Resource services are not implemented yet.

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

// API key from https://octopus.energy/dashboard/new/accounts/personal-details/api-access
// Treat as a secret — never commit it to source control.
const string apiKey = "sk_test_not_a_real_key";
using var client = new OctopusEnergyClient(apiKey);
```

Public catalogue calls work without a key: `using var client = new OctopusEnergyClient();`

See [getting started](docs/tutorial/getting-started.md) and [authentication](docs/how-to/authentication.md).

## Building from source

```bash
git clone https://github.com/markheydon/octopus-energy-dotnet.git
cd octopus-energy-dotnet
dotnet clean OctopusEnergy.slnx && \
dotnet restore OctopusEnergy.slnx && \
dotnet build OctopusEnergy.slnx --no-restore --configuration Release -warnaserror && \
dotnet test OctopusEnergy.slnx --no-build --configuration Release
```

Contributor setup: [docs/contributing-setup.md](docs/contributing-setup.md).

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
