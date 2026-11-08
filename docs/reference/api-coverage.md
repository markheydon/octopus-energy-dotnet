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
| Unit rates / standing charges | REST tariff charge URLs (`standing-charges`, `standard-unit-rates`, `day-unit-rates`, `night-unit-rates`, `ev-device-off-peak-unit-rates`, `ev-device-peak-unit-rates`) | Implemented (`TariffRatesService`) |
| Grid supply points | REST `/v1/industry/grid-supply-points/` | Implemented (`IndustryService`) |
| Electricity meter-point | REST `/v1/electricity-meter-points/{mpan}/` | Implemented (`IndustryService`) |
| Account | REST `/v1/accounts/{number}/` | Implemented |
| Consumption | REST electricity/gas consumption | Implemented (`ConsumptionService`) |

**Typed wire enums (NuGet 1.2.0, on `main` before tag):** `Product.Direction` (`WireField<ProductEnergyDirection>?`), `MeterRegister.Rate` (`WireField<MeterRegisterRate>`), and `TariffCharge.PaymentMethod` (`WireField<ProductPaymentMethod>?`) replace raw `string` properties from **1.1.0**. Unknown wire tokens deserialize without throwing; read `UnparsedValue` when `Value` is `Unknown`. See [products](../how-to/products.md), [accounts](../how-to/accounts.md), [tariff rates](../how-to/tariff-rates.md), and [plan/RELEASE.md](../../plan/RELEASE.md).

How-to guides: [consumption](../how-to/consumption.md), [tariff rates](../how-to/tariff-rates.md), [industry lookups](../how-to/industry.md), [meter readings](../how-to/meter-readings.md), [account billing](../how-to/account-billing.md), [SmartFlex devices](../how-to/devices.md), [smart-meter telemetry](../how-to/smart-meter-telemetry.md), [Octoplus](../how-to/octoplus.md). See also [getting started](../tutorial/getting-started.md), [REST and GraphQL](../explanation/rest-and-graphql.md), and [units, VAT, and time](../explanation/units-vat-and-time.md).

## v1.2 GraphQL (in progress)

