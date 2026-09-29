# Implementation Plan: Restful Booker Automated API Test Suite

## Plan Assumptions

- GitHub is the canonical source repository.
- The test suite is a new root-level .NET 8 solution with one NUnit API-test
  project.
- No credentials, tokens, private keys, or CI secrets are committed.
- Unresolved values are marked `"Not Found"` and block only dependent work.

## Phase 0 — Human and Environment Prerequisites

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P0-1 | Install .NET 8 SDK | Install and verify the .NET 8 SDK; the current environment has only the .NET 8 runtime. | None | High | S | No |
| P0-2 | Provide GitHub repository URL | Provide the approved GitHub repository URL. Add `origin` only after explicit approval through the SDLC Git preflight. | None | High | S | No |
| P0-3 | Define credential contract | Define the environment-variable names used for Restful Booker credentials and the GitHub Actions secret mapping. **Not Found**. | None | High | S | No |
| P0-4 | Define CI retention policy | Define the GitHub Actions report-artifact retention duration. **Not Found**. | None | Medium | S | No |

## Phase 1 — Repository and Project Setup

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P1-1 | Add .NET Git exclusions | Add `bin/`, `obj/`, `TestResults/`, coverage output, IDE user files, and local configuration files to `.gitignore`. | None | High | S | No |
| P1-2 | Create solution and test project | Create the root-level .NET solution and NUnit API-test project targeting .NET 8. | P0-1 | High | M | Yes |
| P1-3 | Add approved packages | Add RestSharp, NUnit, NUnit3TestAdapter, Microsoft.NET.Test.Sdk, and a compatible NUnit XML logger after selection/approval. | P1-2 | High | S | Yes |
| P1-4 | Configure report output | Configure `dotnet test` and the selected NUnit XML logger to write test results to `TestResults/`; prove the command produces a valid XML report. | P1-3 | High | M | Yes |

## Phase 2 — Core Test Infrastructure

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P2-1 | Implement runtime configuration | Implement base-URL resolution: default to the public endpoint and override through `BOOKER_BASE_URL`; validate invalid URLs with clear errors. | P1-2 | High | M | Yes |
| P2-2 | Implement credential handling | Read the approved credential environment variables; avoid logging values; provide a clear skip reason when credentials are unavailable. | P2-1, P0-4 | High | M | Yes |
| P2-3 | Add API models and test data | Add typed auth, booking, booking-date, and booking-ID models plus unique valid booking-data generation. | P1-2 | High | M | Yes |
| P2-4 | Implement safe retry executor | Retry transient network/HTTP 5xx failures up to two times only for safe/idempotent operations; never retry expected 4xx assertions or automatically retry POST/PUT/PATCH. | P1-2 | High | M | Yes |
| P2-5 | Implement API clients | Implement isolated RestSharp authentication and booking clients for token generation, create, get, put, patch, and delete operations. | P2-1, P2-3, P2-4 | High | L | Yes |
| P2-6 | Test infrastructure units | Add unit tests for configuration, missing credentials, invalid URL handling, retry eligibility, retry exhaustion, and model/test-data helpers. | P2-1, P2-2, P2-3, P2-4 | High | M | Yes |

## Phase 3 — API Test Scenarios

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P3-1 | Add unauthenticated read tests | Test valid booking retrieval and explicit non-existent booking HTTP 404 behavior without depending on the CRUD workflow state. | P2-3, P2-5 | High | M | Yes |
| P3-2 | Add ordered CRUD fixture | Add a non-parallelized NUnit fixture that creates one booking, shares its ID through the ordered workflow, validates get/put/patch/delete, verifies final 404, and cleans up in teardown. | P2-2, P2-3, P2-5 | High | L | Yes |
| P3-3 | Add negative authentication tests | Assert invalid/missing-token response behavior for PUT/PATCH/DELETE. Mark the fixture skipped with a clear NUnit reason when required credentials are unavailable. | P2-2, P2-3, P2-5 | High | M | Yes |
| P3-4 | Validate local suite behavior | Run `dotnet test` with and without credentials; verify ordered fixture behavior, expected skips, XML output, and no secret leakage in logs. | P1-4, P3-1, P3-2, P3-3 | High | M | Yes |

## Phase 4 — CI Delivery

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P4-1 | Add GitHub Actions workflow | Create `.github/workflows/api-tests.yml` for Pull Request and manual-dispatch runs; install .NET 8, run the suite, and publish the NUnit XML artifact using the approved retention policy. | P0-3, P0-4, P3-4 | High | M | Yes |
| P4-2 | Validate GitHub Actions behavior | Confirm the canonical GitHub repository workflow executes for the expected event/manual triggers and publishes the XML result artifact. | P4-1 | High | M | Yes |

## Phase 5 — Documentation and Final Validation

| ID | Task | Description | Dependency | Priority | Complexity | Blocked |
|---|---|---|---|---|---|---|
| P5-1 | Document local execution | Update the project README with prerequisites, `BOOKER_BASE_URL`, credential behavior, local `dotnet test` execution, skipped-test behavior, and report location. Do not document secret values. | P3-4 | Medium | S | Yes |
| P5-2 | Document CI configuration | Document GitHub Actions secret setup by variable name only, workflow triggers, and report retrieval. Mark unresolved platform settings `"Not Found"`. | P4-2, P5-1 | Medium | S | Yes |
| P5-3 | Run final verification | Run the full suite and verify report generation, documentation completeness, Git diff scope, GitHub Actions definitions, and absence of committed secrets. | P5-1, P5-2 | High | M | Yes |

## Blocked Tasks

| Task | Blocking dependency | Blocking owner | Required resolution |
|---|---|---|---|
| P1-2 through P5-3 | P0-1 | Human | Install and verify the .NET 8 SDK. |
| P2-2, P3-2, P3-3, P4-1, P4-2 | P0-3 | Human | Define credential environment-variable names and GitHub Actions secret mappings. |
| P4-1 and P4-2 | P0-4 | Human | Define GitHub Actions test-report retention duration. |
| Push/Pull Request actions after implementation | P0-2 | Human | Provide and approve the GitHub repository URL for `origin`. |
| P1-3 and P1-4 | NUnit XML logger selection | Copilot, subject to human approval | Select a compatible logger/package and exact `dotnet test` command during implementation. |

## Implementation Sequence

1. Complete P0-1 through P0-4.
2. Complete Phase 1 setup and prove NUnit XML report output.
3. Implement and unit-test core configuration, credential handling, models,
   retry logic, and API clients.
4. Implement scenario tests, then prove local run behavior with and without
   credentials.
5. Add the GitHub Actions workflow only after the required GitHub remote,
   secret, and retention decisions are complete.
6. Document usage and perform final verification.
