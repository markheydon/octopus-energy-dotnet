---
name: goals-review
description: Audit the repository (or a PR diff) against GOALS.md, SCOPE.md, CONVENTIONS.md, and AGENTS.md project rules. Use when the user asks for a goals review, drift check, project health audit, or whether work still aligns with G1–G6.
---

# Goals Review

Run a structured audit to detect drift from this project's intent. This is a **read-only review** unless the user explicitly asks you to fix findings.

## Required reading (always)

Read these before assessing:

1. [GOALS.md](../../../GOALS.md) - G1–G6 and kill criteria
2. [SCOPE.md](../../../SCOPE.md) - in/out of scope for v1.0
3. [CONVENTIONS.md](../../../CONVENTIONS.md) - coding and SDK patterns
4. [AGENTS.md](../../../AGENTS.md) - project rules, skills routing, allowed actions

## Scope of review

Determine scope from user context:

| Trigger | Scope |
|---------|--------|
| No extra context | Whole-repository health check; exhaustively inventory service list methods |
| PR number or branch | Changes against `main`, plus relevant docs and sample checks |
| `@`-mentioned paths | Those paths against relevant goals |

On `main`, the implemented customer SDK includes REST resources plus curated GraphQL-backed services (milestone **v1.2**). For a whole-repository review, inventory **all** services under `src/OctopusEnergy.Client/Services/` and classify GraphQL list methods per [ADR-0002](../../../adr/adr-0002-graphql-public-list-patterns.md) (Relay page methods, bounded `IReadOnlyList<T>`, not only `IAsyncEnumerable<T>` for REST). Latest NuGet (**1.1.0**) does not yet expose GraphQL public accessors; flag drift when docs imply otherwise. Run the Release build and tests when reviewing code changes, using [contributor setup](../../../docs/contributing-setup.md), and report pass/fail.

## Checklist - map findings to goal IDs

### G1 - Strongly typed customer SDK

- [ ] Public REST resources use typed models and services
- [ ] Serialised properties follow the JSON naming and source-generation conventions in [CONVENTIONS.md](../../../CONVENTIONS.md)
- [ ] New methods represent documented customer API operations, not partner or operations APIs

### G2 - Discoverable developer experience

- [ ] Consumers can discover products, rates, accounts, meters, and consumption without reconstructing Octopus URLs
- [ ] Fail-fast validation is limited to official local contract constraints such as documented `page_size` maxima
- [ ] Documentation and samples show the supported SDK surface and avoid inventing tariff advice or other business rules

### G3 - Automatic pagination

List endpoints expose `IAsyncEnumerable<T>` and follow REST `next` links internally. Consumption also provides explicit single-page `ListElectricityPageAsync` and `ListGasPageAsync` methods. See [pagination](../../../docs/how-to/pagination.md).

#### Service list-method inventory (mandatory)

For **every** service implementation under `src/OctopusEnergy.Client/Services/`, record the public list and single-resource read methods. Do not sample. Classify each list by its behaviour rather than imposing a naming pattern from another SDK.

**Step 1 - enumerate service implementations**

```bash
find src/OctopusEnergy.Client/Services -name '*Service.cs' ! -name 'I[A-Z]*Service.cs'
```

Excludes interface files (`IAccountService.cs`, `IIndustryService.cs`, …) but includes implementations whose names start with `I` followed by a lowercase letter (`IndustryService.cs`).

**Step 2 - classify methods**

For every method, record its name, return shape, whether it follows subsequent pages, and whether it is a single-resource read. Flag mismatches with the documented SDK contract, not simply unfamiliar method names.

| API pagination | Required public list methods | Single-resource reads |
|---|---|---|
| Paginated and auto-following | Descriptive `List*Async` returning `IAsyncEnumerable<T>` | `Get*Async` for a single resource |
| Explicit single-page consumption results | `ListElectricityPageAsync` / `ListGasPageAsync`, alongside their auto-following methods | `Get*Async` for a single resource |

**Step 3 - verify implementation and limits**

For list services:

- [ ] Automatic iteration follows server-provided `next` links and terminates when there is no next page
- [ ] Explicit page methods return pagination metadata without being mistaken for the automatic iteration path
- [ ] `page_size` limits match official REST contract values exposed through `RestPageSizeLimits`

**Step 4 - verify docs, samples, and tests match the SDK**

Cross-check [API coverage](../../../docs/reference/api-coverage.md), [GraphQL allow-list](../../../docs/planning/graphql-customer-allow-list.md) (summary table must match probe statuses), [samples](../../../samples/README.md), relevant how-to docs, and service tests against the inventory. Flag stale method names or examples as drift.

#### Checklist (apply per service)

- [ ] Each list method name and return type make its resource and pagination behaviour clear
- [ ] Auto-following methods correctly handle next-page URLs, cancellation, and termination
- [ ] Single-page methods preserve the documented count and pagination metadata
- [ ] Page-size validation uses the applicable documented limit

#### Required report section

Include a **Service list-method inventory** table in every goals review:

| Service | List method(s) and return shape | Auto-following | Single-resource reads | Status |
|---|---|---|---|---|---|
| `ProductService` | `ListAsync` (`IAsyncEnumerable<Product>`) | Yes | `GetAsync` | OK / drift |

### G4 - Safe and stable

- [ ] Errors use the documented exception hierarchy; HTTP failures are not swallowed
- [ ] API keys remain under caller control and are not logged
- [ ] Public API and release changes follow [VERSIONING.md](../../../VERSIONING.md) and [plan/RELEASE.md](../../../plan/RELEASE.md)

### G5 - Contribution-friendly

- [ ] Significant architectural decisions have ADRs
- [ ] Public API changes are intentional, discoverable, and covered by tests

### G6 - Hide transport details

- [ ] Callers use one client and do not have to choose REST or GraphQL transport for supported resources
- [ ] Authentication and transport implementation details remain internal where applicable
- [ ] Whole-repository reviews include GraphQL services: classify list patterns per ADR-0002, confirm no caller-visible REST vs GraphQL choice, and keep [API coverage](../../../docs/reference/api-coverage.md) and the [allow-list](../../../docs/planning/graphql-customer-allow-list.md) aligned (probe **status**, not only public SDK rows)

### Scope boundaries

- [ ] No partner, operations, data-import, or other explicitly out-of-scope APIs
- [ ] No email/password login or calls to `api.backend.octopus.energy`
- [ ] No UI or CLI in the SDK package
- [ ] No undocumented endpoints or invented business rules
- [ ] Samples demonstrate implemented SDK behaviour only

### Sample app sync (when SDK services changed)

- [ ] Relevant samples remain accurate; do not assume every endpoint must have a sample
- [ ] Live API samples remain opt-in and do not expose credentials

### Documentation

- [ ] UK English in docs and comments
- [ ] User-facing documentation and examples match the implemented API and documented units, VAT, and time behaviour

## Output format

Produce a concise report:

```markdown
# Goals review - <scope>

**Verdict:** Aligned | Minor drift | Significant drift

## Summary
<2–4 sentences>

## Findings

| ID | Severity | Goal | Finding | Evidence |
|----|----------|------|---------|----------|
| F1 | high/medium/low | G2 | ... | file:line or diff |

## Passing checks
- Bullet list of goals/areas with no issues found

## Recommended actions
1. Ordered list - fixes first, then preventive measures
```

Severity guide:

- **high** - violates scope, goal, or public API contract; should block merge
- **medium** - inconsistency or doc/sample drift; fix soon
- **low** - style, minor doc gap, or improvement opportunity

Do not file GitHub issues unless the user asks. Do not edit code unless the user asks you to remediate findings.
