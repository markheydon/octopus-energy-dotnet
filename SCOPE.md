# Scope

**Project:** OctopusEnergy.Client
**Last updated:** 7 November 2026

Release naming: GitHub milestones (**v1.0**, **v1.1**, **v1.2**) and NuGet **1.x** are what most readers need. The GraphQL section below was historically labelled SCOPE phase **v2** — see [release labels](VERSIONING.md#release-labels-quick-reference).

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
- **No** GraphQL in the public v1 surface unless required for a REST gap that blocks G1 (account discovery via `viewer` is planned for milestone **v1.2** GraphQL).

---

## In Scope - v1.2 (GraphQL customer extras)

Not required for v1.0 REST MVP. GitHub milestone **v1.2**; ships as NuGet **1.x** minors (for example **1.2.0**), not package **2.0.0**. REST developer experience is milestone **v1.1** (shipped). Details: [VERSIONING.md](VERSIONING.md#release-labels-quick-reference).

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
| 7 November 2026 | Retitle GraphQL section to v1.2; link release label glossary | Easier scanning for humans and agents |
| 19 September 2026 | Retarget GraphQL extras to milestone v1.2; v1.1 is REST DX | Align SCOPE.md with GitHub milestone reorganisation (#61) |
| 18 September 2026 | Clarify v2 scope phase ships on milestone v1.1 as NuGet `1.x` | Align scope labels with VERSIONING.md |
| 13 September 2026 | Initial draft | Project kickoff |
