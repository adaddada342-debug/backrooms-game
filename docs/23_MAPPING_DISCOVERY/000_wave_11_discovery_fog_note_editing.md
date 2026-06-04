# Wave 11 Mapping Discovery, Fog, and Note Editing

Wave 11 makes the prototype mapping system react to player exploration. It adds runtime room discovery tracking, fog-of-war / unknown room map state, player and current-room tracking, room hover/click detail selection, note editing/deleting prototype UI, a map note details panel, a compass/minimap text prototype, and upgraded mapping reports.

The map now reads generated `SceneAssemblyPlan` data through `GeneratedLevelRuntimeContext` and combines it with local save data. Rooms are discovered when the player starts near them or moves close enough to them. Discovery is saved locally, so the next play session can restore discovered rooms and notes when possible.

Unknown rooms can remain visible but dimmed, or be hidden by changing discovery settings. Notes can be selected on the map, edited locally, deleted locally, and saved back to the same local JSON file. This is still prototype UI and not final art.

Mapping is central to the game fantasy: players interpret space, annotate uncertainty, and gradually build their own understanding of impossible architecture.

Non-goals for Wave 11:

* no AI
* no online asset sources
* no Addressables
* no enemies
* no final UI art
* no multiplayer note sharing
* no cloud persistence
* no runtime level generation
