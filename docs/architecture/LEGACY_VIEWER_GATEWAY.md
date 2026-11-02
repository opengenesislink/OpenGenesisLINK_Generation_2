# Legacy viewer and OpenSimulator gateway

**Status:** interface proposal, not implemented.

## Objective
Allow selected existing viewer workflows (including Firestorm where legally and technically feasible) to reach Generation 2 through a separately deployed compatibility gateway. The native Gen2 protocol remains authoritative.

## Boundary
`Legacy viewer <-> isolated gateway <-> versioned Gen2 APIs <-> native core`

The gateway must not expose internal database connections or simulation data structures to untrusted clients. Apply authentication, session isolation, rate limiting, auditing, protocol parsing limits and explicit feature negotiation.

## Compatibility is selective
Movement, appearance, chat, groups, inventory, scripting, materials, uploads, attachments, land and teleportation need an **individual test matrix**. No OpenSimulator/Second Life protocol feature is assumed to work automatically. Features that cannot be mapped safely should be disabled with a clear error.

## Operating model
Deploy the gateway separately when needed; native grids without legacy viewers need not enable it. Gateway outages must not compromise native simulation state.

## Licensing and provenance
Audit third-party protocol implementations, copyrighted code, specifications and trademarks before importing or distributing anything. Do **not** copy Generation 1/OpenSimulator source into the proprietary Gen2 core without a documented rights and license review.

See [viewer requirements](OGL_VIEWER_GEN2_REQUIREMENTS.md) and [licensing](../legal/LICENSING_STRATEGY.md).
