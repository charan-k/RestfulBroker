# Repository-Wide Copilot Instructions

- This repo follows an Agentic SDLC process with 8 defined stages.
- Always check for existing requirements.md, architecture.md, design-review.md,
  and impl-plan.md before proposing new designs — build on them, don't duplicate.
- Never fabricate data. If information is missing, respond with "Not Found"
  and flag it explicitly rather than guessing.
- All code must include error handling for: missing files, empty API responses,
  and invalid user input.
- Never include secrets, tokens, or credentials in any file or output.
- Prefer small, single-responsibility functions over large ones.
- All new logic must be accompanied by unit tests covering happy path
  and edge cases (empty/missing data).
- The canonical artifact set is: requirements.md, architecture.md,
  design-review.md, impl-plan.md, the Verification report, and CHANGELOG.md.
  Always read these before writing new content; never duplicate them.
- Any change proposed by an agent must be explicitly approved by a human
  before it is applied — this includes implementation edits, PR creation,
  and updates to architecture.md after a design review.
- The "Not Found" marker is canonical. Use it verbatim inside written
  documents (not just chat) so downstream agents can detect unresolved
  items rather than silently absorbing them.
