# Placement Physicality Rules

## Purpose
Make props feel physically placed by real actions.

## Emotional Target
material credibility

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Objects must obey gravity, reachability, use context, and human scale unless anomaly-tagged.
- Wear patterns must align with use.
- Trace placement should respect doors, counters, seating, utilities, and traffic paths.

## Implementation Notes
- Raycast to surfaces, validate support, avoid intersections.
- Use anchor sockets for common trace zones: desk, sink, chair, vent, threshold.
- Add rotation and usage variance, not random chaos.

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
