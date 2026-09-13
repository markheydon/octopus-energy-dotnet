# Release runbook

Maintainer guide for publishing `OctopusEnergy.Client` to NuGet.org. Policy: [VERSIONING.md](../VERSIONING.md).

## Prerequisites

- [ ] NuGet.org Trusted Publishing for `markheydon/octopus-energy-dotnet` and workflow `release.yml`, scoped to `OctopusEnergy.Client`
- [ ] `NUGET_USER` repository secret set
- [ ] `<Version>` in `src/OctopusEnergy.Client/OctopusEnergy.Client.csproj` matches the intended tag
- [ ] `main` is green on CI

## Publish steps

1. Bump `<Version>` if needed.
2. Merge to `main`.
3. Tag and push: `git tag v0.1.0-alpha.1 && git push origin v0.1.0-alpha.1`
4. Monitor the Release workflow.
5. Verify nuget.org and GitHub Releases.

Do not tag a “real” alpha until v1 HTTP behaviour exists; the current `0.1.0-alpha.1` in the csproj is a placeholder for CI pack validation only.
