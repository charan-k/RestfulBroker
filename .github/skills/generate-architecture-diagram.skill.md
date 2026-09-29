# Skill: Generate Architecture Diagram

## Owner Agent
Architecture Agent

## Purpose
Produces a mermaid.js component diagram directly from requirements.md,
giving a visual, reviewable representation of the proposed system structure.

## Trigger
Invoked during Step 2 (Architecture), after requirements.md has been read in full.

## Inputs
- `requirements.md` (file, read-only)

## Outputs
- A mermaid.js code block representing components, data flow direction,
  and external system integrations.
- A short legend explaining each node in the diagram.

## Preconditions
- requirements.md must exist and be non-empty.

## Postconditions
- Diagram is inserted into architecture.md under the "Component Diagram" section.
- No other section of architecture.md is modified by this skill.

## Failure Modes
- If requirements.md is missing or empty, respond "Not Found" and halt —
  do not generate a diagram from assumptions.
- If requirements.md contains conflicting requirements, flag the conflict
  instead of silently picking one interpretation.

## Related Prompt File
`.github/prompts/generate-architecture-diagram.prompt.md`
