# Procedural Trace Metadata Schema

## Purpose
Define metadata for trace placement so procedural systems can reason about props.

## Emotional Target
structured implication

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Every trace needs Category, Freshness, ImpliedActor, Routine, Contradiction, Question, Rarity, and LevelFit.
- Metadata must be editable by designers.
- Do not use traces without metadata in procedural generation.

## Implementation Notes
- Suggested fields: TraceId, Category, EmotionalFunction, Freshness, StoryQuestion, PlacementRules, ExclusionRules, RelatedLevelArchetypes.
- Store in ScriptableObject/DataAsset/JSON depending on engine.
- Support seed-stable placement.

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
