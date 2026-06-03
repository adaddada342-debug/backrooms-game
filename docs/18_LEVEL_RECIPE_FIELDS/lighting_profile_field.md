# Lighting Profile Field

## Purpose
Define LevelRecipe data requirements for Lighting Profile Field.

## Emotional Target
Codex can generate consistent levels because each level carries its emotional and architectural DNA in data.

## Design Rules
- Required subfields: source type, flicker rules, coverage, failure, silence events.
- Every field must support both authored and procedural levels.
- Fields must be readable by validation tools.

## Implementation Notes
- Use JSON/YAML/ScriptableObject/DataAsset equivalent depending on engine.
- Every generated level must output a resolved recipe snapshot.
- Do not bury these values inside scene-only objects.

## Codex Instructions
- If implementing code, create schema validation before content scale-up.
- If generating content, fill all fields before placing assets.

## Common Failure Modes
- Implicit design hidden in scripts.
- No validation.
- Recipes that describe assets but not emotional purpose.

## Related Files
- docs/17_GENERATOR_DESIGN_RULES/generator_validation_reports.md

## Source Basis
Derived from the Wave 03 architecture and level-design extraction of `docs/_SOURCE/deep-research-report.md`. Keep the original report available as the canonical source mirror.
