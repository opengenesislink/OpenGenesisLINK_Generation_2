# OGL GridFabric — HTTPS/TLS endpoint contract

**Status:** planned configuration contract and example, **not** a currently available runnable GridFabric binary.

## External listener

- Default URI: `https://grid.example.org:8002/` (**HTTPS directly on TCP 8002**).
- Allow operators to change the bind address, advertised hostname, listener port and certificate source independently.
- Production mode: TLS required on public endpoints; missing/expired/unreadable certificate => listener must fail closed and raise a clear diagnostic.
- Target .NET 10 Kestrel hosting using endpoint configuration, with TLS 1.2 and TLS 1.3, HTTP/1.1 and HTTP/2. Certificate/hostname validation is mandatory at clients.
- Optionally use SNI for multiple domains and mTLS for authenticated **internal** GridFabric/region-service transport.
- Do not enable public plain HTTP merely because a reverse proxy or port mapping is present. Deploy an explicit internal-only HTTP listener only when permitted by a documented trusted reverse-proxy topology.
- This file uses the term SSL colloquially; the actual transport is modern **TLS**.

## Candidate operator configuration

The [example file](../../examples/gridfabric/appsettings.example.json) uses Kestrel's standard endpoints configuration:

```json
{
  "Kestrel": {
    "Endpoints": {
      "PublicHttps": {
        "Url": "https://0.0.0.0:8002",
        "Protocols": "Http1AndHttp2",
        "SslProtocols": ["Tls12", "Tls13"],
        "Certificate": {
          "Path": "/etc/opengenesislink/gridfabric/tls/server.pfx"
        }
      }
    }
  }
}
```

This is an **illustrative subset** of the full example file. The certificate must contain a usable private key and match the publicly advertised hostname. Keep it outside the repository and restrict file permissions.

Provide any required PFX password through a secret manager or deployment environment, **never** in committed JSON:

```sh
# illustrative environment variable *name* — supply the value through
# your secret store, not a committed file or shell history:
Kestrel__Endpoints__PublicHttps__Certificate__Password
```

Kestrel configuration providers permit endpoint override, for example `Kestrel__Endpoints__PublicHttps__Url` to change the TLS bind port. Operators should also update firewall/LB routing and externally advertised URI; these are not inferred from the binding. On Linux, a non-root process can listen on TCP 8002 without privileged-port capabilities.

## Certificate lifecycle

1. Obtain a trusted certificate for the public **hostname** (not merely for a port).
2. Provision the private key securely and set least-privilege read access for the GridFabric process.
3. Plan automatic renewal and replacement plus alerting before certificate expiry.
4. Validate rotation during active sessions and, where necessary, support safe connection draining.
5. Log only the certificate fingerprint/expiration metadata, never key contents or secrets.

Kestrel supports certificate file monitoring/reloading; the final implementation must test that behavior with its actual deployment and certificate management. Let's Encrypt public ACME issuance is normally validated on standard challenge endpoints (such as 80/443) or through DNS-01; **listening on 8002 alone does not by itself solve certificate issuance**. An ACME/DNS automation or a trusted external certificate provider is therefore required.

## Reverse proxy and firewall

- **Direct TLS:** Client -> `grid.example.org:8002` -> GridFabric Kestrel TLS.
- **Optional front proxy:** Client -> `grid.example.org:443` (TLS) -> controlled upstream to GridFabric (TLS on 8002 can be retained).
- Restrict internal service endpoints to private interfaces/firewall policies and enable mTLS where trust boundaries justify it.
- If forwarding headers are required, trust only explicitly configured proxy IPs/networks. Never trust client-supplied identity or scheme headers by default.
- Rate limiting at the edge should be coordinated or distributed; per-gateway counters alone do not enforce a cluster-wide quota.

## Pre-release security acceptance

A production test must reject expired/incorrect certificates, insecure protocol downgrades, secret leakage, direct access to private endpoints, spoofed forwarding headers and port misconfiguration. Also test certificate rotation, failover and reconnection while logged in.

## Primary platform references

- [Microsoft: Kestrel endpoint configuration (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-10.0)
- [Microsoft: Kestrel security considerations](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/security-considerations?view=aspnetcore-10.0)
