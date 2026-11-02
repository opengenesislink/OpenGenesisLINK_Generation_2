# Native OGL Viewer for Generation 2 — requirements draft

**Status:** product requirements, not a working viewer or published client SDK.

## Objectives
Build a modern first-party viewer for the Gen2 native world protocol, with accessible navigation, responsive content rendering and dependable account/region transitions.

## Functional baseline
- Account login and secure session renewal through documented APIs.
- World entry, avatar presence, controls, camera and movement.
- Objects/scenes, textures, meshes, avatar appearance and permissions.
- Chat and social presence, inventory with explicit asset access rules.
- Atlas territory exploration and interoperable travel negotiation.
- Voice through an optional, permissioned provider abstraction.
- Diagnostics and graceful handling of partial service failure.

## Non-functional baseline
- Versioned network contracts and server-authoritative state.
- No embedded administrator secrets or unrestricted asset retrieval.
- Accessible scaling, keyboard/mouse controls and robust error messages.
- Profiling and performance budgets for asset streaming, rendering and memory.
- Platform-by-platform device/controller testing before any support claim.

Linux/Windows targets and ARM64 viewer feasibility require separately validated GPU/runtime support. A Raspberry Pi console-like experience is a potential client initiative, **not** an existing supported Gen2 deliverable.

## Relationship to other viewers
The native viewer is independent of the optional [legacy gateway](LEGACY_VIEWER_GATEWAY.md). TrixieStorm or other Generation 1 viewers are not silently redefined as Gen2-native clients.

## Open contracts
Rendering pipeline, scene transport, asset format, input abstraction, UI design system, offline caching, device capabilities and accessibility acceptance tests.

See [architecture overview](OVERVIEW.md).
