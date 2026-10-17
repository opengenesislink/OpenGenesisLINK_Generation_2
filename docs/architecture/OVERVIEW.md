# OGL Gen2 architecture — draft v0.1

## Product
A persistent user-generated virtual world, not a general-purpose game hosting platform. Core features: identity, avatars, inventory, assets, land ownership and rentals, in-world building, groups, chat, voice, events, scripting, commerce and economy. Grid operators retain sovereignty.

## Hard boundaries
1. Native server MUST run without OpenSimulator, Firestorm or legacy Hypergrid dependencies.
2. Legacy integrations are isolated optional processes with explicit adapters and feature negotiation.
3. The Atlas is a control-plane directory and spatial authority, never the authoritative runtime for avatar physics, inventory or object state.
4. Each grid owns its simulation, users, content, policies and authoritative state.
5. One authoritative owner per entity/session at a time; operations are auditable and idempotent.

## Candidate technology stack
- C# / .NET 10; Linux amd64, arm64 and Windows amd64.
- PostgreSQL for durable state; PostGIS for Atlas spatial registry; Redis optional cache, not source of truth.
- Content-addressed object storage, S3-compatible backends optional.
- QUIC/HTTP3 to evaluate for transport; versioned wire schemas with capability negotiation.
- Local simulation coordinates plus global signed 64-bit integer millimetre X/Y coordinates. Specify overflow, range and coordinate transforms before implementation.
- ECS/interest management, simulation partitions, snapshot/recovery, modular physics, HydroEngine concept, sandboxed OGLEngine and legacy LSL adapter as separate efforts.

## Services
Identity and federation trust; global Atlas registry; spatial allocation; map tile index; discovery; travel gateway; crossing coordinator; simulation/world nodes; inventory; asset service; scripting; economy; telemetry; operator administration.

## Nonfunctional requirements
Secure-by-default identity, least privilege, data minimization, signed releases, audit logs, rate limits, disaster recovery, observability, protocol evolution, horizontal scale where warranted, single-node deployment first.

## Explicit non-goals for first milestone
A complete Viewer, full Second Life feature parity, transparent Firestorm cross-grid walking, universal script migration, production economy or production-scale concurrency.

## Decision register
- ADR-001: Independent native core, bridges external.
- ADR-002: Independent grids, globally coordinated Atlas.
- ADR-003: Atlas model B, single global map.
- ADR-004: Travel model C, teleport plus optional seamless crossings.
- ADR-005: Multiple freely placed territories per grid.
- ADR-006: Free-to-use proprietary server, closed source, SDK extension permitted subject to final terms.
- ADR-007: Native Viewer later; Firestorm gateway compatibility target.
- ADR-008: C#/.NET 10 server target.
- OPEN: territory sizing, billing/quotas, federation governance, cross-grid economy, runtime scripting model, precise transport and viewer implementation.
