# Wave 1 Implementation Spine

## Purpose
Wave 1 creates the Unity, data, and backend-facing spine for future asset ingestion, validation, package loading, and level construction. It is a clean foundation only: schemas, folder structure, registries, service interfaces, validation models, and tiny placeholder implementations.

This wave does not build the full downloader, importer, generator, or level builder. It does not introduce live AI into the Unity client, online scraping, remote downloads, or runtime asset importing.

## Runtime Boundary
The Unity game client must not run an AI model. Future AI-assisted work belongs in backend, offline, or editor tooling that prepares approved content for the game.

Unity should eventually load approved Level Packages. A Level Package is a validated unit of playable content with a manifest, credits, validation report, and approved asset references. The runtime loader should consume package metadata and local or catalog-backed scenes, not raw untrusted sources.

## Future Asset Pipeline
Future asset downloading, importing, generation, and optimization should happen outside the runtime client. Backend or editor tools may discover candidates, resolve metadata, download files, import assets, validate quality, and generate package data.

Every asset record must carry:

- License name and license URL
- Creator name
- Source URL
- Stable content hash
- Attribution metadata
- Usage context

No asset should be promoted into a playable package without traceable provenance and attribution.

## Validation Requirement
Every generated or assembled level must carry validation proof before it is used by the game. Validation should cover navigation, licensing, attribution, performance, and theme fit. A package is approved only when the validation report passes and contains no blockers.

Warnings may exist for non-blocking issues, but blockers must prevent package approval until resolved.

## First Playable Milestone
The first playable milestone is one Level 0 blockout:

- Basic player movement
- Mapping notes
- One transition
- Fake local LevelPackage data
- Local-only loading through approved package metadata

This milestone proves the data shape and loading path before any downloader, importer, AI generation, or remote catalog work begins.

