# Getting started

`OctopusEnergy.Client` is an **unofficial**, stable customer REST client for the Octopus Energy **customer** APIs. It is not affiliated with Octopus Energy Limited, and the website terms of use are not a developer licence.

A dashboard API key exposes your account structure and smart-meter consumption. Treat it as a secret and do not share it with third-party services you do not trust. Partner enrolment, quoting, and operations APIs are out of scope - see [customer vs partner](../explanation/customer-vs-partner.md).

## 1. Create an API key

Generate a key in the [Octopus dashboard](https://octopus.energy/dashboard/new/accounts/personal-details/api-access).

## 2. Install the package

```bash
dotnet add package OctopusEnergy.Client
```

## 3. Construct the client

Pass your API key for account, consumption, and other authenticated calls:

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

### Testability with interfaces

Depend on `IOctopusEnergyClient` and the resource service interfaces (`IAccountService`, `IConsumptionService`, and so on) in application code so unit tests can substitute behaviour. Construct `OctopusEnergyClient` at the composition root; the concrete client implements `IOctopusEnergyClient` and exposes interface-typed resource properties.

```csharp
public sealed class AccountSummaryService(IOctopusEnergyClient client)
{
    public Task<Account> GetAsync(string accountNumber, CancellationToken cancellationToken = default) =>
        client.Accounts.GetAsync(accountNumber, cancellationToken);
}
```

## 4. List products

The products catalogue is public - no API key required when you use the parameterless constructor:

```csharp
using OctopusEnergy.Client.Models.Products;

await foreach (Product product in client.Products.ListAsync())
{
    Console.WriteLine(product.DisplayName);
}
```

Pagination is automatic. See [products](../how-to/products.md) for filters and product detail.

## 5. Fetch account detail

Account calls require an API key and your account number (`A-XXXXXXXX`):

```csharp
using OctopusEnergy.Client.Models.Accounts;

Account account = await client.Accounts.GetAsync("A-12345678");
Console.WriteLine($"{account.Number}: {account.Properties.Count} properties");
```

See [account detail](../how-to/accounts.md). Electricity and gas consumption are on `client.Consumption` — see [consumption](../how-to/consumption.md).

## File-based apps (.NET 10+)

You can run the quick start from a single `.cs` file with `#:package` (no `.csproj`). The client uses source-generated JSON metadata, so you do not need `JsonSerializerIsReflectionEnabledByDefault=true`.

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

```bash
dotnet run ListProducts.cs
```

See [samples/FileBasedProducts](https://github.com/markheydon/octopus-energy-dotnet/tree/main/samples/FileBasedProducts) for a repo smoke script. Native AOT publish of consumer apps is not fully validated for every SDK code path; REST response deserialisation is covered by CI with reflection-based JSON disabled.

## Next steps

- [Units, VAT, and time](../explanation/units-vat-and-time.md) - kWh, pence, VAT fields, BST, and Agile 16:00
- [Products catalogue](../how-to/products.md)
- [Account detail](../how-to/accounts.md)
- [Consumption](../how-to/consumption.md)
- [Tariff rates](../how-to/tariff-rates.md)
- [Industry lookups](../how-to/industry.md)
- [Authentication](../how-to/authentication.md)
- [Pagination](../how-to/pagination.md)
- [Error handling](../how-to/error-handling.md)
- [API coverage](../reference/api-coverage.md)
