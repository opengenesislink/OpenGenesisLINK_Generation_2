# Grid Registry and Federation Trust — draft v0.1

## Registration
Operator creates GridID and proves possession of a signing key. Directory stores public keys, verified endpoints, protocol versions, capabilities, public metadata, policies, territory IDs and revocation status. Registration does not transfer ownership of grid assets, user accounts or inventories.

## Security
Mutually authenticated server communication where practical; signed descriptors with expiry and key ID; replay protection; nonce/idempotency keys; rate limits; key rotation, revocation and compromise recovery. Transport TLS and application signatures are distinct controls.

## Discovery
Lookup GridID, TerritoryID, permanent destination IDs and global coordinates. Atlas returns verifiable endpoint/capability metadata. Client must not assume map listing implies trust or travel permission.

## Availability
Heartbeats provide *observational* status only. Missing heartbeats do not erase land claims. Cached signed records support degraded discovery while Atlas is unreachable; no fresh allocation during authority partition.

## Governance
Transparent registration, quota, naming, dispute, abuse and appeal processes. Protocol and directory schema should be published independently of proprietary server code. Support eventual compatible directory federation without sacrificing uniqueness of global coordinates.

## Privacy
Opt-in public listing, minimum necessary telemetry, no central inventory replication or global tracking of every resident. Public population counters require explicit policy and aggregation.
