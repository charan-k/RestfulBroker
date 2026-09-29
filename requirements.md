# Requirements: Restful Booker Automated API Test Suite

## Overview

Build an automated API regression test suite for the public Restful Booker API.
The suite will be implemented in C# on .NET 8 using RestSharp and NUnit. It
will validate authentication, ordered booking CRUD behavior, negative/error
scenarios, runtime configuration, and CI execution.

The default API endpoint is `https://restful-booker.herokuapp.com`. It must be
overridable through the `BOOKER_BASE_URL` environment variable.

The solution will create a new root-level .NET solution containing one NUnit
API test project.

## Functional Requirements

1. The suite must target the Restful Booker API and support a default base URL
   of `https://restful-booker.herokuapp.com`.
2. The suite must use the `BOOKER_BASE_URL` environment variable, when
   supplied, instead of the default API base URL.
3. The suite must use C#, .NET 8, RestSharp, and NUnit.
4. The suite must authenticate with the Restful Booker API using externally
   supplied credentials and obtain an authentication token.
5. The suite must not hardcode API credentials in source code, configuration
   committed to Git, logs, test reports, or CI definitions.
6. When required authentication credentials are unavailable, authenticated CRUD
   and negative-authentication tests must be marked as skipped with a clear
   reason in the NUnit XML report. Their absence must not fail the test run.
7. The suite must create a booking using valid booking data and assert HTTP 200
   status, a valid booking identifier, and expected response-body structure and
   values.
8. The CRUD scenarios may execute as a defined ordered workflow and share the
   booking identifier created during workflow setup.
9. The suite must retrieve the workflow booking and assert HTTP 200 status and
   booking details matching the data created for the workflow.
10. The suite must retrieve a non-existent booking identifier and explicitly
    assert HTTP 404.
11. The suite must fully update the workflow booking through `PUT` using a
    valid authentication token and assert the expected updated booking values.
12. The suite must partially update the workflow booking through `PATCH` using
    a valid authentication token and assert that only the requested fields
    change.
13. The suite must delete the workflow booking through `DELETE` using a valid
    authentication token.
14. After deletion, the suite must retrieve the deleted booking identifier and
    explicitly assert HTTP 404.
15. Workflow teardown must delete the created booking when it still exists.
16. The suite must explicitly assert the API response for invalid or missing
    authentication tokens on `PUT`, `PATCH`, and `DELETE`, expecting HTTP 401
    or HTTP 403 as applicable.
17. The suite must assert both HTTP response status codes and relevant response
    body structure/content for every covered API operation.
18. The suite must retry only transient network failures and HTTP 5xx responses
    up to two times with a short backoff.
19. The suite must not retry expected HTTP 4xx assertions, including the
    expected 404, 401, and 403 scenarios.
20. The suite must support local execution through `dotnet test`.
21. The suite must produce NUnit XML test results in the repository-relative
    `TestResults/` directory.
22. The implementation must include and maintain a GitHub Actions CI
    definition at `.github/workflows/api-tests.yml`.
23. The GitHub Actions workflow must execute the API suite on Pull Request
    events and manually triggered workflow runs.
24. The GitHub Actions workflow must publish the NUnit XML result from
    `TestResults/` as a workflow artifact.

## Non-Functional Requirements

1. The test-suite project must build and run with the .NET 8 SDK.
2. Tests must use small, single-responsibility client, configuration, model,
   helper, and test classes.
3. Errors caused by missing configuration, invalid base URLs, missing
   credentials, network failures, malformed API responses, and missing
   test-result directories must be surfaced with clear messages and no
   unhandled exceptions.
4. Test output and CI logs must not expose credentials, authentication tokens,
   private SSH keys, access tokens, or other secrets.
5. The suite must be deterministic except for externally controlled API
   availability and intentionally generated booking data.
6. The implementation must include unit tests for newly introduced test-support
   logic, including configuration loading, missing environment variables, retry
   eligibility, and invalid input behavior.
7. The CI configuration must retain test reports for review after the pipeline
   completes.

## Out of Scope

- Testing Restful Booker UI or browser workflows.
- Performance, load, stress, penetration, or security testing beyond the
  specified API authentication-negative scenarios.
- Creating or maintaining a production Restful Booker API service.
- Automatically creating GitHub Pull Requests without explicit human
  approval.
- Storing Git, Jira, GitHub, or API credentials in committed repository
  files.
- Supporting API clients or test frameworks other than RestSharp and NUnit for
  this initial implementation.

## Open Questions

- **Not Found:** The exact externally supplied credential variable names for
  Restful Booker authentication have not been selected.
- **Not Found:** The GitHub Actions secret names/configuration mechanism for
  Restful Booker credentials have not been selected.
- **Not Found:** A required maximum test-suite execution time has not been
  defined.
- **Not Found:** CI test-report retention duration has not been defined.
