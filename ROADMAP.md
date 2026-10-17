# OGL Generation 2 — development roadmap

**Status: planning only.** Phases below are ordered dependencies, not promised dates or percentages. Acceptance criteria are required before claiming completion.

## M0 — Foundation & legal
- [x] Record architecture and product direction.
- [x] Record Atlas model B, travel model C and freely placed territories.
- [x] Record free-to-use proprietary licensing intention.
- [ ] Move proprietary source development to private GitHub access.
- [ ] Approve EULA, Atlas terms, trademark and third-party license audit.
- [ ] Approve ADRs, wire schema standards and repository conventions.
**Gate:** signed-off architecture and legal publication boundary.

## M1 — Native core skeleton
- [ ] .NET 10 solution, modular services, DI, configuration, structured logs.
- [ ] Linux amd64/arm64 and Windows amd64 build pipelines.
- [ ] Authentication, world node lifecycle, minimal persistent scene, metrics.
- [ ] Unit/integration/security baseline and reproducible builds.
**Gate:** headless single-node world starts, persists and recovers.

## M2 — Atlas control plane
- [ ] GridID/key lifecycle, signed registration and discovery.
- [ ] PostgreSQL/PostGIS territory registry, atomic collision-free allocation.
- [ ] Multiple disjoint territories, map tiles, spatial queries and operator console.
- [ ] Heartbeats, leases, audit, quota and outage behavior.
**Gate:** two independent grids reserve distinct areas; concurrent collisions rejected.

## M3 — Native travel / Hypergrid 2.0
- [ ] Versioned federation protocol and travel grants.
- [ ] Home/destination admission, state machine, fencing and journal.
- [ ] Teleport and interruption/recovery paths.
- [ ] Permission-aware asset and avatar appearance exchange.
**Gate:** repeatable two-grid teleport and negative/recovery tests.

## M4 — World systems
- [ ] Terrain and object persistence, streaming, interest management.
- [ ] Inventory and OGL AssetService Gen2.
- [ ] Building/land parcels, groups, chat, voice adapter.
- [ ] Script sandbox/OGLEngine and LSL compatibility strategy.
**Gate:** persistent user-built world with secure content lifecycle.

## M5 — Optional seamless crossing
- [ ] Bilateral border agreement and capability negotiation.
- [ ] Neighbor prefetch and cross-grid world streaming.
- [ ] Authoritative avatar handoff, epoch fencing and recovery.
- [ ] Vehicle/occupant policy, physics and water seam compatibility.
**Gate:** safe test crossings under latency and failure injection.

## M6 — Firestorm legacy gateway
- [ ] Protocol compatibility research and license audit.
- [ ] Login/agent scene baseline; chat/inventory/objects in staged tiers.
- [ ] Security and regression suite with real Firestorm builds.
**Gate:** documented compatibility matrix with independently reproduced tests.

## M7 — Native OGL Viewer Gen2 (separate project)
- [ ] Select viewer language/graphics stack.
- [ ] Atlas UX, native scene, assets, avatars and world building.
- [ ] Native teleport, seamless streaming and accessibility.
**Gate:** native end-to-end walkthrough and performance evaluation.

## M8 — Beta, hardening & public release
- [ ] Security audit, penetration tests, load tests and recovery drills.
- [ ] Documentation, migration paths, operator onboarding, signed installers.
- [ ] Legal signoff, final EULA, privacy policy and release criteria.
**Gate:** acceptance tests and explicit release approval.

## Cross-cutting workstreams
Security, observability, accessibility, permissions, data portability, moderation, performance and compatibility testing run throughout. No milestones should be marked complete solely from documentation.
