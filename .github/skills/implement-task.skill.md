# Skill: Implement Task

## Owner Agent
Implementation Agent

## Purpose
Executes a single, specific task from impl-plan.md in an isolated,
approval-gated manner — preventing scope creep during implementation.

## Trigger
Invoked during Step 5 (Implementation), once per task, only after the
previous task has been approved by the human reviewer.

## Inputs
- `taskId` (string) — the task identifier/name as listed in impl-plan.md.

## Outputs
- Code changes scoped strictly to the named task.
- A pre-implementation confirmation (task description + dependencies check).
- A post-implementation summary (files changed, tests added, secrets check).

## Preconditions
- impl-plan.md must exist.
- The specified taskId must not be in the "Blocked Tasks" section,
  or its blocking dependency must already be marked complete.

## Postconditions
- Task status in impl-plan.md should be updated to reflect completion
  (if the agent has edit access) or flagged for manual update.
- Tests exist for any new logic introduced.

## Failure Modes
- If taskId doesn't exist in impl-plan.md, respond "Not Found" —
  do not guess which task was intended.
- If the task is blocked, halt and state the blocking dependency explicitly.

## Related Prompt File
`.github/prompts/implement-task.prompt.md`
