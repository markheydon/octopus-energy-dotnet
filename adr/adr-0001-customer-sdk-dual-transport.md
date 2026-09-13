---
title: Customer-only SDK with dual transport hidden from callers
status: accepted
date: 2026-09-13
deciders: Mark Heydon
---

# ADR-0001: Customer-only SDK with dual transport hidden from callers

## Context

Octopus publishes REST and GraphQL on `api.octopus.energy`. The GraphQL schema includes partner enrolment, quoting, and operations tools. Library users will almost always be energy customers with a dashboard API key, not approved Kraken partners.

FreeAgent.NET succeeded by offering one client, typed resources, and hiding OAuth/HTTP details. Octopus needs the same UX despite two protocols.

## Decision

DEC-001: The public package is a **customer** SDK. Partner REST and partner/ops GraphQL are out of scope.

DEC-002: Callers construct **one** `OctopusEnergyClient` with an API key (and optional base URL). They do not choose REST vs GraphQL.

DEC-003: **v1** implements REST customer endpoints only. **v2** may use GraphQL internally for capabilities REST cannot provide.

DEC-004: GraphQL operations are **hand-picked** and runtime-verified. No whole-schema codegen.

DEC-005: Authentication for GraphQL, when added, is `obtainKrakenToken` with **APIKey** only.

## Alternatives considered

ALT-001: Wrap the entire GraphQL schema. Rejected — partner/ops surface, `KT-CT-1113` failures, poor IntelliSense.

ALT-002: REST-only forever. Rejected as a permanent rule — Mini telemetry, Intelligent dispatches, and `viewer` need GraphQL — but accepted as **v1** scope.

ALT-003: Separate NuGet packages for REST and GraphQL. Rejected for v1/v2 — extra versioning cost; revisit only if GraphQL dependencies become heavy.

## Consequences

Positive:

- Matches likely users.
- Aligns with FreeAgent.NET mental model.
- Limits schema churn exposure.

Negative:

- Some documented GraphQL fields will never appear.
- Implementers must maintain an allow-list.
- v1 cannot discover account numbers via `viewer` until v2.

## Follow-up

Runtime spike for GraphQL customer allow-list (GitHub issue). Confirm `Authorization` header scheme before implementing GraphQL transport.
