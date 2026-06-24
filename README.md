# Distributed Microservices with .NET Aspire

A production-style e-commerce backend built with **.NET 10**, **Aspire**, **Clean Architecture**, **CQRS**, **MassTransit**, and **Semantic Kernel**. Demonstrates microservice orchestration, event-driven communication, and AI-powered recommendations.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     .NET Aspire AppHost                     │
│              (Orchestration + Dashboard + OTEL)             │
└───────────────┬─────────────────┬───────────────────────────┘
                │                 │
    ┌───────────▼──────┐  ┌───────▼───────────┐  ┌────────────────────────┐
    │  ProductService  │  │   OrderService    │  │  RecommendationService │
    │  (CRUD + Events) │  │ (CQRS + ACL)      │  │  (Semantic Kernel/AI)  │
    └───────────┬──────┘  └───────┬───────────┘  └────────────────────────┘
                │                 │
                └────────┬────────┘
                         │
              ┌──────────▼──────────┐
              │   RabbitMQ (AMQP)   │
              │  MassTransit broker │
              └─────────────────────┘

Each service has its own PostgreSQL database (Database-per-Service pattern)
```

### Services

| Service | Responsibility | Database |
|---|---|---|
| **ProductService** | Product catalog CRUD, publishes `ProductCreatedIntegrationEvent` | PostgreSQL (ProductDb) |
| **OrderService** | Place & query orders, consumes product events via Anti-Corruption Layer | PostgreSQL (OrderDb) |
| **RecommendationService** | AI-powered product recommendations via Semantic Kernel + OpenAI | Stateless |

---

## Key Technical Decisions

**Clean Architecture + CQRS** — Each service is split into `Domain`, `Application`, `Infrastructure`, and `API` layers. Commands and Queries are fully separated using MediatR.

**Event-Driven via MassTransit** — Services communicate asynchronously through RabbitMQ. `ProductService` publishes `ProductCreatedIntegrationEvent`; `OrderService` consumes it and maintains a local read model (Anti-Corruption Layer), eliminating cross-service queries at runtime.

**Anti-Corruption Layer (ACL)** — `OrderService` stores a local `ProductReadModel` synced from events. The Order domain never directly calls the Product API, preserving service autonomy.

**Semantic Kernel Integration** — `RecommendationService` uses Semantic Kernel with OpenAI chat completion to generate personalized product recommendations based on user browsing categories.

**Shared Building Blocks** — `Shared.BuildingBlocks` exposes base `Entity`, `AggregateRoot`, and CQRS interfaces (`ICommand<T>`, `IQuery<T>`, `IEventHandler<T>`). `Shared.Contracts` holds integration event contracts shared across services.

---

## Project Structure

```
├── src/
│   ├── AppHost/                        # .NET Aspire orchestration entry point
│   ├── ServiceDefaults/                # Shared OpenTelemetry, health checks, resilience
│   ├── Shared/
│   │   ├── Shared.BuildingBlocks/      # Base Entity, AggregateRoot, CQRS interfaces
│   │   └── Shared.Contracts/           # Integration event contracts (cross-service)
│   └── Services/
│       ├── ProductService/
│       │   ├── ProductService.API
│       │   ├── ProductService.Application
│       │   ├── ProductService.Domain
│       │   └── ProductService.Infrastructure
│       ├── OrderService/
│       │   ├── OrderService.API
│       │   ├── OrderService.Application
│       │   ├── OrderService.Domain
│       │   └── OrderService.Infrastructure
│       └── RecommendationService/
│           ├── RecommendationService.API
│           ├── RecommendationService.Application
│           └── RecommendationService.Infrastructure
└── tests/
    ├── OrderService.UnitTests
    ├── ProductService.UnitTests
    └── IntegrationTests
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- OpenAI API key (optional — RecommendationService degrades gracefully without it)

### Run with Aspire (Recommended)

