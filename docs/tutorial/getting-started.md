# Getting started

`OctopusEnergy.Client` is a prerelease SDK. HTTP transport, pagination, typed errors, API-key authentication, and the products catalogue are available. Account and consumption resource methods are still being added.

## 1. Create an API key

Generate a key in the [Octopus dashboard](https://octopus.energy/dashboard/new/accounts/personal-details/api-access). Treat it as a secret.

## 2. Install the package

```bash
dotnet add package OctopusEnergy.Client
```

## 3. Construct the client

For account and consumption calls, pass your API key:

```csharp
using OctopusEnergy.Client;

const string apiKey = "sk_test_not_a_real_key";
using var client = new OctopusEnergyClient(apiKey);
```

Public catalogue endpoints work without authentication:

```csharp
using var client = new OctopusEnergyClient();
```

See [authentication](../how-to/authentication.md) for custom base URLs and supplying your own `HttpClient`.

## 4. Parse tariff codes (optional)

If you already have a tariff code from an account or product response, parse it without building URL segments yourself:

```csharp
TariffCode tariff = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");
string ratesPath = tariff.GetRelativeChargePath(TariffChargeKind.StandardUnitRates);
```

See [tariff codes and GSP](../how-to/tariff-codes.md).

## 5. List products

Public catalogue endpoints work without an API key:

```csharp
using OctopusEnergy.Client.Models.Products;

await foreach (Product product in client.Products.ListAsync(cancellationToken: cancellationToken))
{
    Console.WriteLine(product.DisplayName);
}
```

See [products](../how-to/products.md) for filters, product detail, and GSP tariff maps. Account and consumption services are tracked on feature [#6](https://github.com/markheydon/octopus-energy-dotnet/issues/6).

## Next steps

- [Products catalogue](../how-to/products.md)
- [Authentication](../how-to/authentication.md)
- [Tariff codes and GSP](../how-to/tariff-codes.md)
- [Pagination](../how-to/pagination.md)
- [Error handling](../how-to/error-handling.md)
- [API coverage](../reference/api-coverage.md)
