using System.Net.Http.Headers;
using System.Net.Http.Json;
using OGL.Gen2.Contracts;
using OGL.Gen2.Simulator;

var configDirectory = args.Length == 2 && args[0] == "--config-dir"
    ? args[1]
    : Path.Combine(AppContext.BaseDirectory, "config");

var configuration = SimulatorConfiguration.Load(configDirectory);
var token = Environment.GetEnvironmentVariable("OGL_GRIDFABRIC_SIMULATOR_TOKEN");
if (string.IsNullOrEmpty(token) || token.Length < 32)
    throw new InvalidOperationException("A strong OGL_GRIDFABRIC_SIMULATOR_TOKEN (>=32 characters) is required.");

using var client = new HttpClient
{
    BaseAddress = configuration.GridFabricUrl,
    Timeout = TimeSpan.FromSeconds(10)
};
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

using var shutdown = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) => { eventArgs.Cancel = true; shutdown.Cancel(); };
var leases = new Dictionary<Guid, RegionLeaseResponse>();

Console.WriteLine($"OGL Simulator prototype: {configuration.SimulatorId}; GridFabric: {configuration.GridFabricUrl}");
Console.WriteLine($"Loaded {configuration.Regions.Count} region(s). NO physics, scripts or viewer transport yet.");

try
{
    while (!shutdown.IsCancellationRequested)
    {
        foreach (var region in configuration.Regions)
        {
            try
            {
                if (!leases.TryGetValue(region.RegionId, out var lease))
                {
                    using var response = await client.PostAsJsonAsync(
                        "api/v1/regions/register", region, shutdown.Token);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.Error.WriteLine($"Registration {region.Name}: HTTP {(int)response.StatusCode}");
                        continue;
                    }

                    var registered = await response.Content.ReadFromJsonAsync<RegionLeaseResponse>(
                        cancellationToken: shutdown.Token);
                    if (registered is null || registered.RegionId != region.RegionId)
                        throw new InvalidDataException("Invalid registration response.");
                    leases[region.RegionId] = registered;
                    Console.WriteLine($"Registered {region.Name}: lease until {registered.ExpiresAtUtc:O}");
                }
                else
                {
                    var payload = new RegionLeaseRequest(configuration.GridId, configuration.SimulatorId, lease.LeaseId);
                    using var response = await client.PutAsJsonAsync(
                        $"api/v1/regions/{region.RegionId:D}/lease", payload, shutdown.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        var renewed = await response.Content.ReadFromJsonAsync<RegionLeaseResponse>(
                            cancellationToken: shutdown.Token);
                        if (renewed is null || renewed.LeaseId != lease.LeaseId)
                            throw new InvalidDataException("Invalid heartbeat response.");
                        leases[region.RegionId] = renewed;
                    }
                    else if ((int)response.StatusCode is 404 or 409)
                    {
                        leases.Remove(region.RegionId);
                        Console.Error.WriteLine($"Lease lost for {region.Name}; will register again.");
                    }
                    else Console.Error.WriteLine($"Heartbeat {region.Name}: HTTP {(int)response.StatusCode}");
                }
            }
            catch (OperationCanceledException) when (shutdown.IsCancellationRequested) { break; }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidDataException)
            {
                // A GridFabric outage must not terminate the entire simulator process.
                Console.Error.WriteLine($"GridFabric unavailable for {region.Name}: {ex.Message}");
            }
        }

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(configuration.HeartbeatSeconds), shutdown.Token);
        }
        catch (OperationCanceledException) when (shutdown.IsCancellationRequested) { break; }
    }
}
finally
{
    // Best effort only: leases expire naturally if the process dies.
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    foreach (var region in configuration.Regions)
    {
        if (!leases.TryGetValue(region.RegionId, out var lease)) continue;
        try
        {
            var payload = new RegionLeaseRequest(configuration.GridId, configuration.SimulatorId, lease.LeaseId);
            using var _ = await client.PostAsJsonAsync(
                $"api/v1/regions/{region.RegionId:D}/unregister", payload, timeout.Token);
        }
        catch (Exception) { /* lease expiry is the fallback */ }
    }
}
