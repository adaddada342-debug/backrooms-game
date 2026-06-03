# Question Validation Pass

## Purpose
Validate traces by the question they create.

## Emotional Target
curiosity pressure

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Every placed trace must have a question.
- The question must be legible from context.
- The question must not be immediately answered unless part of a longer chain.

## Implementation Notes
- Debug field: StoryQuestion.
- Editor validator: flag traces with empty/generic questions.
- Review prompt: “What does this make the player wonder?”

## Codex Instructions
- Implement this as data-driven rules, not hardcoded one-off prop spam.
- Add editor/debug validation where practical.
- When unsure, reduce trace density.

## Common Failure Modes
- Random prop scatter.
- Too many clues in one space.
- Immediate explanation.
- Theme-breaking object placement.

## Related Files
- None.
