# Release runbook

Maintainer guide for publishing `OctopusEnergy.Client` to NuGet.org. Policy: [VERSIONING.md](../VERSIONING.md).

## Prerequisites

- [ ] NuGet.org Trusted Publishing for `markheydon/octopus-energy-dotnet` and workflow `release.yml`, scoped to `OctopusEnergy.Client`
- [ ] `NUGET_USER` repository secret set
- [ ] `<Version>` in `src/OctopusEnergy.Client/OctopusEnergy.Client.csproj` matches the intended tag
- [ ] `main` is green on CI

## Publish steps

1. Bump `<Version>` if needed.
2. Merge to `main`.
3. Tag and push: `git tag v1.1.0 && git push origin v1.1.0`
4. Monitor the Release workflow.
5. Verify nuget.org and GitHub Releases.

## First stable release

**1.0.0** (18 September 2026) is the first stable release. It ships the REST customer core: products, tariffs, rates, GSP, account, consumption, pagination, typed errors, and API-key auth. Earlier `0.1.0-alpha.1` was a placeholder for CI pack validation only and must not be used in production.

For subsequent releases, bump `<Version>` in the csproj, merge to `main`, then tag and push (for example `v1.0.1` or `v1.1.0`).

## 1.1.0 — REST developer experience (21 October 2026)

Milestone v1.1. Hosting, testability, workflow helpers, consumption safety, API hygiene, trim-safe REST JSON, and consumer how-tos.

### Source-breaking changes

- `OctopusEnergyClient` resource properties (`Accounts`, `Consumption`, `Industry`, `Products`, `TariffRates`) now return interface types (`IAccountService`, and so on) instead of concrete service classes.
- Supplied `HttpClient` instances are no longer mutated (`DefaultRequestHeaders`, `BaseAddress`). Authentication, `Accept`, and `User-Agent` are applied per request instead. See [authentication](../docs/how-to/authentication.md).
- `TariffCode.GetRelativeChargePath` is obsolete; use `TariffRatesService` instead.
- `GridSupplyPointLookup.Gsp` is obsolete; use `GridSupplyPointLookup.GridSupplyPoint` instead (duplicate wire field).
- `OctopusEnergyParseException` is thrown for successful HTTP responses that cannot be deserialised (replacing `OctopusEnergyException` for that case). Callers catching `OctopusEnergyException` still work.
- `OctopusEnergyTime.AssumeEuropeLondon` now throws for spring-forward gap times and resolves ambiguous autumn-back hours to standard time (GMT). Previously, gap times were accepted with an incorrect offset.
- `ConsumptionPricePeriodMatching` now matches by rate period containing the interval start (`[ValidFrom, ValidTo)`), not exact `ValidFrom == IntervalStart`. This avoids silently dropping overlapping consumption intervals; the matched rate may use a different offset representation from `IntervalStart`. Matching uses interval start only (not full interval overlap). When multiple rates contain the start, the latest `ValidFrom` wins; equal `ValidFrom` ties prefer the first rate in the supplied list (previously duplicate keys in the internal dictionary kept the last rate).

### Unreleased (v1.2 groundwork)

Until NuGet **1.2.x** ships, `<Version>` in `src/OctopusEnergy.Client/OctopusEnergy.Client.csproj` stays on the latest **1.1.x** release while GraphQL customer APIs land on `main`. When you tag **1.2.0** (or the first **1.2.x** public GraphQL release):

1. Bump `<Version>` and add a **1.2.0** section here; move the bullets below out of *Unreleased*.
2. Retitle [API coverage](../docs/reference/api-coverage.md) *v1.2 GraphQL (in progress)* and related consumer wording to match the shipped package.
3. Confirm [graphql-customer-allow-list.md](../docs/planning/graphql-customer-allow-list.md) and the coverage table still agree.