Aspire automatically provisions RabbitMQ and two PostgreSQL instances via Docker.

```bash
git clone https://github.com/hadihamedian/distributed-microservices-aspire
cd distributed-microservices-aspire
dotnet run --project src/AppHost
```

The Aspire dashboard opens at `https://localhost:17149` and shows all services, logs, traces, and metrics in one place.

### Run with Docker Compose

```bash
docker-compose up --build
```

| Service | URL |
|---|---|
| ProductService | http://localhost:8081/swagger |
| OrderService | http://localhost:8082/swagger |
| RecommendationService | http://localhost:8083/swagger |
| RabbitMQ Management | http://localhost:15672 (guest/guest) |

### Configure OpenAI (Optional)

Set your API key before running:

```bash
# Aspire (user secrets)
dotnet user-secrets set "OpenAI:ApiKey" "your-key-here" --project src/AppHost

# Docker Compose
# Edit the OpenAI__ApiKey env variable in docker-compose.yml
```

Without a valid key, RecommendationService returns an empty list — no crash.

---

## API Reference

### ProductService — `http://localhost:8081`

```
POST   /api/products          Create a product
GET    /api/products          List all products
GET    /api/products/{id}     Get product by ID
```

**Create product example:**
```json
POST /api/products
{
  "name": "Wireless Keyboard",
  "description": "Compact mechanical keyboard",
  "price": 89.99,
  "stockQuantity": 150,
  "category": "Electronics"
}
```

### OrderService — `http://localhost:8082`

```
POST   /api/orders            Place an order
GET    /api/orders/{id}       Get order by ID
```

**Place order example:**
```json
POST /api/orders
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "items": [
    {
      "productId": "your-product-id",
      "quantity": 2,
      "unitPrice": 89.99
    }
  ]
}
```

### RecommendationService — `http://localhost:8083`

```
GET    /api/recommendations?userId={id}&categories=Electronics,Accessories
```

**Response:**
```json
[
  {
    "name": "Mechanical Gaming Mouse",
    "category": "Electronics",
    "reason": "Complements keyboard purchases in the Electronics category"
  }
]
```

---

## Event Flow

```
1. Client → POST /api/products
2. ProductService saves Product to ProductDb
3. ProductService publishes ProductCreatedIntegrationEvent → RabbitMQ
4. OrderService consumes event → saves ProductReadModel to OrderDb (ACL)
5. Client → POST /api/orders (uses ProductId from local read model)
6. OrderService saves Order → publishes OrderPlacedIntegrationEvent → RabbitMQ
```

---

## Running Tests

```bash
# Unit tests
dotnet test tests/ProductService.UnitTests
dotnet test tests/OrderService.UnitTests

# All tests
dotnet test
```

Unit tests cover command handlers using Moq and FluentAssertions. Integration tests use Testcontainers to spin up a real PostgreSQL instance.

---

## Observability

All services are instrumented with OpenTelemetry via `ServiceDefaults`:

- **Traces** — distributed tracing across service boundaries (ASP.NET Core + HTTP client)
- **Metrics** — runtime, HTTP, and custom metrics
- **Logs** — structured logging exported via OTLP

When running via Aspire, the built-in dashboard provides traces, logs, and metrics without any additional setup.

---

## CI/CD

GitHub Actions pipeline (`.github/workflows/ci.yml`) runs on every push to `main` and `develop`:

1. Build solution
2. Run ProductService unit tests
3. Run OrderService unit tests

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Orchestration | .NET Aspire (Latest) |
| Messaging | MassTransit 8 + RabbitMQ |
| Persistence | PostgreSQL + EF Core 10 |
| CQRS | MediatR 14 |
| AI | Semantic Kernel 1.x + OpenAI |
| Observability | OpenTelemetry + Aspire Dashboard |
| Testing | xUnit + Moq + FluentAssertions + Testcontainers |
| Containerization | Docker + Docker Compose |
