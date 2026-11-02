# OpenGenesisLINK Generation 2 — Roadmap

Status: planning baseline, not a release commitment. Phases are ordered by technical dependency; dates and versions are intentionally not promised.

## Phase 0 — Foundation and governance
- [x] Repository initialized with architecture index.
- [x] Initial architecture, federation and legal decision documents added.
- [x] Documentation link validation in CI.
- [ ] Confirm governing proprietary license/EULA and contributor terms with the rights holders.
- [ ] Record architecture decisions using ADRs and agree on measurable acceptance criteria.
- [ ] Define the separation between public specifications/SDK and private server implementation.

**Exit gate:** approved architecture baseline and documented legal clearance before implementation distribution.

## Phase 1 — Prototype contracts
- [ ] Specify canonical identifiers, protocol versioning, schemas and error handling.
- [ ] Prototype the Global Atlas registry and its allocation/conflict rules.
- [ ] Model grid authentication, key rotation, signed discovery and trust boundaries.
- [ ] Define contracts for world state, identity, assets, inventory and scene events.
- [ ] Establish tracing, metrics, security threat model and interoperability test cases.

**Exit gate:** contract tests and a threat-model review; none of these constitute production readiness.

## Phase 2 — Native core
- [ ] Build an independent C#/.NET 10 simulation core in an appropriately licensed private codebase.
- [ ] Implement durable grid/account/region state with supported database adapters.
- [ ] Implement region scheduling, movement, scene graph and asset/inventory interfaces.
- [ ] Establish script sandbox, capability permissions and resource limits.
- [ ] Validate recovery, durability, latency, memory and multi-region load.

**Exit gate:** repeatable simulation, persistence, recovery and performance benchmarks.

## Phase 3 — Federation and travel
- [ ] Implement independently operated grid registration and Atlas territories.
- [ ] Prototype Hypergrid 2.0 travel (proposed OGL protocol; not a current standard).
- [ ] Validate identity federation, permissions and content export policies.
- [ ] Test adjacent-grid handoff and documented fallback to teleport.

**Exit gate:** security and interoperability tests across two independently operated grids.

## Phase 4 — Client and compatibility
- [ ] Publish a versioned public SDK/specification surface where legally authorized.
- [ ] Prototype native OGL Viewer against the new contracts.
- [ ] Prototype the isolated Firestorm/OpenSimulator gateway.
- [ ] Validate accessibility, input devices and platform targets separately.

**Exit gate:** tested native client flow; legacy feature matrix with explicit limitations.

## Phase 5 — Operations and release
- [ ] Package, deploy, observe, back up and restore services.
- [ ] Conduct security review, incident exercises, upgrade/rollback tests and capacity testing.
- [ ] Finalize EULA, notices, distribution terms and third-party license compliance.
- [ ] Publish release artifacts only after all release gates have been met.

A successful build, a prototype or published documentation does **not** equal a released product.
