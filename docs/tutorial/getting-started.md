# Getting started

`OctopusEnergy.Client` is an **unofficial**, prerelease SDK for the Octopus Energy **customer** APIs. It is not affiliated with Octopus Energy Limited, and the website terms of use are not a developer licence.

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

## 4. List products

The products catalogue is public - no API key required when you use the parameterless constructor:

```csharp
using OctopusEnergy.Client.Models.Products;

await foreach (Product product in client.Products.ListAsync(cancellationToken))
{
    Console.WriteLine(product.DisplayName);
}
```

Pagination is automatic. See [products](../how-to/products.md) for filters and product detail.

## 5. Fetch account detail

Account calls require an API key and your account number (`A-XXXXXXXX`):

```csharp
using OctopusEnergy.Client.Models.Accounts;

Account account = await client.Accounts.GetAsync("A-12345678", cancellationToken);
Console.WriteLine($"{account.Number}: {account.Properties.Count} properties");
```

See [account detail](../how-to/accounts.md). Electricity and gas consumption are available on `client.Consumption`; a dedicated how-to will follow.

## Next steps

- [Units, VAT, and time](../explanation/units-vat-and-time.md) - kWh, pence, VAT fields, BST, and Agile 16:00
- [Products catalogue](../how-to/products.md)
- [Account detail](../how-to/accounts.md)
- [Authentication](../how-to/authentication.md)
- [Pagination](../how-to/pagination.md)
- [Error handling](../how-to/error-handling.md)
- [API coverage](../reference/api-coverage.md)
