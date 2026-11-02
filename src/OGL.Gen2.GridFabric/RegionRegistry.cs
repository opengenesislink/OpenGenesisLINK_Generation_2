using System.Collections.Concurrent;
using OGL.Gen2.Contracts;

namespace OGL.Gen2.GridFabric;

/// <summary>
/// Prototype-only in-memory registry. NOT durable, distributed, or suitable for
/// a production multi-gateway deployment. Replace with a fenced lease store.
/// </summary>
public sealed class RegionRegistry(Guid gridId, TimeSpan leaseDuration)
{
    private sealed record Lease(RegionRegistrationRequest Region, Guid LeaseId, DateTimeOffset ExpiresAtUtc);

    private readonly ConcurrentDictionary<Guid, Lease> _regions = new();

    public bool TryRegister(RegionRegistrationRequest region, out RegionLeaseResponse? response)
    {
        response = null;
        if (region.GridId != gridId || region.RegionId == Guid.Empty ||
            string.IsNullOrWhiteSpace(region.SimulatorId) || region.SimulatorId.Length > 128 ||
            string.IsNullOrWhiteSpace(region.Name) || region.Name.Length > 256 ||
            string.IsNullOrWhiteSpace(region.AdvertisedHost) || region.AdvertisedHost.Length > 253 ||
            region.Port is < 1 or > 65535 ||
            region.SizeX is < 1 or > 4096 || region.SizeY is < 1 or > 4096)
        {
            return false;
        }

        while (true)
        {
            var now = DateTimeOffset.UtcNow;
            var newLease = new Lease(region, Guid.NewGuid(), now.Add(leaseDuration));
            if (!_regions.TryGetValue(region.RegionId, out var existing))
            {
                if (!_regions.TryAdd(region.RegionId, newLease)) continue;
            }
            else
            {
                // A live lease cannot be stolen even by a process using the same SimulatorId.
                // A restarted process waits for lease expiration in this prototype.
                if (existing.ExpiresAtUtc > now) return false;
                if (!_regions.TryUpdate(region.RegionId, newLease, existing)) continue;
            }

            response = new RegionLeaseResponse(region.RegionId, newLease.LeaseId, newLease.ExpiresAtUtc);
            return true;
        }
    }

    public RegionLeaseResponse? Renew(Guid regionId, RegionLeaseRequest request)
    {
        while (_regions.TryGetValue(regionId, out var existing))
        {
            if (existing.ExpiresAtUtc <= DateTimeOffset.UtcNow ||
                existing.LeaseId != request.LeaseId ||
                existing.Region.GridId != request.GridId ||
                !string.Equals(existing.Region.SimulatorId, request.SimulatorId, StringComparison.Ordinal))
            {
                return null;
            }

            var updated = existing with { ExpiresAtUtc = DateTimeOffset.UtcNow.Add(leaseDuration) };
            if (_regions.TryUpdate(regionId, updated, existing))
                return new RegionLeaseResponse(regionId, updated.LeaseId, updated.ExpiresAtUtc);
        }
        return null;
    }

    public bool Unregister(Guid regionId, RegionLeaseRequest request)
    {
        if (!_regions.TryGetValue(regionId, out var existing) ||
            existing.LeaseId != request.LeaseId ||
            existing.Region.GridId != request.GridId ||
            existing.Region.SimulatorId != request.SimulatorId) return false;

        // Compare-and-remove to avoid deleting a concurrently replaced lease.
        return ((ICollection<KeyValuePair<Guid, Lease>>)_regions)
            .Remove(new KeyValuePair<Guid, Lease>(regionId, existing));
    }

    public IReadOnlyList<RegionSnapshot> GetActive()
    {
        var now = DateTimeOffset.UtcNow;
        return _regions.Values
            .Where(x => x.ExpiresAtUtc > now)
            .Select(x => new RegionSnapshot(
                x.Region.GridId, x.Region.SimulatorId, x.Region.RegionId,
                x.Region.Name, x.Region.LocalGridX, x.Region.LocalGridY,
                x.Region.SizeX, x.Region.SizeY, x.Region.AdvertisedHost,
                x.Region.Port, x.ExpiresAtUtc))
            .OrderBy(x => x.Name, StringComparer.Ordinal)
            .ToList();
    }
}
