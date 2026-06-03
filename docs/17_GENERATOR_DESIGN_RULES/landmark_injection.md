# Landmark Injection

## Purpose
Define the procedural generation rule 'Landmark Injection' for Codex and generator systems.

## Emotional Target
Procedural output should feel authored by an impossible place, not shuffled by a dice bag.

## Design Rules
- Use hierarchical generation: macro layout, zone graph, room grammar, micro detail, anomaly pass.
- Bind every generated element to emotional and functional purpose.
- Validate generated spaces for legibility, drift, and emotional thesis.
- Keep anomalies rare enough to matter.

## Implementation Notes
- Represent rules as data wherever possible.
- Emit generation debug reports.
- Store generator seed, level recipe ID, zone graph, and anomaly placements.
- Support deterministic regeneration.

## Codex Instructions
- Before coding, identify which LevelRecipe fields this rule requires.
- Do not implement random placement without validation.
- Do not create irreversible spaghetti inside scene-only scripts.

## Common Failure Modes
- Random room soup.
- No memory landmarks.
- Overgeneration of weirdness.
- Performance-heavy procedural meshes without caching.

## Related Files
- docs/18_LEVEL_RECIPE_FIELDS/level_recipe_master_schema.md

## Source Basis
Derived from the Wave 03 architecture and level-design extraction of `docs/_SOURCE/deep-research-report.md`. Keep the original report available as the canonical source mirror.
