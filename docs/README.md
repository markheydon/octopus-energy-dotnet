# OctopusEnergy.Client documentation

Unofficial .NET SDK for the Octopus Energy **customer** APIs. These pages help you install the package, authenticate with a dashboard API key, and call products, tariffs, rates, account, and consumption endpoints.

**New here?** Start with [Getting started](tutorial/getting-started.md).

## Guides

| Topic | What it covers |
|---|---|
| [Getting started](tutorial/getting-started.md) | Install the package, construct the client, first API calls |
| [Authentication](how-to/authentication.md) | API keys and HTTP Basic |
| [Pagination](how-to/pagination.md) | Listing resources without managing `next` links |
| [Products catalogue](how-to/products.md) | List products and load product detail |
| [Account detail](how-to/accounts.md) | Fetch properties, meters, and agreements |
| [Tariff codes and GSP](how-to/tariff-codes.md) | Parse tariff codes and build charge paths |
| [Error handling](how-to/error-handling.md) | API failures and exception types |

## Reference and concepts

| Topic | What it covers |
|---|---|
| [API coverage](reference/api-coverage.md) | What the SDK implements today |
| [Units, VAT, and time](explanation/units-vat-and-time.md) | kWh, pence, VAT fields, BST, Agile 16:00 |
| [Customer vs partner](explanation/customer-vs-partner.md) | Why the public surface is small |
| [REST and GraphQL](explanation/rest-and-graphql.md) | Dual transport (v1 is REST) |

## Related links

- [Repository README](https://github.com/markheydon/octopus-energy-dotnet/blob/main/README.md)
- [VERSIONING.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/VERSIONING.md)
- [SUPPORT.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/SUPPORT.md)
- [Octopus API documentation](https://docs.octopus.energy/)
