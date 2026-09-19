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

A [products console sample](samples/README.md) smoke-tests the live UK API: public products without a key, and optionally HTTP Basic auth when `OCTOPUS_ENERGY_API_KEY` is set. It is opt-in and not run in CI. Additional samples may follow as the SDK surface grows; each must reflect implemented SDK behaviour only.

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

## Pull request workflow

When the user asks you to open a pull request (or a plan ends with that step), follow [`.github/PULL_REQUEST_TEMPLATE.md`](.github/PULL_REQUEST_TEMPLATE.md) and match the shape of recent story PRs (for example [#76](https://github.com/markheydon/octopus-energy-dotnet/pull/76)).

### Before opening

1. Branch from `main` with a descriptive name (for example `cursor/issue-57-consumption-rate-overlap`).
2. Run and pass (Release configuration):

```bash
dotnet format OctopusEnergy.slnx --verify-no-changes
dotnet build OctopusEnergy.slnx -c Release -warnaserror
dotnet test OctopusEnergy.slnx -c Release --no-build
```

3. Update related docs, `plan/RELEASE.md` when public behaviour changes, and issue-linked acceptance criteria.

### Title

Use the issue type prefix and link the issue number:

- `[Story] Short description (#NNN)`
- `[Chore] Short description (#NNN)`

### Body

Start from the PR template. Tick applicable **Type of Change** and **Checklist** boxes (mark completed items `[x]`). Add these sections when they apply:

| Section | Content |
|---|---|
| **Summary** | One to three bullets: what changed and why. |
| **Type of Change** | Checkboxes from the template. |
| **Checklist** | All applicable template items checked when true. |
| **API changes** | New or changed public types/methods (SDK work). |
| **GOAL impact** | Which `GOALS.md` items are helped (for example G2, G4). |
| **API risk** | Breaking vs additive; behaviour changes; obsoletions. Note `plan/RELEASE.md` updates. |
| **Milestone** | GitHub milestone when known (for example v1.1). |
| **Related Issues** | `Closes #NNN` for the primary issue. |

Omit empty sections. Do not claim checks passed unless you ran them.

### Labels

Follow [plan/LABEL_STRATEGY.md](plan/LABEL_STRATEGY.md). Every pull request needs exactly one label from each required group:

| Group | On open PR | Examples |
|---|---|---|
| `type/*` | Match the title prefix | `type/story`, `type/chore`, `type/documentation` |
| `status/*` | `status/in-review` | `status/done` after merge |
| `priority/*` | Copy from the linked issue when present; otherwise `priority/medium` | `priority/high`, `priority/medium` |

The `[Story]` / `[Chore]` title prefix must agree with the `type/*` label. Do not use retired unprefixed labels (see LABEL_STRATEGY.md).

### Milestone

When the PR closes a tracking issue, set the **same GitHub milestone** as that issue (for example both on `v1.1`). Check with:

```bash
gh issue view NNN --json milestone
```

Copy the milestone title onto the PR at creation time, or immediately after if GitHub did not inherit it:

```bash
gh pr edit --milestone "v1.1"
```

If the linked issue has **no** milestone, leave the PR unset unless the user or plan specifies one. Document intentional mismatches in the PR body (for example a `1.0.x` patch off the v1.1 timebox).

### Create the PR

Use `gh` from the repository root:

```bash
git push -u origin HEAD
gh pr create \
  --title "[Story] … (#NNN)" \
  --milestone "v1.1" \
  --label "type/story" \
  --label "status/in-review" \
  --label "priority/medium" \
  --body "$(cat <<'EOF'
## Summary
…

## Type of Change
- [x] SDK code

## Checklist
- [x] …

## GOAL impact
…

## API risk
…

## Milestone

v1.1

## Related Issues

Closes #NNN
EOF
)"
```

After creation, confirm labels and milestone match the linked issue (`gh pr view --json labels,milestone`).

Return the PR URL to the user. Do not merge without human review.
