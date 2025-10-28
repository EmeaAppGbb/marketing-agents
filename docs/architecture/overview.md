# Architecture Overview

The Marketing Agents Platform is a modern, cloud-native application built with .NET 9 and .NET Aspire. This document provides a high-level overview of the system architecture, design decisions, and key components.

## System Architecture

### High-Level Design

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Layer                            │
│  ┌──────────────────┐          ┌──────────────────┐         │
│  │   Next.js Web    │          │  Mobile Apps     │         │
│  │   (React + TS)   │          │  (Future)        │         │
│  └────────┬─────────┘          └────────┬─────────┘         │
└───────────┼────────────────────────────┼───────────────────┘
            │                            │
            │ HTTP/REST + SignalR        │
            │                            │
┌───────────┼────────────────────────────┼───────────────────┐
│           │     Application Layer      │                   │
│  ┌────────▼─────────────────────────────▼────────┐         │
│  │     MarketingAgents.Api (ASP.NET Core)        │         │
│  │  ┌──────────────┐    ┌──────────────┐         │         │
│  │  │  REST API    │    │  SignalR Hub │         │         │
│  │  │  Endpoints   │    │  (Real-time) │         │         │
│  │  └──────────────┘    └──────────────┘         │         │
│  └───────────────────────┬────────────────────────┘         │
│                          │                                  │
│                          │ Service Discovery                │
│                          │                                  │
│  ┌───────────────────────▼────────────────────────┐         │
│  │   MarketingAgents.AgentHost                    │         │
│  │  ┌──────────────────────────────────────┐      │         │
│  │  │  Agent Orchestration Services        │      │         │
│  │  │  • Copywriting Agent                 │      │         │
│  │  │  • Short Copy Agent                  │      │         │
│  │  │  • Visual Concept Agent              │      │         │
│  │  │  • Audit/Compliance Agent            │      │         │
│  │  └──────────────────────────────────────┘      │         │
│  └─────────────────────┬──────────────────────────┘         │
└────────────────────────┼───────────────────────────────────┘
                         │
┌────────────────────────┼───────────────────────────────────┐
│                        │  Data & Infrastructure Layer      │
│  ┌─────────────────────▼──────────┐  ┌──────────────────┐  │
│  │  Azure Cosmos DB               │  │  Redis Cache     │  │
│  │  • Campaigns                   │  │  • Sessions      │  │
│  │  • Agent Responses             │  │  • Rate Limits   │  │
│  │  • Audit Logs                  │  └──────────────────┘  │
│  └────────────────────────────────┘                        │
│                                                             │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  Azure AI Foundry (Agent Runtime)                      │ │
│  │  • LLM Models (GPT-4, GPT-3.5)                         │ │
│  │  • Prompt Assets                                       │ │
│  │  • Safety Routing                                      │ │
│  └────────────────────────────────────────────────────────┘ │
│                                                             │
│  ┌────────────────────────────────────────────────────────┐ │
│  │  OpenTelemetry (Observability)                         │ │
│  │  • Distributed Tracing                                 │ │
│  │  • Metrics & Logs                                      │ │
│  │  • Aspire Dashboard (Local Dev)                        │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Component Responsibilities

#### MarketingAgents.Api
- **Purpose**: HTTP API layer handling client requests
- **Responsibilities**:
  - RESTful endpoint definitions using Minimal APIs
  - Request validation and response formatting
  - SignalR hub hosting for real-time updates
  - Authentication and authorization
  - OpenAPI/Swagger documentation generation
- **Technology**: ASP.NET Core 9, Minimal APIs, SignalR

#### MarketingAgents.AgentHost
- **Purpose**: Background service for long-running agent workflows
- **Responsibilities**:
  - AI agent orchestration (sequential and concurrent workflows)
  - Campaign generation via multi-agent collaboration
  - Compliance checking and audit logging
  - Retry logic with feedback loops
  - Real-time progress notifications via SignalR
- **Technology**: ASP.NET Core 9, Microsoft Agent Framework, Azure AI

#### MarketingAgents.ServiceDefaults
- **Purpose**: Shared service configuration for all .NET projects
- **Responsibilities**:
  - OpenTelemetry configuration (logs, metrics, traces)
  - Health check setup
  - Service discovery client configuration
  - HTTP resilience patterns (circuit breakers, retries)
- **Technology**: .NET Aspire extensions, OpenTelemetry

#### MarketingAgents.AppHost
- **Purpose**: Local development orchestrator (Aspire)
- **Responsibilities**:
  - Service discovery and endpoint management
  - Container orchestration (Redis, Cosmos DB emulator)
  - Dependency injection of connection strings
  - Telemetry aggregation and dashboard
- **Technology**: .NET Aspire 9.5.1
- **Note**: AppHost is **NOT deployed to production**

## Design Principles

### 1. Separation of Concerns

**API Layer** (MarketingAgents.Api):
- Handles HTTP communication only
- Thin controllers/endpoints
- No business logic
- Fast response times (< 100ms for health checks)

**Agent Layer** (MarketingAgents.AgentHost):
- Long-running agent workflows
- Business logic and orchestration
- Retry mechanisms and resilience
- Can scale independently from API

**Benefits**:
- API remains responsive even during long agent operations
- Independent scaling of workloads
- Clear boundaries for testing

### 2. Service Discovery

All services communicate via **logical names** instead of hardcoded URLs:

