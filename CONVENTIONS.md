> This file defines coding and design conventions for the OctopusEnergy.Client SDK.

# Conventions

**Project:** OctopusEnergy.Client
**Last updated:** 8 November 2026

When in doubt, follow this file. To change a convention, update it and add an ADR for significant architecture changes.

---

## Project Structure

```
src/
└── OctopusEnergy.Client/
    ├── OctopusEnergyClient.cs           # Public entry point
    ├── OctopusEnergyException.cs        # Public exception hierarchy (root)
    ├── OctopusEnergy*Exception.cs
    ├── RestPageSizeLimits.cs            # Public contract limits (pagination)
    ├── TariffCode.cs                    # Tariff code parse/format and charge paths
    ├── GridSupplyPoint.cs               # UK distribution region (GSP)
    ├── GridSupplyPointParser.cs         # group_id (_C) and letter mapping
    ├── EnergyFuel.cs / TariffRegisterKind.cs / TariffChargeKind.cs
    ├── Infrastructure/                  # Internal plumbing
    │   ├── Authentication/
    │   ├── Configuration/
    │   ├── Http/                        # REST transport and pagination internals
    │   ├── GraphQL/                     # Kraken token obtain/refresh and GraphQL transport (v1.2)
    │   └── Serialization/                 # JSON converters (for example GridSupplyPointJsonConverter)
    ├── Models/                          # Resource-grouped models
    │   ├── Accounts/                    # Account, property, meter models
    │   ├── Common/                      # Shared wire types (pagination, errors)
    │   ├── Consumption/                 # Consumption interval and list request models
    │   ├── Industry/                    # GSP and MPAN lookup models
    │   ├── Products/                    # Product catalogue and tariff charge models
    │   ├── AccountBilling/              # GraphQL bills, transactions, payments
    │   ├── MeterReadings/               # GraphQL meter reading models
    │   ├── RateLimits/                  # GraphQL rate-limit query models
    │   └── Viewer/                      # GraphQL viewer discovery models
    └── Services/                        # Resource-oriented services
        ├── Accounts/                    # AccountService
        ├── Consumption/                 # ConsumptionService
        ├── Industry/                    # IndustryService
        ├── Products/                    # ProductService, TariffRatesService
        ├── AccountBilling/              # AccountBillingService (GraphQL-backed)
        ├── MeterReadings/               # MeterReadingService (GraphQL-backed)
        ├── RateLimits/                  # RateLimitService (GraphQL-backed)
        └── Viewer/                      # ViewerService (GraphQL-backed)

tests/
└── OctopusEnergy.Client.Tests/
    ├── OctopusEnergyClientTests.cs
    ├── Infrastructure/
    ├── Services/
    └── TestSupport/                     # Recorded fixtures (no live keys in CI)

samples/
├── FileBasedProducts/                   # File-based quick-start smoke (CI)
└── ProductsConsole/                     # Opt-in live API smoke (not run in CI)
```

Place every **public** type in the `OctopusEnergy.Client` namespace at the project root (one type per file). Subfolders group internal code and resource-specific models or services; folder names do not have to mirror namespaces. Optional physical subfolders such as `Exceptions/` are fine if the namespace stays `OctopusEnergy.Client`.

**Namespace layout:**

- `OctopusEnergy.Client` - `OctopusEnergyClient`, public exception types, and other caller-facing helpers (for example pagination limits)
- `OctopusEnergy.Client.Infrastructure.*` - transport and other internal plumbing
- `OctopusEnergy.Client.Models.[Resource]`
- `OctopusEnergy.Client.Services.[Resource].[Resource]Service`

**Naming:**

- Service: `[Resource]Service`
- Response wrapper: `[Resource]Response` where the wire envelope needs one
- Resource model: `[Resource]`
- Exception: `OctopusEnergy[Context]Exception`
- Test class: `[ClassName]Tests`
- Test method: `Method_State_Expected`

---

## Patterns in Use

- **Client + Services** - one `OctopusEnergyClient` with discoverable resources.
- **Strongly typed contracts** - explicit models; `JsonPropertyName` on every serialised property.
- **REST JSON source generation** - when adding types deserialised by `RestClient`, register them on `Infrastructure/Serialization/OctopusEnergyJsonContext`.
- **Exception hierarchy** - SDK-specific types, not raw `HttpRequestException` as the public contract.
- **Async-first** - cancellation-aware.
- **Customer allow-list** - do not add a method because it exists on the GraphQL schema. Add it because a customer API key can call it (documented + runtime-verified).
- **No full schema codegen** - hand-picked operations.
- **Documented contract validation only** - e.g. REST `page_size` maxima from Octopus docs.

---

## Naming Quick Reference

| Thing | Convention | Example |
|---|---|---|
| Main client | `[Product]Client` | `OctopusEnergyClient` |
| Service | `[Resource]Service` | `ProductService` |
| Request | `[Resource][Action]Request` | `ConsumptionListRequest` |
| Exception | `[Product][Context]Exception` | `OctopusEnergyApiException` |
| Test class | `[ClassName]Tests` | `ProductServiceTests` |
| Test method | `Method_State_Expected` | `ListAsync_WhenNextPage_FollowsLink` |

---

## Things We Don't Do Here

- No app-style controllers or databases
- No `.Result` or `.Wait()` on async code
- No commented-out code on `main`
- No `TODO` without a linked GitHub Issue number
- No partner APIs on the public surface
- No logging of API keys or JWTs

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 8 November 2026 | Add RateLimits to project tree | Goals review follow-up |
| 7 November 2026 | Extend project tree (Consumption, Industry, TariffRates, samples) | Goals review doc drift (#20 groundwork) |
| 7 November 2026 | GraphQL folder note uses milestone v1.2 | Release-label doc tidy |
| 18 September 2026 | Add Account service and models to tree | Issue #13 |
| 18 September 2026 | Add Products service and models to tree | Issue #10 |
| 18 September 2026 | Note GridSupplyPointJsonConverter under Serialization | Review follow-up #33 |
| 18 September 2026 | Add tariff/GSP helper types to tree | Issue #15 |
| 18 September 2026 | Clarify project-root public types | Align tree with PR #31 layout |
| 13 September 2026 | Initial draft | Project kickoff |
