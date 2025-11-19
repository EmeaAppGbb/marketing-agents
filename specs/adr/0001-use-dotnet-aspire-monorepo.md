# ADR 0001: Use .NET Aspire with Monorepo Architecture

**Status**: Accepted

**Date**: 30 October 2025

## Context

The marketing agents application requires a full-stack architecture with backend API services, frontend web application, AI agent orchestration, and local development environment. The system needs to be cloud-native, observable, and developer-friendly.

## Decision Drivers

- Need for distributed application orchestration during local development
- Requirement for type-safe, high-performance backend (AGENTS.md mandates .NET 9 + ASP.NET Core)
- Need for modern frontend with Server/Client component split (AGENTS.md mandates Next.js 14)
- Requirement for built-in telemetry, service discovery, and resilience
- Need to support multiple projects in a cohesive structure

## Considered Options

1. **.NET Aspire monorepo** (AppHost + multiple projects)
2. Separate repositories for backend and frontend
3. Docker Compose for orchestration

## Decision Outcome

**Chosen: .NET Aspire monorepo architecture**

### Project Structure
```
/
├── MarketingAgents.AppHost/        # Aspire orchestration for local dev
├── MarketingAgents.Api/            # Backend API (ASP.NET Core Minimal APIs)
├── MarketingAgents.Web/            # Frontend (Next.js App Router)
├── MarketingAgents.ServiceDefaults/ # Shared telemetry, service discovery
├── MarketingAgents.AgentHost/      # AI agent orchestration service
├── packages/sdk/                   # Generated API clients
└── specs/                          # Documentation and ADRs
```

### Rationale

- **Aspire provides**: Local orchestration, service discovery, automatic telemetry, dashboard for monitoring
- **Monorepo benefits**: Atomic commits across stack, shared tooling, easier refactoring
- **AGENTS.md compliance**: Follows canonical stack exactly (.NET 9, Next.js 14, Aspire for local dev)
- **ServiceDefaults pattern**: Shared configuration for OpenTelemetry, health checks, and resilience

## Consequences

### Positive
- Built-in observability (logs, metrics, traces) via Aspire dashboard
- Automatic service discovery between projects
- Consistent development experience across team
- Easy addition of new services (Redis, Cosmos DB) via Aspire integrations

### Negative
- Aspire is local-dev only; must deploy to Azure Container Apps or similar for production
- Learning curve for developers new to Aspire
- Requires .NET SDK even for frontend-only development

### Implementation Notes
- Use `dotnet new aspire-apphost` for AppHost project
- Use `dotnet new aspire-servicedefaults` for ServiceDefaults project
- Reference ServiceDefaults in all .NET projects and call `builder.AddServiceDefaults()` in `Program.cs`
- Use `.AddNpmApp()` to integrate Next.js frontend with Aspire
- Deploy using `azd up` for Azure Container Apps
