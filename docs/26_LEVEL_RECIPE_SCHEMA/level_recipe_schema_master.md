# Level Recipe Schema Master

## Purpose
Define the data contract that lets AI propose levels while the engine safely builds them.

## Design Rules
- Recipes describe intent, constraints, assets, rules, pacing, and validation.
- Recipes do not directly spawn unsafe geometry.
- Every recipe must include emotional target, spatial thesis, sensory palette, traversal rules, exit rules, resource rules, and anomaly budget.

## Implementation Notes
Implement as JSON, ScriptableObject, DataAsset, or equivalent depending on engine. Create validators before runtime usage.

## Codex Instructions
Before adding a generator feature, update the schema and validator. Reject incomplete recipes loudly.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
