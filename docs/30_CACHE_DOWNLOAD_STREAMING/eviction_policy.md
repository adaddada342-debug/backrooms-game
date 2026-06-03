# Eviction Policy

## Purpose
Old content can be removed safely while preserving discovery records.

## Design Rules
- Storage growth supports the fantasy, but must stay respectful.
- Credits travel with content.
- Validation happens before runtime load.
- Cache must be inspectable and repairable.

## Implementation Notes
Implement manifest parsing, local registry, size accounting, dependency resolution, and debug UI before real downloads.

## Codex Instructions
Do not build live downloads before local pack simulation works.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
