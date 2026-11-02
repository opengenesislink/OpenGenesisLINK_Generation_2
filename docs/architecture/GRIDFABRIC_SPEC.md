# OGL GridFabric — distributed grid services

**Status:** architecture proposal, not implemented. **Name:** OGL GridFabric is a working name, subject to product and trademark review.

## Mission and non-goals

GridFabric is Generation 2's replacement for the logical *grid service* responsibilities historically consolidated in OpenSimulator's ROBUST. It is **not** a larger ROBUST process, and is **not** the per-region scene simulation engine.

A small grid should be operable with one modest deployment, while a large grid can deploy independently replicated and partitioned components. The topology, not an oversized single executable, supplies capacity.

**Stretch capacity goal:** 500,000 simultaneous authenticated sessions **across a grid's distributed regions**, not 500,000 avatars within one region or 500,000 requests per second. This is a target to prove with workload-specific testing, **not a measured performance claim**. Separately validate 500,000 registered accounts and peak login bursts. See [capacity and benchmarking](GRIDFABRIC_CAPACITY_VALIDATION.md).

## Control and data paths

```text
Viewer / region nodes / admin clients / partner grids
                     |
           HTTPS/TLS edge :8002
                     |
        Load balancer / ingress (HA)
                     |
     +--------------------------------+
     |  Stateless GridFabric gateways |
     |       N independently scaled   |
     +--------------------------------+
          |  route + authorize
          v
  +----------------+------------------+
  | Identity       | Sessions         |
  | Grid Directory | Presence         |
  | Inventory      | Asset Metadata   |
  | Groups/Social  | Economy          |
  | Federation     | Operations       |
  +----------------+------------------+
         |          |              |
   partitioned DB  cache       event backbone
         |          |              |
     durable data  ephemeral     async event flows
         |
  object storage / CDN for large asset payloads

  Region simulation, movement, physics, scene tick and most live
  region-local interactions run on REGIONS, NOT through GridFabric.
```

The optional Global Atlas is a **separate federation control service**. It must not be consulted synchronously for every routine in-grid interaction. Local grids retain authority over residents, assets, permissions and regions.

## Functional boundaries

| Domain | Responsibility | Scalability boundary |
| --- | --- | --- |
| Edge/Gateway | TLS termination, admission, auth forwarding, API routing, protocol negotiation | stateless replicated instances |
| Identity | accounts, login, authentication, revocation and policy | partitioned by grid and identity |
| Session | durable session index, token refresh, routing and expiry | partitioned session keys; cache not sole authority |
| Grid/Region Directory | region lookup, leases, destination discovery, estate metadata | partitions and cached lookups |
| Presence | online state, subscriptions, friends-aware fan-out | partitioned by user/subscription, bounded events |
| Inventory | inventory metadata, permissions, references and optimistic concurrency | account/owner partitions |
| Assets | metadata, hashes, access policies, storage routing | object store for content; no bulk blobs through login gateway |
| Social | groups, contacts, messages and permissions | partitioned/async where applicable |
| Economy | balances, double-entry ledger, idempotent transfers | transactional authority; never cache as source of truth |
| Federation | trust, handoff grants, remote policy and travel | independent of routine local scene updates |
| Operations | config, capability discovery, metrics, health, audit | protected administrative surface |

These are **logical boundaries**. During development, low-traffic modules can share a process to reduce operating complexity, provided their contracts and state ownership remain isolated. Deploy and scale independently when measurements justify it.

## Interaction design

- Public APIs must use versioned contracts; authenticated access and explicit error codes.
- Use HTTP/1.1 or HTTP/2 for outward-facing endpoints as negotiated; consider gRPC for trusted internal service-to-service RPC.
- Background work is asynchronous through a durable event backbone. Prefer at-least-once delivery with idempotent consumers and bounded retry/dead-letter handling, not unsupported exactly-once assumptions.
- Avoid synchronous chains of multiple downstream services on every login or region lookup.
- Region engines hold high-frequency movement/physics paths locally; avoid per-tick calls into GridFabric.
- Client requests and service calls use deadlines, cancellation, bounded concurrency, backpressure and overload shedding.
- Store large asset bytes in an object store and serve via permission-checked, short-lived delivery URLs or a controlled CDN; asset metadata stays in the dedicated service.

