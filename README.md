# Restful Booker API Test Suite

This repository contains a .NET 8 NUnit API test suite for the public Restful Booker API. The project validates configuration, unauthenticated API behavior, and authenticated booking CRUD flows while avoiding hardcoded secrets.

## Prerequisites

- .NET 8 SDK
- Optional credentials for authenticated tests:
  - `BOOKER_USERNAME`
  - `BOOKER_PASSWORD`
  - Optional override for the API endpoint: `BOOKER_BASE_URL`

## Local execution

Set the environment variables before running the suite:

```powershell
$env:BOOKER_BASE_URL = "https://restful-booker.herokuapp.com"
$env:BOOKER_USERNAME = "<username>"
$env:BOOKER_PASSWORD = "<password>"

dotnet test .\RestfulBookerApiTests\RestfulBookerApiTests.csproj --logger "nunit;LogFileName=restful-booker.xml" --results-directory .\TestResults
```

When the credential variables are missing, authenticated tests are intentionally skipped and the reason is emitted in the NUnit output.

## Results

NUnit XML output is written to the repository-relative `TestResults/` directory.

## CI

GitHub Actions is the supported CI platform for this project. The workflow in `.github/workflows/api-tests.yml` runs the suite on pull requests and manual dispatch, then uploads the `TestResults/*.xml` artifact.
