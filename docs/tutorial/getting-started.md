# Getting started

`OctopusEnergy.Client` is a prerelease SDK. HTTP transport, pagination, typed errors, and API-key authentication are available. Resource methods for products, account, and consumption are still being added.

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

## 4. Call resource methods

Resource services are not implemented yet. Track progress on feature [#6](https://github.com/markheydon/octopus-energy-dotnet/issues/6) (REST customer core) on milestone [v1.0](https://github.com/markheydon/octopus-energy-dotnet/milestone/1).

## Next steps

- [Authentication](../how-to/authentication.md)
- [Pagination](../how-to/pagination.md)
- [Error handling](../how-to/error-handling.md)
- [API coverage](../reference/api-coverage.md)
