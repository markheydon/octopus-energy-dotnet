# Goals

**Project:** OctopusEnergy.Client
**Owner:** Mark Heydon
**Last updated:** 13 September 2026

---

## Why This Exists

Octopus Energy publishes a useful customer REST API and a much larger Kraken GraphQL schema. The GraphQL docs mix customer, partner, and operations fields. .NET developers who only have a dashboard API key still end up reading tariff URL templates, GSP letters, VAT fields, and GraphQL auth by hand.

This project should be the same kind of library as FreeAgent.NET: strongly typed, boring to depend on, and honest about what a **customer** can call.

---

## Goals for v1.0

- **G1:** Provide a clean, strongly typed .NET SDK for the **customer** Octopus Energy REST API (catalogue, account, consumption).
- **G2:** Deliver at least a Stripe.NET-quality developer experience. Callers should get products, rates, meters, and consumption from IntelliSense without reconstructing Octopus URLs. Fail fast only on **official, local contract constraints** (for example documented `page_size` maxima). Do not invent extra checks or encode tariff advice.
- **G3:** Support automatic pagination without consumers managing REST `next` links.
- **G4:** Be safe, stable, and boring to depend on. API keys stay with the caller; the SDK never logs secrets.
- **G5:** Enable external contributions without destabilising the public API.
- **G6:** Hide the REST vs GraphQL split. One client, API-key construction; GraphQL JWT exchange is an implementation detail when needed.

---

## Success Looks Like

- Used in at least one application owned by the author.
- Pagination, timezones, units, and errors work without consumer workarounds.
- Package published to NuGet with documentation that states units, VAT, and BST behaviour.
- Partner-only operations are not in the public surface.

---

## Kill Criteria

- Octopus ships an official .NET SDK that supersedes this work.
- Maintenance cost outweighs personal value.
- Customer API access is withdrawn or becomes partner-only in practice.
- The SDK is not used in any real project after initial integration.

---

## What This Is NOT For

(See also: SCOPE.md)

- No UI, CLI, or home-automation host (Home Assistant is a consumer, not this repo).
- No partner enrolment, quoting, data-import, or Kraken Hub organisation APIs.
- No email/password login, captcha bypass, or `api.backend.octopus.energy`.
- No unofficial OAuth app registration flow unless Octopus documents a public customer OAuth product.
- No wholesale price modelling or “cheapest slot” opinions beyond returning documented rates.

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 13 September 2026 | Initial draft | Project kickoff |
