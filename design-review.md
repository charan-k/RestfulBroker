# Design Review: Restful Booker Automated API Test Suite

## Risks Identified

| # | Severity | Risk | Why it matters | Suggested mitigation |
|---|---|---|---|---|
| 1 | Resolved | The architecture requires GitHub Actions only while the canonical source repository is GitHub. | The risk is resolved at the architectural level by using the canonical GitHub repository directly for Pull Request and manual-dispatch runs. | Before implementation, define the GitHub Actions secret names, report-retention duration, and any protected-branch policy. |
| 2 | High | The retry component applies network/HTTP 5xx retries to the Booking API client without distinguishing idempotent from non-idempotent operations. | Retrying a failed `POST` after an ambiguous network/server failure can create duplicate bookings. Retrying `PUT`/`PATCH` may repeat externally visible updates. | Retry only safe/idempotent operations by default (`GET` and cleanup `DELETE`). For `POST`, `PUT`, and `PATCH`, surface the failure clearly unless an operation-specific idempotency strategy is approved. |
| 3 | High | The ordered shared booking workflow has no explicit NUnit concurrency rule. | Parallel execution can race on the shared booking ID, cause teardown to delete data during an active test, and make results nondeterministic. | Mark the CRUD fixture non-parallelizable; ensure CI does not shard/parallelize its ordered tests; allow independent read-only negative tests to remain separate. |
| 4 | High | Missing credentials intentionally skip authenticated CRUD and negative-authentication tests, allowing CI to pass without coverage of most core operations. | A green pipeline can represent only unauthenticated coverage, reducing regression confidence. | Label the run as degraded when authenticated tests are skipped. Human must decide whether protected-branch/release pipelines require credentials and should fail when they are absent. |
| 5 | Medium | NUnit XML output is required, but the architecture does not specify the logger/package and command needed to produce it. | `dotnet test` does not guarantee the required NUnit XML artifact without compatible logger configuration. | Select a compatible NUnit XML logger package and document the exact `dotnet test` logger/output command in the implementation plan. |
| 6 | Medium | The public Restful Booker API is shared, externally controlled, and may be unavailable, rate-limited, or changed. | Tests may be flaky; booking cleanup can fail; status/body assertions can drift from the public demo API. | Retain limited retry behavior for safe operations, use unique test data, always attempt teardown, and emit clear diagnostics without secrets. |
| 7 | Medium | The .NET 8 SDK is marked `"Not Found"` in the current environment. | Phase 5 cannot create, restore, build, or test the required .NET 8 project until the SDK is installed. | Install and verify the .NET 8 SDK before the Phase 5 Git/.NET preflight completes. |
| 8 | Medium | The GitHub repository URL and remote configuration are `"Not Found"`. | Approved implementation cannot be pushed or validated against the canonical repository until the remote is configured. | Human must provide the approved GitHub repository URL; the SDLC Git preflight must add `origin` only after explicit approval. |
| 9 | Low | CI secret names, report retention duration, and maximum suite duration are `"Not Found"`. | CI definitions may be incomplete or retain artifacts inconsistently across platforms. | Define these settings before finalizing CI configuration; do not hardcode credentials or assume retention defaults. |

## Gaps vs Requirements

1. **Requirements 22-24 are architecturally addressed.** The canonical GitHub
   repository runs directly in GitHub Actions for Pull Requests and manual
   dispatch. The exact GitHub Actions secret names and protected-branch
   policy remain **Not Found**.
2. **Requirement 18 needs safe-operation scope.** The architecture promises
   retries for transient network/5xx failures but does not prevent duplicate
   side effects on `POST`, `PUT`, or `PATCH`.
3. **Requirement 8 needs execution-isolation detail.** The ordered shared
   booking workflow is represented, but the architecture does not explicitly
   prevent parallel execution.
4. **Requirement 21 needs implementation detail.** The architecture identifies
   `TestResults/` but does not define the NUnit logger/package or exact command
   that generates the required XML report.
5. **Requirements 4, 6, and 16 are conditionally covered.** Authenticated
   tests can be skipped when credentials are absent, but no CI policy
   distinguishes a fully covered run from a degraded run.
6. **Not Found:** Exact authentication environment-variable names remain
   undefined.
7. **Not Found:** Exact GitHub Actions secret names and the protected-branch
   CI policy remain undefined.

## Agreed Decisions

- The suite will use C#, .NET 8, RestSharp, and NUnit.
- The default base URL is `https://restful-booker.herokuapp.com`;
  `BOOKER_BASE_URL` overrides it.
- The CRUD lifecycle may be ordered and share one booking ID; teardown must
  delete it when possible.
- Missing credentials skip authenticated tests with a clear NUnit XML reason
  rather than failing the run.
- NUnit XML results must be written under `TestResults/`.
- GitHub Actions is the required CI mechanism for the approved
  requirements.
- Approved: the canonical GitHub repository runs directly in GitHub Actions
  for Pull Request and manual-dispatch triggers.
- **Not Found:** The exact GitHub Actions secret names and the policy for
  credential absence on protected/release branches.

## Action Items

| # | Owner | Action |
|---|---|---|
| 1 | Human | Provide the exact GitHub Actions secret names and any protected-branch CI policy. |
| 2 | Copilot | Confirm the GitHub repository URL used by the Phase 5 preflight and the workflow trigger configuration. |
| 3 | Copilot | Amend retry architecture so automatic retries are restricted to safe/idempotent operations by default. |
| 4 | Copilot | Add a non-parallelized ordered CRUD fixture rule to the architecture and implementation plan. |
| 5 | Human | Decide whether missing credentials may pass protected/release CI pipelines or must fail those pipelines. |
| 6 | Copilot | Select and document the NUnit XML logger/package and CLI command during implementation planning. |
| 7 | Human | Install the .NET 8 SDK before Phase 5. |
| 8 | Human | Provide the report-retention duration before GitHub Actions definitions are implemented. |

## Applied Architecture Remediation

The following architecture changes were approved and applied:

1. Add this item to Assumptions & Risks:

   ```markdown
   | GitHub Actions execution topology | Approved: the canonical GitHub repository runs directly in GitHub Actions for Pull Request and manual-dispatch triggers. |
   ```

2. Replace the Retry executor responsibility with:

   ```markdown
   | Retry executor | Retry network and HTTP 5xx failures up to two times only for safe/idempotent operations by default. Do not automatically retry POST, PUT, or PATCH without an approved idempotency strategy. Never retry expected 4xx assertions. |
   ```

3. Replace the Ordered CRUD fixture responsibility with:

   ```markdown
   | Ordered CRUD fixture | Own one shared booking ID and execute the approved lifecycle sequence in a non-parallelized NUnit fixture. Perform cleanup in teardown when the booking remains. |
   ```

4. Add this NUnit XML logger risk item:

   ```markdown
   | NUnit XML logger | **Not Found.** Select a compatible logger package and exact `dotnet test` command before implementation so `TestResults/` receives the required XML artifact. |
   ```

5. Add this degraded-coverage risk item:

   ```markdown
   | Missing credentials in CI | Authenticated tests are skipped by approved requirement. Mark the CI run as degraded when this occurs. **Not Found:** whether protected/release branches must fail instead. |
   ```

## Re-Review Outcome

The GitHub-only topology resolves the prior critical topology gap. No
unresolved critical risks remain. Implementation Planning may proceed, while
Phase 5 remains blocked until the .NET 8 SDK and the GitHub repository remote
are configured.