## No single bottleneck — including infrastructure

A replicated Gateway by itself does **not** eliminate bottlenecks. Design and load-test these failure domains separately:

1. **Ingress:** multiple gateway replicas, redundant load balancing, failure-aware routing; no required sticky login session.
2. **Databases:** avoid a single global primary for all hot identity/session/inventory operations. Partition at documented ownership boundaries; replication is not a substitute for write scaling.
3. **Cache:** bounded TTL and eviction; use it for acceleration, not the only durable session, economy or inventory authority.
4. **Event backbone:** partition topics/subjects and consumer groups; isolate traffic classes and limit fan-out.
5. **Directory and discovery:** bounded positive/negative caching, lease expiry and stale-read semantics.
6. **Global Atlas:** registration and cross-grid travel may rely on it, but local login/scene operation should continue during Atlas unavailability where local policies permit.
7. **Secrets, certificates and observability:** design redundant operation and safe failure modes; failed telemetry must not block logins.

PostgreSQL, Redis, a distributed message bus and S3-compatible object storage are **candidate technologies**, not mandatory dependencies or proof of performance. Their HA topology, cost, recovery and licensing must be reviewed separately.

## Consistency and correctness

- Single authority per mutable data partition, explicit version/ETag or transactional compare-and-swap.
- Inventory writes, permission updates and monetary transfers require documented consistency boundaries.
- Economy transfers need durable ledger semantics, idempotency keys and auditable reconciliation; no speculative balance edits from cache.
- Session leases and region registrations need split-brain fencing, expiry and recovery rules.
- Cross-grid workflows use explicit prepare/commit/compensate states and never assume a globally atomic transaction.
- Define restore objectives (RPO/RTO) and test data migration/rollback before production.

## Deployment profiles

**Starter:** one GridFabric instance, local trusted service stack, and an external or embedded database chosen for development; still HTTPS by default for exposed public endpoints.

**HA:** at least two gateways across separate failure domains, replicated services, redundant load balancing, highly available database/cache/event components, active monitoring and tested restore.

**Large scale:** N gateways and service replicas, partitioned data by workload ownership, separate asset-delivery plane, region-local clusters, automated admission/scale decisions and multi-zone disaster recovery.

Instance counts are deployment choices determined by actual tests, not fixed user-to-node ratios.

## Observability and resilience

- Every public operation has request IDs, end-to-end traces, latency histograms, rate/error indicators and correlation IDs.
- Measure active sessions separately from connected transports, unique avatars, requests/s, login/s, queue depth and region simulator load.
- Expose authenticated/isolated admin controls plus internal liveness and dependency-aware readiness.
- Use rolling deployments and version-tolerant migrations; no all-at-once mandatory restart.
- Test node loss, data-node failover, brownouts, traffic skew, network partition and overload scenarios.

## Public TLS contract

The default direct public endpoint is **`https://<grid-hostname>:8002`**, with a **configurable bind address, port and certificate**. Port 8002 is HTTPS/TLS, *not* cleartext HTTP. Public 443 via reverse proxy is optional. Configure and validate production certificates before exposing the service. See [GridFabric TLS contract and example](GRIDFABRIC_TLS_CONFIGURATION.md).

## Integration dependencies

- [Global Atlas](GLOBAL_ATLAS_SPEC.md) and [federation travel](HYPERGRID_2_PROTOCOL.md) remain separate trust domains.
- [Native architecture](OVERVIEW.md) defines the region/scene boundary.
- [Capacity validation](GRIDFABRIC_CAPACITY_VALIDATION.md) defines evidence needed before claiming 500k capacity.
- [Licensing](../legal/LICENSING_STRATEGY.md): no private source is published through this proposal.

## Open decisions

Service contract schemas, identity issuer/JWKS model, user and region partitioning, selected database and message bus, cross-region read strategy, concrete SLOs, failover budgets, certificate lifecycle implementation, number of network ports, and admin access policy.
