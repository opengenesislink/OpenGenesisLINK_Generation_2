# Build the Generation 2 starter solution

**Status:** public self-authored bootstrap scaffold. This is NOT a functional virtual-world platform; the proprietary native simulation engine is not included.

## Prerequisites

- .NET 10 SDK on Linux x86_64/ARM64 or Windows x86_64.
- A valid TLS certificate for GridFabric.
- A strong randomly generated simulator-registration token, provisioned out of band (same value at GridFabric and the simulator, until per-node credentials are implemented).

## Compile

On Linux, from the repository root run `./compile.sh`; on Windows PowerShell run `./compile.ps1`. Both scripts build all projects, execute the smoke test and publish two separate applications into `publish/`.

Alternatively, execute the commands directly:

```sh
dotnet restore OpenGenesisLINK.Generation2.sln
dotnet build OpenGenesisLINK.Generation2.sln -c Release
dotnet run --project tests/OGL.Gen2.SmokeTests -c Release --no-build
```

Separate build commands:

```sh
dotnet publish src/OGL.Gen2.GridFabric -c Release -o ./publish/gridfabric
dotnet publish src/OGL.Gen2.Simulator -c Release -o ./publish/simulator
```

Binaries will be under `publish/` (ignored by Git). These commands compile the independent ASP.NET Core GridFabric gateway, INI-enabled simulator control client, and shared contracts.

## First local connection

1. Provision a development/trusted certificate with `dotnet dev-certs https --trust`, or install a real cert and set `Kestrel__Endpoints__PublicHttps__Certificate__Path` and, if needed, `Kestrel__Endpoints__PublicHttps__Certificate__Password` using a secure secret store. **Never turn off TLS certificate validation.** Trust the certificate in the simulator host as appropriate.
2. Use separate terminals, both started from the repository root. Set the **same** strong token as `OGL_GRIDFABRIC_SIMULATOR_TOKEN` in each process's environment. Do not paste a token into an INI or commit it.
3. Start GridFabric: `dotnet run --project src/OGL.Gen2.GridFabric`. The HTTPS listener defaults to TCP 8002. In production, an explicit certificate path is required.
4. Copy `examples/simulator/config/` into a private local configuration directory (or keep using the example path during test). Set `GridFabric.ini` `Url = https://localhost:8002` for local access; the TLS certificate must be trusted and match `localhost`.
5. Start the simulator: `dotnet run --project src/OGL.Gen2.Simulator -- --config-dir examples/simulator/config`.
6. Inspect `https://localhost:8002/health/live` and `https://localhost:8002/api/v1/regions` with a certificate-validating client. Both Magdeburg and Gummybear Island should be registered if all steps succeed.

The example INIs initially point to `https://grid.example.org:8002` and are **not runnable as-is** against localhost. The example simulator client endpoints (9100/9101) are advertised metadata only, not opened sockets.

## Directory layout

```text
OpenGenesisLINK.Generation2.sln
Directory.Build.props
global.json
src/
  OGL.Gen2.Contracts/
    OGL.Gen2.Contracts.csproj
    RegionContracts.cs
  OGL.Gen2.GridFabric/
    OGL.Gen2.GridFabric.csproj
    Program.cs
    RegionRegistry.cs
    appsettings.json
  OGL.Gen2.Simulator/
    OGL.Gen2.Simulator.csproj
    Program.cs
    IniReader.cs
    SimulatorConfiguration.cs
tests/
  OGL.Gen2.SmokeTests/
examples/
  simulator/config/{Simulator.ini,GridFabric.ini,Regions/*.ini}
```

## Known limitations (intentional)

- The registry is **in-memory on one GridFabric process** and loses registrations on restart. No partitioning, HA, sharding or database persistence exists yet.
- One shared prototype bearer token authorizes all simulator registrations, rather than unique per-node mTLS identities. Do not deploy on the public internet as a secure multi-tenant service.
- Simulator does **not** host a scene, open viewer ports, run physics/scripts, or persist world data. It just parses INIs and registers/renews example regions over HTTPS.
- Restarting a simulator before its prior 90-second lease expires can produce HTTP 409 until the lease expires, absent clean unregister; correct fencing and takeover are pending.
- There is no production-grade load balancing, login, inventory, federation, Atlas, economy or 500k concurrent session performance.
- License/EULA and the distribution of future proprietary source require a separate explicit decision. This public prototype does **not** include the future closed-source server core.
