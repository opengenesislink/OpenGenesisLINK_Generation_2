# Global Atlas and territories — proposed specification

**Status:** design proposal.

## Goal
Provide discovery and conflict-free placement of grid territories in one logical coordinate space. A grid may register **multiple non-contiguous territories**. The Atlas does not own the grids' region data, accounts, land or inventory.

## Suggested model
- `grid_id`: durable globally unique grid identifier.
- `territory_id`: durable identifier unique to a territory.
- `atlas_origin` and `atlas_extent`: explicit placement and bounds in a canonical coordinate system.
- `region_mapping`: reversible mapping between the grid's local region coordinates and Atlas placement.
- `grid_endpoint`: authenticated discovery entry, with protocol version and supported capabilities.
- `lease`, `revision`, `owner_key_id`: expiration, concurrency control and authority metadata.

The choice of units, precision, signed coordinate limits, region size and vertical layering is **unresolved**. Protocols must not assume a fixed 256 m region.

## Allocation lifecycle
1. Grid operator authenticates using the grid registry.
2. Client submits a territory proposal with bounds and desired placement.
3. Atlas validates ownership, quotas, overlapping claims and revision.
4. Atlas atomically accepts or rejects a claim and returns an immutable `territory_id`.
5. Renewals, relocation and removal follow versioned, auditable operations.
6. Expired claims become unavailable after a documented grace/recovery policy.

Two accepted active territories must never overlap in the same allocation layer unless an explicit future overlay model is separately standardized.

## Read and privacy concerns
Publish only discovery information needed for navigation. Do not put private user coordinates or identity data into the public registry. Define pagination, cache lifetime, staleness indicators and signed responses before deployment.

## Verification
Concurrent competing claims, expired leases, relocation, disconnected Atlas operation, key revocation, negative coordinates, oversized requests and territory ownership changes must have deterministic tests.

See [grid registry](GRID_REGISTRY_SPEC.md) and [travel protocol](HYPERGRID_2_PROTOCOL.md).
