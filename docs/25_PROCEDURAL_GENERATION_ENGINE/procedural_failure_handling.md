# Procedural Failure Handling

## Purpose
Invalid generated chunks must fail safely with fallback spaces and logs.

## Design Rules
- Keep output deterministic.
- Use emotional phase controls.
- Validate before spawn.
- Log failures.
- Preserve the Backrooms thesis.

## Implementation Notes
Implement as small modules that compose into the full generation pipeline. Expose values to debug UI and recipe data.

## Codex Instructions
Read the manifesto first. Add only the minimum code needed for this concept, with clear extension points.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
