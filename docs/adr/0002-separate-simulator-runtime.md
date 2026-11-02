# ADR 0002 — Standalone simulator executable with independent INI files

- **Date:** 2026-11-02
- **Status:** proposed architecture; executable contracts not yet implemented
- **Context:** Generation 2 should retain OpenSimulator's convenient operator model: a central ROBUST-like process and separately launched simulator processes with independent configuration. However, Gen2 replaces ROBUST with the distributed GridFabric design and does not inherit OpenSimulator's internal core.

## Decision

Implement an independent `OGL.Simulator` runtime separate from `OGL.GridFabric`. Each simulator has `config/Simulator.ini`, `config/GridFabric.ini` and one or more `config/Regions/*.ini`. Each instance authenticates to one owning GridFabric, registers its regions by stable UUID, renews leases and maintains simulator-local physics/script/scene/avatars without per-tick centralized RPC.

GridFabric HTTPS defaults to configurable TCP 8002; simulator ports are separately configured. One host may run multiple simulator processes; one process may run multiple regions. GridFabric and simulators restart independently.

## Why

Operationally familiar to OpenSimulator grid owners, portable across hosts, isolates high-frequency simulation from central service load, supports distributed deployment and independent rolling upgrades.

## Consequences and risks

Needs a configuration loader/schema, simulator identity management, mutual authentication, region ownership fencing, reliable reconnect, per-region health, simulator-state persistence, network topology and cross-region handoff. Source compatibility with OpenSimulator is **not** implied.

## Acceptance

See [simulator registration and failure modes](../architecture/SIMULATOR_GRIDFABRIC_SPEC.md). The INIs in `examples/simulator/` are templates only; no executable has been released.
