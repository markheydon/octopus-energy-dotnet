# Contributing to OctopusEnergy.Client

Thanks for your interest.

This project aims to provide a modern, strongly typed .NET SDK for the **Octopus Energy customer APIs**.

## Before You Start

- Read [GOALS.md](GOALS.md), [SCOPE.md](SCOPE.md), [CONVENTIONS.md](CONVENTIONS.md), and [VERSIONING.md](VERSIONING.md).
- Read [docs/planning/coding-notes.md](docs/planning/coding-notes.md) before adding endpoints.
- Partner-only APIs are out of scope.

## Ways to Contribute

- Report bugs using the bug report template.
- Propose capabilities using the feature or story templates.
- Improve documentation and tests.
- Submit fixes for confirmed defects.

For setup and usage questions, use [GitHub Discussions](https://github.com/markheydon/octopus-energy-dotnet/discussions) first.

## Development Setup

See **[docs/contributing-setup.md](docs/contributing-setup.md)**.

Requirements:

- .NET 8.0 SDK and runtime (`net8.0` target).
- .NET 10.0 SDK and runtime (primary focus).

```bash
dotnet clean OctopusEnergy.slnx && \
dotnet restore OctopusEnergy.slnx && \
dotnet build OctopusEnergy.slnx --no-restore --configuration Release -warnaserror && \
dotnet test OctopusEnergy.slnx --no-build --configuration Release
```

`TreatWarningsAsErrors` is set in [`Directory.Build.props`](Directory.Build.props).

Verify formatting:

```bash
dotnet format OctopusEnergy.slnx --verify-no-changes
```

## Branch and Pull Request Workflow

1. Fork (or use a feature branch) from `main`.
2. Keep changes focused.
3. Add or update tests for behavioural changes.
4. Update docs affected by your change.
5. Open a pull request using the PR template.

## Pull Request Expectations

- Follow `CONVENTIONS.md`.
- Preserve backwards compatibility unless a breaking change is discussed.
- Do not commit API keys, JWTs, or live account numbers in fixtures (redact).
- Ensure CI is green before requesting final review.

## Code of Conduct

[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
