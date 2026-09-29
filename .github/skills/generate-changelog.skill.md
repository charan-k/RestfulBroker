# Skill: Generate Changelog

## Owner Agent
PR Agent

## Purpose
Produces a changelog entry in Keep a Changelog format (Added/Changed/Fixed)
reflecting only the actual diff introduced by this Merge Request.

## Trigger
Invoked during Step 8 (Merge Request), alongside the Merge Request description generation.

## Inputs
- The code diff / list of files changed in the current implementation.

## Outputs
- A changelog snippet with Added/Changed/Fixed subsections.

## Preconditions
- At least one file must have been modified as part of this PR's scope.

## Postconditions
- Changelog entry is ready to append to CHANGELOG.md (manually or via
  a follow-up edit action).

## Failure Modes
- If no meaningful changes are detected (e.g., only whitespace/formatting),
  state that explicitly rather than inventing a changelog entry.

## Related Prompt File
`.github/prompts/generate-changelog.prompt.md`
