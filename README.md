# Distributed Rate Limiter (gRPC, .NET 10, PostgreSQL)

A standalone, distributed rate-limiting service implemented in .NET 10, using gRPC as the primary API and PostgreSQL for durable state with optimistic concurrency control (OCC).
This project is designed as a portfolio-grade backend service, demonstrating correct handling of concurrency, clean layering, and production-oriented trade-offs.

## Overview

#### The service provides centralized rate limiting and quota enforcement for downstream services (e.g. API gateways, backend services), supporting:
* Token Bucket rate limiting
* Per-subject and per-resource limits
* Deterministic behavior under concurrency
* Horizontal scalability
* gRPC-first API
* PostgreSQL-backed state (no in-memory assumptions)

## Layered Design
| Layer | Responsibility |
|-------|----------------|
| Domain | Pure rate-limiting logic (Token Bucket, policies, decisions) |
| Persistence | EF Core + PostgreSQL, optimistic concurrency |
| Application | Orchestration, retry logic, consistency |
| Transport (gRPC) | API surface only (no business logic) |

#### This separation ensures:
* Deterministic behavior
* Testability
* No framework leakage into the domain

## Rate Limiting Model
#### Algorithm
* Token Bucket
* Fractional tokens supported
* Configurable capacity and refill rate
* Burst handling up to capacity

## Concurrency Strategy
* Optimistic concurrency control
* No distributed locks
* No transactions spanning domain logic
* Safe for horizontal scaling

Each token bucket state is versioned and updated with:
```sql
UPDATE ... WHERE version = X
```

Conflicts are retried in a bounded loop.

**Migrations are managed via EF Core and applied automatically on startup (for development / demo purposes).**

## Running Locally
### Prerequisites
* Docker
* Docker Compose
* PostgreSQL (optional if using containerized DB)

### Option 1: Run with PostgreSQL via Docker
```bash 
docker compose up --build 
```

This starts:
* PostgreSQL
* RateLimiter gRPC service on port 8080

### Option 2: Use Existing PostgreSQL (Local Testing)

**If PostgreSQL is already running on your machine:**

```yaml
environment:
  ConnectionStrings__Postgres: Host=host.docker.internal;Port=5432;Database=ratelimiter;Username=postgres;Password=***
```

**Note:** **localhost** will not work inside Docker containers.

## Database Migrations

Migrations live in **RateLimiter.Persistence**.

To add a migration:
```bash
dotnet ef migrations add InitialCreate \
  --project src/RateLimiter.Persistence \
  --startup-project src/RateLimiter.Api
```

To apply migrations manually:
```bash
dotnet ef database update \
  --project src/RateLimiter.Persistence \
  --startup-project src/RateLimiter.Api
```

Migrations are also applied automatically on container startup (development only).

## Testing the Service

Example using grpcurl:
```bash
grpcurl -plaintext localhost:8080 \
  ratelimiter.RateLimiter/Check \
  -d '{
    "subject": "user-1",
    "resource": "/orders",
    "cost": 1
  }'
```



### Project Structure
```bash
src/
├── RateLimiter.Api          # gRPC + application orchestration
├── RateLimiter.Domain       # Pure rate-limiting logic
├── RateLimiter.Persistence  # EF Core + PostgreSQL
└── RateLimiter.Tests        # Unit & concurrency tests
```

## Future Enhancements
* Redis adapter for hot-path optimization
* gRPC streaming for metrics
* Admin API for policy management
* Prometheus / OpenTelemetry integration
* mTLS for service authentication