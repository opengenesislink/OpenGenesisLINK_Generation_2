# Firestorm / OpenSimulator Legacy Viewer Gateway — draft v0.1

## Principle
Optional external compatibility service translating selected legacy viewer protocols into native OGL Gen2 contracts. Native core must not depend on OpenSimulator assemblies or the legacy packet model.

## Compatibility tiers (targets, not validated)
Tier 0: authentication/login, basic scene/agent presence and movement.
Tier 1: chat, camera, teleports, basic appearance and inventory reads.
Tier 2: assets, object editing, mesh upload, groups, land and scripts, subject to security and protocol feasibility.
Tier 3: features requiring native Viewer (global Atlas UI, native streaming, advanced avatars and guaranteed seamless border crossing) are explicitly out of scope for unmodified Firestorm.

## Technical investigations
Login service compatibility, UDP circuit lifecycle, HTTP capabilities, LLSD, object update formats, asset identifiers, permission translation, texture/mesh encoding, inventory consistency, event queues, session teardown and region size constraints. Build a protocol compliance test harness before promising support.

## Licensing boundary
Avoid copying Firestorm or OpenSimulator code without auditing actual component licenses and obligations. A separate process is an architectural isolation boundary, **not** a blanket legal exemption. Keep dependency SBOM and legal review.

## Security
Validate all legacy inputs, bound message sizes, prevent capability spoofing, enforce permission checks in native authoritative services. Never allow a gateway to bypass grid travel admission or asset export controls.
