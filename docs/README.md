# OctopusEnergy.Client documentation

Documentation in this folder is for **developers using the NuGet package** - except `planning/`, which is for SDK implementers.

Internal policy: repository root ([`GOALS.md`](https://github.com/markheydon/octopus-energy-dotnet/blob/main/GOALS.md), [`SCOPE.md`](https://github.com/markheydon/octopus-energy-dotnet/blob/main/SCOPE.md), [`CONVENTIONS.md`](https://github.com/markheydon/octopus-energy-dotnet/blob/main/CONVENTIONS.md)) and [`plan/`](https://github.com/markheydon/octopus-energy-dotnet/tree/main/plan). ADRs: [`adr/`](https://github.com/markheydon/octopus-energy-dotnet/tree/main/adr).

## How we write consumer docs

Content follows the [Diátaxis](https://diataxis.fr/) framework:

- **Tutorials** (`tutorial/`) - learning-oriented; one successful path for newcomers.
- **How-to guides** (`how-to/`) - problem-oriented recipes (pagination, auth, a resource).
- **Reference** (`reference/`) - what the SDK implements today.
- **Explanation** (`explanation/`) - understanding-oriented (units, VAT, BST, Agile 16:00, customer vs partner).

README files (repository root and NuGet package) are short entry points with links into `docs/`. Implementer research and maxima stay in [coding notes](planning/coding-notes.md); consumer pages restate what callers need without duplicating the full planning dump. Partner APIs stay out of scope. GraphQL consumer docs ride with v2; v1 is REST.

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
| [Units, VAT, and time](explanation/units-vat-and-time.md) | Explanation | kWh, pence, VAT, BST, Agile 16:00 |
| [Customer vs partner](explanation/customer-vs-partner.md) | Explanation | Why the public surface is small |
| [REST and GraphQL](explanation/rest-and-graphql.md) | Explanation | Dual transport |
| [Coding notes](planning/coding-notes.md) | Planning | Implementers |
| [Contributor setup](contributing-setup.md) | How-to | Contributors |

## Related links

- [Repository README](https://github.com/markheydon/octopus-energy-dotnet/blob/main/README.md)
- [VERSIONING.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/VERSIONING.md)
- [SUPPORT.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/SUPPORT.md)
- [Octopus API documentation](https://docs.octopus.energy/)
