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
| Account EUI64 discovery (`smartDevices`) | Allowed | — | `account → electricityAgreements \| gasAgreements → meterPoint → meters → smartDevices`. Empty on probe account (no CAD/Home Mini path). | [graphql-allow-list-account-smart-meter-devices.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-account-smart-meter-devices.json) |
| `rateLimitInfo` | Allowed | — | | [graphql-rate-limit-info.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-rate-limit-info.json) |
| `readingConsentGranularity(mpan:)` | Allowed | — | Requires MPAN argument; public SDK: `MeterReadings.GetReadingConsentGranularityAsync`. | [graphql-allow-list-reading-consent.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-reading-consent.json) |
| `devices(accountNumber:)` | Allowed | — | Returned an empty list on the probe account (no SmartFlex devices enrolled). | [graphql-allow-list-devices-empty.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-devices-empty.json) |
| `createElectricityMeterReading` | Allowed (mutation) | `KT-GB-11007` | Mutation executed; historic `readAt` and probe inputs yielded “meter not found”, not a disabled-field code. | [graphql-allow-list-create-electricity-reading.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-create-electricity-reading.json) |
| `createGasMeterReading` | Not probed | `KT-CT-1111` (fake MPRN probe) | Probe account had no gas meters; fake MPRN returned unauthorised. Public SDK: `MeterReadings.CreateGasAsync` (#99) shipped from schema and fixtures only — re-probe on a gas-capable account before flipping status to **Allowed (mutation)**. | — |
| `initiateProductSwitch` | Allowed (mutation) | `KT-CT-7899` | Nil `quotedProductId` probe; confirms customer mutation path, not a successful switch. Public SDK: `ProductSwitch.InitiateAsync`. | [graphql-allow-list-initiate-product-switch.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-initiate-product-switch.json) |
| `flexPlannedDispatches(deviceId:)` | Allowed (when entitled) | `KT-CT-1111` (unentitled probe) | Customer field per [developer docs](https://developer.octopus.energy/graphql/reference/queries/flexplanneddispatches/). Probe account has no enrolled devices — placeholder or foreign `deviceId` returns `KT-CT-1111`, not `KT-CT-1113`/`4177`. Re-probe with a real id from `devices` when hardware is present. | [graphql-flex-planned-dispatches.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-flex-planned-dispatches.json), [graphql-allow-list-flex-dispatches-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-flex-dispatches-denied.json) |
| `smartMeterTelemetry(deviceId:)` | Allowed (when entitled) | `KT-CT-1111` (unentitled probe) | Customer field per [developer docs](https://developer.octopus.energy/graphql/reference/queries/smartmetertelemetry/). Probe account has no Home Mini EUI64 — placeholder `deviceId` returns `KT-CT-1111`, not `KT-CT-1113`/`4177`. Field limit 125/h; back off on `KT-CT-1199`. Re-probe with EUI64 from `smartDevices` when hardware is present. | [graphql-smart-meter-telemetry.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-smart-meter-telemetry.json), [graphql-allow-list-smart-meter-telemetry-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-smart-meter-telemetry-denied.json), [graphql-error-kt-ct-1199.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-error-kt-ct-1199.json) |
| `loyaltyPointsBalance` | Denied (unentitled) | `KT-CT-1111` | Requires `input.accountNumber`; probe account not enrolled in Octoplus. Customer field on public host. Re-test when enrolled. | [graphql-allow-list-loyalty-points-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-loyalty-points-denied.json) |
| `loyaltyPointLedgers` | Denied (unentitled) | `KT-CT-1111` (expected on probe account) | List query on public host; wrap when customer token succeeds on an enrolled account. | [graphql-allow-list-loyalty-ledgers-denied.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-allow-list-loyalty-ledgers-denied.json) |
| `octoplusAccountInfo` | Allowed (when entitled) | `KT-CT-1111` (expected unentitled) | Returns `enrollmentStatus` for customer keys on public host (schema verified 8 November 2026). | [graphql-octoplus-account-info.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-octoplus-account-info.json) |
| `wheelOfFortuneSpins` | Allowed (when entitled) | `KT-CT-1111` (expected unentitled) | Deprecated public query; spin allowance per fuel. | [graphql-wheel-of-fortune-spins.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-wheel-of-fortune-spins.json) |
| `joinOctoplusCampaign` | Allowed (mutation) | business / permission codes expected on probe | Customer mutation on public host. | [graphql-join-octoplus-campaign.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-join-octoplus-campaign.json) |
| `redeemLoyaltyPointsForAccountCredit` | Allowed (mutation) | validation / permission codes expected on probe | Zero-point probe confirms mutation path, not a persisted redemption. | [graphql-redeem-loyalty-points-validation.json](../../tests/OctopusEnergy.Client.Tests/TestSupport/Fixtures/graphql-redeem-loyalty-points-validation.json) |
| `savingSessions` / `joinSavingSessionsEvent` | Do not wrap | — | **Not present** on public `Query` / `Mutation` (introspection 8 November 2026). Community integrations use `api.backend.octopus.energy`, which is out of SDK scope. | — |

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
- **No gas** on the primary property — gas reading mutation not fully verified. The public SDK wraps `createGasMeterReading` from schema and unit fixtures (#99); runtime status stays **Not probed** until a gas-capable dashboard key or Octopus sandbox account is available (see **Future testing** below).
- **No SmartFlex / Home Mini / Octoplus** enrolment on this account — `devices` returns an empty list; `smartDevices` under agreements is empty; `flexPlannedDispatches`, `smartMeterTelemetry`, `loyaltyPointsBalance`, and related Octoplus fields return `KT-CT-1111` when the account user is not entitled. Treat as “not entitled on this account”, not as permission to wrap partner APIs. Customer-reachable telemetry and Octoplus helpers are still wrapped (#26, #27) with documented rate limits. `savingSessions` is absent from the public schema (see matrix above).
- Field rate limits (`KT-CT-1199`) were not hit; back off if reprobing telemetry.

## Future testing

The maintainer probe account is electricity-only. Gas mutation allow-list verification and optional live sample smoke depend on a dashboard API key tied to a property with gas, or a future Octopus-provided sandbox account for SDK development. When that exists, re-run `LiveGraphQlAllowListProbeTests`, refresh the `createGasMeterReading` row and redacted fixtures here, and update the gas **Allow-list status** column in [API coverage](../reference/api-coverage.md).

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

When re-probing or shipping public SDK methods for operations that are **Allowed** here but not yet wrapped, update this probe matrix and the **Allow-list status** column in [API coverage](../reference/api-coverage.md) in the same change. Gas mutation SDK is wrapped (#99) while runtime status remains **Not probed** until a gas-capable account re-probe.

- [API coverage](../reference/api-coverage.md) — consumer-facing summary table (must match probe statuses here)  
- [coding notes §5](coding-notes.md) — candidate list and complexity notes  
- [ADR-0001](../../adr/adr-0001-customer-sdk-dual-transport.md) — dual transport and allow-list follow-up  
- Issue [#21](https://github.com/markheydon/octopus-energy-dotnet/issues/21) — this spike
