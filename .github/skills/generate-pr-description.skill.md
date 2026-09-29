# Skill: Generate PR Description

## Owner Agent
PR Agent

## Purpose
Assembles the complete, structured Merge Request description required to close the
agentic SDLC loop, pulling only from verified project artifacts.

## Trigger
Invoked during Step 8 (Merge Request), after Verification (Step 7) has passed with no
blocking test failures.

## Inputs
- requirements.md
- impl-plan.md
- Verification Agent's test/documentation report

## Outputs
- A Merge Request description containing exactly five sections: Summary, Changes Made,
  Test Evidence, Known Limitations, Reviewer Checklist.

## Preconditions
- All four upstream artifacts (requirements.md, architecture.md,
  design-review.md, impl-plan.md) must exist.
- Verification must have completed with a pass/fail result already known.

## Postconditions
- Merge Request description is complete and ready to paste into the actual Merge Request —
  no placeholder text remains.

## Failure Modes
- If any upstream artifact is missing, halt and list exactly which one(s) —
  do not generate a Merge Request description with fabricated traceability.
- If test evidence is unavailable, mark "Test Evidence: Not Found" rather
  than omitting the section.

## Related Prompt File
`.github/prompts/generate-pr-description.prompt.md`
