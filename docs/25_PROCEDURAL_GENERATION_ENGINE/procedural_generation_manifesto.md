# Procedural Generation Manifesto

## Purpose
Define procedural generation as emotional architecture, not random content multiplication. The generator must create infinite reasons to keep walking, not infinite copies of corridors.

## Design Rules
- Use grammar before randomness.
- Every generated region needs a sensory thesis.
- Repetition must drift slowly.
- Rare anomalies must be expensive.
- Navigation must remain partly legible, then betray confidence selectively.
- Generated output must be seed-based and debuggable.

## Implementation Notes
Use hierarchical generation: Level Recipe -> Region Graph -> Spatial Grammar -> Module Placement -> Trace Injection -> Lighting Pass -> Audio Pass -> Validation Pass -> Save/Discovery Registration.

## Codex Instructions
When implementing generation, build small deterministic subsystems. Never create giant all-knowing generator classes. Always include seed, bounds, validation, logging, and debug visualisation.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
