# OpenGenesisLINK Generation 2

Next-generation federated virtual-world platform. **Early public .NET 10 bootstrap prototype is available; no native scene simulation, stable release, or proprietary core is published here.**

## Vision
A persistent, user-created virtual world with avatars, land, building, scripting, inventory, social systems, economy and independent grids. OGL Gen2 uses a new native server core; OpenSimulator/Firestorm compatibility is provided only through optional isolated gateways.

## Decisions
- Native server: C# / .NET 10 (target); Linux x86_64/ARM64 and Windows x86_64.
- Simulators: separate **OGL Simulator** application instances, each with its own `Simulator.ini`, `GridFabric.ini` and `Regions/*.ini`, securely connected to GridFabric. One process may host multiple regions; GridFabric and simulators restart independently.
- Grid services: proposed **OGL GridFabric** (working name), a horizontally scalable replacement for ROBUST-style grid services, with configurable direct HTTPS/TLS (default TCP 8002) and a 500,000-concurrent-session **stretch design goal**, not proven throughput.
- Federation: Hypergrid 2.0 with centrally coordinated, globally positioned grid territories.
- Global Atlas: one shared coordinate space, multiple freely placed territories per grid.
- Travel: atlas-based teleportation; optional bilateral seamless crossing for adjacent compatible territories.
- Viewer: native OGL Viewer later; Firestorm compatibility via a separately developed legacy gateway.
- Licensing direction: free-of-charge proprietary binary distribution, closed server source, third-party add-ons through documented SDK. **No finalized EULA has been issued.**
- Each grid retains control of accounts, assets, land, content and policies.

## Build the first prototype
- [.NET 10 solution and build instructions](BUILD.md): separate GridFabric HTTPS process, simulator INI loader and region registration/heartbeat over HTTPS. These are early, non-production proof-of-concept components.
- [Solution file](OpenGenesisLINK.Generation2.sln), [GridFabric source](src/OGL.Gen2.GridFabric/Program.cs), [Simulator source](src/OGL.Gen2.Simulator/Program.cs) and [shared protocol contracts](src/OGL.Gen2.Contracts/RegionContracts.cs).

## Documentation
- [Architecture overview](docs/architecture/OVERVIEW.md)
- [GridFabric design](docs/architecture/GRIDFABRIC_SPEC.md)
- [Separate OGL Simulator and GridFabric connection](docs/architecture/SIMULATOR_GRIDFABRIC_SPEC.md)
- [ADR 0002 — Independent simulator runtime](docs/adr/0002-separate-simulator-runtime.md)
- [Sample simulator INIs](examples/simulator/config/Simulator.ini)
- [GridFabric TLS and port configuration](docs/architecture/GRIDFABRIC_TLS_CONFIGURATION.md)
- [GridFabric capacity and load-testing plan](docs/architecture/GRIDFABRIC_CAPACITY_VALIDATION.md)
- [ADR 0001 — distributed grid services](docs/adr/0001-gridfabric-distributed-grid-services.md)
- [Global Atlas and territories](docs/architecture/GLOBAL_ATLAS_SPEC.md)
- [Grid registration](docs/architecture/GRID_REGISTRY_SPEC.md)
- [Hypergrid 2.0 travel](docs/architecture/HYPERGRID_2_PROTOCOL.md)
- [Seamless crossing](docs/architecture/SEAMLESS_CROSSING_SPEC.md)
- [Firestorm gateway](docs/architecture/LEGACY_VIEWER_GATEWAY.md)
- [Viewer requirements](docs/architecture/OGL_VIEWER_GEN2_REQUIREMENTS.md)
- [Licensing policy](docs/legal/LICENSING_STRATEGY.md)
- [Roadmap](ROADMAP.md)

## Status
Architecture specifications remain **proposals**. A public **buildable control-plane prototype** supports local INI parsing, a TLS listener, in-memory region registration and heartbeat; it is not a functioning world simulator and is not production secure or horizontally scalable. Performance targets and interoperability require independent testing.

Copyright © 2026 OpenGenesisLINK contributors / respective rights holders. All rights reserved. No permission to copy, modify or redistribute proprietary software is granted by this README. Third-party licenses remain applicable.
