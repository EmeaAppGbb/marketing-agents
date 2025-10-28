# ADR 0001: Backend Scaffolding Architecture - .NET 9 with Aspire Orchestration

## Status

**Accepted** - 2025-01-XX

## Context

The Marketing Agents application requires a foundational backend infrastructure that supports:
- AI agent orchestration and execution
- Real-time campaign artifact streaming
- RESTful API endpoints for frontend consumption
- Local development environment with service discovery
- Cloud-native deployment readiness
- High observability and reliability
- Rapid iteration and testing

Key requirements from [AGENTS.md](../../AGENTS.md):
- .NET 9 + ASP.NET Core Minimal APIs for backend
- .NET Aspire for local development orchestration
- Type-safe end-to-end implementation
- Azure Cosmos DB for data persistence
- SignalR for real-time communication
- OpenTelemetry for observability by default
- Central Package Management for dependency governance

## Decision Drivers

1. **Developer Experience**: Need rapid local development setup with minimal configuration
2. **Service Discovery**: Multiple services (API, AgentHost) must communicate seamlessly
3. **Observability**: Built-in telemetry, logging, and tracing required from day one
4. **Cloud Readiness**: Local development should mirror cloud deployment patterns
5. **Type Safety**: Strong typing throughout the stack to prevent runtime errors
6. **Testing**: Easy integration and unit testing infrastructure
7. **Scalability**: Architecture must support future microservices if needed

## Considered Options

### Option 1: Single Monolithic ASP.NET Core Project
**Structure:**
```
MarketingAgents/
├── Controllers/
├── Services/
├── Agents/
└── Program.cs
```

**Pros:**
- Simplest initial setup
- Single deployment unit
- No inter-service communication complexity
- Fast local development

**Cons:**
- Agent execution coupled with API layer
- Difficult to scale agent workloads independently
- Long-running agent tasks block API threads
- Limited isolation between concerns
- Harder to add distributed tracing
- Single point of failure

### Option 2: Multi-Repository Microservices with Docker Compose
**Structure:**
```
marketing-agents-api/          (separate repo)
marketing-agents-agent-host/   (separate repo)
docker-compose.yml             (orchestration)
```

**Pros:**
- Complete service isolation
- Independent deployment pipelines
- Team autonomy per service
- Clear service boundaries

**Cons:**
- Complex local development setup
- No shared code/libraries between services
- Difficult dependency management
- Manual service discovery configuration
- Harder to coordinate changes across services
- CI/CD complexity multiplied
- Slower iteration cycles

### Option 3: Monorepo with .NET Aspire Orchestration ✅ **SELECTED**
**Structure:**
```
marketing-agents/
├── MarketingAgents.AppHost/          # Aspire orchestrator
├── MarketingAgents.ServiceDefaults/  # Shared configuration
├── MarketingAgents.Api/              # REST API service
├── MarketingAgents.AgentHost/        # Agent execution service
└── MarketingAgents.*.Tests/          # Test projects
```

**Pros:**
- **Automatic service discovery** via Aspire dashboard
- **Built-in observability** (OpenTelemetry, logs, metrics, traces)
- **Shared configuration** via ServiceDefaults project
- **Local development parity** with cloud deployment
- **Easy testing** with WebApplicationFactory
- **Dependency injection** configured automatically
- **Resource management** (Redis, Cosmos DB emulators) built-in
- **Isolated concerns** while maintaining code sharing
- **Rapid iteration** with hot reload and debugging
- **Future-proof** for additional services or agents

**Cons:**
- Aspire learning curve for new developers
- AppHost not deployed to production (dev-only)
- Slightly more complex than monolith for simple apps

## Decision Outcome

**Chosen Option: Monorepo with .NET Aspire Orchestration (Option 3)**

### Rationale

1. **Aspire Provides Immediate Value:**
   - Eliminates manual service discovery configuration
   - Automatic OpenTelemetry integration across services
   - Built-in health checks and resilience patterns
   - Resource orchestration (Redis, Cosmos DB) with one line of code

2. **Separation of Concerns Without Complexity:**
   - `MarketingAgents.Api`: Handles HTTP requests, validation, response formatting
   - `MarketingAgents.AgentHost`: Executes long-running agent workflows without blocking API
   - Communication via service discovery (HTTP) and future SignalR hubs
   - Shared code via `ServiceDefaults` project

3. **Testing Excellence:**
   - Integration tests use `WebApplicationFactory` to test full HTTP pipelines
   - Services can be tested independently or together
   - Testcontainers integration for databases and dependencies
   - Coverage reporting across all projects

