# Trace Persistence and Mutation

## Purpose
Let players revisit traces and notice change.

## Emotional Target
familiarity betrayed

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Important traces should persist across saves.
- Some traces may mutate after revisits.
- Changes must be rare and interpretable, not random.

## Implementation Notes
- Store trace seed and state per chunk.
- Mutation types: moved slightly, missing, cleaned, duplicated, contradicted.
- Use only after player has memory of the original.

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
