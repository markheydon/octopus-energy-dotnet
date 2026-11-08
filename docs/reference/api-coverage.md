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

## v1.2 GraphQL (in progress)

Curated GraphQL customer operations (milestone **v1.2**) are listed in [SCOPE.md](../../SCOPE.md). NuGet **1.2.x** when the public surface ships — see [release labels](../../VERSIONING.md#release-labels-quick-reference). Unreleased groundwork is documented in [plan/RELEASE.md](../../plan/RELEASE.md) (section *Unreleased (v1.2 groundwork)*).

**Public SDK surface:** REST for catalogue, account, and consumption; one GraphQL-backed operation (`RateLimits.GetRateLimitInfoAsync`) for operational limits. Additional curated GraphQL stories are in progress.

### Infrastructure (unreleased, internal)

| Capability | Status |
|---|---|
| GraphQL HTTP transport to the Kraken endpoint | Implemented (`internal`) |
| `obtainKrakenToken` with dashboard **API key only** (no email/password) | Implemented (`internal`) |
| JWT cache, refresh via `refreshToken`, `KT-CT-1112` retry | Implemented (`internal`) |
| Kraken `errorCode` mapping (`KT-CT-1188`, `1189`, `1199`, token errors) | Implemented (public exception types) |
| `GraphQlRelayLimits` and `ValidateFirst` | Implemented (public) |
| `OctopusEnergyClient.GraphQl` accessor | Implemented (`internal`) |

### Customer operations (planned public)

Runtime allow-list (8 November 2026): [graphql-customer-allow-list.md](../planning/graphql-customer-allow-list.md).

| Area | Allow-list status | Public SDK |
|---|---|---|
| `viewer` / multi-account | Allowed | Planned (#22) |
| `account` (minimal projection, meter discovery) | Allowed | Planned (#23) |
| `account` (bills, transactions, payments) | Not probed | Planned (#23) |
| Electricity meter reading mutation | Allowed (mutation) | Planned (#24) |
| Gas meter reading mutation | Not probed on spike account | Planned (#24) |
| `devices(accountNumber)` | Allowed (empty on probe account) | Planned (#25) |
| `flexPlannedDispatches` | Denied (`KT-CT-1111` on probe account) | Planned (#25) |
| `smartMeterTelemetry` | Denied (`KT-CT-1111` on probe account) | Planned (#26) |
| `loyaltyPointsBalance` (Octoplus proxy) | Denied (`KT-CT-1111` on probe account) | Planned (#27) |
| `initiateProductSwitch` | Allowed (mutation) | Planned (#28) |
| `rateLimitInfo` | Allowed | Implemented (`client.RateLimits`) |
| `readingConsentGranularity` | Allowed | Planned |

Keep this table aligned with [SCOPE.md](../../SCOPE.md), [plan/RELEASE.md](../../plan/RELEASE.md), and the [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md) (probe matrix is authoritative for **Allow-list status**). Update both documents in the same change when re-probing or shipping public GraphQL methods.

## Explicitly not covered

Partner quotes, account creation, data-import, and operations GraphQL. See [customer vs partner](../explanation/customer-vs-partner.md).
