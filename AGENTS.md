# Agent Instructions

This file defines repository-specific operating rules for AI coding and writing agents in this project.

## Purpose

- Keep agent behaviour consistent with project goals, scope, and architecture.
- Reduce accidental wrapping of partner-only Kraken APIs.
- Ensure human review remains the final gate for meaningful changes.

## Core Context

Read these before non-trivial changes:

- `GOALS.md`
- `SCOPE.md`
- `CONVENTIONS.md`
- `docs/planning/coding-notes.md` - research that affects implementation
- `adr/` - accepted decisions

## Language and Spelling

All documentation, comments, and user-facing text **must use UK English**.

## Tech Stack

- .NET 8.0 (LTS) and .NET 10.0 (LTS), with .NET 10 as the primary focus.
- xUnit v3 with Microsoft.Testing.Platform.
- No database (API client only).

## Architecture

- SDK-oriented: main client, resource services, typed models, HTTP/auth internals.
- Dual transport is an implementation detail. Do not force callers to choose REST vs GraphQL for v1 REST resources.
- Customer-only public API. Partner and operations GraphQL/REST stay out of `Services/`.

## Sample apps

A [products console sample](samples/README.md) lists products and fetches product detail against the live UK API using `OCTOPUS_ENERGY_API_KEY`. It is opt-in and not run in CI. Additional samples may follow as the SDK surface grows; each must reflect implemented SDK behaviour only.

There is **no** Blazor sample yet. Do not add one until v1 has a broader HTTP surface worth demonstrating.

## Skills

Project skills live in `.agents/skills/`. Read the matching `SKILL.md` when the task matches.

| Skill | Use when |
|---|---|
| `create-architectural-decision-record` | Creating or major-updating an ADR |
| `documentation-writer` | Diátaxis-aligned documentation |
| `project-documentation` | Project-aware docs placement |
| `pr-address-review` | Addressing open PR review threads |

## Task Routing

- **SDK work** (`src/`, `tests/`): `CONVENTIONS.md` and coding notes.
- **Documentation** (`**/*.md` except `adr/`): documentation skills.
- **ADRs** (`adr/*.md`): ADR skill only.

## Allowed Actions

- Suggest and implement code within existing SDK patterns.
- Add or update tests.
- Update repository documentation and plans.
- Raise GitHub Issues using repository templates.
- Open draft pull requests for human review.

## Not Allowed Without Explicit Instruction

- Add or remove NuGet packages.
- Modify CI/CD pipeline behaviour.
- Change authentication logic.
- Change secrets or environment configuration.
- Introduce architecture pattern changes without an ADR.
- Add partner-only endpoints.
- Call `api.backend.octopus.energy` or email/password login.

## ADR Routing

- Store ADRs only in repository-root `adr/` using `adr-NNNN-[title-slug].md`.
- Do not place ADRs under `docs/`.

## Issue Formatting

Use:

- `.github/ISSUE_TEMPLATE/feature_request.yml` or `story_request.yml` for user-facing work.
- `.github/ISSUE_TEMPLATE/chore_request.yml` for maintenance and docs process.
- `.github/ISSUE_TEMPLATE/bug_report.yml` for defects.

For all issue types: link `GOALS.md`, include scope, acceptance criteria, and risks.

## Review Requirements

- All agent-authored pull requests require human review before merge.
- Flag GOAL impact, breaking API risk, and test coverage in the PR description.
