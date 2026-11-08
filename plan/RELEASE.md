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

Until NuGet **1.2.x** ships, `<Version>` in `src/OctopusEnergy.Client/OctopusEnergy.Client.csproj` stays on the latest **1.1.x** release while GraphQL customer APIs land on `main`. Consumers installing from NuGet see **1.1.0** only; GraphQL-backed public accessors require **`main`** or a future **1.2.x** tag (documented in [API coverage](../docs/reference/api-coverage.md)).

#### Source-breaking changes

- `Product.Direction`, `MeterRegister.Rate`, and `TariffCharge.PaymentMethod` are now `WireField<T>` (or nullable) instead of `string`. Use `.Value` for the enum (`ProductEnergyDirection`, `MeterRegisterRate`, `ProductPaymentMethod`) and `.UnparsedValue` when the API returns an unknown token.
- `ProductPaymentMethod` adds `Unknown` (additive enum member; existing numeric values unchanged).

#### 1.2.0 tag checklist

Complete this before tagging **1.2.0** (or the first **1.2.x** public GraphQL release):

**Documentation prep (can land before the tag):**

- [x] README *Status* and *Roadmap* state NuGet **1.1.0** vs GraphQL on `main`.
- [x] [API coverage](../docs/reference/api-coverage.md) explains NuGet vs source and clarifies REST account vs optional GraphQL account projection (#23).
- [x] Gas mutation: public SDK with allow-list **Not probed** documented ([allow-list](../docs/planning/graphql-customer-allow-list.md) *Deferred live verification*).

**Release gate (do at tag time):**

1. [ ] From the repository root, pass Release checks:

   ```bash
   dotnet format OctopusEnergy.slnx --verify-no-changes
   dotnet build OctopusEnergy.slnx -c Release -warnaserror
   dotnet test OctopusEnergy.slnx -c Release --no-build
   ```

2. [ ] Bump `<Version>` in `src/OctopusEnergy.Client/OctopusEnergy.Client.csproj` to **1.2.0** (or the intended **1.2.x** patch).
3. [ ] Add a **1.2.0 — GraphQL customer extras** section below; move every bullet out of *Unreleased* into that section with the release date.
4. [ ] Retitle [API coverage](../docs/reference/api-coverage.md): change *v1.2 GraphQL (in progress)* to shipped wording; update [README.md](../README.md) *Status* and *Roadmap* so NuGet **1.2.x** matches the package.
5. [ ] Confirm [graphql-customer-allow-list.md](../docs/planning/graphql-customer-allow-list.md) and the [API coverage](../docs/reference/api-coverage.md) customer-operations table agree on every **Public SDK** row (method names, probe status, planned vs implemented).
6. [ ] Optionally re-run live allow-list probes (`OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1` and `OCTOPUS_ENERGY_API_KEY`) and refresh fixtures when probe results change.
7. [ ] **Gas mutation (optional before tag):** re-probe `createGasMeterReading` on a gas-capable dashboard key and flip the allow-list row to **Allowed (mutation)**. **Deferred** when no such key is available (8 November 2026); shipping **1.2.0** without this step is acceptable if docs stay aligned.
8. [ ] Tag and push (`git tag v1.2.0 && git push origin v1.2.0`); complete [Publish steps](#publish-steps).

- **Internal GraphQL authentication (issue #20).** Authenticated clients obtain and cache a Kraken JWT via `obtainKrakenToken` (API key only), refresh using `refreshToken`, and retry GraphQL requests once on `KT-CT-1112`. `OctopusEnergyClient.GraphQl` remains `internal`. Status tables: [API coverage](../docs/reference/api-coverage.md#v12-graphql-in-progress).
- **GraphQL operational limits (issue #29).** `GraphQlRelayLimits`, Kraken exception types, `GraphQlRelayPageInfo`, and `client.RateLimits.GetRateLimitInfoAsync` (hourly points allowance). No automatic retry on `KT-CT-1199`.
- **Public GraphQL surface (v1.2).** `IOctopusEnergyClient.RateLimits` (`IRateLimitService`, `RateLimitInfo`, `PointsAllowanceRateLimit`) and `IOctopusEnergyClient.Viewer` (`IViewerService`, `ViewerDetail` with deduplicated `AccountNumbers` from `viewer` and portfolios). GraphQL failures with Kraken `extensions.errorCode` throw `OctopusEnergyKrakenException` (a subtype of `OctopusEnergyApiException`). `OctopusEnergyApiException` is no longer `sealed` so Kraken subtypes can be introduced; existing `catch (OctopusEnergyApiException)` handlers still catch GraphQL errors.
- **Meter readings (#24 electricity, #99 gas, #100 consent).** `IOctopusEnergyClient.MeterReadings` (`IMeterReadingService`) wraps `createElectricityMeterReading` (`ElectricityMeterReadingCreate`, `ElectricityMeterReadingResult`), `createGasMeterReading` (`GasMeterReadingCreate`, `GasMeterReadingResult`), and `readingConsentGranularity` (`GetReadingConsentGranularityAsync`, `ReadingConsentGranularity`) for dashboard API keys. Gas allow-list status remains **Not probed** on the maintainer account (no gas meter); see [graphql-customer-allow-list.md](../docs/planning/graphql-customer-allow-list.md).
- **Account billing (issue #23).** `IOctopusEnergyClient.AccountBilling` (`IAccountBillingService`, `GraphQlRelayPage<T>`) lists bills, ledger transactions, and payments via separate GraphQL queries (`ListBillsPageAsync`, `ListTransactionsPageAsync`, `ListPaymentsPageAsync`). Default page size `AccountBillingListDefaults.DefaultFirst` (25).
- **SmartFlex devices (issue #25).** `IOctopusEnergyClient.Devices` (`IDeviceService`, `SmartFlexDevice`, `FlexPlannedDispatch`) lists enrolled hardware via `devices(accountNumber:)` and planned windows via `flexPlannedDispatches(deviceId:)` only (not deprecated `plannedDispatches`). Empty device lists are returned without exception; unauthorised device ids surface as `OctopusEnergyKrakenException`.
- **Home Mini telemetry (issue #26).** `IOctopusEnergyClient.SmartMeterTelemetry` (`ISmartMeterTelemetryService`, `SmartMeterDevice`, `SmartMeterTelemetryReading`, `SmartMeterTelemetryRange`) discovers EUI64 ids via agreement `smartDevices` and reads `smartMeterTelemetry(deviceId:)`. Field rate limits (`KT-CT-1199`, documented 125/h) are not retried automatically.
- **Octoplus (issue #27).** `IOctopusEnergyClient.Octoplus` (`IOctoplusService`) wraps public-host `octoplusAccountInfo`, `loyaltyPointLedgers`, `loyaltyPointsBalance`, deprecated `wheelOfFortuneSpins`, `joinOctoplusCampaign`, and `redeemLoyaltyPointsForAccountCredit`. `savingSessions` is not on the public GraphQL schema (backend host is out of scope).
- **Product switch (issue #28).** `IOctopusEnergyClient.ProductSwitch` (`IProductSwitchService`, `ProductSwitchInitiate`, `ProductSwitchResult`) wraps `initiateProductSwitch` when the dashboard key has switch-product or renewal permission. Obtain quoted product or offer identifiers outside the SDK.
- **ProductsConsole sample.** Authenticated smoke exercises `Devices`, `SmartMeterTelemetry`, and `Octoplus` when `OCTOPUS_ENERGY_ACCOUNT_NUMBER` is set (device counts only; empty lists and `KT-CT-1111` are treated as successful probe paths).

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
- **EV device tariff rates (issue #101).** `ITariffRatesService.ListEvDeviceOffPeakUnitRatesAsync` and `ListEvDevicePeakUnitRatesAsync` wrap the official `ev-device-off-peak-unit-rates` and `ev-device-peak-unit-rates` REST paths for electricity tariffs.
- **Trim-safe REST JSON** — `System.Text.Json` source generation for all REST wire models used by `RestClient`, so file-based apps and hosts with `JsonSerializerIsReflectionEnabledByDefault=false` work without consumer MSBuild workarounds. Does not claim full Native AOT compatibility for the entire package (see issue #68).
