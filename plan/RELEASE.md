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
3. Tag and push: `git tag v1.0.0 && git push origin v1.0.0`
4. Monitor the Release workflow.
5. Verify nuget.org and GitHub Releases.

## First stable release

**1.0.0** (18 September 2026) is the first stable release. It ships the REST customer core: products, tariffs, rates, GSP, account, consumption, pagination, typed errors, and API-key auth. Earlier `0.1.0-alpha.1` was a placeholder for CI pack validation only and must not be used in production.

For subsequent releases, bump `<Version>` in the csproj, merge to `main`, then tag and push (for example `v1.0.1` or `v1.1.0`).

## Unreleased breaking changes

The client interface work (#54) includes source-breaking API changes for the next minor release:

- `OctopusEnergyClient` resource properties (`Accounts`, `Consumption`, `Industry`, `Products`, `TariffRates`) now return interface types (`IAccountService`, and so on) instead of concrete service classes.
- Supplied `HttpClient` instances are no longer mutated (`DefaultRequestHeaders`, `BaseAddress`). Authentication, `Accept`, and `User-Agent` are applied per request instead. See [authentication](../docs/how-to/authentication.md).
- `TariffCode.GetRelativeChargePath` is obsolete; use `TariffRatesService` instead.
- `GridSupplyPointLookup.Gsp` is obsolete; use `GridSupplyPointLookup.GridSupplyPoint` instead (duplicate wire field).
- `OctopusEnergyParseException` is thrown for successful HTTP responses that cannot be deserialised (replacing `OctopusEnergyException` for that case). Callers catching `OctopusEnergyException` still work.
- `OctopusEnergyTime.AssumeEuropeLondon` now throws for spring-forward gap times and resolves ambiguous autumn-back hours to standard time (GMT). Previously, gap times were accepted with an incorrect offset.

## Unreleased additive changes

- `PaginatedResult<T>` — public type for a single REST list page (`Count`, `Next`, `Previous`, `Results`).
- `ListElectricityPageAsync` / `ListGasPageAsync` on `IConsumptionService` — fetch one consumption page and read total `count` without auto-pagination.
- `RestPageSizeLimits.Validate` — rejects `page_size` less than 1 for consumption and tariff rate requests (previously sent to the API).
