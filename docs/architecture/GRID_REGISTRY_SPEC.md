# Grid registration and trust registry — proposed specification

**Status:** design proposal.

## Purpose
Register independently operated grids and provide verifiable discovery. Registration is not an endorsement of a grid's safety, uptime, content or operator.

## Candidate record
`grid_id`, public name, verified endpoint/domain, protocol versions, capability list, owner public key, key ID, record revision, last verified timestamp and expiry/lease.

## Registration flow
1. Operator submits metadata and a public key.
2. Atlas verifies control over the announced endpoint/domain through a challenge.
3. The operator signs the registration request.
4. Atlas checks duplicate identities, rate limits, revoked keys and schema validity.
5. Atlas issues a versioned record and audit reference; periodic signed renewals maintain it.

Define how key rotation, operator transfer, grid suspension and recovery work before accepting production registrations.

## Trust boundaries
- The registry may provide **discovery**, but must not grant access to a grid's accounts or assets.
- Remote capabilities are untrusted until independently checked and negotiated.
- Private endpoints, administrator secrets and sensitive personal data must never appear in public records.
- Use bounded validity and replay protection for signed operations.
- Unavailable registry services should not erase a grid's locally owned world state.

## Required tests
Spoofed endpoints, tampered records, stale signatures, replay, key compromise, duplicated names, large records, lease expiry and denial-of-service limits.

See [Atlas](GLOBAL_ATLAS_SPEC.md).
