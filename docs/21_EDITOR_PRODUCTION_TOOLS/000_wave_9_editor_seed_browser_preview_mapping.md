# Wave 9: Editor Seed Browser, Preview, and Mapping Prototype

Wave 9 makes the content-generation pipeline easier to operate, inspect, repeat, and debug from inside the Unity editor.

This wave introduces:

- Seed browser/editor window.
- Layout preview summaries.
- Route visualization.
- Node-bound landmark placement.
- Primitive material asset generation.
- Soundscape clip slot scaffolding.
- First mapping-note placement prototype.

The workflow remains local and editor-driven. No AI is run, no external assets are downloaded, and no real asset importing happens. The seed browser helps compare seeds and generate scenes without manually running disconnected menu items.

Route visualization helps verify generated layout quality. Node-bound landmark placement replaces the older fallback landmark distribution, so debug landmarks are attached to specific generated rooms. Mapping notes are the first step toward the player-facing exploration and mapping fantasy.

Primitive material assets are optional generated Unity assets, not final art.

## Non-Goals

- No AI.
- No online asset sources.
- No Addressables.
- No enemies.
- No final art pass.
- No final audio pass.
- No runtime level generation.
- No multiplayer.
