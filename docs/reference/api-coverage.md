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
| Tariff rates service (`TariffRatesService`; `TariffCode.GetRelativeChargePath` obsolete) | Implemented |
| Product catalogue lookup helpers (`ProductDetail.TryGetTariff`, `ParsedTariffCode`) | Implemented |
| Consumption meter-point overloads (`ConsumptionService`) | Implemented |
| Consumption–rate join helper (`ConsumptionPricePeriodMatching`; overlap/containment match) | Implemented |

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

How-to guides: [consumption](../how-to/consumption.md), [tariff rates](../how-to/tariff-rates.md), [industry lookups](../how-to/industry.md). See also [getting started](../tutorial/getting-started.md) and [units, VAT, and time](../explanation/units-vat-and-time.md).

## v2 (planned)

GraphQL customer operations listed in [SCOPE.md](../../SCOPE.md). None started.

## Explicitly not covered

Partner quotes, account creation, data-import, and operations GraphQL. See [customer vs partner](../explanation/customer-vs-partner.md).
