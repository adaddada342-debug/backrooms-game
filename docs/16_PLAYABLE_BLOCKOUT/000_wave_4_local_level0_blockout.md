# Wave 4 Local Level 0 Blockout

## Purpose
Wave 4 proves the first playable Unity path from fake/local package metadata to a local Level 0 blockout scene and transition trigger.

This is not the full game. It is a local blockout and loading-path proof that uses Unity primitives only.

## Runtime Proof
The Wave 4 scene path proves that the runtime loader can target a local scene from package-style metadata. The blockout includes a primitive player capsule, simple first-person movement, placeholder rooms and corridors, fluorescent light bars, and one transition trigger.

## Data Boundary
Wave 4 uses fake/local Level 0 metadata. It does not use real assets, downloading, importing, Addressables, AI, multiplayer, enemies, survival systems, or procedural generation.

## Scene Assembly Boundary
The primitive scene builder is an editor-only tool. It creates a local blockout scene with generated primitive geometry and writes an assembly report. Runtime components stay small: a simple first-person controller, a transition trigger, and the existing level loader path.

## Future Replacement Path
Future waves can replace the primitive blockout with a data-driven room grammar, scene assembly validation, navigation sanity checks, mapping note placement tests, and safer scene transition loop tests.

