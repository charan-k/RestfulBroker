# Skill: Generate Clarifying Questions

## Owner Agent
Requirements Agent

## Purpose
Turns a raw, possibly ambiguous user story into a structured set of clarifying
questions so that requirements.md is built on confirmed facts, not assumptions.

## Trigger
Invoked at the start of Step 1 (Requirements), immediately after a user story
is pasted into chat, and before any requirements.md content is drafted.

## Inputs
- `userStory` (string) — the raw text of the user story from JIRA/Confluence/Word.

## Outputs
- A categorized list of clarifying questions, grouped under:
  - Scope boundaries
  - Data/input sources
  - Edge cases & error conditions
  - Non-functional requirements

## Preconditions
- No requirements.md exists yet, OR the user has indicated the story is new/changed.

## Postconditions
- No file is written by this skill. It only produces questions.
- Human must respond before the Requirements Agent proceeds to drafting requirements.md.

## Failure Modes
- If the user story is empty or unreadable, respond "Not Found" and ask
  the user to repaste it — do not generate generic filler questions.

## Related Prompt File
`.github/prompts/generate-clarifying-questions.prompt.md`
