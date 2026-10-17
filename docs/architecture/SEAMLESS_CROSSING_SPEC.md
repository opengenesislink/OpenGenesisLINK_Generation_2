# Seamless cross-grid border crossing — draft v0.1

## Preconditions
Two Atlas Territories share a nonzero boundary segment. Both operators sign a border agreement covering segment, direction, admission rules, capability requirements, validity and revocation. The viewer can stream the destination before handoff.

## Stages
1. Detect approach with hysteresis and prefetch neighboring terrain, object and avatar appearance.
2. Authenticate and negotiate border capability and physics/coordinate transforms.
3. Destination reserves shadow session, receives validated minimal state snapshot.
4. Freeze/fence source authority at a deterministic handoff boundary.
5. Destination commits new epoch, returns durable receipt; source relinquishes authority.
6. Viewer switches authoritative stream without duplicating entity.
7. On uncertainty enter recovery, reconcile journals/epochs and safely return or resume.

## State to transfer
Avatar position, velocity, orientation, animation and seated state; attachment references; permissions; relevant physics parameters. Do not assume arbitrary live script stacks can migrate. Scripts require explicit suspend/serialize/resume support and compatible runtime.

## Vehicles and groups
Treat vehicle plus occupants as a transfer group with admission checks for every participant. If atomic group handoff is unavailable, refuse seamless crossing and offer safe teleport or stop at border. Never silently split seated riders across authoritative nodes.

## Terrain, physics, water
Adjacent terrain/height and material seams, water surfaces and HydroEngine boundaries need interoperable contracts; visual continuity does not guarantee physical continuity. Define behavior for incompatible physics engines.

## Failure modes
Source crash, destination crash, network partition, duplicated commit, token expiry, policy revocation, packet reordering, atlas outage. Persist journal and use fencing; two-phase messaging alone cannot guarantee correctness.

## Rollout
Experimental opt-in only after teleport protocol, streaming and recovery are proven. Instrument latency, crossing success, avatar loss/duplication, vehicle recovery and network jitter. No guarantee of seamless crossings with unmodified Firestorm.
