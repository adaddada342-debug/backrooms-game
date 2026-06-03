# Freshness System

## Purpose
Control how close implied people feel to the player.

## Emotional Target
near-miss unease

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Immediate traces must be rare.
- Recent traces should not always mean danger.
- Old traces should create loneliness and history.
- Active traces should imply systems more often than people.

## Implementation Notes
- Freshness enum: Ancient, Old, Recent, Immediate, Active.
- Material hooks: wetness, warmth, dust absence, motion, sound, light.
- Use freshness decay over real or simulated time when useful.

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
