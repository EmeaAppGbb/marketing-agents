# Use .NET Aspire for Local Development Orchestration

* Status: accepted
* Deciders: Development Team, Architecture Team
* Date: 2025-10-29

## Context and Problem Statement

The Marketing Agents Platform consists of multiple services (API, AgentHost, frontend) and dependencies (Cosmos DB, Redis, Azure AI). Developers need a consistent, efficient way to run and debug the entire system locally without manual configuration of each service and dependency.

How can we simplify local development and provide a smooth path from local to cloud deployment?

## Decision Drivers

* Developer experience - minimize setup time and configuration complexity
* Service discovery - services must be able to communicate without hardcoded URLs
* Observability - need built-in logging, tracing, and metrics for debugging
* Cloud alignment - local development should mirror cloud deployment
* Dependency management - automatic provisioning of databases, caches, etc.
* Deployment simplicity - easy path from local to Azure deployment

## Considered Options

* Option 1: .NET Aspire for orchestration
* Option 2: Docker Compose
* Option 3: Manual service startup with scripts

## Decision Outcome

Chosen option: ".NET Aspire for orchestration", because it provides the best developer experience with built-in service discovery, observability, and a direct path to Azure Container Apps deployment. It's purpose-built for .NET cloud-native applications and aligns perfectly with our technology stack.

### Positive Consequences

* **Simplified setup**: Single command (`dotnet run --project MarketingAgents.AppHost`) starts all services
* **Built-in observability**: Aspire dashboard provides logs, traces, and metrics out-of-the-box
* **Automatic service discovery**: Services reference each other by logical names, no hardcoded URLs
* **Azure deployment**: `azd up` deploys to Azure Container Apps with generated infrastructure
* **Strong typing**: Service references are strongly typed in C#
* **Consistent development**: All developers have identical local environments

### Negative Consequences

* **Learning curve**: Developers must learn Aspire-specific patterns
* **.NET dependency**: Requires .NET 9 SDK even for non-.NET developers working on frontend
* **Early adoption risk**: Aspire is relatively new (released 2024), potential for breaking changes
* **Local-only**: Aspire AppHost is not deployed to production, creates separation between local and production orchestration

## Pros and Cons of the Options

### Option 1: .NET Aspire for orchestration

.NET Aspire is a cloud-ready stack for building distributed applications, with built-in service discovery, observability, and deployment tools.

* Good, because it provides a unified development dashboard (http://localhost:15888)
* Good, because service discovery is automatic via logical service names
* Good, because it includes OpenTelemetry for distributed tracing across all services
* Good, because `azd up` command generates and deploys infrastructure to Azure
* Good, because it supports both .NET and non-.NET services (e.g., Next.js via Node.js hosting)
* Good, because ServiceDefaults project provides consistent telemetry and resilience across services
* Good, because it automatically manages container lifecycle for Redis, Cosmos DB emulator, etc.
* Bad, because it's a relatively new technology with limited community resources
* Bad, because AppHost is local-only and not deployed to production
* Bad, because non-.NET developers need .NET SDK installed for local development

### Option 2: Docker Compose

Docker Compose is a tool for defining and running multi-container Docker applications.

* Good, because it's widely adopted and well-understood
* Good, because it's language-agnostic
* Good, because docker-compose.yml can be reused in CI/CD pipelines
* Good, because extensive documentation and community support
* Bad, because no built-in service discovery (requires manual configuration of URLs)
* Bad, because no integrated observability dashboard
* Bad, because requires manual configuration of networking, environment variables for each service
* Bad, because no direct path to cloud deployment (separate deployment manifests needed)
* Bad, because less developer-friendly for .NET-specific features (hot reload, debugging)

### Option 3: Manual service startup with scripts

Use shell scripts or npm scripts to start each service individually.

* Good, because maximum flexibility and control
* Good, because no additional tooling required beyond runtime SDKs
* Good, because easy to understand (explicit commands)
* Bad, because requires developers to manually start 5+ services/containers in correct order
* Bad, because no service discovery (hardcoded URLs in configuration files)
* Bad, because no centralized logging or observability
* Bad, because prone to configuration drift between developers
* Bad, because time-consuming setup (5-10 minutes to start all services)
* Bad, because no deployment assistance

## Links

* [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
* [Aspire Overview](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview)
* [Deploy .NET Aspire to Azure Container Apps](https://learn.microsoft.com/dotnet/aspire/deployment/azure/aca-deployment)
* Related ADR: [0002-use-cosmos-db-for-data-persistence.md](0002-use-cosmos-db-for-data-persistence.md) (future)
