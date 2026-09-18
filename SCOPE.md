# Scope

**Project:** OctopusEnergy.Client
**Last updated:** 13 September 2026

---

## In Scope - v1.0 (REST customer core)

- Authenticated HTTP client using the customer dashboard **API key** (HTTP Basic, empty password) against `https://api.octopus.energy/v1/`.
- Configurable base URL (default UK host) so other Kraken retail hosts can be tried later without claiming support.
- Strongly typed models and resource services for:
  - Energy products and product detail (tariffs by GSP and payment method)
  - Unit rates and standing charges (electricity standard/day/night, gas)
  - Grid supply points by postcode
  - Electricity meter-point GSP lookup
  - Account (`GET /v1/accounts/{account_number}/`) including properties, meters, registers, agreements, export flag
  - Electricity and gas consumption (including export MPANs reported as `consumption`)
- REST pagination abstracted (`count` / `next` / `previous` / `results`).
- Typed exception hierarchy for HTTP and API errors.
- DateTimeOffset handling; document Europe/London default and BST interval formats.
- Documented units (kWh vs gas m³, pence, VAT inc/exc).
- NuGet packaging pipeline.
- Consumer docs: getting started, pagination, errors, API coverage.
- **No** GraphQL in the public v1 surface unless required for a REST gap that blocks G1 (account discovery via `viewer` is a v2 item).

---

## In Scope - v2 (GraphQL customer extras)

Not required for v1.0. Tracked as a separate GitHub milestone.

- `obtainKrakenToken` with **API key only**, token cache and refresh, `Authorization` header as verified against the live IDE.
- Curated GraphQL operations that succeed with a customer token (verified allow-list, not the full schema):
  - `viewer` / portfolios / multi-account
  - richer `account` projections (bills, transactions, payments) with complexity-aware queries
  - meter reading mutations
  - SmartFlex/Intelligent devices and `flexPlannedDispatches`
  - Home Mini `smartMeterTelemetry`
  - Octoplus / saving sessions / wheel of fortune as an optional module
  - product switch mutation if customer permissions allow
  - `rateLimitInfo`
- GraphQL pagination (`first` ≤ 100, Relay cursors), complexity (200), hourly points (default 50,000), node limit (10,000).
- Typed mapping of `errors.extensions.errorCode` (`KT-CT-*`).

---

## Out of Scope

Do not implement without an explicit scope change.

- Partner REST: create quote, share quote, create account, business tariff renewal.
- Kraken data-import REST.
- GraphQL partner/ops: `partnerViewer`, leads, opportunities, Ink/call-centre, create product/rates, masquerade, collection processes, sales funnels.
- Email/password `obtainKrakenToken`, hCaptcha, browser cookie scraping.
- `https://api.backend.octopus.energy`.
- Whole-schema GraphQL codegen.
- Official-partner-only Intelligent control if it fails for customer tokens at runtime.
- Sample Blazor probe app (deferred until there is a broader SDK surface worth demonstrating). A console products probe lives in `samples/ProductsConsole/` for local live smoke checks.

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 13 September 2026 | Initial draft | Project kickoff |
