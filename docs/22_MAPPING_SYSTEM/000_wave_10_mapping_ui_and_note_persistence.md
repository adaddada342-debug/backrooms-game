# Wave 10: Mapping UI and Note Persistence

Wave 10 creates the first playable proof of the exploration and mapping fantasy:

```text
Generated layout data
-> Runtime level context
-> Player places notes
-> Notes persist locally
-> Map UI reads room graph and notes
-> Player inspects discovered space
```

This wave introduces:

- Runtime level context.
- Persistent local map note storage.
- Prototype map UI canvas.
- Room graph visualization.
- Connection line visualization.
- Route overlay visualization.
- Landmark and note map markers.
- Room identity/debug labels.
- Seed browser preview mini-map data.
- Mapping reports.

This is a prototype mapping system, not final UI. It does not save to cloud, does not network notes, and does not implement full undiscovered-room logic yet. It uses generated `SceneAssemblyPlan` data so the player can interpret space, annotate uncertainty, and build an early understanding of impossible architecture.

## Non-Goals

- No AI.
- No online asset sources.
- No Addressables.
- No enemies.
- No final UI art.
- No multiplayer note sharing.
- No cloud persistence.
- No runtime level generation.
