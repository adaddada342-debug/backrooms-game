# Wave 2 Asset Pipeline Contracts

## Purpose
Wave 2 defines the asset ingestion contract only. It creates local data models, interfaces, mock validators, and editor-only test tooling for a future advanced downloader, importer, validator, and level-package builder pipeline.

Real source integrations come later. This wave does not scrape websites, call external APIs, add API keys, download files, import remote assets, implement Addressables, run AI locally, or generate procedural levels.

## Runtime Boundary
Asset downloading, importing, generation, and source discovery belong in backend, offline, or Unity Editor tooling. Runtime Unity builds must only consume approved package metadata and approved assets.

Runtime Unity builds must never consume unapproved raw assets. Raw external files must stay quarantined until they pass provenance, license, quality, import, optimization, attribution, and theme checks.

## Asset Approval Requirements
Every external asset must pass:

- Provenance checks
- License checks
- Quality checks
- Import checks
- Optimization checks
- Attribution checks
- Theme and level-fit checks

Every promoted asset must carry creator, source URL, license, hash, attribution text, and validation proof.

## Intended Future Flow
1. Discover candidate asset.
2. Resolve metadata.
3. Check license.
4. Queue download.
5. Download to quarantine.
6. Hash file.
7. Import into Unity/editor workspace.
8. Analyze geometry/materials/textures.
9. Validate theme tags.
10. Generate attribution.
11. Promote to approved asset library.
12. Make asset available to the level package builder.

## Wave 2 Deliverable
Wave 2 establishes contracts and local mock behavior so later waves can add real integrations safely. The mocks are test doubles: they work with `mock://` and `local://` inputs, never contact the internet, and never import files.