```csharp
// In Api project, calling AgentHost:
var client = httpClientFactory.CreateClient();
client.BaseAddress = new Uri("https+http://agenthost");
```

Aspire automatically resolves `agenthost` to the correct endpoint:
- **Local Dev**: `http://localhost:5002`
- **Azure**: `https://agenthost.azurecontainerapps.io`

**Benefits**:
- No configuration changes between environments
- Automatic load balancing in production
- Service mesh compatibility

### 3. Observability by Default

Every service is instrumented with **OpenTelemetry** automatically:

- **Traces**: Distributed tracing across service boundaries
- **Metrics**: HTTP request durations, CPU, memory, GC stats
- **Logs**: Structured logging with correlation IDs

**Implementation**:
```csharp
// In ServiceDefaults/Extensions.cs
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());
```

**Benefits**:
- Zero-config observability in local dev (Aspire dashboard)
- Production-ready telemetry export (OTLP)
- Root cause analysis via distributed traces

### 4. Type Safety End-to-End

**Backend**:
- Nullable reference types enabled project-wide
- Record types for DTOs and immutable data
- Fail builds on nullable warnings

**Frontend** (future):
- TypeScript strict mode
- Generated types from OpenAPI schemas
- No `any` types allowed

**Benefits**:
- Catch errors at compile time
- IntelliSense-driven development
- Safe refactoring

### 5. Resilience Patterns

HTTP clients automatically include resilience via **Polly**:

```csharp
// In ServiceDefaults/Extensions.cs
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(); // Circuit breaker, retries, timeouts
    http.AddServiceDiscovery();          // Automatic endpoint resolution
});
```

**Patterns Applied**:
- **Retry**: Transient errors retried with exponential backoff
- **Circuit Breaker**: Fast-fail after repeated failures
- **Timeout**: Prevent indefinite waits
- **Bulkhead Isolation**: Limit concurrent calls

## Data Flow

### Campaign Creation Flow

```
1. User submits campaign brief via REST API
   POST /api/campaigns
   ↓
2. Api service validates request and persists to Cosmos DB
   ↓
3. Api service invokes AgentHost via service discovery
   POST https+http://agenthost/orchestrate
   ↓
4. AgentHost starts orchestration workflow:
   a. Copywriting Agent generates long-form copy
   b. Short Copy Agent creates social media variants
   c. Visual Concept Agent generates poster designs
   d. Audit Agent validates compliance
   ↓
5. AgentHost streams progress via SignalR hub
   SignalR → Api → Client (real-time updates)
   ↓
6. AgentHost persists final campaign to Cosmos DB
   ↓
7. Api returns campaign ID to client
   Response: { "campaignId": "12345" }
```

### Real-Time Updates Flow

```
1. Client establishes SignalR connection
   WebSocket → Api service
   ↓
2. Client subscribes to campaign updates
   SignalR: JoinCampaignGroup(campaignId)
   ↓
3. AgentHost generates artifacts (copy, visuals)
   ↓
4. AgentHost emits events via SignalR hub
   SignalR: SendCampaignUpdate(campaignId, artifact)
   ↓
5. Client receives real-time updates
   UI updates with new artifacts
```

## Technology Choices

See **[ADR 0001: Backend Scaffolding Architecture](decisions.md)** for detailed rationale.

### Why .NET Aspire?

**Problem**: Complex local development setup with multiple services, containers, and dependencies.

**Solution**: Aspire provides:
- One-command startup: `dotnet run --project AppHost`
- Automatic service discovery
- Built-in observability dashboard
- Container orchestration (Redis, Cosmos DB)

**Trade-off**: Aspire learning curve, but massive productivity gains.

### Why Cosmos DB?

**Requirements**:
- Global distribution support
- Low-latency reads/writes
- Flexible schema for campaign artifacts
- Multi-region active-active replication

**Solution**: Azure Cosmos DB with SQL API

**Trade-off**: Higher cost than relational DB, but essential for scale.

### Why SignalR?

**Requirements**:
- Real-time bidirectional communication
- Automatic reconnection
- Backpressure handling
- Cross-platform support

**Solution**: ASP.NET Core SignalR with WebSocket transport

**Trade-off**: More complex than polling, but required for live updates.

## Deployment Architecture

### Local Development
- **Orchestration**: .NET Aspire AppHost
- **Containers**: Docker (Redis, Cosmos DB emulator)
- **Observability**: Aspire dashboard (http://localhost:15888)

### Production (Azure Container Apps)
- **Deployment**: `azd up` or `aspire publish`
- **Services**: Independent containers with auto-scaling
- **Service Discovery**: Built-in Envoy proxy
- **Observability**: Azure Monitor + Application Insights

## Security Considerations

### Authentication & Authorization
- **API**: Azure AD B2C tokens (JWT)
- **SignalR**: Connection authentication via query string token
- **Service-to-Service**: Managed Identity (Azure)

### Secrets Management
- **Local Dev**: .NET User Secrets
- **Production**: Azure Key Vault with automatic injection

### Data Protection
- **At Rest**: Cosmos DB encryption by default
- **In Transit**: TLS 1.2+ for all HTTP/SignalR connections
- **PII**: Data classification and region restrictions

## Next Steps

- **[Decisions (ADRs)](decisions.md)** - Architecture decision records
- **[Development Setup](../development/setup.md)** - Start building
- **System Design** (coming soon) - Detailed component interactions
- **Data Flow** (coming soon) - Sequence diagrams for key workflows
