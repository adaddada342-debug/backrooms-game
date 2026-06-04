# Wave 7: Layout Debugging and Readability

Wave 7 makes generated layouts easier to inspect, compare, debug, and play-test before real assets or AI-assisted systems enter the pipeline.

This wave introduces:

- Landmark visual placeholders.
- Seed comparison reports.
- Layout gizmos and debug components.
- Route readability scoring.
- Segmented primitive wall openings.
- Transition loop safety.

The project is still primitive-only and local-only. The generated scene remains a blockout, and the goal is to evaluate whether a layout is understandable, traversable, and inspectable in the Unity editor.

Seed comparison is used to evaluate variation and stability across deterministic Level 0 layouts. Route readability is tracked separately from basic route existence: a layout can have a connected route and still be hard to read.

Landmark placeholders are visual debug anchors, not final assets. They help confirm whether the landmark list supports navigation and memory without requiring a real art pass.

Segmented wall openings replace the Wave 4.1 simplification that omitted an entire wall when an opening existed. Wave 7 creates approximate wall sections around door openings while keeping debug opening markers visible.

## Non-Goals

- No AI.
- No online asset sources.
- No Addressables.
- No enemies.
- No real art pass.
- No final procedural generation.
