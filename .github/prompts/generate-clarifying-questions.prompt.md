---
mode: 'requirements-agent'
description: 'Generate structured clarifying questions from a raw user story'
---
Read the user story provided in chat. Do not assume any missing details.

Produce a categorized list of clarifying questions grouped under these
exact four headings:

### Scope boundaries
- (2-5 questions about what is in / out of scope)

### Data / input sources
- (2-5 questions about where inputs come from and their formats)

### Edge cases & error conditions
- (2-5 questions about failure modes, empty inputs, "Not Found" cases)

### Non-functional requirements
- (2-5 questions about performance, security, availability, compliance)

Rules:
- Every question must be answerable with a concrete fact — no vague prompts.
- If the story is empty or unreadable, output only: "Not Found — please
  repaste the user story." Do not fabricate questions.