Curated GraphQL customer operations (milestone **v1.2**) are listed in [SCOPE.md](../../SCOPE.md). NuGet **1.2.x** when the public surface ships — see [release labels](../../VERSIONING.md#release-labels-quick-reference). Unreleased groundwork is documented in [plan/RELEASE.md](../../plan/RELEASE.md) (section *Unreleased (v1.2 groundwork)*).

**NuGet vs source:** The latest published package on NuGet.org is **1.1.0** (REST developer experience). The v1.2 GraphQL-backed accessors below are implemented on `main` and ship in the next **1.2.x** release after the [1.2.0 tag checklist](../../plan/RELEASE.md#120-tag-checklist). Build from source or wait for that tag to consume them from NuGet.

**Public SDK surface:** REST for catalogue, account, and consumption; GraphQL-backed viewer discovery (`Viewer.GetViewerAsync`), operational limits (`RateLimits.GetRateLimitInfoAsync`), meter readings (`MeterReadings.CreateElectricityAsync`, `MeterReadings.CreateGasAsync`, `MeterReadings.GetReadingConsentGranularityAsync`), account billing lists (`AccountBilling.ListBillsPageAsync`, `ListTransactionsPageAsync`, `ListPaymentsPageAsync`), SmartFlex devices (`Devices.ListDevicesAsync`, `Devices.ListFlexPlannedDispatchesAsync`), Home Mini telemetry (`SmartMeterTelemetry.ListSmartDevicesAsync`, `SmartMeterTelemetry.GetTelemetryAsync`), Octoplus helpers (`Octoplus.GetAccountInfoAsync`, `ListLoyaltyPointLedgersAsync`, `GetLoyaltyPointsBalanceAsync`, `GetWheelOfFortuneSpinsAsync`, `JoinCampaignAsync`, `RedeemPointsForAccountCreditAsync`), and product switch (`ProductSwitch.InitiateAsync`). The curated v1.2 customer surface above is implemented on `main`; optional follow-ups are a dedicated GraphQL account projection ([#23](https://github.com/markheydon/octopus-energy-dotnet/issues/23)) and a live allow-list re-probe for gas meter readings when a gas-capable dashboard key is available.

### Infrastructure (unreleased, internal)

| Capability | Status |
|---|---|
| GraphQL HTTP transport to the Kraken endpoint | Implemented (`internal`) |
| `obtainKrakenToken` with dashboard **API key only** (no email/password) | Implemented (`internal`) |
| JWT cache, refresh via `refreshToken`, `KT-CT-1112` retry | Implemented (`internal`) |
| Kraken `errorCode` mapping (`KT-CT-1188`, `1189`, `1199`, token errors) | Implemented (public exception types) |
| `GraphQlRelayLimits` and `ValidateFirst` | Implemented (public) |
| `OctopusEnergyClient.GraphQl` accessor | Implemented (`internal`) |

### Customer operations (implemented on `main`; NuGet **1.2.x** when tagged)

Runtime allow-list (8 November 2026): [graphql-customer-allow-list.md](../planning/graphql-customer-allow-list.md).

| Area | Allow-list status | Public SDK |
|---|---|---|
| `viewer` / multi-account | Allowed | Implemented (`ViewerService`) |
| `account` (minimal GraphQL projection) | Allowed | **Not a separate public service** — use REST `Accounts.GetAsync`. A dedicated GraphQL account projection remains optional future work (#23). |
| Account meter discovery (GraphQL fields) | Allowed | **Not a separate public service** — MPAN, MPRN, and serials from REST `Accounts.GetAsync` today. GraphQL-only discovery fields are optional future work (#23). |
| `account` → `bills` | Allowed | Implemented (#23, billing — `AccountBilling.ListBillsPageAsync`) |
| `account` → `transactions` | Allowed | Implemented (#23, billing — `AccountBilling.ListTransactionsPageAsync`) |
| `account` → `payments` | Allowed | Implemented (#23, billing — `AccountBilling.ListPaymentsPageAsync`) |
| Electricity meter reading mutation | Allowed (mutation) | Implemented (`MeterReadings.CreateElectricityAsync`) |
| Gas meter reading mutation | Not probed — **deferred** until a gas-capable dashboard key (schema + fixtures only today) | Implemented (`MeterReadings.CreateGasAsync`, #99); allow-list row unchanged until live re-probe |
| `devices(accountNumber)` | Allowed (empty on probe account) | Implemented (#25 — `Devices.ListDevicesAsync`) |
| `flexPlannedDispatches` | Allowed when entitled (`KT-CT-1111` without enrolled device) | Implemented (#25 — `Devices.ListFlexPlannedDispatchesAsync`) |
| `smartMeterTelemetry` | Allowed when entitled (`KT-CT-1111` without EUI64) | Implemented (#26 — `SmartMeterTelemetry.GetTelemetryAsync`, `ListSmartDevicesAsync`) |
| `loyaltyPointsBalance` | Denied unentitled (`KT-CT-1111` on probe account) | Implemented (#27 — `Octoplus.GetLoyaltyPointsBalanceAsync`) |
| `loyaltyPointLedgers` | Denied unentitled (expected on probe account) | Implemented (#27 — `Octoplus.ListLoyaltyPointLedgersAsync`) |
| `octoplusAccountInfo` | Allowed when entitled | Implemented (#27 — `Octoplus.GetAccountInfoAsync`) |
| `wheelOfFortuneSpins` | Allowed when entitled (deprecated field) | Implemented (#27 — `Octoplus.GetWheelOfFortuneSpinsAsync`) |
| `joinOctoplusCampaign` | Allowed (mutation) | Implemented (#27 — `Octoplus.JoinCampaignAsync`) |
| `redeemLoyaltyPointsForAccountCredit` | Allowed (mutation) | Implemented (#27 — `Octoplus.RedeemPointsForAccountCreditAsync`) |
| `savingSessions` / join (backend host) | Not on public GraphQL schema | Out of scope — see allow-list |
| `initiateProductSwitch` | Allowed (mutation) | Implemented (#28 — `ProductSwitch.InitiateAsync`) |
| `rateLimitInfo` | Allowed | Implemented (`client.RateLimits`) |
| `readingConsentGranularity` | Allowed | Implemented (`MeterReadings.GetReadingConsentGranularityAsync`, #100) |

Keep this table aligned with [SCOPE.md](../../SCOPE.md), [plan/RELEASE.md](../../plan/RELEASE.md), and the [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md) (probe matrix is authoritative for **Allow-list status**). Update both documents in the same change when re-probing or shipping public GraphQL methods.

## Explicitly not covered

Partner quotes, account creation, data-import, and operations GraphQL. See [customer vs partner](../explanation/customer-vs-partner.md).
