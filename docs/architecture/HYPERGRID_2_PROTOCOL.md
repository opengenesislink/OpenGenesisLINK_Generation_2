# Hypergrid 2.0 — federation travel protocol, draft v0.1

## Scope
Atlas-addressed travel among independently operated OGL Gen2 grids; replaces user-facing hop:// addresses. Legacy hop addresses may be accepted **only** by optional compatibility bridge.

## Parties
Viewer, home identity authority, source Travel Gateway, destination Travel Gateway, destination simulation node, Atlas Discovery. Atlas is **not** in the realtime simulation data path.

## Destination
Immutable GridID + TerritoryID + DestinationID or global X/Y/Z. Lookup resolves current endpoints. Use versioned canonical serialization, explicit coordinate frame, negotiated capability set.

## Teleport state machine
REQUESTED → VALIDATING → PREPARING → READY → COMMITTING → ACTIVE.
Failure states: REJECTED, ABORTED, TIMED_OUT, RECOVERING. Every attempt has globally unique TravelID, idempotency key, deadlines, and durable journal. Destination admission precedes authoritative source handoff.

## Safety invariants
- Exactly one authoritative active avatar session, enforced by monotonic epoch/fencing token.
- No inventory ownership transfer simply because of travel.
- Permission/export checks for all cross-grid assets.
- Destination may reject admission; source remains authoritative until confirmed commit.
- Retries must not duplicate avatar state, currency transactions or attachments.

## Data contracts
Signed short-lived travel grant; authenticated home identity; minimum avatar appearance descriptor; capability manifest; attachment references and permissions; session transfer envelope; commit receipt; audit correlation IDs. Specify privacy consent and retention before release.

## Protocol evolution
Semantic capability flags, schema versions, backwards-compatible optional fields, explicit unsupported-feature behavior, downgrade resistance. No untrusted deserialization into arbitrary runtime types.

## Initial deliverable
Two local test grids, successful authorized teleport, denied teleport, duplicate request, interrupted transfer, expired token and recovery tests. Do not claim real-time Firestorm interoperability.
