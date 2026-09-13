# Versioning Policy

**Project:** OctopusEnergy.Client
**Last updated:** 13 September 2026

---

## Overview

This project uses [Semantic Versioning 2.0.0](https://semver.org/). Until the MVP in [SCOPE.md](SCOPE.md) is complete, published packages carry a prerelease tag.

> **Prerelease packages do not carry stability guarantees.** Public APIs may change between prerelease versions without a major version bump. Pin to an exact version if stability matters.

---

## Current Stage: Alpha

Tag format: `0.x.y-alpha.n` (for example `0.1.0-alpha.1`).

- Core architecture is being established.
- Breaking changes between alpha releases are possible and will be noted in release notes.
- Not recommended for production use.

---

## Stage Progression

| Stage | Tag format | Entry criteria | Exit criteria |
|---|---|---|---|
| **Alpha** | `0.x.y-alpha.n` | Project inception | All v1.0 items in SCOPE.md implemented and tested |
| **Beta** | `0.x.y-beta.n` | v1 feature-complete | No known blocking issues; API surface ready for feedback |
| **Stable** | `1.0.0` and above | Beta sign-off | Goals in GOALS.md met |

v2 GraphQL work may ship as later `1.x` minors after `1.0.0`, or as `0.2.0-alpha` if it lands before REST MVP is stable. Do not block `1.0.0` on v2.

---

## Criteria for First Stable 1.0.0

1. [In Scope — v1.0](SCOPE.md) implemented and covered by tests (recorded fixtures; live tests optional and not in CI).
2. Goals `G1`–`G6` in [GOALS.md](GOALS.md) met for the REST customer core.
3. SDK used in at least one application owned by the author.
4. No known breaking changes planned immediately.
5. Public API reviewed.

---

## Release Workflow

Releases are triggered by pushing a Git tag matching `v*.*.*` (for example `v0.1.0-alpha.1`).

See [plan/RELEASE.md](plan/RELEASE.md).

The workflow builds, tests, packs `OctopusEnergy.Client`, publishes to NuGet.org, and creates a GitHub Release.

---

## Compatibility Expectations

- **Alpha:** expect breaking changes.
- **Beta:** breaking changes avoided where possible and called out.
- **Stable:** SemVer; breaking changes only in major versions.

```xml
<PackageReference Include="OctopusEnergy.Client" Version="0.1.0-alpha.1" />
```

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 13 September 2026 | Initial draft | Project kickoff |
