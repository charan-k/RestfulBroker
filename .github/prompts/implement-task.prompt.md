---
mode: 'implementation-agent'
description: 'Implement a single task from impl-plan.md with pre/post approval gates'
---
You are implementing exactly ONE task from impl-plan.md, identified by its
task ID/name. Do not touch any other task.

## Step 1 — Pre-implementation confirmation (WAIT for human approval)
State the following, then STOP and wait for explicit approval:
- Task ID / name (as it appears in impl-plan.md)
- Task description (verbatim from the plan)
- Dependencies check: list each dependency and its current status
- Planned files to be created or modified
- Planned approach in 2-3 sentences
- Any ambiguity — if present, ask instead of guessing

Do not write code yet.

## Step 2 — Implementation (only after approval)
- Change only the files listed above.
- Follow repo-wide conventions (error handling, no hardcoded secrets,
  small functions).
- If new logic is added, call `/generate-tests` for the affected file(s).
- Do not expand scope, even if you spot adjacent issues — log them separately
  as follow-ups instead.

## Step 3 — Post-implementation summary (WAIT for human approval)
Report:
- Files actually changed (path + line-count delta)
- Tests added or updated (path + count)
- Secrets check: confirm no credentials/tokens/keys committed
- Any deviations from the pre-implementation plan and why

Then STOP. Do not move to the next task until the human approves.

## Failure Modes
- If the task ID is not found in impl-plan.md, respond "Not Found" and halt —
  do not guess which task was intended.
- If the task is listed under "Blocked Tasks" and the blocker is not yet done,
  halt and state the blocking dependency explicitly.
