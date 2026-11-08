---
title: "ADR-0002: GraphQL public list patterns"
status: accepted
date: 2026-11-08
authors: Mark Heydon
tags: ["architecture", "graphql", "pagination"]
supersedes: ""
superseded_by: ""
---

# ADR-0002: GraphQL public list patterns

## Status

**Accepted**

## Context

REST customer lists expose automatic pagination via `IAsyncEnumerable<T>` (G3). GraphQL customer operations use Relay connections, per-request complexity limits (200), and heterogeneous list shapes. The v1.2 surface includes account billing connections, bounded device and telemetry lists, and Octoplus ledger arrays.

Callers must not choose REST versus GraphQL transport (ADR-0001). Public method shapes must make pagination behaviour obvious without mirroring REST naming where GraphQL contracts differ.

## Decision

DEC-001: **REST list resources** continue to use `IAsyncEnumerable<T>` with internal `next` following, plus explicit single-page consumption methods where documented.

DEC-002: **GraphQL Relay connections** expose **explicit page methods** returning `GraphQlRelayPage<T>`. Callers advance with `after` cursors until `PageInfo.HasNextPage` is false. Do not add `IAsyncEnumerable` wrappers that hide complexity across separate GraphQL queries.

DEC-003: **Bounded GraphQL list fields** that return a single query payload (for example `devices`, `loyaltyPointLedgers`, smart-device discovery) may return `IReadOnlyList<T>` when the curated query does not paginate on the wire or the connection is small enough to fetch in one request.

DEC-004: **GraphQL mutations and singleton queries** (viewer, rate limits, Octoplus account info, product switch) use `Get*Async`, `Create*Async`, or imperative names; they are not list pagination.

DEC-005: Validate documented Relay limits with `GraphQlRelayLimits.ValidateFirst` before GraphQL calls that accept `first`.

## Consequences

### Positive

- **POS-001**: Account billing stays under the complexity cap by using three queries instead of one auto-draining enumerator.
- **POS-002**: Method names and return types signal whether callers must page (Relay) or read a single payload (`IReadOnlyList<T>`).
- **POS-003**: Aligns with consumer [pagination](../docs/how-to/pagination.md) and existing `AccountBillingService` implementation.

### Negative

- **NEG-001**: GraphQL list ergonomics differ from REST; contributors must choose the correct pattern per operation.
- **NEG-002**: No universal `IAsyncEnumerable` helper for Relay without risking complexity or node-limit failures.

## Alternatives considered

### Uniform `IAsyncEnumerable` for all GraphQL lists

- **ALT-001**: **Description**: Auto-follow Relay cursors in every list service.
- **ALT-002**: **Rejection Reason**: Multi-resource billing and complexity caps make blind auto-pagination unsafe; consumers may need cursor control.

### Mirror REST `List*Async` names on non-paginated GraphQL arrays

- **ALT-003**: **Description**: Name every GraphQL array fetch `List*Async` even when there is only one HTTP round trip.
- **ALT-004**: **Rejection Reason**: Accepted where the name matches customer docs (`ListDevicesAsync`); Relay pages use `List*BillsPageAsync` to emphasise single-page Relay semantics.

## Implementation notes

- **IMP-001**: New GraphQL list operations: classify as Relay page, bounded list, or neither before naming the public method.
- **IMP-002**: Extend [api coverage](../docs/reference/api-coverage.md) and the [allow-list](../docs/planning/graphql-customer-allow-list.md) when adding operations.
- **IMP-003**: Document paging in the relevant how-to; keep [coding notes](../docs/planning/coding-notes.md) section 6 in sync.

## References

- **REF-001**: [ADR-0001](adr-0001-customer-sdk-dual-transport.md)
- **REF-002**: [SCOPE.md](../SCOPE.md) — GraphQL pagination limits
- **REF-003**: [docs/how-to/pagination.md](../docs/how-to/pagination.md)
