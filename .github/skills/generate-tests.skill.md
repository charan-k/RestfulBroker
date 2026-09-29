# Skill: Generate Tests

## Owner Agent
Implementation Agent (primary), Verification Agent (reused)

## Purpose
Generates unit and integration tests for a given file/module, ensuring both
the happy path and edge cases (missing/empty/"Not Found" data) are covered.

## Trigger
- During Step 5 (Implementation), immediately after a task's code is written.
- During Step 7 (Verification), to close any coverage gaps found during review.

## Inputs
- `targetFile` (string) — path to the file/module needing test coverage.

## Outputs
- New or updated test file(s), following the repo's existing test framework
  and naming conventions.
- A brief note on what scenarios were covered vs. what may still be missing.

## Preconditions
- The target file must exist and contain at least one exported/testable unit.
- The repo's existing test framework must be detectable (e.g., via package.json,
  existing test folder conventions).

## Postconditions
- Tests must be runnable via the repo's existing test command.
- No test should be written that mocks away the exact condition it claims to test.

## Failure Modes
- If no test framework can be detected, respond "Not Found" and ask the
  human which framework to use — do not introduce a new one unprompted.
- If the target file has no clearly testable units (e.g., pure config),
  state that explicitly rather than fabricating trivial tests.

## Related Prompt File
`.github/prompts/generate-tests.prompt.md`
