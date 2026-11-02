# Seamless crossing between territories — proposed specification

**Status:** research proposal, not guaranteed functionality.

## Scope
A seamless crossing is an opt-in, bilateral handoff between **adjacent and compatible** territories operated by separate grids. Atlas adjacency does not automatically authorize handoff.

## Preconditions
- Both operators explicitly enable the same crossing.
- Mapping and coordinate frames are agreed and non-conflicting.
- Both sides negotiate supported avatar/scene transfer capabilities.
- Authorization, trust and local access policies are enforced on both ends.
- The handoff path satisfies measured latency and state-consistency requirements.

## Candidate protocol
Prepare destination state and reservation; pause or snapshot only the authoritative movable state needed for transfer; commit destination ownership once; acknowledge and release source ownership. Use idempotency IDs, timeouts and rollback. Never assume a distributed transaction can atomically span independently operated grids.

## Fallback
On negotiation failure, high latency, destination refusal or uncertain ownership, use a safe stop or explicit teleport; do not silently duplicate objects or avatars. Cross-grid physics constraints, vehicles, attachments, scripts and inventory require dedicated compatibility policies.

## Acceptance criteria to define
End-to-end crossing latency percentile targets, object loss/duplication rate, behavior under network partitions, memory and CPU budgets, user-visible fallback, and cross-version interoperability.

See [Atlas](GLOBAL_ATLAS_SPEC.md) and [travel](HYPERGRID_2_PROTOCOL.md).
