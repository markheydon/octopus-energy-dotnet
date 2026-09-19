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

## 1.1.0 — REST developer experience (19 September 2026)

Milestone v1.1. Hosting, testability, workflow helpers, consumption safety, API hygiene, and consumer how-tos.

### Source-breaking changes

- `OctopusEnergyClient` resource properties (`Accounts`, `Consumption`, `Industry`, `Products`, `TariffRates`) now return interface types (`IAccountService`, and so on) instead of concrete service classes.
- Supplied `HttpClient` instances are no longer mutated (`DefaultRequestHeaders`, `BaseAddress`). Authentication, `Accept`, and `User-Agent` are applied per request instead. See [authentication](../docs/how-to/authentication.md).
- `TariffCode.GetRelativeChargePath` is obsolete; use `TariffRatesService` instead.
- `GridSupplyPointLookup.Gsp` is obsolete; use `GridSupplyPointLookup.GridSupplyPoint` instead (duplicate wire field).
- `OctopusEnergyParseException` is thrown for successful HTTP responses that cannot be deserialised (replacing `OctopusEnergyException` for that case). Callers catching `OctopusEnergyException` still work.
- `OctopusEnergyTime.AssumeEuropeLondon` now throws for spring-forward gap times and resolves ambiguous autumn-back hours to standard time (GMT). Previously, gap times were accepted with an incorrect offset.
- `ConsumptionPricePeriodMatching` now matches by rate period containing the interval start (`[ValidFrom, ValidTo)`), not exact `ValidFrom == IntervalStart`. This avoids silently dropping overlapping consumption intervals; the matched rate may use a different offset representation from `IntervalStart`. Matching uses interval start only (not full interval overlap). When multiple rates contain the start, the latest `ValidFrom` wins; equal `ValidFrom` ties prefer the first rate in the supplied list (previously duplicate keys in the internal dictionary kept the last rate).

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
