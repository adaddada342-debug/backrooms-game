# Wave 5 Level Grammar

## Purpose
Wave 5 introduces the data architecture for room grammar, atmosphere profiles, assembly validation, landmarks, scene generation rules, and level identity. It remains editor/data-side only.

This wave does not implement AI, downloading, importing, Addressables, multiplayer, enemies, procedural mesh generation, runtime generation, or final gameplay.

## Core Concepts
Geometry is the physical shape placed in a Unity scene: floors, walls, ceilings, lights, triggers, and primitive blockout objects.

A room is a semantic space within a level grammar. It can become geometry later, but it is first a design unit with a room type, allowed neighbors, landmark rules, traversal role, and size constraints.

A landmark is a memorable identity marker. It helps navigation, creates recognition, and supports the emotional logic of a level. A landmark is not just decoration; it is a navigational and atmospheric anchor.

Atmosphere is the sensory and psychological treatment of a level: light color, fog, exposure, hum, silence, pressure, safety, and ambience density.

Level Identity is the high-level promise of a Backrooms level. It defines what the space is, how it should feel, what belongs there, what must never appear there, and how navigation should behave.

A Traversal Route is the readable path through a generated or assembled layout. It is the proof that the player can move through the intended experience rather than only look at disconnected rooms.

## Backrooms Level Pipeline
A Backrooms level is not geometry.

A Backrooms level is:

Identity
-> Grammar
-> Layout
-> Atmosphere
-> Assets
-> Scene

Future AI or backend systems may generate grammar, but Unity consumes grammar. The Unity-side job is to validate grammar, assemble local scenes, and refuse content that violates identity, route, atmosphere, or safety constraints.

## Level 0 Direction
Wave 5 production data targets classic Level 0: endless office-like liminality, yellow wallpaper, damp carpet, fluorescent hum, repetition, partial safety, isolation, and uneasy familiarity. It is not a monster arena.

