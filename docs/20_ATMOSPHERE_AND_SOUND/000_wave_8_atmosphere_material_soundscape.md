# Wave 8: Atmosphere, Material, and Soundscape Foundation

Wave 8 creates the production foundation for how primitive Level 0 scenes receive visual and audio atmosphere.

The target pipeline is:

```text
AtmosphereProfile
-> MaterialLibrary
-> Room material roles
-> Scene atmosphere application
-> Soundscape plan
-> Runtime ambience components
-> Debug report
```

This wave introduces:

- Primitive material library.
- Material role mapping.
- Atmosphere application foundation.
- Local placeholder soundscape.
- Fluorescent hum and flicker systems.
- Atmosphere reporting.

This is still primitive-only. No real art assets are imported, and no external audio files are required. The soundscape system may create `AudioSource` objects with placeholder/null clips for now. The goal is to prove architecture and scene wiring, not final sensory quality.

HDRP Volume support should be scaffolded carefully and kept non-brittle. The game should continue to work if HDRP-specific types are unavailable.

## Non-Goals

- No AI.
- No downloading.
- No importing.
- No Addressables.
- No enemies.
- No final art pass.
- No final audio pass.
- No runtime procedural generation.
