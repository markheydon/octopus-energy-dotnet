# OctopusEnergy.Client

Unofficial .NET client for the **Octopus Energy customer APIs** (public REST catalogue plus authenticated account and consumption data). Not affiliated with Octopus Energy Limited.

Dashboard API keys expose account structure and consumption. Treat keys as secrets and do not share them with untrusted third parties. This library is not a developer licence from Octopus Energy.

## Install

```bash
dotnet add package OctopusEnergy.Client
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

## Documentation

Full guides (authentication, products, tariffs, consumption, units, VAT, and time zones): [markheydon.me.uk/octopus-energy-dotnet](https://markheydon.me.uk/octopus-energy-dotnet/).

Source and issue tracker: [github.com/markheydon/octopus-energy-dotnet](https://github.com/markheydon/octopus-energy-dotnet)
