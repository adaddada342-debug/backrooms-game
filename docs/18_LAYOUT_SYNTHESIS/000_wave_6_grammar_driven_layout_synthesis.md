# Wave 6: Grammar-Driven Layout Synthesis

Wave 6 introduces deterministic editor-side layout synthesis for Level 0.

The synthesizer does not create final geometry. It creates semantic layout data that remains compatible with `SceneAssemblyPlan`. The existing primitive scene builder is still responsible for turning that plan into Unity primitive rooms, connectors, lights, openings, and transition triggers.

## Pipeline

```text
LevelIdentityProfile
-> RoomGrammarProfile
-> RoomArchetypes
-> LayoutSynthesisRequest
-> Route spine
-> Branch/dead-end placement
-> Landmark placement
-> Opening generation
-> SceneAssemblyPlan
-> Assembly validation
-> Primitive scene generation
```

## Determinism

The synthesizer uses the request seed and `System.Random`. It does not use Unity global random state. The same seed and same profiles should produce the same semantic layout.

## Output Contract

The primary output is still `SceneAssemblyPlan`. Future waves may replace this simple synthesizer with richer grammar solvers or AI-assisted backend tooling, but Unity-side consumers should keep depending on the `SceneAssemblyPlan` contract.

## Non-Goals

- No AI.
- No runtime generation.
- No asset downloading.
- No real importing.
- No Addressables.
- No enemies.
- No final procedural art.
