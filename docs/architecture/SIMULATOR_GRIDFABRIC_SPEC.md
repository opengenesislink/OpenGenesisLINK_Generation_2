# OGL Simulator — separate executable with GridFabric connectivity

**Status:** design specification with a **buildable early control-plane prototype** (INI loading, HTTPS register/heartbeat). This is not an actual 3D region simulation server. This design intentionally follows the **operational separation** familiar from OpenSimulator (separate ROBUST and region simulator processes) without adopting OpenSimulator's simulation implementation or promising its exact wire compatibility.

## Runtime and deployment

Two independently startable application roles are proposed:

- **OGL GridFabric** (working name): independent grid-services application or replicated cluster, providing identity, sessions, region directory, inventory/asset metadata, presence, economy, federation, operations, and the public TLS edge (default **TCP 8002**, configurable).
- **OGL Simulator**: separate .NET 10 executable/service that owns one or multiple configured regions, scene state, physics, scripts, movement, connected avatars and region-local communication.

Initial executable/project names: `OGL.Gen2.GridFabric` and `OGL.Gen2.Simulator`. Product/branding names remain provisional; no release artifacts have been shipped. Each may run on Linux or Windows, on the same host or across independent machines. A simulator process can be restarted or upgraded without restarting GridFabric or other simulator processes. A process failure affects its own assigned regions; operators may prefer one region per process for better fault isolation.

```text
                         Native Viewer / World API
                                  |
                             HTTPS / TLS
                                  |
                     +--------------------------+
                     | GridFabric Gateway(s)    |
                     | https://grid:8002        |
                     | Identity / Directory /   |
                     | Session / Inventory etc. |
                     +--------------------------+
                        ^            ^            ^
                authenticated, encrypted simulator control-plane RPC
                        |            |            |
           +------------+--+  +------+--------+ +--------+----------+
           | OGL Simulator A|  | OGL Simulator B| | OGL Simulator C   |
           | Simulator.ini  |  | Simulator.ini  | | Simulator.ini     |
           | GridFabric.ini |  | GridFabric.ini | | GridFabric.ini    |
           | Regions/*.ini  |  | Regions/*.ini  | | Regions/*.ini     |
           | Magdeburg      |  | Hamburg        | | Freiburg          |
           | Gummybear      |  | Jenseits       | | Karlsruhe         |
           +----------------+  +---------------+ +-------------------+
                |                      |                    |
           viewer/scene traffic handled directly by the region hosts
```

This arrangement concerns **one owning grid**. A distinct operator's grid uses its own GridFabric installation; global discovery/federation occurs via the separately designed Global Atlas and trust protocol, not by sharing a single universal GridFabric database.

## Explicit configuration layout

All files are independent and local to the owning simulator instance. The future loader must define deterministic file inclusion and precedence; `GridFabric.ini` is not automatically understood by .NET or OpenSimulator.

```text
OGL-Simulator/
  OGL.Simulator                 # planned executable (or .exe on Windows)
  config/
    Simulator.ini               # process lifecycle, physics, scripts, local network
    GridFabric.ini              # owning grid endpoint, TLS, service identity, reconnect
    Regions/
      Magdeburg.ini             # UUID, dimensions, placement, endpoints, startup policy
      Gummybear-Island.ini
```

Early prototype [build instructions](../../BUILD.md). Example templates: [Simulator.ini](../../examples/simulator/config/Simulator.ini), [GridFabric.ini](../../examples/simulator/config/GridFabric.ini), [Magdeburg region](../../examples/simulator/config/Regions/Magdeburg.ini) and [Gummybear region](../../examples/simulator/config/Regions/Gummybear-Island.ini).

The final configuration loader should support a primary config path, an explicit include directory, environment/secret-manager overrides, schema validation, safe defaults, change audit and actionable startup errors.

### File contracts

| File | Owned configuration | Not stored there |
| --- | --- | --- |
| `Simulator.ini` | local process identity, modules, physics/script limits, logging, region configuration directory | GridFabric database credentials, public account records |
| `GridFabric.ini` | HTTPS base URL (e.g. `https://grid.example.org:8002`), TLS validation, simulator identity, trust/auth mode, reconnect policies | plaintext shared secrets or administrator credentials |
| `Regions/*.ini` | per-region stable UUID, display name, grid-local placement, size, declared viewer network binding, startup state | central authentication/identity store and economy ledger |

Each simulator is **logically bound to one owning GridFabric/grid** in the initial design. Connecting one simulator simultaneously to two unrelated grids would require explicitly specified multi-tenancy and strict isolation; it is not assumed.

## Secure simulator connection

