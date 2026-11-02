using OGL.Gen2.Simulator;

// No third-party test dependencies. This validates the initial INI parser and
// contract setup. End-to-end TLS/registration tests are a subsequent phase.
var root = Directory.GetCurrentDirectory();
var configDir = Path.Combine(root, "examples", "simulator", "config");
if (!Directory.Exists(configDir))
    throw new DirectoryNotFoundException("Run smoke tests from the repository root.");

var settings = SimulatorConfiguration.Load(configDir);
Assert(settings.Regions.Count == 2, "Expected two example regions.");
Assert(settings.GridFabricUrl.Scheme == "https", "GridFabric transport must use HTTPS.");
Assert(settings.GridFabricUrl.Port == 8002, "Default GridFabric port should be 8002.");
Assert(settings.Regions.Select(x => x.RegionId).Distinct().Count() == 2,
    "Region identifiers must be unique.");
Assert(settings.Regions.All(x => x.GridId == settings.GridId), "GridId mismatch.");
Assert(settings.Regions.All(x => x.Port != 8002), "Simulator client ports must differ from GridFabric.");
Console.WriteLine("PASS: simulator INI load and contracts.");

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
