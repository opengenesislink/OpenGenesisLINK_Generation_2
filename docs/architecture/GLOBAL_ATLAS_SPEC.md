# Global Atlas & spatial allocation — draft v0.1

## Model
A single global 2D address space containing **Atlas Territories**, independently placed rectangular reservations. One grid may own multiple disjoint territories. Territories are not resident-owned **Land Parcels**; those are local grid constructs. A territory contains local simulation cells independent of its public geographic outline.

## Coordinates
Global X/Y are signed 64-bit integers representing millimetres. Store rectangle as half-open [minX,maxX) × [minY,maxY), validate nonzero positive extents, arithmetic overflow and maximum policy limits. No implicit wrapping. Rendering/physics use rebased local origins. Z belongs to world/simulation contracts and is not part of territory collision allocation.

## Registry records
GridID, TerritoryID (immutable UUIDs); operator ownership; rectangle; public display name; lifecycle state; version; policy; endpoint references; creation and renewal timestamps. Permanent destination IDs resolve to current coordinates, not vice versa.

## Allocation protocol
1. Authenticate operator and verify quota.
2. Validate rectangle and request idempotency key.
3. Acquire serializable transaction / spatial exclusion protection.
4. Reject intersecting active/reserved allocations; adjacency is permitted.
5. Commit allocation, monotonic registry version, audit event.
6. Publish signed territory descriptor and map metadata.

## Lifecycle
REQUESTED → RESERVED → ACTIVE → SUSPENDED → RETIRED → RELEASED. Offline heartbeat does NOT immediately release territory. Expiration, appeals, abuse handling and reclaim periods are governance policy, not hard-coded assumptions. Moving/resizing must use a staged migration with durable aliases for destination IDs.

## Map delivery
Multi-resolution tiles with explicit tile schema, signed metadata, cache controls, operator-provided imagery and moderation/visibility policy. Do not trust tiles as authority for travel or parcel ownership. Offline areas may display cached imagery with status.

## Borders
Detect geometric adjacency and calculate border segments. Seamless crossing requires **two-sided** agreements, compatible protocol/capabilities, policy and operational readiness. Geometric proximity alone never grants access.

## Persistence
PostgreSQL/PostGIS candidate. Use transactional spatial exclusion and concurrency tests, not a read-then-write collision check. Replicate public directory for reads, retain a single consistent allocation authority per coordinate namespace.

## Open questions
Maximum rectangle size; fractional dimensions vs internal tiling; territory quota fairness; geographic labels; global namespace governance; recovery after registry partition; privacy and map moderation.
