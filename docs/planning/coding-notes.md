# Coding notes (research)

**Last updated:** 8 November 2026  
**Audience:** implementers of this SDK, not package consumers.  
**Status:** planning. Runtime customer GraphQL allow-list verified **8 November 2026** — see [graphql-customer-allow-list.md](graphql-customer-allow-list.md).

These notes are the findings that should shape milestones **v1.0**–**v1.2** code ([release labels](../../VERSIONING.md#release-labels-quick-reference)). Official docs: [docs.octopus.energy](https://docs.octopus.energy/), [developer.octopus.energy](https://developer.octopus.energy/), GraphQL IDE at [api.octopus.energy/v1/graphql/](https://api.octopus.energy/v1/graphql/), deprecations at [announcements.kraken.tech](https://announcements.kraken.tech/announcements/public/).

---

## 1. Access model

There are three audiences. This library only targets **customer**.

| Audience | Credentials | Typical capability |
|---|---|---|
| Public | none | Products, some industry lookups |
| Customer | dashboard API key | Own account, consumption, GraphQL `viewer` / own-account fields |
| Partner / organisation | Kraken-issued org key | Enrolment, quotes, data-import, Hub configuration |

The published GraphQL schema is a **superset**. Many fields list Allowed Viewers: Customer, Partner User, Third party. That is not a guarantee. Runtime still returns `KT-CT-1111` / `KT-CT-4177` (unauthorised) or `KT-CT-1113` (disabled field).

**Rule:** do not wrap an operation because it appears in the schema. Wrap it because (a) docs say customer and (b) a dashboard API key succeeded.

`KT-CT-11334`: account user API access is not enabled.

---

## 2. Transports

| | REST | GraphQL |
|---|---|---|
| Base | `https://api.octopus.energy/v1/` | `https://api.octopus.energy/v1/graphql/` |
| Auth | HTTP Basic: API key as username, empty password | JWT from `obtainKrakenToken`, then `Authorization` |
| Errors | HTTP status + JSON | Usually **HTTP 200** with `errors[]` |
| Pagination | `count`, `next`, `previous`, `results` | Relay; `first` required and **≤ 100** |
| v1 use | Catalogue, account, consumption | Do not require for v1 REST stories |

Configurable base URL. Default UK host. Other Kraken retailers use different hosts (for example Japan `api.oejp-kraken.energy`). Do not claim multi-region support in v1.

**Never** call `https://api.backend.octopus.energy` (web-app internal).

`auth.octopus.energy` exists for Kraken OAuth / PKCE. That is for **registered applications**, not dashboard keys. Out of scope until Octopus documents a public customer OAuth product for third-party libraries.

---

## 3. Authentication details

### REST

```
Authorization: Basic base64("{apiKey}:")
```

Docs also mention passing a GraphQL token on REST. Prefer Basic with the API key for v1 REST; one credential, fewer moving parts.

Key location: [dashboard API access](https://octopus.energy/dashboard/new/accounts/personal-details/api-access). Treat as a secret.

### GraphQL (milestone v1.2)

Mutation `obtainKrakenToken(input: ObtainJSONWebTokenInput!)`.

For this library, **only** `APIKey`. Do not implement email/password (rate limits plus captcha/OAuth on the consumer site).

Community reports: access token ~1 hour, refresh ~7 days. The wire field `refreshExpiresIn` is documented as a **Unix timestamp** when the refresh token expires (not a duration). The SDK treats it as seconds since epoch (`long` on the wire).

**Recorded header scheme (7 November 2026):** set `Authorization` to the **raw Kraken token** returned by `obtainKrakenToken` (no `JWT`, `Token`, or `Bearer` prefix). This matches the legacy Kraken GraphiQL guidance. Re-confirm with a dashboard API key via `LiveGraphQlTokenProbeTests` (`OCTOPUS_ENERGY_ENABLE_LIVE_TESTS=1` and `OCTOPUS_ENERGY_API_KEY`) before changing internals. Retry once on `KT-CT-1112` after refresh.

Rate limits on the mutation include `email_from_graphql_input_fields` 5/m, `failed_logins_per_ip` 3/m, `ip` 50/m.

---

## 4. REST endpoints in v1

### Public

- `GET /v1/products/` - `brand`, `is_variable`, `is_green`, `is_tracker`, `is_prepay`, `is_business`, `available_at`
- `GET /v1/products/{product_code}/` - `tariffs_active_at`
- Unit rates / standing charges:
  - `/v1/products/{product}/electricity-tariffs/{tariff}/standing-charges/`
  - `.../standard-unit-rates/`, `day-unit-rates/`, `night-unit-rates/`
  - `/v1/products/{product}/gas-tariffs/{tariff}/standing-charges/`
  - `.../standard-unit-rates/`
  - Query: `period_from`, `period_to`, `page_size` (default 100, **max 1,500**)
- `GET /v1/industry/grid-supply-points/?postcode=`
- `GET /v1/electricity-meter-points/{mpan}/` → `gsp`, `profile_class`

### Authenticated

- `GET /v1/accounts/{account_number}/` - account numbers look like `A-XXXXXXXX`
- `GET /v1/electricity-meter-points/{mpan}/meters/{serial}/consumption/`
- `GET /v1/gas-meter-points/{mprn}/meters/{serial}/consumption/`
  - `period_from` / `period_to` ISO 8601; include `Z` or offset
  - `page_size` default 100, **max 25,000**
  - `order_by`: `period` or `-period` (default newest first)
  - `group_by`: `hour` | `day` | `week` | `month` | `quarter`
  - Empty list for non-smart meters
  - Export MPANs still use field name `consumption`

### Partner REST - do not wrap

- `POST /v1/quotes/`
- Quote share email
- `POST /v1/accounts/` (create)
- `POST /v1/accounts/{n}/tariff-renewal/`
- Data-import APIs

---

## 5. GraphQL (milestone v1.2) — candidate customer operations

The authoritative runtime matrix is [graphql-customer-allow-list.md](graphql-customer-allow-list.md). Re-probe before shipping a new public method.

Likely useful (high level):

- `viewer` - discover accounts/portfolios so callers may omit `A-…`
- `account(accountNumber)` - large type; use a **default projection** under complexity 200
- Meter `consumption` fields (`startAt` timezone **required**)
- `devices`, `flexPlannedDispatches` (`plannedDispatches` deprecated; removal around January 2026)
- `smartMeterTelemetry` - Home Mini / CAD; `deviceId` is EUI64; nested discovery path is `account → electricityAgreements → meterPoint → meters → smartDevices`; field rate limits can be aggressive (community: on the order of 100/hour if over-polled)
- Octoplus / saving sessions / rewards / wheel of fortune
- `createElectricityMeterReading` / `createGasMeterReading`
- `initiateProductSwitch` (permission: switch product)
- Heat pump / battery if present
- `rateLimitInfo`, `readingConsentGranularity`

Do not wrap: `partnerViewer`, leads, opportunities, Ink, call-centre, create product/rates, masquerade, collection processes, most sales-funnel mutations.

GraphQL docs examples are **autogenerated** and are not real payloads.

---

## 6. Pagination, limits, errors

REST: follow `next` until null. Auto-pagination in the client (G3).

GraphQL:

- `first` mandatory, max 100, else error
- Relay: when paging with `after`, `hasPreviousPage` is always false (spec)
- Complexity cap **200** per request (`KT-CT-1188`)
- Hourly points default **50,000** account users, BLOCK mode (`rateLimitInfo`)
- Max **10,000** nodes (`KT-CT-1189`)
- Field rate limits `KT-CT-1199`

**Public list shapes (v1.2):** see [ADR-0002](../../adr/adr-0002-graphql-public-list-patterns.md).

| Pattern | When | Public shape | Example |
|---|---|---|---|
| REST auto-pagination | REST `next` links | `IAsyncEnumerable<T>` | `ProductService.ListAsync` |
| REST explicit page | Caller needs `count` / one page | `PaginatedResult<T>` | `ListElectricityPageAsync` |
| GraphQL Relay | Connection with cursors | `Task<GraphQlRelayPage<T>>` per page | `ListBillsPageAsync` |
| GraphQL bounded list | Single curated query, no caller paging | `Task<IReadOnlyList<T>>` | `ListDevicesAsync`, `ListLoyaltyPointLedgersAsync` |

Do not add blind `IAsyncEnumerable` auto-follow helpers for Relay billing lists; separate queries keep complexity under the cap.

Public exception types should map REST status codes and GraphQL `extensions.errorCode` without leaking entire GraphQL envelopes as the only API.

---

## 7. Timezones, units, money, rounding

- Datetimes without offset are treated as **Europe/London**. Always send offsets from the SDK.
- Consumption intervals can switch between `Z` and `+01:00` around BST. Agile **prices** stay UTC. Use `DateTimeOffset`.
- Electricity consumption: kWh, 0.001 precision. Billing rounding is **half to even** to 0.01 kWh before multiplying by price. Do not invent a cost calculator in v1 unless it is clearly documented as a helper and matches this rule.
- Gas: SMETS1 kWh, SMETS2 **m³**. Model units explicitly.
- Rates: p/kWh and p/day, often `value_exc_vat` and `value_inc_vat` (domestic VAT 5%). Quotes/bills often **pence**.
- `group_by=day` uses local midnight, not UTC.

---

## 8. Tariff codes and GSPs

Example: `E-1R-AGILE-FLEX-22-11-25-C`

- Fuel `E` or `G`
- Register count `1R` / `2R`
- Product code
- GSP letter `A`–`P` (product JSON keys `_A` … `_P`)

Hide encoding behind typed helpers (`TariffCode`, GSP enum). Do not require callers to concatenate URLs.

Variable tariffs auto-extend. Fixed products fall back to variable at end of term. Multiple MPANs: import vs export; REST JSON is not always obvious - tariff codes help.

`brand=OCTOPUS_ENERGY` is not the only brand on the UK host.

---

## 9. Agile, Go, Tracker

- Agile day-ahead rates typically by **16:00 Europe/London**. Before that, a short day (46 vs 48 half-hours) is normal.
- Agile “day” follows CET market index (roughly 23:00–23:00 UK).
- Go: long `valid_from`/`valid_to` windows, not 48 slots.
- Economy 7: separate day/night rate URLs (`E-2R-…`).
- Tracker: GraphQL daily pricing (`trackerTariffDailyPricing`) is milestone **v1.2**.
- Price vs consumption period matching: price endpoint returns exact requested periods; consumption returns overlapping intervals. Off-by-one around clock changes is a test fixture candidate.

---

## 10. Account shape

`viewer` → portfolios → accounts → properties → electricity/gas meter points → meters (old and new serials) → agreements.

REST account is enough for v1 if the caller supplies `account_number`. Discovering it without a bill is a v1.2 GraphQL `viewer` story.

---

## 11. Testing

No public sandbox. CI must use **recorded fixtures**. Optional live checks behind explicit env vars, never default-on, never commit keys. The [products console sample](../../samples/README.md) is the current local live smoke; it is not run in CI.

The maintainer probe account has no gas meter, so `createGasMeterReading` allow-list verification is deferred. Gas SDK behaviour is covered by schema-aligned unit fixtures (#99). Re-probe when a gas-capable dashboard key or Octopus sandbox account is available — see [graphql-customer-allow-list.md](graphql-customer-allow-list.md) **Future testing**.

Home Assistant’s client ([BottlecapDave/HomeAssistant-OctopusEnergy](https://github.com/BottlecapDave/HomeAssistant-OctopusEnergy)) is the best map of what customer tokens actually do. ImpSoft.OctopusEnergy and aiooctopusenergy cover REST only.

---

## 12. Churn to watch

- `plannedDispatches` → `flexPlannedDispatches`
- `estimateMeterReadings` removal scheduled **10 August 2026**
- Other GraphQL deprecations on the announcements board
- `obtainKrakenToken` email/password path is fragile; API-key path is the library contract

Pin GraphQL documents; do not chase every schema field.

---

## 13. Legal / product

Unofficial library. Dashboard keys expose account structure and consumption. Do not encourage sharing keys. Website terms of use are not a developer licence. Same unofficial posture as other community clients.
