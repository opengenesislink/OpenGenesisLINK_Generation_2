# OpenGenesisLINK Generation 2

Next-generation federated virtual-world platform. **Architecture and planning stage — no server implementation is published here.**

## Vision
A persistent, user-created virtual world with avatars, land, building, scripting, inventory, social systems, economy and independent grids. OGL Gen2 uses a new native server core; OpenSimulator/Firestorm compatibility is provided only through optional isolated gateways.

## Decisions
- Native server: C# / .NET 10 (target); Linux x86_64/ARM64 and Windows x86_64.
- Federation: Hypergrid 2.0 with centrally coordinated, globally positioned grid territories.
- Global Atlas: one shared coordinate space, multiple freely placed territories per grid.
- Travel: atlas-based teleportation; optional bilateral seamless crossing for adjacent compatible territories.
- Viewer: native OGL Viewer later; Firestorm compatibility via a separately developed legacy gateway.
- Licensing direction: free-of-charge proprietary binary distribution, closed server source, third-party add-ons through documented SDK. **No finalized EULA has been issued.**
- Each grid retains control of accounts, assets, land, content and policies.

## Documentation
- [Architecture overview](docs/architecture/OVERVIEW.md)
- [Global Atlas and territories](docs/architecture/GLOBAL_ATLAS_SPEC.md)
- [Grid registration](docs/architecture/GRID_REGISTRY_SPEC.md)
- [Hypergrid 2.0 travel](docs/architecture/HYPERGRID_2_PROTOCOL.md)
- [Seamless crossing](docs/architecture/SEAMLESS_CROSSING_SPEC.md)
- [Firestorm gateway](docs/architecture/LEGACY_VIEWER_GATEWAY.md)
- [Viewer requirements](docs/architecture/OGL_VIEWER_GEN2_REQUIREMENTS.md)
- [Licensing policy](docs/legal/LICENSING_STRATEGY.md)
- [Roadmap](ROADMAP.md)

## Status
Architecture specifications are **proposals**, not implemented features. Performance targets require measurement; interoperability requires prototype testing. No source code, binaries or working protocol endpoints are claimed.

Copyright © 2026 OpenGenesisLINK contributors / respective rights holders. All rights reserved. No permission to copy, modify or redistribute proprietary software is granted by this README. Third-party licenses remain applicable.
