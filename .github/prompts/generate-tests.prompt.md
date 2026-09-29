---
mode: 'implementation-agent'
description: 'Generate unit + integration tests for a target file, covering happy path and edge cases'
---
You are generating tests for a single target file/module. Do not modify
production code — only add or update tests.

## Step 1 — Detect the test framework
Inspect the repo to identify the existing test framework and conventions:
- Language manifest (package.json, requirements.txt, pom.xml, go.mod, *.csproj)
- Existing test folder layout and naming pattern
- Test runner command (npm test, pytest, mvn test, go test, dotnet test, etc.)

If no test framework can be detected, output "Not Found — please specify
which framework to use" and STOP. Do not introduce a new framework unprompted.

## Step 2 — Analyse the target file
Identify each exported / publicly reachable unit in the target file. If the
file has no clearly testable units (e.g., pure config, constants only),
state that explicitly and STOP — do not fabricate trivial tests.

## Step 3 — Generate tests
For each testable unit, produce tests covering:
- Happy path (typical valid input → expected output)
- Edge cases: empty input, missing/undefined fields, "Not Found" data,
  boundary values
- Error conditions: exceptions, API failures, invalid input
- Integration path where the unit calls external systems or other modules

Rules:
- Follow the repo's existing test file naming and folder location.
- Do not mock away the exact condition the test claims to verify.
- Each test name must describe the scenario in plain English.

## Step 4 — Coverage report
After writing the tests, report:
- New/updated test file paths and test counts
- Scenarios covered (bullet list)
- Scenarios NOT covered and why (bullet list) — mark as "Not Found" if the
  target file makes them unreachable

## Failure Modes
- Framework not detectable → "Not Found" and halt.
- Target file missing or empty → "Not Found" and halt.
- No testable units in the file → state explicitly and halt; do not
  fabricate placeholder tests.
