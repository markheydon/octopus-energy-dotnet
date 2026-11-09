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

## 1.2.0 — GraphQL customer extras (9 November 2026)

Milestone **v1.2**: curated GraphQL-backed customer APIs on the same `OctopusEnergyClient`, typed REST wire fields, dependency injection registration, and trim-safe GraphQL JSON.

### Source-breaking changes

- `Product.Direction`, `MeterRegister.Rate`, and `TariffCharge.PaymentMethod` are now `WireField<T>` (or nullable) instead of `string`. Use `.Value` for the enum (`ProductEnergyDirection`, `MeterRegisterRate`, `ProductPaymentMethod`) and `.UnparsedValue` when the API returns an unknown token.
- `ProductPaymentMethod` adds `Unknown` (additive enum member; existing numeric values unchanged).

### Features

- **Internal GraphQL authentication (issue #20).** Authenticated clients obtain and cache a Kraken JWT via `obtainKrakenToken` (API key only), refresh using `refreshToken`, and retry GraphQL requests once on `KT-CT-1112`. `OctopusEnergyClient.GraphQl` remains `internal`. Status tables: [API coverage](../docs/reference/api-coverage.md#v12-graphql-shipped).
- **GraphQL operational limits (issue #29).** `GraphQlRelayLimits`, Kraken exception types, `GraphQlRelayPageInfo`, and `client.RateLimits.GetRateLimitInfoAsync` (hourly points allowance). No automatic retry on `KT-CT-1199`.
- **Public GraphQL surface (v1.2).** `IOctopusEnergyClient.RateLimits` (`IRateLimitService`, `RateLimitInfo`, `PointsAllowanceRateLimit`) and `IOctopusEnergyClient.Viewer` (`IViewerService`, `ViewerDetail` with deduplicated `AccountNumbers` from `viewer` and portfolios). GraphQL failures with Kraken `extensions.errorCode` throw `OctopusEnergyKrakenException` (a subtype of `OctopusEnergyApiException`). `OctopusEnergyApiException` is no longer `sealed` so Kraken subtypes can be introduced; existing `catch (OctopusEnergyApiException)` handlers still catch GraphQL errors.
- **Meter readings (#24 electricity, #99 gas, #100 consent).** `IOctopusEnergyClient.MeterReadings` (`IMeterReadingService`) wraps `createElectricityMeterReading` (`ElectricityMeterReadingCreate`, `ElectricityMeterReadingResult`), `createGasMeterReading` (`GasMeterReadingCreate`, `GasMeterReadingResult`), and `readingConsentGranularity` (`GetReadingConsentGranularityAsync`, `ReadingConsentGranularity`) for dashboard API keys. Gas allow-list status remains **Not probed** on the maintainer account (no gas meter); see [graphql-customer-allow-list.md](../docs/planning/graphql-customer-allow-list.md).
- **Account billing (issue #23).** `IOctopusEnergyClient.AccountBilling` (`IAccountBillingService`, `GraphQlRelayPage<T>`) lists bills, ledger transactions, and payments via separate GraphQL queries (`ListBillsPageAsync`, `ListTransactionsPageAsync`, `ListPaymentsPageAsync`). Default page size `AccountBillingListDefaults.DefaultFirst` (25).
- **SmartFlex devices (issue #25).** `IOctopusEnergyClient.Devices` (`IDeviceService`, `SmartFlexDevice`, `FlexPlannedDispatch`) lists enrolled hardware via `devices(accountNumber:)` and planned windows via `flexPlannedDispatches(deviceId:)` only (not deprecated `plannedDispatches`). Empty device lists are returned without exception; unauthorised device ids surface as `OctopusEnergyKrakenException`.
- **Home Mini telemetry (issue #26).** `IOctopusEnergyClient.SmartMeterTelemetry` (`ISmartMeterTelemetryService`, `SmartMeterDevice`, `SmartMeterTelemetryReading`, `SmartMeterTelemetryRange`) discovers EUI64 ids via agreement `smartDevices` and reads `smartMeterTelemetry(deviceId:)`. Field rate limits (`KT-CT-1199`, documented 125/h) are not retried automatically.
- **Octoplus (issue #27).** `IOctopusEnergyClient.Octoplus` (`IOctoplusService`) wraps public-host `octoplusAccountInfo`, `loyaltyPointLedgers`, `loyaltyPointsBalance`, deprecated `wheelOfFortuneSpins`, `joinOctoplusCampaign`, and `redeemLoyaltyPointsForAccountCredit`. `savingSessions` is not on the public GraphQL schema (backend host is out of scope).
- **Product switch (issue #28).** `IOctopusEnergyClient.ProductSwitch` (`IProductSwitchService`, `ProductSwitchInitiate`, `ProductSwitchResult`) wraps `initiateProductSwitch` when the dashboard key has switch-product or renewal permission. Obtain quoted product or offer identifiers outside the SDK.
- **ProductsConsole sample.** Authenticated smoke exercises `Devices`, `SmartMeterTelemetry`, and `Octoplus` when `OCTOPUS_ENERGY_ACCOUNT_NUMBER` is set (device counts only; empty lists and `KT-CT-1111` are treated as successful probe paths). Electricity and gas meter reading **mutations** run only when `OCTOPUS_ENERGY_ENABLE_METER_READING_MUTATIONS=1` (see [samples/README.md](../samples/README.md)); Kraken query vs mutation outcomes are recorded separately in smoke output.
- **GraphQL JSON source generation.** GraphQL request envelopes and service response DTOs deserialize via `OctopusEnergyJsonContext` (no reflection fallback in the client assembly). `GraphQlClient.ExecuteAsync<TData>` / `ExecuteAsync<TData,TVariables>` replace untyped variable bags; `RestClient` uses the same context-only deserializer for API error bodies.
- **Dependency injection.** `AddOctopusEnergyClient(IServiceCollection, Action<OctopusEnergyClientOptions>)` registers a singleton `IOctopusEnergyClient` with `OctopusEnergyClientHandler` (API key on outbound requests). Documented in [authentication](../docs/how-to/authentication.md).
- **Tracker tariff daily pricing.** No public GraphQL SDK wrapper until allow-list live probe; use REST tracker URLs or tariff rate lists ([coding notes](../docs/planning/coding-notes.md)).
- **Trim-safe GraphQL JSON** — GraphQL envelopes and service DTOs share `OctopusEnergyJsonContext` with REST; the client assembly does not fall back to reflection. Does not claim full Native AOT compatibility for the entire package (see issue #68).

### 1.2.0 tag checklist (completed 9 November 2026)

**Documentation prep:**

- [x] README *Status* and *Roadmap* state NuGet **1.2.x** after release.
- [x] [API coverage](../docs/reference/api-coverage.md) explains NuGet vs source and clarifies REST account vs optional GraphQL account projection (#23).
- [x] Gas mutation: public SDK with allow-list **Not probed** documented ([allow-list](../docs/planning/graphql-customer-allow-list.md) *Deferred live verification*).
- [x] Consumer docs for `WireField<T>` and version guidance ([VERSIONING.md](../VERSIONING.md), [API coverage](../docs/reference/api-coverage.md), [NuGet package README](../src/OctopusEnergy.Client/README.md)).

**Release gate:**

1. [x] Release checks passed (`dotnet format`, `build -c Release -warnaserror`, `test -c Release --no-build`).
2. [x] `<Version>` **1.2.0** and package description updated in `OctopusEnergy.Client.csproj`.
3. [x] **1.2.0 — GraphQL customer extras** section in this file.
4. [x] [API coverage](../docs/reference/api-coverage.md) and [README.md](../README.md) updated for NuGet **1.2.0**.
5. [x] [graphql-customer-allow-list.md](../docs/planning/graphql-customer-allow-list.md) aligned with API coverage **Public SDK** rows.
6. [ ] Optional live allow-list re-probe (not run for this tag).
7. [x] Gas mutation live re-probe **deferred** (no gas-capable dashboard key; docs aligned).
8. [ ] Tag `v1.2.0` and [Publish steps](#publish-steps).
