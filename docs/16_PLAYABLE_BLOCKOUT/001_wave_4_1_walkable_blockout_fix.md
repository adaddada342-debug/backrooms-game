# Wave 4.1 Walkable Blockout Fix

## Purpose
Wave 4.1 fixes physical walkability in the primitive Level 0 blockout. The Wave 4 generator created rooms as closed primitive boxes, which could block the player from reaching the corridor and transition trigger.

## Fix
Rooms now carry optional opening data. The primitive scene builder uses those openings to avoid placing blocking wall segments where connections attach.

The goal is a simple playable route:

1. Player spawn.
2. Spawn office.
3. Long corridor.
4. Transition room.
5. Transition trigger.

The side dead-end room is also reachable as a basic branch.

## Boundary
This remains primitive-only and local-only. It does not add real assets, procedural generation, downloading, importing, AI, Addressables, enemies, or final gameplay.

## Limitation
Wave 4.1 uses a temporary primitive simplification: if a wall has an opening, that wall is omitted instead of being cut with boolean geometry. The result may look rough, but the test route should be walkable.

