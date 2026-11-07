---
name: code-review
description: >-
  Review Octopus Energy .NET SDK changes for pull request approval: correctness,
  security, public API contract, architecture, and maintainability. Use when
  the user asks for a code review or findings on a branch, PR, or local diff.
  Read-only unless the user explicitly asks to implement fixes.
---

# Code review

Review as a principal architect and senior developer. Treat the change set like a pull request from another experienced developer. Focus only on issues that would influence your decision to **approve or reject** the pull request. Lead with findings, ordered by severity. Do not make code changes unless the user explicitly asks.

For project-goal or scope drift, use the `goals-review` skill. Mention such drift here only when it also creates a correctness or maintainability issue that should block approval.

## Scope

Decide what to diff before reading code:

| User intent | Diff |
| --- | --- |
| Default (branch work, PR, “review my changes”) | Changes since merge-base with the repository default branch (usually `main`), including committed, staged, and unstaged edits |
| “Uncommitted”, “dirty”, “working tree” only | Staged + unstaged diff only |
| Named PR, branch, or GitHub link | Check out that head branch locally when needed, then use the branch diff row above |

Repository path: use the git root that contains the changes (may be a subfolder of the workspace).

If there is no diff, say so in one sentence and stop.

## Focus on finding

1. Bugs, defects, or logic errors, including HTTP and JSON mapping mistakes.
2. Security concerns, such as leaked API keys or unsafe authentication handling.
3. SDK convention or scope violations. Ground findings in [CONVENTIONS.md](../../../CONVENTIONS.md), [GOALS.md](../../../GOALS.md), [SCOPE.md](../../../SCOPE.md), and relevant [ADRs](../../../adr/).
4. Public API contract problems, such as untyped public contracts, incorrect pagination, serialization gaps, or validations beyond documented local constraints.
5. Architectural issues that would be expensive to fix later, such as layer bleed or a significant pattern change without an ADR.
6. Unnecessary complexity.
7. Missing or inadequate tests when behaviour changes. Consider whether the [samples](../../../samples/README.md) remain accurate when a public SDK surface changes; do not require a sample for every operation.

## Do not comment on

- Formatting issues (CI and `dotnet format` own formatting).
- Naming preferences unless they materially affect readability or public API discoverability.
- Personal coding style preferences.
- Minor optimisations without measurable value.
- Endpoints or resources not implemented in this change set.
- Planned future work.
- Stub implementations that are clearly placeholders.
- Missing functionality unless the current code creates a defect.
- Whole-repo List API mechanical inventories (use `goals-review`).

## How to review

1. Establish the diff (see **Scope**).
2. Read changed code and enough surrounding context to judge behaviour and integration.
3. Trace relevant call paths when changes touch REST transport, API-key authentication, JSON source generation (`JsonPropertyName` on serialised properties), pagination, or services exposed by `OctopusEnergyClient`.
4. Check that pagination follows the documented REST `next` links, and that `page_size` validation uses official limits from `RestPageSizeLimits`.
5. Check relevant tests and, where appropriate, the file-based and Products console samples. Live API checks are opt-in; default tests use recorded fixtures.
6. When unsure whether something is a real issue, investigate callers and run targeted tests before reporting.
7. Report only issues you would expect a senior engineer to raise during a real pull request review.

Meaningful changes should pass the Release build and test gate in [contributor setup](../../../docs/contributing-setup.md). Use that when judging whether a finding blocks approval.

## Output

Lead with findings. Keep summary and praise minimal.

For each issue, provide:

- **Severity** — Critical, High, Medium, or Low.
- **Location** — `path/to/file.cs:line` (or code citation when reporting in chat).
- **Why it matters** — tie to approve/reject risk.
- **Suggested improvement** — concrete, actionable fix direction (do not implement unless asked).

Present issues in a compact markdown table sorted by severity (highest first):

| Severity | Location | Why it matters | Suggested improvement |
| --- | --- | --- | --- |
| … | … | … | … |

Use UK English in prose.

## Closing

End with a brief explanation of overall merge readiness (approve, approve with nits, or reject) based only on the issues above.

If no significant issues are found, say so explicitly. Mention residual risks or test gaps only when they are material to approval. Use UK English.

## After the review

- Do not implement fixes, commit, or push unless the user explicitly asks.
- If the user asks to fix findings, apply minimal correct diffs, match existing conventions, then run the contributing build/test commands before claiming the review is clear.
