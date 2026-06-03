# Cache, Download, and Streaming Manifesto

## Purpose
The game should grow as the player becomes lost. Storage growth becomes evidence of exploration, not a predatory lock.

## Design Rules
- Base install must remain reasonable.
- Level packs download on demand.
- Shared assets must be reused aggressively.
- Old areas can be evicted and redownloaded.
- Cache size should be visible and diegetically framed.
- Credits/licensing metadata travels with every asset pack.

## Implementation Notes
Use addressable/content bundle systems, local cache database, asset dependency graph, size budgeting, and validation before loading new packs.

## Codex Instructions
Do not block content behind storage purchases. Make cache management transparent and respectful.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
