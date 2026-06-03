# Deviation from Familiarity

## Purpose
Wrongness must have a baseline.

## Source Basis
Procedural generation should use deviation-from-familiarity, not uniform randomness.

## Design Rules
- Prefer implication over exposition.
- Preserve the project thesis: exploration, liminality, absence, and scale over combat or loud horror.
- Make the player inspect, infer, compare, and doubt rather than simply react.
- Every implementation must support the long emotional arc from strange calm to derealised dread.

## Implementation Notes
- Represent this principle with spatial layout, material choice, lighting, sound, UI restraint, and pacing.
- Use subtle variations before obvious anomalies.
- Add debug/review metadata so generated content can be evaluated against this principle.
- If implemented procedurally, expose tuning values for rarity, intensity, duration, and player readability.

## Codex Instructions
- Before editing code or content related to this concept, search the docs for `deviation_from_familiarity` and adjacent Wave 02 concepts.
- Do not add entities, combat, boss logic, gore, or chase sequences to satisfy this principle.
- If a feature reduces ambiguity too quickly, redesign it.
- When generating level recipes, include an explicit `psychologicalFunction` field.

## Common Failure Modes
- Turning the concept into a lore dump.
- Over-explaining the anomaly.
- Replacing slow interpretation with immediate danger.
- Using randomness where grammar is required.

## Related Files
- `docs/07_UNCANNY_ARCHITECTURE/000_uncanny_architecture_index.md`
- `docs/05_PSYCHOLOGY_FOUNDATIONS/013_ordinary_reality_slightly_off.md`
- `docs/06_LIMINALITY_ENGINE/004_context_obliteration.md`
- `docs/10_CURIOSITY_DREAD/004_information_gap_pacing.md`
