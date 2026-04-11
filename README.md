# BeaTraction

A real-time attraction registration platform. Users browse attraction schedules and register for time slots, while administrators manage attractions, schedules, and registrations. Capacity is enforced atomically using Redis, with live slot availability pushed to all connected clients via SignalR.

## Tech Stack

**Backend** — ASP.NET Core 8, MediatR (CQRS), Entity Framework Core, FluentValidation, SignalR, Redis, MinIO, PostgreSQL, JWT

**Frontend** — React 19, TypeScript, Vite, React Router v7, Tailwind CSS, shadcn/ui, SignalR client

**Infrastructure** — Docker, PostgreSQL, Redis, MinIO

## Features

- Real-time capacity updates via SignalR — no refresh needed
- Atomic registration with Redis Lua scripts to prevent over-capacity under concurrent load
- Role-based access: `admin` for full CRUD, `user` for registration
- Image uploads for attractions stored in MinIO (S3-compatible)
- JWT authentication via HttpOnly cookies

## Getting Started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose
- (Optional, for local dev) [.NET 8 SDK](https://dotnet.microsoft.com/download) and [Node.js 20+](https://nodejs.org/)

### Running with Docker

```bash
# 1. Clone the repository
git clone <repo-url>
cd BeaTraction

# 2. Set up environment variables
cp .env.example .env
# Edit .env and fill in the required values (see below)

# 3. Start all services
docker-compose up
```

| Service      | URL                         |
|--------------|-----------------------------|
| Frontend     | http://localhost:5173       |
| Backend API  | http://localhost:5271/api   |
| Swagger Docs | http://localhost:5271/swagger |
| MinIO Console| http://localhost:9001       |

### Running Locally (without Docker)

**Backend:**
```bash
cd BeaTraction.WebAPI
dotnet restore
dotnet run
```

**Frontend:**
```bash
cd BeaTraction.Frontend
npm install
npm run dev
```

> You still need PostgreSQL, Redis, and MinIO running. The easiest way is to start only the infra services:
> ```bash
> docker-compose up postgres redis minio
> ```

## Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# Database
POSTGRES_DB=
POSTGRES_USER=
POSTGRES_PASSWORD=

# JWT
JWT_SECRET=
JWT_ISSUER=
JWT_AUDIENCE=
JWT_EXPIRATION_MINUTES=

# MinIO
MINIO_ROOT_USER=
MINIO_ROOT_PASSWORD=
MINIO_BUCKET_NAME=

# Frontend
VITE_API_URL=
VITE_API_BASE_URL=
```

## Architecture

The backend follows **Clean Architecture**, with strict dependency rules enforced by project references — inner layers have zero knowledge of outer layers.

```
┌─────────────────────────────────────────────┐
│             BeaTraction.WebAPI              │  ← Presentation
│         (Controllers, Startup config)        │
├─────────────────────────────────────────────┤
│          BeaTraction.Infrastructure         │  ← Infrastructure
│  (EF Core, Redis, MinIO, SignalR, JWT)      │
├─────────────────────────────────────────────┤
│           BeaTraction.Application           │  ← Application
│   (CQRS Handlers, DTOs, FluentValidation)   │
├─────────────────────────────────────────────┤
│             BeaTraction.Domain              │  ← Domain (core)
│    (Entities, Repository Interfaces,        │
│         Domain Events — no dependencies)    │
└─────────────────────────────────────────────┘
```

**Dependency rule**: `WebAPI` → `Infrastructure` → `Application` → `Domain`. The `Domain` layer has no NuGet dependencies — it is pure business logic.

**Key patterns:**
- **CQRS via MediatR** — every operation is a `Command` or `Query` with its own `Handler` and `Validator`, living in `Application/Commands/` or `Application/Queries/`
- **Repository pattern** — interfaces defined in `Domain/Interfaces/`, implemented in `Infrastructure/Repositories/`, injected via DI
- **Domain Events** — entities raise events (`RegistrationCreatedEvent`, etc.) dispatched by MediatR and handled in `Application/EventHandlers/` to trigger SignalR broadcasts
- **Atomic capacity enforcement** — registration counts are managed with Redis Lua scripts, making the check-and-increment operation atomic and race-condition-free

### Request Flow

```
HTTP Request
    → Controller (WebAPI)
        → MediatR Dispatch
            → FluentValidation (pipeline behavior)
            → Command/Query Handler (Application)
                → Repository (Infrastructure)
                    → PostgreSQL
                → Redis Lua script (atomic capacity check)
                → MinIO (image storage, if applicable)
            → Domain Event → EventHandler → SignalR broadcast
```

### Real-time Flow

Clients connect to `/hubs/attractions` (SignalR) and join a group per attraction. When a registration is created or deleted, the backend broadcasts the updated available count to all clients watching that attraction.

## API Endpoints

Full interactive documentation available at `http://localhost:5271/swagger` when running in development.

## Performance

The registration endpoint was stress tested with [k6](https://k6.io) against a locally running Docker stack. All three test scenarios passed with **zero overbooking**.

| Test | Throughput | Concurrent VUs | Avg Response | p95 Response | Result |
|------|-----------|----------------|-------------|--------------|--------|
| Stress | 100 req/s × 30s | 1,000 | 5.4 ms | 9 ms | PASSED |
| Spike | 10 → **500** → 10 req/s | 1,000 | 9.1 ms | 33 ms | PASSED |
| Extreme | **200 req/s** × 30s | up to **5,000** | 4.6 ms | 9 ms | PASSED |

The Redis Lua script that guards capacity is the key reason these numbers hold: the check-and-increment is a single atomic operation, so even at 500 req/s there are no race conditions and no phantom bookings.

> Note: these results were recorded on a single developer machine. Production throughput will vary with hardware and network conditions.

## Stress Testing

Load test scripts are in `stress-tests/`:

```bash
cd stress-tests
npm install
node registration-stress-test.js    # Sustained load
node registration-spike-test.js     # Spike traffic
node registration-extreme-test.js   # Extreme concurrency
```
