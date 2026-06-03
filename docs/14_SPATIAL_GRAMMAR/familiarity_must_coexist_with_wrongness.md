# Familiarity Must Coexist With Wrongness

## Purpose
Define how to use the spatial rule 'Familiarity Must Coexist With Wrongness' in level construction.

## Emotional Target
The player senses a real architectural rule being bent just enough to become mentally invasive.

## Design Rules
- Establish the normal version of the rule first.
- Apply deviation slowly.
- Do not make the rule obvious as a puzzle mechanic.
- Let the player notice through repeated exposure.

## Implementation Notes
- Represent this rule as generator constraints, not one-off scripted chaos.
- Add debug labels only in editor, never to player UI.
- Use seeded variation so players can share discoveries.

## Codex Instructions
- When generating a level, explicitly state whether this rule is active.
- If active, document how the player can infer it without direct explanation.

## Common Failure Modes
- Turning spatial wrongness into a gimmick.
- Breaking navigation so badly the player blames the game, not the world.
- Using randomness where grammar is needed.

## Related Files
- docs/13_ARCHITECTURAL_PHILOSOPHY/former_function_must_be_legible.md

## Source Basis
Derived from the Wave 03 architecture and level-design extraction of `docs/_SOURCE/deep-research-report.md`. Keep the original report available as the canonical source mirror.
