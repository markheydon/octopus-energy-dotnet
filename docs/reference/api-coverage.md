# API coverage

## Infrastructure (implemented)

| Capability | Status |
|---|---|
| REST HTTP transport | Implemented |
| REST pagination (`next` following) | Implemented |
| Typed exception hierarchy | Implemented |
| API-key HTTP Basic authentication | Implemented |
| Configurable base URL (default UK host) | Implemented |
| `DateTimeOffset` models; Europe/London default (`OctopusEnergyTime`) | Implemented |
| Tariff code parse/format (`TariffCode`) | Implemented |
| GSP group id mapping (`GridSupplyPoint`, `GridSupplyPointParser`) | Implemented |
| Charge list relative paths (`TariffCode.GetRelativeChargePath`) | Implemented |
| Consumption–rate join helper (`ConsumptionPricePeriodMatching`) | Implemented |

## v1 resources

| Area | Upstream | SDK |
|---|---|---|
| Products | REST `/v1/products/` | Implemented |
| Product / tariffs | REST `/v1/products/{code}/` | Implemented |
| Unit rates / standing charges | REST tariff charge URLs | Implemented (`TariffRatesService`) |
| Grid supply points | REST `/v1/industry/grid-supply-points/` | Implemented (`IndustryService`) |
| Electricity meter-point | REST `/v1/electricity-meter-points/{mpan}/` | Implemented (`IndustryService`) |
| Account | REST `/v1/accounts/{number}/` | Implemented |
| Consumption | REST electricity/gas consumption | Implemented (`ConsumptionService`) |

Dedicated how-to guides for consumption, tariff rates, and industry lookups are still to follow. See [getting started](../tutorial/getting-started.md) and [units, VAT, and time](../explanation/units-vat-and-time.md).

## v2 (planned)

GraphQL customer operations listed in [SCOPE.md](../../SCOPE.md). None started.

## Explicitly not covered

Partner quotes, account creation, data-import, and operations GraphQL. See [customer vs partner](../explanation/customer-vs-partner.md).
