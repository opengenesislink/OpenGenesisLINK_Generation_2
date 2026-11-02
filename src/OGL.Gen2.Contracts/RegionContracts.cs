namespace OGL.Gen2.Contracts;

// Control-plane DTOs only. Wire format is JSON over HTTPS for this prototype.
// There is no scene simulation or production identity/federation protocol yet.
public sealed record RegionRegistrationRequest(
    Guid GridId,
    string SimulatorId,
    Guid RegionId,
    string Name,
    int LocalGridX,
    int LocalGridY,
    int SizeX,
    int SizeY,
    string AdvertisedHost,
    int Port);

public sealed record RegionLeaseRequest(Guid GridId, string SimulatorId, Guid LeaseId);

public sealed record RegionLeaseResponse(Guid RegionId, Guid LeaseId, DateTimeOffset ExpiresAtUtc);

public sealed record RegionSnapshot(
    Guid GridId,
    string SimulatorId,
    Guid RegionId,
    string Name,
    int LocalGridX,
    int LocalGridY,
    int SizeX,
    int SizeY,
    string AdvertisedHost,
    int Port,
    DateTimeOffset ExpiresAtUtc);
