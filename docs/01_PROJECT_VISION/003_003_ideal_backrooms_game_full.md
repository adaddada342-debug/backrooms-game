# 003 Ideal Backrooms Game Full

## Purpose
Full Wave 01 mirror of the report section defining the ideal game.

## Source Extract
The ideal Backrooms exploration game is *not* best described as survival horror, walking simulator, open-world sandbox, or extraction game. It is best described as an **atmospheric cartographic obsession simulator** wrapped in liminal horror. Its fundamental loop is:

**notice → test → record → compare → doubt → adapt → go deeper.** citeturn43view0turn41view3turn41view2turn42view0

The **core gameplay loop** should begin with directed but fragile curiosity. The player enters a sector, samples its sensory profile, identifies a few stable anchors, tests whether local rules hold, and decides whether to push deeper, retreat, or establish a micro-base. The “reward” for this is not primarily loot. It is improved theory, more precise maps, stronger environmental recognition, and occasional profound discoveries that rescale the player’s understanding of the whole. Consumables exist, but they are in service of exploration. Water matters because it lets you stay out longer; batteries matter because they alter what can be perceived; chalk, tape, thread, and markers matter because they let you externalise cognition. citeturn28view0turn41view2turn42view0

The **exploration systems** should support multiple kinds of curiosity simultaneously. Perceptual curiosity is driven by strange sightlines, odd sounds, or out-of-place objects. Manipulatory curiosity is driven by doors, panels, rolling shutters, valves, lifts, drawers, vending machines, fuse boxes, and movable furniture. Conceptual curiosity is driven by inconsistent layout logic, repeated room families, numbering systems, and traces that imply hidden rules. Social curiosity is driven indirectly through evidence of other wanderers—notes, camps, provenance marks, transactional traces, missing names. The player should often discover something long before understanding its role. citeturn43view0turn41view3

The **discovery systems** should distinguish between *micro discoveries* and *ontological discoveries*. Micro discoveries are things like a new exit trick, a safe water source, a recurrent acoustic cue, a maintenance pattern, or a room family that always sits near a more important anomaly. Ontological discoveries are things like proof that two distant sectors overlap materially, that one level remembers another, that the map cannot be global, or that certain front-world memories are unreliable after long exposure. The game must heavily ration ontological discoveries. Each one should reorder dozens of earlier observations. citeturn41view2turn35view1turn29view1

The **mapping systems** should be some of the best ever made in games, but intentionally imperfect. The player should be able to sketch, annotate, name, colour-code, and attach sensory notes to locations. They should be able to pin relationships with uncertainty markers such as “seems connected”, “heard water here”, “probably loops”, “do not trust exit sign”, or “safe only while hum continues”. Some maps should be local and accurate. Others should start reliable and drift. Multiplayer annotations should not instantly merge into objective truth; they should remain sourced, contested, and time-stamped. This keeps mapping as a *human struggle against incomprehensible space*, not as a solved UI problem. citeturn28view0turn41view2turn39view0

The **multiplayer systems** should preserve loneliness rather than erase it. Standard real-time co-op with unrestricted voice chat is psychologically disastrous for this premise because companionship collapses liminality into banter. A stronger model is asymmetric or delayed presence: ghost traces of other players’ routes, dropped artefacts, asynchronous notation, intermittent line-of-sight sightings at impossible distances, or rare radio windows with distortion and lag. Direct co-presence, when allowed, should be unstable. Two players may try to meet and repeatedly miss each other because the environment refuses shared certainty. That would turn the pain of separation into a system rather than a cutscene. citeturn15view0turn23search20turn27view1

The **survival systems** should be light but nontrivial. Hunger and thirst can exist, but not as farming chores. More interesting are fatigue, overstimulation, flashlight dependence, disorientation, map unreliability under stress, and derealisation-like effects after prolonged exposure. Resting should help, but safe rest should be socially or ontologically suspicious: quiet break rooms, drained indoor pools, maintenance closets, warmed utility chambers, camps built by unknown people. Medicines, stimulants, and calming tools should alter perception in trade-off heavy ways: better focus but worse sleep; clearer hearing but intensified hum; reduced panic but dulled curiosity. The aim is to make the player manage the *cost of staying with the building*, not merely calories. citeturn29view1turn32search14turn32search17

The **VR implications** are enormous. VR would make scale, corridor geometry, angular visibility, sound direction, and body vulnerability vastly stronger. It would also dramatically increase the impact of false familiarity, because head movement and proprioception make repeated spaces easier to learn and therefore more painful when subverted. In VR, tiny anomalies matter more: a ceiling panel slightly too low, a room whose reverb seems too large for its size, a handrail warmer than the air, a faint office smell in a pool sector. VR would also make stillness a mechanic. Standing in silence in VR while hearing impossible maintenance sounds could be more effective than any chase encounter. citeturn28view0turn27view1

The **procedural generation principles** should be strict. Generation should begin with a semantic grammar of place types, not noise fields. Regions need macro-structure, circulation hierarchies, service relationships, scent/light/acoustic identities, and memory anchors. The system should know the difference between a transit hall, dead-end nest, service void, office comb, pool cluster, storage spine, atrium wound, refuge pocket, and contamination seam. Once the grammar exists, anomalies can deform it. This supports the exact structure required by uncanny-place research: strong familiarity first, meaningful deviation second. Broadly, generation should follow four layers: typology, circulation, traces, anomaly. Only the last layer should be heavily surprising. citeturn27view1turn42view0turn44search0turn44search8

The **long-term progression** should be epistemic, not power-fantasy. Instead of levelling combat stats, the player deepens observational competence, environmental literacy, local trust networks, and archival depth. Possible long-term unlocks include better notation tools, improved environmental sensors, access to inter-sector transport rituals, or the right to use someone else’s map archive. The player should feel more *read* by the Backrooms as they progress, not simply stronger. Long-term play should culminate in the real horror: the player becomes fluent in an environment no human should be fluent in. citeturn41view3turn43view0turn35view1

The **global discovery systems** should make the whole player base feel like an expeditionary literature, not a leaderboard. Shared findings should arrive as contested cartographic reports, recovered audio, partial schematics, player-submitted anomaly clusters, and community hypotheses. Major global events should not be boss raids. They should be things like: a sector family has begun duplicating; doors marked 17 have disappeared game-wide; warm rooms now smell of rain; a known safe camp’s lights have all been replaced with different bulbs. This keeps the social layer interpretive and archival rather than competitive. citeturn41view2turn39view0

The single clearest design target is this emotional sentence:

**The player should begin by exploring a strange place, then gradually realise they are living inside a machine for turning ordinary architecture into unstable memory.**

If the game achieves that, it will feel more like the Backrooms than a hundred monsters ever could. citeturn22view0turn34view0turn27view1

## Design Rules
- Preserve the Backrooms as an affective, architectural, liminal experience rather than a monster-first horror game.
- Every feature must serve scale, absence, isolation, mystery, uncertainty, exploration, or spatial wrongness.
- Do not solve unease with constant threat, lore dumping, entities, or objective spam.

## Implementation Notes
- Prefer data-driven definitions, stable schemas, and testable systems.
- Any future code must be traceable back to a design rule or emotional target.
- If a mechanic makes the player feel powerful, safe, or fully certain, justify it as temporary and suspicious.

## Codex Instructions
- Read related files before implementation.
- Do not invent new mechanics that contradict the emotional target.
- When uncertain, preserve ambiguity and ask for a design decision rather than filling with generic horror.

## Related Files
- docs/00_BRAIN/004_project_thesis.md
- docs/_SOURCE/deep-research-report.md

## Notes
- Wave 01 foundation file.
