# Generation 2 architecture overview

**Status:** design proposal — not implemented or production-tested.

## Principles
1. **Native core:** C# with .NET 10 as the target, without OpenSimulator as the simulation engine.
2. **Grid sovereignty:** each grid operates its own users, assets, land, rules and data stores.
3. **Explicit trust boundaries:** the Atlas performs discovery and territorial coordination, not unrestricted access to grid data.
4. **Capability-based interoperability:** cross-grid actions require scoped authorization, explicit consent where applicable, and audit trails.
5. **Optional compatibility:** legacy protocols live only in isolated adapters and do not define native core internals.
6. **Measurable performance:** architecture decisions require evidence from profiling and multi-region benchmarks.

## Logical components
| Component | Responsibility | Authority |
| --- | --- | --- |
| Native simulation core | scene state, physics integration, region ticking, object lifecycle | owning grid |
| OGL GridFabric | distributed gateway and grid service coordination (proposed replacement for ROBUST-like services) | owning grid |
| Identity and session service | account login, session lifecycle, capabilities | owning grid |
| Inventory and asset services | metadata, immutable assets, authorization and references | owning grid |
| Script runtime | sandboxed scripting and per-script quotas | owning grid |
| World API | versioned client/admin integrations, scoped operations | owning grid |
| Federation/travel adapter | discovery, policy validation, handoff and audit | each participating grid |
| Global Atlas | global coordinate/territory registry and discovery metadata | Atlas service |
| Legacy gateway (optional) | protocol translation for existing viewers | isolated per deployment |
| Native OGL Viewer (later) | world rendering and user experience | client side |

## Deployment boundaries
A single-node installation should be possible before distributed deployment is considered. Internal modules may later be split into services without changing public contracts. State must remain recoverable; transport and persistence are implementation choices, not guarantees.

Target platforms: Linux x86_64, Linux ARM64, and Windows x86_64. Windows ARM64 is a potential later target, not an initial commitment.

## Security baseline
- TLS for transport, authenticated service identities and short-lived tokens.
- Authorization on each operation; deny by default between trust domains.
- Auditable administrative actions, least-privilege secrets and key rotation.
- Strict isolation for untrusted scripts, user content and the legacy gateway.
- Rate limits, bounded queues, backpressure and safe cancellation.
- No assumption that an Atlas listing or remote grid is trustworthy.

## Contract work still required
Canonical identifiers, scene/event formats, asset formats, coordinate units, persistence schema, API version rules, synchronization and consistency guarantees, privacy retention, and explicit migration policy.

Related specifications: [GridFabric](GRIDFABRIC_SPEC.md), [GridFabric TLS](GRIDFABRIC_TLS_CONFIGURATION.md), [GridFabric capacity](GRIDFABRIC_CAPACITY_VALIDATION.md), [Atlas](GLOBAL_ATLAS_SPEC.md), [registry](GRID_REGISTRY_SPEC.md), [travel](HYPERGRID_2_PROTOCOL.md), [crossing](SEAMLESS_CROSSING_SPEC.md), [legacy](LEGACY_VIEWER_GATEWAY.md).
