# Skill: Check Dependency Safety

## Owner Agent
Code Review Agent

## Purpose
Scans dependency manifest files for known-vulnerable or significantly
outdated package versions, satisfying the "Dependency Safety" review checklist item.

## Trigger
Invoked during Step 6 (Review), as part of the fixed 7-point code review checklist.

## Inputs
- Dependency manifest files present in the repo (e.g., package.json,
  requirements.txt, pom.xml, go.mod).

## Outputs
- A table: Package | Current Version | Status | Recommended Action.
- An overall PASS/FAIL/PARTIAL verdict for the "Dependency Safety" checklist item.

## Preconditions
- At least one recognizable dependency manifest file must exist in the repo.

## Postconditions
- No files are modified by this skill — it only reports findings.
  Actual version upgrades are a separate, human-approved implementation task.

## Failure Modes
- If vulnerability data isn't available for a specific package, mark its
  status "Not Found" rather than assuming safety.
- If no manifest file is found at all, state that dependency scanning
  could not be performed — do not report a false "all clear."

## Related Prompt File
`.github/prompts/check-dependency-safety.prompt.md`
