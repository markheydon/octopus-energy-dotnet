# OctopusEnergy.Client

Unofficial .NET client for the **Octopus Energy customer APIs** (REST catalogue, account, and consumption; curated GraphQL where REST does not cover discovery and operational limits). Not affiliated with Octopus Energy Limited.

Stable `1.x` with SemVer guarantees. **1.1.0** on [nuget.org](https://www.nuget.org/packages/OctopusEnergy.Client/) ships the REST customer core and developer experience (hosting, interfaces, workflow helpers, trim-safe REST JSON). Milestone **v1.2** (NuGet **1.2.x**, not **2.0.0**) adds GraphQL-backed `Viewer`, `RateLimits`, `MeterReadings`, `AccountBilling`, `Devices`, `SmartMeterTelemetry`, `Octoplus`, and `ProductSwitch` on the same client (transport is internal). That GraphQL surface is on repository `main` until **1.2.0** is tagged — pin **1.1.0** when you only need REST. See [release labels](https://github.com/markheydon/octopus-energy-dotnet/blob/main/VERSIONING.md#release-labels-quick-reference) and [API coverage](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/reference/api-coverage.md).

Dashboard API keys expose account structure and consumption. Treat keys as secrets and do not share them with untrusted third parties. This library is not a developer licence from Octopus Energy.

**Targets:** .NET 8.0 and .NET 10.0 (`net8.0`; `net10.0`).

## Install

```bash
dotnet add package OctopusEnergy.Client
```

Pin an exact version when you need reproducible builds (for example REST-only on **1.1.0**):

```bash
dotnet add package OctopusEnergy.Client --version 1.1.0
```

## Quick example

Public catalogue endpoints work without authentication:

```csharp
using OctopusEnergy.Client;

using var client = new OctopusEnergyClient();

await foreach (var product in client.Products.ListAsync())
{
    Console.WriteLine(product.DisplayName);
}
```

For account and consumption calls, pass your API key from the [Octopus dashboard](https://octopus.energy/dashboard/new/accounts/personal-details/api-access).

### File-based apps (.NET 10+)

```csharp
#:package OctopusEnergy.Client@1.1.0
#:property PublishAot=false

using OctopusEnergy.Client;

using var client = new OctopusEnergyClient();

await foreach (var product in client.Products.ListAsync())
{
    Console.WriteLine(product.DisplayName);
}
```

Use `@1.2.x` after **1.2.0** publishes when you need GraphQL-backed resources. `PublishAot=false` avoids native publish constraints unrelated to REST JSON. SDK-style projects and file-based apps work without `JsonSerializerIsReflectionEnabledByDefault`.

## Upgrading to 1.2.x

**1.2.0** introduces source-breaking changes on some REST wire fields: `Product.Direction`, `MeterRegister.Rate`, and `TariffCharge.PaymentMethod` become `WireField<T>` instead of `string`. Use `.Value` for the enum and `.UnparsedValue` for unknown API tokens. See the [release runbook](https://github.com/markheydon/octopus-energy-dotnet/blob/main/plan/RELEASE.md) and consumer how-tos on the documentation site.

## Documentation

Full guides (authentication, products, tariffs, consumption, GraphQL-backed billing and devices, units, VAT, and time zones): [markheydon.me.uk/octopus-energy-dotnet](https://markheydon.me.uk/octopus-energy-dotnet/).

Source and issue tracker: [github.com/markheydon/octopus-energy-dotnet](https://github.com/markheydon/octopus-energy-dotnet)
