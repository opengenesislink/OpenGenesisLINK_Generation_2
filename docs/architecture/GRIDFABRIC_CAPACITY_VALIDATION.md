# OGL GridFabric — capacity and reliability validation plan

**Status:** proposed performance qualification plan, not an accomplished benchmark.

## Define the actual numbers

| Metric | Requirement / interpretation |
| --- | --- |
| Stored accounts | Functional baseline: ≥500,000 distinct registered account records |
| Concurrent authenticated sessions | **Stretch goal:** ≥500,000 concurrently valid/active sessions distributed across region servers |
| Avatars in one region | **Not implied** by the 500,000 session goal; separately bounded by region-engine load tests |
| Requests per second | No fixed claim until the API operation mix and per-session activity are measured |
| Login bursts | Test independently, including reconnect storms and expired-session refresh |
| Simultaneous live socket connections | Measure separately from sessions; may differ greatly by transport architecture |
| Regions | Determine from measured region/simulator workloads and grid topology |

**Illustration only:** at one GridFabric request per session per 10 seconds, 500,000 active sessions produce ~50,000 requests/sec on average. At one request/session/sec, they produce ~500,000 requests/sec. This illustrates why "users" alone cannot specify infrastructure capacity.

## Workload model

1. Login and authentication, including refresh/expiry.
2. Region discovery and teleport destinations.
3. Inventory list/page/query and permission changes.
4. Asset metadata lookup vs actual binary asset delivery.
5. Presence subscribe/publish fan-out, social/group operations.
6. Economy reads vs strongly consistent financial writes.
7. Atlas/federation travel as a **separate, lower-frequency** workflow.
8. Failure and recovery: node loss, database leader change, cache loss, message backlog, cold-cache operation.

Model realistic hot-spot distributions (popular grid/region, large inventories, busy social groups), not only uniform random users.

## Proposed incremental qualification gates

- **Functional:** correctness tests with ≥500,000 account records; safety under duplicate/retried operations.
- **Scale 1:** 10,000 concurrent sessions with traffic mix and metrics.
- **Scale 2:** 50,000 concurrent sessions.
- **Scale 3:** 100,000 concurrent sessions.
- **Stretch:** 500,000 concurrent sessions with N nodes and distributed regions.
- **Recovery/soak:** several hours at each accepted scale plus forced gateway/service/cache/database failures. Record data loss, queue drain, reconnect behavior and session consistency.

Do not mark a level passed solely because the TCP sessions connect. Require realistic application operations and authoritative state correctness.

## Observability needed for every result

Document generator topology and bottlenecks, node specs, CPU/RAM, storage IOPS, database size, network bandwidth, geographic latency, TLS mode, session/request mix, data skew, duration, error/retry rates, p50/p95/p99 latency, throughput and cost. Test generators must not be the bottleneck.

**Candidate, not promised, acceptance targets:** >=99.9% successful non-invalid operations during steady-state benchmark; zero confirmed duplicate economy transfers, zero unauthorized inventory access, zero unacknowledged authoritative state duplication. Latency and recovery targets must be agreed per operation before asserting production SLA.

## Scalability tests

- Increase **gateway replicas** while holding database pressure constant.
- Increase **data partitions** while holding gateway count constant.
- Independently increase **region simulators**, connected sessions and request rate.
- Inject 10x hot-region lookups and burst login traffic.
- Measure cache-hit rate, partitions, event backlog, admission control and fairness.
- Fail an ingress node, a service node and a data dependency separately; verify limited blast radius.
- Re-run before release with TLS 1.2/1.3 enabled on the designated edge port (default 8002).

## Readiness decision

Only publish a user-scale claim when the benchmark evidence, representative workload, infrastructure limits and failure semantics are public or reproducible internally with recorded reports.
