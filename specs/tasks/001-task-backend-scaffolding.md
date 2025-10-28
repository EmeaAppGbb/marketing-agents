# Task 001: Backend Scaffolding - Project Structure & Aspire Setup

## Description
Create the foundational backend infrastructure for the Marketing Agents application using .NET 9, ASP.NET Core Minimal APIs, and .NET Aspire orchestration. This includes setting up the monorepo structure, configuring service discovery, telemetry, and establishing the base projects required for the application.

## Dependencies
None - This is a foundational task

## Technical Requirements

### Project Structure
Create the following .NET projects:
- **MarketingAgents.AppHost**: .NET Aspire orchestration project for local development
- **MarketingAgents.Api**: Backend API service using ASP.NET Core Minimal APIs
- **MarketingAgents.AgentHost**: Dedicated agent execution and orchestration service
- **MarketingAgents.ServiceDefaults**: Shared service defaults for telemetry and service discovery

### Aspire Configuration
- Configure AppHost to orchestrate all services with proper service discovery
- Set up local emulators for dependencies (Cosmos DB Emulator, Redis)
- Configure service references using `.WithReference()` for automatic discovery
- Enable OpenTelemetry for logs, metrics, and traces via ServiceDefaults
- Configure health checks and Aspire dashboard integration

### Project Configuration
- Enable nullable reference types in all projects (`<Nullable>enable</Nullable>`)
- Configure nullable warnings as errors (`<WarningsAsErrors>nullable</WarningsAsErrors>`)
- Set up Central Package Management with `Directory.Packages.props`
- Configure Roslyn analyzers (StyleCop, SonarAnalyzer.CSharp, Microsoft.CodeAnalysis.NetAnalyzers)
- Create `.editorconfig` for consistent coding standards
- Set up file-scoped namespaces and global usings

### API Project Setup
- Configure Program.cs with middleware pipeline
- Set up Minimal APIs endpoint structure
- Create `Endpoints/` folder for route handlers
- Create `Services/` folder for domain logic
- Create `Models/` folder for DTOs and domain entities (use record types)
- Configure dependency injection
- Enable Swagger/OpenAPI documentation generation

### Service Defaults Configuration
- Configure OpenTelemetry (OTLP export)
- Set up service discovery
- Configure HTTP resilience with Polly (circuit breakers, retries, timeouts)
- Set up health checks with `MapHealthChecks()`
- Configure structured logging with correlation IDs

### Development Environment
- Configure .NET User Secrets for local development
- Set up comprehensive `.gitignore` for build artifacts, dependencies, secrets
- Create initial `README.md` with setup instructions

## Acceptance Criteria
- [ ] All four .NET projects created and building successfully
- [ ] Aspire dashboard accessible at default URL showing all services
- [ ] Service discovery working between Api and AgentHost projects
- [ ] Health check endpoints responding successfully
- [ ] OpenTelemetry traces visible in Aspire dashboard
- [ ] Swagger UI accessible for API project
- [ ] Nullable reference types enabled and building without warnings
- [ ] Central Package Management configured with latest stable packages
- [ ] Code quality analyzers running and passing
- [ ] Development environment documented in README.md

## Testing Requirements
- [ ] Unit tests project created: `MarketingAgents.Api.Tests`
- [ ] Integration tests project created: `MarketingAgents.Api.IntegrationTests`
- [ ] Test projects configured with xUnit and FluentAssertions
- [ ] Sample health check integration test using `WebApplicationFactory<TProgram>`
- [ ] Test coverage reporting configured
- [ ] All tests passing in CI pipeline

## Non-Functional Requirements
- Application startup time < 5 seconds
- Health check response time < 100ms
- Aspire dashboard responsive and showing all telemetry

## Out of Scope
- Actual agent implementation (covered in subsequent tasks)
- Database schema implementation (covered in persistence task)
- Authentication/authorization setup
- Production deployment configuration

## Notes
- Follow AGENTS.md guidance for .NET Aspire integration
- Use Microsoft Docs MCP server for latest Aspire best practices
- Ensure all services reference ServiceDefaults project
- Configure for local development only - Aspire not for production deployment
