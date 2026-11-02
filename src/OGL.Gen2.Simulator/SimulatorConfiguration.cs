using OGL.Gen2.Contracts;

namespace OGL.Gen2.Simulator;

public sealed record SimulatorConfiguration(
    Guid GridId,
    string SimulatorId,
    Uri GridFabricUrl,
    int HeartbeatSeconds,
    IReadOnlyList<RegionRegistrationRequest> Regions)
{
    public static SimulatorConfiguration Load(string configDirectory)
    {
        var path = Path.GetFullPath(configDirectory);
        var sim = IniReader.Load(Path.Combine(path, "Simulator.ini"));
        var grid = IniReader.Load(Path.Combine(path, sim.Get("Simulator", "GridConnectionFile", "GridFabric.ini")));

        var simulatorId = sim.Require("Simulator", "InstanceId");
        if (simulatorId.Length > 128)
            throw new FormatException("Simulator InstanceId must be <= 128 characters.");

        if (!Guid.TryParse(grid.Require("GridFabric", "GridId"), out var gridId) || gridId == Guid.Empty)
            throw new FormatException("GridFabric GridId must be a nonempty GUID.");

        if (!Uri.TryCreate(grid.Require("GridFabric", "Url"), UriKind.Absolute, out var baseUrl) ||
            baseUrl.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(baseUrl.UserInfo) ||
            !string.IsNullOrEmpty(baseUrl.Query) || !string.IsNullOrEmpty(baseUrl.Fragment) ||
            baseUrl.AbsolutePath != "/")
            throw new FormatException("GridFabric Url must be an HTTPS origin (e.g. https://grid.example.org:8002).");

        if (!string.Equals(grid.Require("Security", "AuthenticationMode"), "BearerToken", StringComparison.OrdinalIgnoreCase) ||
            !bool.TryParse(grid.Require("Security", "ValidateServerCertificate"), out var validate) || !validate)
            throw new FormatException("The bootstrap requires BearerToken auth and certificate validation.");

        if (!int.TryParse(grid.Require("Connection", "HeartbeatIntervalSeconds"), out var heartbeat) ||
            heartbeat is < 5 or > 120)
            throw new FormatException("HeartbeatIntervalSeconds must be 5–120.");

        if (!int.TryParse(grid.Require("Connection", "RegionLeaseSeconds"), out var lease) ||
            heartbeat >= lease)
            throw new FormatException("HeartbeatIntervalSeconds must be less than RegionLeaseSeconds.");

        var regionDirectory = sim.Get("Simulator", "RegionsDirectory", "Regions");
        if (Path.IsPathRooted(regionDirectory) || regionDirectory.Split('/', '\\').Contains(".."))
            throw new FormatException("RegionsDirectory must be a relative directory below config.");
        var regionPath = Path.Combine(path, regionDirectory);
        if (!Directory.Exists(regionPath))
            throw new DirectoryNotFoundException(regionPath);

        var regions = new List<RegionRegistrationRequest>();
        foreach (var regionFile in Directory.EnumerateFiles(regionPath, "*.ini").OrderBy(x => x, StringComparer.Ordinal))
        {
            var ini = IniReader.Load(regionFile);
            if (!bool.Parse(ini.Get("Region", "Enabled", "true"))) continue;
            var regionId = Guid.Parse(ini.Require("Region", "RegionId"));
            if (regionId == Guid.Empty) throw new FormatException("RegionId cannot be empty.");
            var host = ini.Require("ClientEndpoint", "AdvertisedHost");
            var port = int.Parse(ini.Require("ClientEndpoint", "Port"));
            var x = int.Parse(ini.Require("Region", "SizeX"));
            var y = int.Parse(ini.Require("Region", "SizeY"));
            if (port is < 1 or > 65535 || x is < 1 or > 4096 || y is < 1 or > 4096 ||
                host.Length > 253)
                throw new FormatException($"Invalid region dimensions or endpoint: {regionFile}");

            regions.Add(new RegionRegistrationRequest(gridId, simulatorId, regionId,
                ini.Require("Region", "Name"),
                int.Parse(ini.Require("Region", "LocalGridX")),
                int.Parse(ini.Require("Region", "LocalGridY")), x, y, host, port));
        }
        if (regions.Count == 0) throw new FormatException("No enabled regions were found.");
        if (regions.Select(x => x.RegionId).Distinct().Count() != regions.Count)
            throw new FormatException("Each region needs a unique RegionId.");
        return new SimulatorConfiguration(gridId, simulatorId, baseUrl, heartbeat, regions);
    }
}
