# ADR 0001 — Separate distributed GridFabric from native region simulation

- **Date:** 2026-11-02
- **Status:** proposed; requires engineering validation and final name approval
- **Context:** Generation 2 requires grid services that do not become a central ROBUST-like throughput bottleneck. A target of 500,000 concurrent sessions was requested as an intentionally ambitious design guide. Native region simulation is an independently scaling data path. Public HTTPS on port 8002 must be supported by default, with configurable port/certificate.

## Decision proposed

Use **OGL GridFabric** as a provisional name for a logical grid-service layer. Separate public stateless gateway instances from identity/session, region directory, inventory/assets, presence/social, economy and federation services. Allow shared-process small deployments and independently partitioned services for scaling. Keep movement/physics and scene ticking on region simulators. Use Kestrel TLS directly on configurable public port 8002 by default.

## Alternatives

1. Expand one central ROBUST-like process — simple, but hard to isolate failure and data-plane load.
2. Require dozens of microservices from day one — scalable in principle, but too operationally expensive for small grids.
3. **Chosen proposal:** service boundaries with flexible deployment and measured horizontal scale.

## Consequences

Requires explicit service contracts, distributed tracing, ownership and consistency rules, capacity qualification, failure handling and more complex operations at scale. Splitting into more processes is not itself a guarantee against database, cache, bus or Atlas bottlenecks.

## Validation gates

- Prove production TLS/certificate/port behavior.
- Establish workload-specific load tests up to the aspirational 500k concurrent session level.
- Demonstrate failure recovery and correct economy/inventory/session behavior.
- Finalize naming/trademark and licensing review.
