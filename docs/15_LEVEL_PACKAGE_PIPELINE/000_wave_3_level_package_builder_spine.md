# Wave 3 Level Package Builder Spine

## Purpose
Wave 3 connects approved assets to level packages. It adds the approved asset library, explicit package dependency graph, first level package builder contract, and a fake/local Level 0 package generator.

This wave proves the data path only. It does not create real scenes, instantiate objects, download assets, import assets, scrape websites, use Addressables, run AI, or implement final procedural generation.

## Runtime Boundary
Runtime Unity builds consume approved package metadata and approved assets only. Asset downloading, importing, source discovery, and generation remain editor, offline, or backend-facing.

A generated level package must reference only approved assets. Asset dependencies must be explicit and traceable so every package can be audited before runtime use.

## Level Package Requirements
Every level package must carry:

- Manifest
- Dependency graph
- Credits ID
- Validation report ID
- Scene name or scene address
- Schema version
- Package version

The dependency graph lists the exact assets used by the package, their role, local path, prefab path, required status, and tags. Required dependencies must exist in the approved asset library and must be approved for runtime.

## Local Level 0 Generator
The local Level 0 generator is fake/local only. Its purpose is to prove that approved assets can be selected, expressed as package dependencies, and written into package metadata.

It does not generate final content. Future waves can replace the fake generator with real grammar-driven generation once blockout scene contracts, assembly plans, and validation flows exist.

## Future Replacement Path
Later waves can replace the fake builder with a grammar-driven level package builder. That builder should still preserve the same safety rules:

- Use only approved runtime assets.
- Keep dependencies explicit and traceable.
- Generate credits and validation references.
- Produce package metadata before runtime loading.
- Refuse packages that fail dependency or validation checks.

