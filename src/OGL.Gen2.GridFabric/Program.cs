using System.Security.Cryptography;
using System.Text;
using OGL.Gen2.Contracts;
using OGL.Gen2.GridFabric;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

if (!Guid.TryParse(configuration["GridFabric:GridId"], out var gridId) || gridId == Guid.Empty)
    throw new InvalidOperationException("Configure a nonempty GridFabric:GridId.");

if (!int.TryParse(configuration["GridFabric:LeaseSeconds"], out var seconds) || seconds is < 30 or > 3600)
    throw new InvalidOperationException("GridFabric:LeaseSeconds must be between 30 and 3600.");

var token = Environment.GetEnvironmentVariable("OGL_GRIDFABRIC_SIMULATOR_TOKEN");
if (string.IsNullOrEmpty(token) || token.Length < 32)
    throw new InvalidOperationException("Set OGL_GRIDFABRIC_SIMULATOR_TOKEN to a strong random secret (>=32 characters).");

var endpoint = configuration["Kestrel:Endpoints:PublicHttps:Url"];
if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var publicUri) || publicUri.Scheme != Uri.UriSchemeHttps)
    throw new InvalidOperationException("GridFabric requires a Kestrel HTTPS PublicHttps endpoint.");

if (!builder.Environment.IsDevelopment() &&
    string.IsNullOrWhiteSpace(configuration["Kestrel:Endpoints:PublicHttps:Certificate:Path"]))
    throw new InvalidOperationException("Production requires an explicit TLS certificate file path.");

builder.Services.AddSingleton(new RegionRegistry(gridId, TimeSpan.FromSeconds(seconds)));
var app = builder.Build();

bool IsAuthorized(HttpRequest request)
{
    var authorization = request.Headers.Authorization.ToString();
    if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        return false;
    var received = authorization["Bearer ".Length..].Trim();
    if (received.Length != token.Length) return false;
    return CryptographicOperations.FixedTimeEquals(
        SHA256.HashData(Encoding.UTF8.GetBytes(received)),
        SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "ready", gridId }));

// Discovery is intentionally a small public prototype. Authorization, pagination,
// transport choices and visibility policy must be completed before production.
app.MapGet("/api/v1/regions", (RegionRegistry registry) => Results.Ok(registry.GetActive()));

app.MapPost("/api/v1/regions/register",
    (HttpRequest request, RegionRegistrationRequest region, RegionRegistry registry) =>
    {
        if (!IsAuthorized(request)) return Results.Unauthorized();
        if (region.GridId != gridId) return Results.BadRequest(new { error = "grid_mismatch" });
        if (!registry.TryRegister(region, out var lease))
            return Results.Conflict(new { error = "invalid_or_already_leased_region" });
        return Results.Ok(lease);
    });

app.MapPut("/api/v1/regions/{regionId:guid}/lease",
    (HttpRequest request, Guid regionId, RegionLeaseRequest payload, RegionRegistry registry) =>
    {
        if (!IsAuthorized(request)) return Results.Unauthorized();
        var lease = registry.Renew(regionId, payload);
        return lease is null
            ? Results.Conflict(new { error = "lease_missing_or_stale" })
            : Results.Ok(lease);
    });

app.MapPost("/api/v1/regions/{regionId:guid}/unregister",
    (HttpRequest request, Guid regionId, RegionLeaseRequest payload, RegionRegistry registry) =>
    {
        if (!IsAuthorized(request)) return Results.Unauthorized();
        return registry.Unregister(regionId, payload)
            ? Results.NoContent()
            : Results.Conflict(new { error = "lease_missing_or_stale" });
    });

app.Logger.LogInformation("OGL GridFabric prototype starting for grid {GridId}, HTTPS {Endpoint}", gridId, endpoint);
await app.RunAsync();
