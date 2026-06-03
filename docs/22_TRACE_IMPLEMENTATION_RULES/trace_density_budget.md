# Trace Density Budget

## Purpose
Prevent procedural environments from becoming prop soup.

## Emotional Target
emptiness preserved

## Source Basis
Derived from the uploaded Backrooms research report and its environmental storytelling library.

## Design Rules
- Most rooms should contain no major trace.
- Major traces should be rare enough to become memorable.
- Trace density should increase near camps, maintenance zones, service desks, archives, and transit hubs.

## Implementation Notes
- Expose per-level budgets: MinorTraceChance, MajorTraceChance, SignatureTraceChance.
- Allow emotional stage to modify density.
- Create debug overlay for trace counts per generated chunk.

## Codex Instructions
- Implement this as data-driven rules, not hardcoded one-off prop spam.
- Add editor/debug validation where practical.
- When unsure, reduce trace density.

## Common Failure Modes
- Random prop scatter.
- Too many clues in one space.
- Immediate explanation.
- Theme-breaking object placement.

## Related Files
- None.
