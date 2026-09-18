# Versioning Policy

**Project:** OctopusEnergy.Client
**Last updated:** 18 September 2026

---

## Overview

This project uses [Semantic Versioning 2.0.0](https://semver.org/). The REST customer core shipped as **1.0.0** on 18 September 2026. Later releases follow SemVer; breaking changes appear only in major versions.

---

## Current Stage: Stable

Tag format: `1.x.y` (for example `1.0.0`).

- v1.0 REST customer core is complete (see [SCOPE.md](SCOPE.md)).
- Public API changes follow SemVer.
- Suitable for production use; pin to an exact version if you need reproducible builds.

---

## Stage Progression

| Stage | Tag format | Entry criteria | Exit criteria |
|---|---|---|---|
| **Alpha** | `0.x.y-alpha.n` | Project inception | All v1.0 items in SCOPE.md implemented and tested |
| **Beta** | `0.x.y-beta.n` | v1 feature-complete | No known blocking issues; API surface ready for feedback |
| **Stable** | `1.0.0` and above | Beta sign-off | Goals in GOALS.md met |

**Shipped:** `1.0.0` (18 September 2026) — first stable release of the REST customer core.

v2 GraphQL work may ship as later `1.x` minors after `1.0.0`, or as `0.2.0-alpha` if it lands before REST MVP is stable. Do not block `1.0.0` on v2.

---

## Criteria for First Stable 1.0.0

1. [In Scope - v1.0](SCOPE.md) implemented and covered by tests (recorded fixtures; live tests optional and not in CI).
2. Goals `G1`–`G6` in [GOALS.md](GOALS.md) met for the REST customer core.
3. SDK used in at least one application owned by the author.
4. No known breaking changes planned immediately.
5. Public API reviewed.

All criteria were met for the `1.0.0` release.

---

## Release Workflow

Releases are triggered by pushing a Git tag matching `v*.*.*` (for example `v1.0.0`).

See [plan/RELEASE.md](plan/RELEASE.md).

The workflow builds, tests, packs `OctopusEnergy.Client`, publishes to NuGet.org, and creates a GitHub Release.

---

## Compatibility Expectations

- **Alpha:** expect breaking changes.
- **Beta:** breaking changes avoided where possible and called out.
- **Stable:** SemVer; breaking changes only in major versions.

```xml
<PackageReference Include="OctopusEnergy.Client" Version="1.0.0" />
```

---

## Revision History

| Date | Change | Reason |
|---|---|---|
| 18 September 2026 | Promote to Stable `1.0.0` | REST customer core shipped |
| 13 September 2026 | Initial draft | Project kickoff |
