# Codex Wave 05 Task Rules

## Purpose
Constrain Codex so it can implement systems without scope explosion.

## Design Rules
- One system at a time.
- Plan before editing.
- Add tests/debug views before content.
- Prefer data-driven architecture.
- Never implement monsters as default pressure.
- Do not invent lore to justify code.

## Implementation Notes
Use task slices: schema -> validator -> debug UI -> minimal runtime -> save support -> tests -> tuning hooks.

## Codex Instructions
If a task touches generation, survival, streaming, or discovery, read the relevant Wave 05 folder first.

## Common Failure Modes
- Treating this as generic survival or horror content.
- Adding complexity before the base loop proves itself.
- Ignoring scale, silence, absence, and uncertainty.

## Related Files
- `docs/_INDEX/WAVE_05_INDEX.md`
- `docs/00_BRAIN/brain.md`
- `docs/01_PROJECT_VISION/project_vision.md`
