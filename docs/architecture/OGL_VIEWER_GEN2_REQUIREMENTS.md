# OGL Viewer Gen2 requirements — future project, draft v0.1

The native viewer is planned **after** the server core and protocols; this file captures contracts now, not an implementation commitment.

## Core
Native OGL authentication/session and capability negotiation; global Atlas with continuous map and territory ownership; permanent named destinations; teleport; optional seamless crossing with multi-source streaming and authority switch; friend/group/chat/voice; inventory, building and land tools; economy and safety controls.

## Rendering
glTF-compatible mesh pipelines under evaluation, PBR materials, terrain and water, lighting, LOD, animation and modern avatar rig. Interest management, progressive asset streaming, origin rebasing, resource budgets, accessibility and fallback graphics settings.

## Atlas UX
Zoomable global coordinate map, search by grid/territory/destination, filters and favorites, operator-published map layers, offline status, trust/admission notices, visible border crossing capability. Never expose technical hop addresses as primary navigation.

## Protocol contracts
Versioned network messages; identity/travel tokens; signed atlas metadata; asset manifests and permissions; avatar and object state; deterministic authority epochs; telemetry and error taxonomy. Support multiple adjacent grid streams during prefetch.

## Platform/implementation
Viewer language and graphics backend remain an **open decision**; C++ is a candidate, not approved. Desktop first, additional platforms assessed separately. Firestorm remains an optional compatibility client via legacy gateway, not the baseline for native viewer design.

## Acceptance
Viewer connects to local grid, displays scene/avatar, browses Atlas, teleports, reports errors, and ultimately crosses a test border without duplicate authority. Measure performance on supported hardware.