4. **Cloud Deployment Strategy:**
   - Aspire generates deployment manifests for Azure Container Apps
   - `azd up` command deploys entire stack with infrastructure as code
   - Services scale independently in production
   - Local development mirrors cloud architecture

5. **Alignment with AGENTS.md Standards:**
   - Central Package Management via `Directory.Packages.props`
   - Nullable reference types enabled project-wide
   - Roslyn analyzers enforced in `Directory.Build.props`
   - EditorConfig for consistent code style
   - Latest NuGet packages (Aspire 9.5.1, .NET 9)

### Implementation Details

**Project Structure:**
- **AppHost**: Orchestrates all services, configures dependencies (Redis, Cosmos DB), enables service discovery
- **ServiceDefaults**: Shared configuration for OpenTelemetry, health checks, service discovery, HTTP resilience
- **Api**: RESTful endpoints using Minimal APIs, Swagger/OpenAPI documentation
- **AgentHost**: Background service for agent orchestration and execution
- **Test Projects**: Separate unit and integration test projects for each service

**Key Technologies:**
- .NET 9 SDK with ASP.NET Core Minimal APIs
- Aspire 9.5.1 for orchestration and service discovery
- OpenTelemetry 1.13.1 for distributed tracing
- xUnit 2.9.3 + FluentAssertions for testing
- StyleCop + SonarAnalyzer for code quality

**Service Communication:**
- HTTP via service discovery (e.g., `https+http://apiservice`)
- Future: SignalR hubs for real-time bidirectional communication
- Automatic endpoint resolution via Aspire

**Local Development:**
1. `dotnet run --project MarketingAgents.AppHost` starts all services
2. Aspire dashboard accessible at `http://localhost:15888`
3. Services automatically discover each other
4. OpenTelemetry traces visible in dashboard
5. Health checks monitored in real-time

**Production Deployment:**
- AppHost is **NOT deployed** to production
- Use `azd up` to deploy to Azure Container Apps
- Or use `aspire publish` to generate manifests for Kubernetes/Docker Compose
- Services deployed as independent containers with auto-scaling

## Consequences

### Positive

✅ **Developer Productivity**: New developers run one command (`dotnet run --project AppHost`) to start entire stack  
✅ **Observability**: OpenTelemetry traces, logs, and metrics available immediately without manual configuration  
✅ **Testability**: Integration tests use real service interactions via WebApplicationFactory  
✅ **Scalability**: AgentHost can scale independently from API in production  
✅ **Maintainability**: Shared ServiceDefaults ensures consistent configuration across services  
✅ **Cloud Parity**: Local development environment mirrors Azure Container Apps deployment  
✅ **Future-Proof**: Easy to add new services (e.g., VisualConceptService, ComplianceService)  
✅ **Type Safety**: Strong typing enforced across all projects with nullable reference types  

### Negative

⚠️ **Learning Curve**: Developers unfamiliar with Aspire need to learn new concepts (service discovery, orchestration)  
⚠️ **Deployment Complexity**: Must use `azd` or `aspire publish` instead of simple `dotnet publish`  
⚠️ **Aspire Dependency**: Locked into Aspire for local development (though deployment is flexible)  
⚠️ **Preview Tooling**: Some Aspire features still in preview, may have breaking changes  

### Mitigations

- **Documentation**: Comprehensive MkDocs guides for local setup and deployment
- **Training**: Include Aspire tutorials in onboarding docs
- **Fallback**: Services can be run independently with manual configuration if needed
- **Version Pinning**: Use stable Aspire 9.5.1 release, not preview versions

## Related Decisions

- **ADR 0002** (future): Database persistence strategy (Azure Cosmos DB)
- **ADR 0003** (future): Real-time communication architecture (SignalR)
- **ADR 0004** (future): Agent orchestration patterns (Microsoft Agent Framework)
- **ADR 0005** (future): Deployment strategy (Azure Container Apps vs Kubernetes)

## References

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [AGENTS.md - Canonical Stack](../../AGENTS.md#canonical-stack)
- [Task 001 - Backend Scaffolding Specification](../tasks/001-task-backend-scaffolding.md)
- [Microsoft Learn - Service Discovery in .NET Aspire](https://learn.microsoft.com/dotnet/aspire/service-discovery/overview)
- [Microsoft Learn - OpenTelemetry in .NET Aspire](https://learn.microsoft.com/dotnet/aspire/fundamentals/telemetry)

---

**Authors:** AI Development Team  
**Last Updated:** 2025-01-XX  
**Review Status:** Approved