- **Internal GraphQL authentication (issue #20).** Authenticated clients obtain and cache a Kraken JWT via `obtainKrakenToken` (API key only), refresh using `refreshToken`, and retry GraphQL requests once on `KT-CT-1112`. `OctopusEnergyClient.GraphQl` remains `internal`. Status tables: [API coverage](../docs/reference/api-coverage.md#v12-graphql-in-progress).
- **GraphQL operational limits (issue #29).** `GraphQlRelayLimits`, Kraken exception types, `GraphQlRelayPageInfo`, and `client.RateLimits.GetRateLimitInfoAsync` (hourly points allowance). No automatic retry on `KT-CT-1199`.
- **Public GraphQL surface (v1.2).** `IOctopusEnergyClient.RateLimits` (`IRateLimitService`, `RateLimitInfo`, `PointsAllowanceRateLimit`) and `IOctopusEnergyClient.Viewer` (`IViewerService`, `ViewerDetail` with deduplicated `AccountNumbers` from `viewer` and portfolios). GraphQL failures with Kraken `extensions.errorCode` throw `OctopusEnergyKrakenException` (a subtype of `OctopusEnergyApiException`). `OctopusEnergyApiException` is no longer `sealed` so Kraken subtypes can be introduced; existing `catch (OctopusEnergyApiException)` handlers still catch GraphQL errors.
- **Electricity meter readings (issue #24, partial).** `IOctopusEnergyClient.MeterReadings` (`IMeterReadingService`, `ElectricityMeterReadingCreate`, `ElectricityMeterReadingResult`) wraps `createElectricityMeterReading` for dashboard API keys. Gas remains on the same story (#24) and is unshipped pending allow-list re-probe on an account with gas.
- **Account billing (issue #23).** `IOctopusEnergyClient.AccountBilling` (`IAccountBillingService`, `GraphQlRelayPage<T>`) lists bills, ledger transactions, and payments via separate GraphQL queries (`ListBillsPageAsync`, `ListTransactionsPageAsync`, `ListPaymentsPageAsync`). Default page size `AccountBillingListDefaults.DefaultFirst` (25).
- **SmartFlex devices (issue #25).** `IOctopusEnergyClient.Devices` (`IDeviceService`, `SmartFlexDevice`, `FlexPlannedDispatch`) lists enrolled hardware via `devices(accountNumber:)` and planned windows via `flexPlannedDispatches(deviceId:)` only (not deprecated `plannedDispatches`). Empty device lists are returned without exception; unauthorised device ids surface as `OctopusEnergyKrakenException`.
- **Home Mini telemetry (issue #26).** `IOctopusEnergyClient.SmartMeterTelemetry` (`ISmartMeterTelemetryService`, `SmartMeterDevice`, `SmartMeterTelemetryReading`, `SmartMeterTelemetryRange`) discovers EUI64 ids via agreement `smartDevices` and reads `smartMeterTelemetry(deviceId:)`. Field rate limits (`KT-CT-1199`, documented 125/h) are not retried automatically.
- **Octoplus (issue #27).** `IOctopusEnergyClient.Octoplus` (`IOctoplusService`) wraps public-host `octoplusAccountInfo`, `loyaltyPointLedgers`, `loyaltyPointsBalance`, deprecated `wheelOfFortuneSpins`, `joinOctoplusCampaign`, and `redeemLoyaltyPointsForAccountCredit`. `savingSessions` is not on the public GraphQL schema (backend host is out of scope).
- **ProductsConsole sample.** Authenticated smoke exercises `Devices` and `SmartMeterTelemetry` when `OCTOPUS_ENERGY_ACCOUNT_NUMBER` is set (device counts only; empty lists and `KT-CT-1111` are treated as successful probe paths).

### Additive changes

- **HTTP 429/503 retry (enabled by default).** REST calls retry transient `429 Too Many Requests` and `503 Service Unavailable` responses up to three times after the first failure, honouring `Retry-After` when present (capped at 60 seconds) or using exponential backoff otherwise. Pass `OctopusEnergyRetryOptions.Disabled` to any `OctopusEnergyClient` constructor for fail-fast behaviour. Disable retries when timing or custom Polly policies matter.
- `OctopusEnergyRetryOptions` — configure retry attempts, base delay, and maximum delay for REST transport.
- `IOctopusEnergyClient` and resource service interfaces for testability.
- `OctopusEnergyClientHandler` for `IHttpClientFactory` registration.
- Workflow helpers: `ParsedTariffCode`, meter-point consumption overloads, `ProductDetail.TryGetTariff`.
- `PaginatedResult<T>` — public type for a single REST list page (`Count`, `Next`, `Previous`, `Results`).
- `ListElectricityPageAsync` / `ListGasPageAsync` on `IConsumptionService` — fetch one consumption page and read total `count` without auto-pagination.
- `RestPageSizeLimits.Validate` — rejects `page_size` less than 1 for consumption and tariff rate requests (previously sent to the API).
- Consumer how-tos: [consumption](../docs/how-to/consumption.md), [tariff rates](../docs/how-to/tariff-rates.md), [industry lookups](../docs/how-to/industry.md).
- **Trim-safe REST JSON** — `System.Text.Json` source generation for all REST wire models used by `RestClient`, so file-based apps and hosts with `JsonSerializerIsReflectionEnabledByDefault=false` work without consumer MSBuild workarounds. Does not claim full Native AOT compatibility for the entire package (see issue #68).
