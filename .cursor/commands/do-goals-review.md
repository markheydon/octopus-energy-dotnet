# Goals review

Run the `goals-review` skill (`.agents/skills/goals-review/SKILL.md`) against this repository.

If I provided a PR number, branch name, or `@`-mentioned paths after this command, limit the audit to that scope; otherwise perform a whole-repo health check.

For a whole-repository review, inventory public list and single-resource read methods for every service under `src/OctopusEnergy.Client/Services/`. Classify automatic `IAsyncEnumerable<T>` pagination and the explicit consumption page methods according to the skill; do not impose another SDK's naming convention. Cross-check relevant docs and samples.

Deliver the structured report from the skill. Do not change code unless I explicitly ask you to fix findings.