1. Read and validate simulator, grid connection and region INIs before serving clients.
2. Establish HTTPS using the configured URL/port, validate server certificate chain and hostname, negotiate a supported API/protocol version.
3. Authenticate the simulator with a scoped service identity (candidate: mTLS client certificate; alternatively short-lived tokens with securely provisioned credentials).
4. Obtain grid identity and supported service capabilities; ensure the simulator is authorized for this grid.
5. Register each enabled region with a **stable Region UUID**, the simulator process identity, local coordinates, dimensions, advertised reachable endpoint and a revision/lease.
6. Receive acceptance or a specific conflict/policy error, then announce only accepted healthy regions as available to clients.
7. Renew region leases/heartbeats, publish bounded health/capacity events, and reconcile state after reconnect.

All register/renew/unregister operations must be idempotent and concurrency-safe. Use fencing generations to prevent an older duplicate simulator from taking ownership of a region. Region registration must not depend on every viewer position update or physics tick. An acknowledged registration is not enough to serve clients if advertised endpoints are unreachable.

### TLS and ports

- `8002/tcp` is **GridFabric HTTPS** by default, configurable via GridFabric and simulator connection configs.
- Region/client ports and transports belong **to each simulator/region** and must be independent from 8002 (example placeholders 9100 and 9101); protocol requirements will be finalized separately.
- Public listeners must use modern TLS wherever applicable. Do not disable certificate verification to make a connection succeed.
- A reverse proxy or load balancer may front GridFabric; simulators need the stable advertised HTTPS URL rather than fixed IP addresses.
- An eventual mTLS implementation must define termination identity propagation and trusted proxy boundaries; never trust arbitrary forwarded client-cert headers.

## Outage and recovery behavior

| Event | Proposed safe behavior |
| --- | --- |
| GridFabric unavailable at simulator startup | simulator may load local scene data, but does not advertise newly registered regions or accept fresh unauthenticated logins; retry with exponential backoff and jitter |
| GridFabric unavailable while residents are present | keep region-local physics and scripted movement running for **already-authorized** sessions where technically safe; queue **bounded, explicitly allowed** telemetry; block sensitive actions requiring central authorization |
| Inventory/economy service unavailable | do not fabricate success or commit speculative balance/inventory changes; show service-unavailable errors |
| Heartbeat or region lease expires | directory marks region unhealthy/unavailable for new arrivals; existing simulator state persists locally for reconciliation |
| Duplicate region UUID or conflicting lease owner | reject or fence the stale claimant; never announce duplicate authoritative instances |
| Simulator restarts | reload own INIs and durable scene state, authenticate, reconcile registrations, and restore eligibility without restarting GridFabric |
| GridFabric recovers | reauthenticate, compare revisions/lease generations, resynchronize supported state idempotently; do not blindly replay expired transactions |

The exact offline grace period, queue policy, region transition semantics and persistence guarantees remain open design decisions. These modes require acceptance tests with network partitions and rolling upgrades.

## Responsibilities: prevent another bottleneck

- **Simulator owns:** local physics, collisions, script execution, scene update, object transforms, avatar movement, region-local events and relevance filtering.
- **GridFabric owns:** identity/session authority, region discovery, account-scoped inventory and asset metadata, authenticated global services, economy ledger, federation and management.
- **Large assets:** obtain authorized retrieval information from asset services, then transfer through object storage/streaming delivery; no need to proxy all bulk bytes through GridFabric.
- **Connections:** use pooled channels for low-frequency authenticated control traffic, bounded retries, local caches with invalidation and service-specific deadline policies.

Do not synchronously call GridFabric every simulator frame or for every avatar coordinate update. Region-to-region data paths are separately designed, authenticated and measured; they must not require the Atlas for local movement.

## Near-term implementation acceptance

1. Boot GridFabric in HTTPS mode, configurable listener `:8002`.
2. Boot independent `OGL.Simulator` process reading separate INI files.
3. Validate certificate, authenticate and register exactly one region UUID.
4. Show the region in GridFabric directory; verify a second separate simulator can register its own region.
5. Restart a simulator without impacting GridFabric or the other simulator.
6. Reject duplicate region ownership, bad certificates and unauthorized simulator identities.
7. Simulate GridFabric outage, enforce safe offline behavior and demonstrate reconnection.
8. Add multi-region-per-process support and load testing without any per-tick central dependency.

Related: [GridFabric architecture](GRIDFABRIC_SPEC.md), [GridFabric TLS](GRIDFABRIC_TLS_CONFIGURATION.md), [capacity plan](GRIDFABRIC_CAPACITY_VALIDATION.md), [native architecture](OVERVIEW.md), [ADR 0002](../adr/0002-separate-simulator-runtime.md).
