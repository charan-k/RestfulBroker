---
mode: 'code-review-agent'
description: 'Scan dependency manifests for known-vulnerable package versions'
---
Scan all dependency manifest files in the repo (package.json, requirements.txt,
pom.xml, go.mod, etc. — whichever apply).

For each dependency:
- Flag if the version is known-vulnerable or significantly outdated.
- Suggest the minimum safe version to upgrade to.
- If no vulnerability data is available for a package, mark it "Not Found"
  rather than assuming it's safe.

Output as a table: Package | Current Version | Status | Recommended Action.
