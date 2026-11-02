# Hypergrid 2.0 — OGL federation/travel proposal

**Status:** proposed protocol name, **not** an implemented service or established external standard.

## Objectives
Permit an avatar to travel between independent participating OGL grids while each grid remains authoritative for its own users, scenes, inventory and policy. Do not promise compatibility with OpenSimulator's existing Hypergrid wire protocol; use a separate compatibility adapter if required.

## Proposed state machine
1. **Discover:** resolve destination grid/territory and verify endpoint metadata.
2. **Negotiate:** agree on compatible protocol versions, requested capabilities and policy restrictions.
3. **Authorize:** source authenticates the session; destination independently approves a time-limited, audience-bound travel grant.
4. **Prepare:** destination reserves a session/arrival and reports supported appearance/asset permissions.
5. **Commit:** source hands off active presence with idempotency protection; destination acknowledges activation.
6. **Finalize or rollback:** source closes the old presence on success, or restores it if preparation/commit fails.

Actual payload schemas, redirects, timeouts and the treatment of network partitions require separate specifications.

## Non-negotiable security properties
- Signed and scoped requests with nonce/replay prevention and short expiration.
- Each destination enforces its own bans, access rules and capability permissions.
- No raw passwords, privileged tokens or unrestricted inventory credentials move between grids.
- Asset copies/transfers require ownership and license-policy checks; lack of permission means no transfer.
- Audit outcomes without exposing unnecessary user information.
- Timeouts and retries must not create duplicate active authoritative presences.

## Compatibility and resilience
Treat versions as independently negotiated capabilities; an incompatible destination should fail clearly. If seamless transfer is unavailable, the implementation may offer a separately negotiated teleport instead.

## Test matrix
Malicious destination/source, revoked keys, expired tokens, disconnected source, destination timeout, duplicate commits, asset denial, incompatible version and rollback after partial handoff.

See [registry](GRID_REGISTRY_SPEC.md) and [seamless crossing](SEAMLESS_CROSSING_SPEC.md).
