## Summary

Describe what changed and why.

## Type of Change

- [ ] Documentation
- [ ] Workflow or automation
- [ ] Script update
- [ ] Agent skill or Cursor rule update
- [ ] SDK code
- [ ] Other

## Checklist

- [ ] I reviewed related docs and updated them where needed
- [ ] I kept changes scoped and avoided unrelated edits
- [ ] I preserved label strategy alignment with [plan/LABEL_STRATEGY.md](plan/LABEL_STRATEGY.md)
- [ ] `dotnet format OctopusEnergy.slnx --verify-no-changes` passes (or I ran `dotnet format` and committed the result)
- [ ] `dotnet clean` → `restore` → `build -warnaserror` → `test --no-build` passes for `OctopusEnergy.slnx` (Release)
- [ ] I did not commit API keys, JWTs, or live account identifiers
- [ ] I did not add partner-only endpoints

## Related Issues

Link related issues here (e.g. Closes #123)
