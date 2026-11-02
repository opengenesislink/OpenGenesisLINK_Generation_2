#!/usr/bin/env bash
# Build and publish the self-authored Generation 2 control-plane prototype.
set -euo pipefail
cd "$(dirname "$0")"
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: .NET 10 SDK is required." >&2
  exit 1
fi
dotnet restore OpenGenesisLINK.Generation2.sln
dotnet build OpenGenesisLINK.Generation2.sln -c Release --no-restore
dotnet run --project tests/OGL.Gen2.SmokeTests -c Release --no-build
dotnet publish src/OGL.Gen2.GridFabric -c Release --no-build -o publish/gridfabric
dotnet publish src/OGL.Gen2.Simulator -c Release --no-build -o publish/simulator
echo "Built bootstrap executables in publish/gridfabric and publish/simulator."
