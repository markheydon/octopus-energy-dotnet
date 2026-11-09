# Implementation plan

Work is tracked as GitHub Issues using **feature → story** (no epic unless a theme later spans multiple features).

**Features** (what we are building):

- [#6 REST customer core](https://github.com/markheydon/octopus-energy-dotnet/issues/6) - SCOPE.md v1.0
- [#7 GraphQL customer extras](https://github.com/markheydon/octopus-energy-dotnet/issues/7) - milestone **v1.2** ([SCOPE.md](../SCOPE.md) GraphQL section; legacy alias “SCOPE v2”)

**Milestones** (when it is meant to ship - version timeboxes, not copies of the feature titles):

- [v1.0](https://github.com/markheydon/octopus-energy-dotnet/milestone/1) - first REST release
- [v1.1](https://github.com/markheydon/octopus-energy-dotnet/milestone/3) - REST developer experience (NuGet `1.x` minors)
- [v1.2](https://github.com/markheydon/octopus-energy-dotnet/milestone/2) - GraphQL extras (#7) and REST typed wire fields (#66); NuGet `1.2.x`
- [v1.3](https://github.com/markheydon/octopus-energy-dotnet/milestone/4) - Public surface consolidation (#65, #67) and packaging hardening (#68, #64); NuGet `1.3.x` (former **v1.4** milestone merged here, November 2026)

Stories and enablers are sub-issues of the relevant feature and also sit on the matching milestone.

Ongoing chores without a version timebox: for example API staleness (#63).

See [docs/planning/coding-notes.md](../docs/planning/coding-notes.md).
