# Security Policy

## Reporting a Vulnerability

Please do not report security vulnerabilities via public GitHub issues or discussions.

Use GitHub's private vulnerability reporting flow for this repository:

1. Go to the repository Security tab.
2. Select Report a vulnerability.
3. Submit a private report with reproduction details, impact, and any suggested remediation.

## What to Include

- A clear description of the vulnerability.
- Affected package and version.
- Reproduction steps or proof of concept.
- Potential impact and attack surface.
- Any suggested fix or mitigation.

Do not include live Octopus API keys.

## Response Expectations

Best-effort targets:

- Initial acknowledgement: within 5 working days.
- Triage outcome: within 10 working days when reproducible details are provided.
- Ongoing updates at key milestones until resolution.

## Disclosure Process

Reports are reviewed privately. A fix is released. Public disclosure follows coordinated timing.

## Supported Versions

This project is in prerelease. Security fixes apply on a best-effort basis to the latest prerelease line. See [VERSIONING.md](VERSIONING.md).

## Scope

This policy covers:

- The Octopus Energy .NET SDK code in this repository.
- Published NuGet packages produced from this repository.

Octopus Energy / Kraken API behaviour, account compromise via leaked customer keys, and third-party services are out of scope.
