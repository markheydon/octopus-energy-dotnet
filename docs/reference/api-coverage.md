# API coverage

Nothing is implemented yet except a placeholder `OctopusEnergyClient` type.

## v1 (planned)

| Area | Upstream | SDK |
|---|---|---|
| Products | REST `/v1/products/` | Not started |
| Product / tariffs | REST `/v1/products/{code}/` | Not started |
| Unit rates / standing charges | REST tariff charge URLs | Not started |
| Grid supply points | REST `/v1/industry/grid-supply-points/` | Not started |
| Electricity meter-point | REST `/v1/electricity-meter-points/{mpan}/` | Not started |
| Account | REST `/v1/accounts/{number}/` | Not started |
| Consumption | REST electricity/gas consumption | Not started |

## v2 (planned)

GraphQL customer operations listed in [SCOPE.md](../../SCOPE.md). None started.

## Explicitly not covered

Partner quotes, account creation, data-import, and operations GraphQL. See [customer vs partner](../explanation/customer-vs-partner.md).
