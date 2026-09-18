# OctopusEnergy.Client documentation

Documentation in this folder is for **developers using the NuGet package** - except `planning/`, which is for SDK implementers.

Internal policy: repository root (`GOALS.md`, `SCOPE.md`, `CONVENTIONS.md`) and [`plan/`](../plan/). ADRs: [`adr/`](../adr/).

## Documentation map

| Document | Type | Audience |
|---|---|---|
| [Getting started](tutorial/getting-started.md) | Tutorial | New SDK consumers |
| [Authentication](how-to/authentication.md) | How-to | API keys and HTTP Basic |
| [Pagination](how-to/pagination.md) | How-to | Listing resources |
| [Products catalogue](how-to/products.md) | How-to | List products and load product detail |
| [Account detail](how-to/accounts.md) | How-to | Fetch properties, meters, and agreements |
| [Tariff codes and GSP](how-to/tariff-codes.md) | How-to | Parse tariff codes and build charge paths |
| [Error handling](how-to/error-handling.md) | How-to | API failures |
| [API coverage](reference/api-coverage.md) | Reference | What the SDK implements today |
| [Customer vs partner](explanation/customer-vs-partner.md) | Explanation | Why the public surface is small |
| [REST and GraphQL](explanation/rest-and-graphql.md) | Explanation | Dual transport |
| [Coding notes](planning/coding-notes.md) | Planning | Implementers |
| [Contributor setup](contributing-setup.md) | How-to | Contributors |

## Related links

- [README](../README.md)
- [VERSIONING.md](../VERSIONING.md)
- [SUPPORT.md](../SUPPORT.md)
- [Octopus API documentation](https://docs.octopus.energy/)
