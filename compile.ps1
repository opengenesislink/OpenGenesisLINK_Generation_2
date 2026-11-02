# Build and publish the Generation 2 prototype on Windows / PowerShell.
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) { throw ".NET 10 SDK is required." }
function Invoke-Dotnet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed with exit code $LASTEXITCODE" }
}
Invoke-Dotnet restore OpenGenesisLINK.Generation2.sln
Invoke-Dotnet build OpenGenesisLINK.Generation2.sln -c Release --no-restore
Invoke-Dotnet run --project tests/OGL.Gen2.SmokeTests -c Release --no-build
Invoke-Dotnet publish src/OGL.Gen2.GridFabric -c Release --no-build -o publish/gridfabric
Invoke-Dotnet publish src/OGL.Gen2.Simulator -c Release --no-build -o publish/simulator
Write-Host "Built bootstrap executables in publish/gridfabric and publish/simulator."
