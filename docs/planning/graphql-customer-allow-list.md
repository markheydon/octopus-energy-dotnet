# GraphQL customer allow-list (runtime verified)

**Last verified:** 8 November 2026 (billing connections re-probed same day)  
**Host:** `https://api.octopus.energy/v1/` (UK Kraken)  
**Credential:** dashboard **API key only** (`obtainKrakenToken` with `APIKey`)  
**Header scheme:** raw JWT in `Authorization` (no `Bearer` prefix) — see [coding notes §3](coding-notes.md) and issue #20  
**Probe harness:** `LiveGraphQlAllowListProbeTests` with `OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1` and `OCTOPUS_ENERGY_API_KEY`

This artefact is the gate for milestone **v1.2** GraphQL stories ([#22](https://github.com/markheydon/octopus-energy-dotnet/issues/22)–[#28](https://github.com/markheydon/octopus-energy-dotnet/issues/28)). Do not add a public SDK method unless the operation is **allowed** here (or re-verified and this document updated).

## How to read statuses

| Status | Meaning |
|---|---|
| **Allowed** | Dashboard key returned data (HTTP 200, no `errors[]`, or empty list where valid). |
| **Allowed (mutation)** | Mutation is customer-reachable; probe used safe/invalid inputs and recorded a business or validation `KT-*` code, not `KT-CT-1113` / `KT-CT-4177`. |
| **Denied** | `KT-CT-1111`, `KT-CT-1113`, `KT-CT-4177`, or equivalent unauthorised/disabled at runtime. |
| **Not probed** | Probe account lacked prerequisites (for example no gas meter); wrap only after a successful probe on a suitable account. |
| **Do not wrap** | Partner/operations surface — never expose in the SDK regardless of schema text. |

`KT-CT-11334` (API access not enabled on the account) prevents token obtain entirely; that is an account setting, not an SDK defect.

## Customer operations (probe matrix)

| Operation | Status | Runtime code(s) | Notes | Redacted fixture |
|---|---|---|---|---|
| `obtainKrakenToken` (`APIKey`) | Allowed | — | Confirmed in #20; raw `Authorization` header. | [graphql-obtain-token.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-obtain-token.json) |
| `viewer` → `accounts { number }` and `portfolios { accounts { number } }` | Allowed | — | Multi-account list merged with portfolio accounts (deduplicated); no silent default account. | [graphql-allow-list-viewer.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-viewer.json) |
| `account(accountNumber)` | Allowed | — | Minimal projection succeeded. | [graphql-allow-list-account.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account.json) |
| `account` → `bills` | Allowed | — | Relay `first: 1`, `includeBillsWithoutPDF: true`. | [graphql-allow-list-account-bills.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account-bills.json) |
| `account` → `transactions` | Allowed | — | Relay `first: 1`; amounts use `tax` (not `vat`); use `__typename` for concrete type. | [graphql-allow-list-account-transactions.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account-transactions.json) |
| `account` → `payments` | Allowed | — | Relay `first: 1`; amounts in pence. | [graphql-allow-list-account-payments.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account-payments.json) |
| Account meter discovery | Allowed | — | `account.properties.electricityMeterPoints` / `gasMeterPoints` for MPAN/MPRN. | [graphql-allow-list-account-meter-points.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account-meter-points.json) |
| `rateLimitInfo` | Allowed | — | | [graphql-rate-limit-info.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-rate-limit-info.json) |
| `readingConsentGranularity(mpan:)` | Allowed | — | Requires MPAN argument. | [graphql-allow-list-reading-consent.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-reading-consent.json) |
| `devices(accountNumber:)` | Allowed | — | Returned an empty list on the probe account (no SmartFlex devices enrolled). | [graphql-allow-list-devices-empty.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-devices-empty.json) |
| `createElectricityMeterReading` | Allowed (mutation) | `KT-GB-11007` | Mutation executed; historic `readAt` and probe inputs yielded “meter not found”, not a disabled-field code. | [graphql-allow-list-create-electricity-reading.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-create-electricity-reading.json) |
| `createGasMeterReading` | Not probed | `KT-CT-1111` (fake MPRN probe) | Probe account had no gas meters; fake MPRN returned unauthorised. Re-probe on an account with gas before wrapping. | — |
| `initiateProductSwitch` | Allowed (mutation) | `KT-CT-7899` | Nil `quotedProductId` probe; confirms customer mutation path, not a successful switch. | [graphql-allow-list-initiate-product-switch.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-initiate-product-switch.json) |
| `flexPlannedDispatches(deviceId:)` | Allowed (when entitled) | `KT-CT-1111` (unentitled probe) | Customer field per [developer docs](https://developer.octopus.energy/graphql/reference/queries/flexplanneddispatches/). Probe account has no enrolled devices — placeholder or foreign `deviceId` returns `KT-CT-1111`, not `KT-CT-1113`/`4177`. Re-probe with a real id from `devices` when hardware is present. | [graphql-flex-planned-dispatches.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-flex-planned-dispatches.json), [graphql-allow-list-flex-dispatches-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-flex-dispatches-denied.json) |
| `smartMeterTelemetry(deviceId:)` | Denied | `KT-CT-1111` | Placeholder device id; re-test with Home Mini / CAD EUI64 when present (#26). | [graphql-allow-list-smart-meter-telemetry-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-smart-meter-telemetry-denied.json) |
| `loyaltyPointsBalance` | Denied | `KT-CT-1111` | Used as Octoplus/loyalty proxy; probe account not authorised. Re-test when enrolled (#27). | [graphql-allow-list-loyalty-points-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-loyalty-points-denied.json) |

## Do not wrap (partner / operations)

Runtime probes **must** stay denied for customer keys. Static list (also [SCOPE.md](../../SCOPE.md)) plus observed codes on 8 November 2026:

| Surface | Schema hint | Observed with dashboard key | SDK action |
|---|---|---|---|
| `partnerViewer` | Partner viewer | `KT-CT-1111` | Do not wrap |
| `leads` | Partner CRM | `KT-CT-1111` | Do not wrap |
| Opportunities, Ink, call-centre | Ops | Not probed (expect deny) | Do not wrap |
| Create product / rates, masquerade | Ops | Not probed (expect deny) | Do not wrap |
| Collection processes, sales funnels | Ops | Not probed (expect deny) | Do not wrap |
| Data-import REST, partner quotes | REST | N/A (out of scope) | Do not wrap |

Fixtures: [graphql-allow-list-partner-viewer-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-partner-viewer-denied.json), [graphql-allow-list-leads-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-leads-denied.json).

## Probe account caveats

Results reflect one domestic dashboard key with API access enabled:

- **Multi-account** `viewer.accounts` returned two account numbers.
- **No gas** on the primary property — gas reading mutation not fully verified.
- **No SmartFlex / Home Mini / Octoplus** enrolment on this account — `devices` returns an empty list; `flexPlannedDispatches`, `smartMeterTelemetry`, and `loyaltyPointsBalance` return `KT-CT-1111` when probed without a valid enrolled device id. Treat as “not entitled on this account”, not as permission to wrap partner APIs.
- Field rate limits (`KT-CT-1199`) were not hit; back off if reprobing telemetry.

## Regenerating

```bash
export OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1
export OCTOPUS_ENERGY_API_KEY='…'   # never commit
# optional: OCTOPUS_ENERGY_ACCOUNT_NUMBER='A-…' if viewer does not list accounts
dotnet test tests/OctopusEnergy.Client.Tests/OctopusEnergy.Client.Tests.csproj \
  -c Release --filter "FullyQualifiedName~LiveGraphQlAllowListProbeTests" \
  --output detailed
```

Copy redacted envelopes from test output into this document and into `tests/.../Fixtures/graphql-allow-list-*.json` when re-verifying.

## Related

- [API coverage](../reference/api-coverage.md) — consumer-facing summary table (must match probe statuses here)  
- [coding notes §5](coding-notes.md) — candidate list and complexity notes  
- [ADR-0001](../../adr/adr-0001-customer-sdk-dual-transport.md) — dual transport and allow-list follow-up  
- Issue [#21](https://github.com/markheydon/octopus-energy-dotnet/issues/21) — this spike
