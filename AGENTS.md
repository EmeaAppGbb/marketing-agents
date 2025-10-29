# AGENTS.md

## Mission

Build agents and applications that turn specifications into production-ready experiences with high reliability, speed, and delight. Every implementation decision must optimize for developer ergonomics, observable quality, and rapid iteration.

## Guiding Principles

- **Specification driven:** Treat written product specs, OpenAPI contracts, JSON Schemas, and design tokens as the source of truth. Automate code generation and validation wherever possible.
- **Type-safe end to end:** Enforce strong typing across the entire stack. Fail builds on type regressions and violations of type safety rules.
  - Backend: Strong typing via record types and nullable reference types in C#
  - Frontend: Full TypeScript coverage with strict mode enabled
- **Quality gates first:** Unit, integration, contract, and e2e tests run on every PR. Blocks merge when coverage or quality thresholds dip below agreed budgets (≥85% coverage).
- **Observability by default:** Emit structured logs, traces, and metrics for every service. Expose key health indicators through standardized dashboards and alerting.
- **Security & compliance everywhere:** Least privilege access, secret scanning, dependency hygiene, and threat modeling are non-negotiable steps in planning and delivery.
- **Never reinvent the wheel:** NEVER implement features that already exist in established frameworks and libraries. Always leverage existing, well-tested implementations from the canonical stack.
  - **Do NOT** implement custom agent orchestration logic—use Microsoft Agent Framework
  - **Do NOT** implement custom OAuth 2.0 flows—use Microsoft Identity Platform (MSAL)
  - **Do NOT** implement custom authentication/authorization—use ASP.NET Core Identity or Microsoft Entra ID
  - **Do NOT** implement custom real-time communication protocols—use SignalR
  - **Do NOT** implement custom HTTP resilience patterns—use Polly via Microsoft.Extensions.Http.Resilience
  - **Do NOT** implement custom telemetry—use OpenTelemetry via Aspire ServiceDefaults
  - **Do NOT** implement custom service discovery—use .NET Aspire service discovery
  - When in doubt, research the framework/library documentation first using Microsoft Docs MCP to find existing solutions
- **Always use latest packages:** Always fetch and use the latest stable versions of packages and dependencies unless there's a specific compatibility constraint. Update dependencies regularly.
  - Backend: Latest NuGet packages
  - Frontend: Latest npm packages
- **Modular, self-contained tasks:** Design each feature task to be independently implementable with minimal cross-dependencies. Modules and classes should be self-contained and loosely coupled to enable safe refactoring and parallel development.
- **MCP tool usage for learning**: When researching services, frameworks, or libraries during development, always query Microsoft Docs MCP (`microsoft.docs.mcp`) first for official Microsoft documentation. Only use Context7 MCP if information cannot be found in Microsoft Docs.

## Canonical Stack

| Layer | Technology | Notes |
| --- | --- | --- |
| **Backend Framework** | .NET 9 + ASP.NET Core (Minimal APIs) | Cloud-native, high-performance APIs with built-in DI, middleware pipeline, and async-first patterns. Strong typing via record types and nullable reference types. |
| **Frontend Framework** | Next.js 14 (App Router) + React 18 + TypeScript | Embrace Server Components + Client Components split. Support alternative adapters (e.g., Nuxt/Vue) only with explicit approval. |
| **Orchestration** | .NET Aspire | Local development orchestrator for distributed apps with service discovery, telemetry, and resource management. Not for production—deploy to Azure Container Apps or Kubernetes. |
| **Agent Runtime** | Microsoft Agent Framework + Azure AI Foundry | Central orchestration for LLM/agent workflows, prompt assets, safety routing, and evaluation. Use **Microsoft.Agents.AI** and **Microsoft.Extensions.AI** NuGet packages. |
| **Database** | Azure Cosmos DB | Primary database for all data storage. Use native Azure Cosmos DB SDK (`Microsoft.Azure.Cosmos`) directly—**do NOT use Entity Framework**. |
| **Realtime** | WebSocket + SignalR | Native ASP.NET Core SignalR for real-time communication with automatic reconnection, backpressure handling, and protocol negotiation. Frontend consumes SignalR endpoints for bidirectional communication. |
| **Backend Testing** | xUnit (preferred), NUnit, or MSTest + FluentAssertions | Keep coverage ≥ 85%. Contract tests from OpenAPI schema. Integration tests with WebApplicationFactory. |
| **Frontend Testing** | Vitest + React Testing Library + Playwright | Unit tests with Vitest, interaction tests with Testing Library, E2E tests with Playwright. Keep coverage ≥ 85%. |
| **Backend Code Quality** | Roslyn analyzers + EditorConfig + CSharpier or dotnet format | Automated via pre-commit hooks and IDE integration. |
| **Frontend Code Quality** | ESLint (Flat config) + Prettier + Stylelint | Automated via pre-commit hooks and IDE integration. |
| **State Management** | TanStack Query for remote data, Zustand or Jotai for local state | Cache consistency and optimistic updates with TanStack Query. |
| **Backend Package Manager** | NuGet with Central Package Management | Central package versioning via `Directory.Packages.props`. |
| **Frontend Package Manager** | pnpm (workspace mode) | Deterministic installs for Node.js packages. |
| **Monorepo Tooling** | Nx (preferred) or Turborepo | Enforce consistent build graphs, caching, and affected-only CI pipelines. |
| **CI/CD** | GitHub Actions with reusable workflows | Stages: lint → test → build artifacts → security scan → deploy. Use `dotnet` CLI for backend builds and tests. |
| **Environment Secrets** | Azure Key Vault or GitHub OIDC → Cloud secret store | `.env` and User Secrets for local dev only; never commit secrets. |

## Architecture Blueprint

### 1. Monorepo Layout

```
/
├── {appname}.AppHost/           # .NET Aspire orchestration project for local dev
├── {appname}.Api/               # Backend API service (ASP.NET Core Minimal APIs)
├── {appname}.Web/               # Frontend web app (Next.js App Router)
├── {appname}.ServiceDefaults/   # Shared service defaults (telemetry, service discovery)
├── {appname}.AgentHost/         # Agent framework host for orchestration & execution
├── packages/
│   └── sdk/                     # Generated API clients from OpenAPI specs
├── infra/                       # IaC (Bicep/Terraform) and deployment manifests
├── docs/                        # MkDocs documentation
├── specs/                       # Product specifications and ADRs
│   ├── adr/                     # Architecture Decision Records (MADRs)
│   ├── journal/                 # Engineering journal entries
│   └── product-specs/           # Product requirement documents
├── mkdocs.yml                   # MkDocs configuration
└── README.md
```

### 2. Domain Boundaries

- Separate presentation, orchestration, and domain logic. No cross-layer imports without contracts.
- Use CQRS-inspired patterns for read/write segregation when scaling event-driven workloads.
- Follow Clean Architecture or Vertical Slice Architecture patterns within projects.

### 3. Data Flow

- **API contracts defined via backend endpoints** → OpenAPI spec published → clients generated into `/packages/sdk`.
- **Frontend consumes the SDK** via TanStack Query to ensure cache consistency and retries.
- **Realtime channels** use WebSocket/SignalR hubs with automatic reconnection and backpressure handling.
- **Service-to-service communication** uses service discovery with automatic endpoint resolution.

## Development Playbook

### Backend Development (ASP.NET Core + .NET Aspire)

#### 1. Project Structure

- **`Program.cs`**: Application startup, middleware pipeline, and Minimal API endpoint definitions
- **`Endpoints/`**: Endpoint groups and route handlers (Minimal APIs pattern)
- **`Services/`**: Domain logic, orchestrating agents and external services (registered via DI)
- **`Models/`**: DTOs, domain entities, and data models (use records for immutability)
- **`Data/`**: Cosmos DB repository implementations and database client configurations
  - **Prefer Cosmos DB for data persistence**: Use Azure Cosmos DB as the primary database
  - **Do NOT use Entity Framework**: Use the native Azure Cosmos DB SDK (`Microsoft.Azure.Cosmos`) directly
  - Implement repository pattern with async methods for CRUD operations
  - **Auto-initialize database in development**: Create containers and databases automatically on startup in development mode

#### 2. .NET Aspire Integration

**Always consult Microsoft Docs MCP server**: Use the Microsoft Docs MCP server (`microsoft_code_sample_search` and `microsoft_docs_search` tools) to retrieve the latest code samples, snippets, best practices, and integrations for .NET Aspire.

- Reference the `ServiceDefaults` project from all .NET services for shared telemetry, service discovery, and resilience
- Call `builder.AddServiceDefaults()` early in `Program.cs` to enable Aspire integrations
- Use `builder.AddAzureCosmosClient()`, `builder.AddRedisClient()`, `builder.AddAzureServiceBusClient()`, etc., from Aspire hosting integrations
- Define dependencies in `AppHost/Program.cs` using `WithReference()` for automatic service discovery and connection string injection

**AppHost Project Structure:**
- Create an AppHost project using `dotnet new aspire-apphost`
- Define your app model in `Program.cs` using `DistributedApplication.CreateBuilder()`
- **All service hosts must be defined in the AppHost**: Add resources using `.AddProject<T>()`, `.AddContainer()`, `.AddRedis()`, `.AddAzureCosmosDB()`, `.AddAzureServiceBus()`, etc.
- **Use Aspire client integrations in services**: Wire up Aspire clients in consuming services to leverage service discovery and configuration
- **Use local emulators for development**: Configure Aspire to use local emulators (Cosmos DB Emulator, Azurite, Redis locally) via `.RunAsEmulator()` or `.RunAsContainer()`
- For Node.js/Next.js frontends: Add `Aspire.Hosting.NodeJs` package and use `.AddNpmApp("webapp", "../{appname}.Web")`
- Express dependencies with `.WithReference()` for automatic service discovery
- Use `.WaitFor()` to ensure startup ordering between services

**ServiceDefaults Project:**
- Create a shared ServiceDefaults project using `dotnet new aspire-servicedefaults`
- All .NET services should reference this project and call `builder.AddServiceDefaults()` in `Program.cs`
- ServiceDefaults configures: OpenTelemetry (logs, metrics, traces), service discovery, HTTP resilience (Polly), and health checks

**Service Discovery:**
- Services reference each other using logical names defined in the AppHost (e.g., `"apiservice"`)
- Use `https+http://apiservice` as the base URL in HttpClient configurations—Aspire resolves to actual endpoints

**Deployment:**
- Aspire is for local development only. **Do not deploy the AppHost to production**
- Use `azd init` and `azd up` to deploy to Azure Container Apps with auto-generated infrastructure
- Run `aspire publish` to generate deployment manifests for custom targets (Kubernetes, Docker Compose)

#### 3. Type Safety & C# Best Practices

- **Nullable reference types**: Enable in all projects
  - Add `<Nullable>enable</Nullable>` to `.csproj` files
  - Treat nullable warnings as errors: `<WarningsAsErrors>nullable</WarningsAsErrors>`
- **Use record types**: Prefer `record` for DTOs and immutable data models
- **Use init-only properties**: Leverage `init` accessors for immutable objects
- **Pattern matching**: Use modern C# pattern matching for cleaner code
- **File-scoped namespaces**: Use file-scoped namespace declarations
- **Global usings**: Define common usings in `GlobalUsings.cs`
- **Required members**: Use `required` keyword for mandatory properties in C# 11+

#### 4. Async Excellence & Best Practices

- All I/O operations must be async (`async`/`await`)
- Never block with `.Result` or `.Wait()`
- Use ASP.NET Core's built-in dependency injection with scoped, singleton, and transient lifetimes
- Minimize middleware complexity
- Keep hot code paths fast and avoid long-running tasks in the request pipeline
- Use `IAsyncEnumerable<T>` for streaming responses and pagination

#### 5. Realtime & Media

- SignalR hubs for real-time bidirectional communication (WebSockets with fallback)
- Define strongly-typed hub contracts
- Ensure proper backpressure handling in SignalR streaming methods using `IAsyncEnumerable<T>` or `ChannelReader<T>`
- For WebRTC signaling, use SignalR or dedicated WebSocket endpoints
- Offload TURN/STUN to Azure Communication Services
- Configure CORS policies for SignalR cross-origin connections

#### 6. Agents Integration

**Always use AI Toolkit best practices**: Before implementing agent code, use `#aitk-get_agent_code_gen_best_practices` to retrieve the latest best practices and guidance for Microsoft Agent Framework code generation.

**Always use Microsoft Docs MCP server**: Search for code samples and snippets when working with Microsoft Agents Framework using the Microsoft Docs MCP server (`microsoft_code_sample_search` tool).

**Required NuGet packages**: Install `Microsoft.Agents.AI`, `Microsoft.Agents.AI.OpenAI`, `Microsoft.Extensions.AI`, and `Microsoft.Extensions.AI.OpenAI` packages.

**Configuration and Setup:**
- **Prefer Aspire's integration for AI Inference clients**: When available, always use Aspire's integration for AI Inference clients via `.AddChatClient()` or similar Aspire hosting integrations
- **Use `ChatClientAgent` for all agents**: Wrap all agents using `ChatClientAgent` from `Microsoft.Agents.AI` with proper agent descriptions, names, and instructions
- **Agent construction pattern**: Create agents with `new ChatClientAgent(chatClient, instructions: "...", name: "AgentName")` in service constructors
- **Configuration chain**: `Azure OpenAI Client → ChatClient → .AsIChatClient() → ChatClientAgent → AIAgent` is the proper initialization chain
- **Use `IChatClient` abstraction**: All agent services should depend on `IChatClient` in constructors for flexibility and testability, but wrap it in `ChatClientAgent`

**Agent Execution:**
- **Use `AIAgent.RunAsync()` pattern**: Call agents using `await _agent.RunAsync(prompt)` instead of direct `IChatClient` calls
- **Never use `IChatClient` directly for agent operations**: Always wrap in `ChatClientAgent` and use `AIAgent.RunAsync()`

**Tools and Capabilities:**
- **Implement tools using `AIFunctionFactory`**: Define agent capabilities as tools using `AIFunctionFactory.Create()` with clear descriptions and parameter documentation
- **Tool registration pattern**: Register tools in agent constructor using `tools: [AIFunctionFactory.Create(Method1), AIFunctionFactory.Create(Method2)]` parameter
- **Equip agents with domain-specific tools**: Each agent should have 3+ relevant tools that enable specific capabilities (e.g., validation, optimization, analysis)
- **Prefer AI-powered over rule-based logic**: Use LLM intelligence with tools for decision-making rather than simple string matching or rule engines

**Orchestration Patterns:**
- **Implement orchestration patterns**: Support both sequential (one-after-another) and concurrent (parallel) agent execution patterns via orchestrator services
- Use pattern matching for agent routing
- **Retry logic with compliance feedback**: Implement retry loops (5+ attempts) with exponential backoff. Pass rejection feedback from compliance agents to the next retry attempt
- **Campaign brief construction**: Build comprehensive context strings that include theme, product details, target audience, and optional revision feedback from previous iterations

**Architecture:**
- **AgentHost architecture**: Create a dedicated `{appname}.AgentHost` project for distributed agent execution, orchestration, and background processing separate from the API layer
- **Agent providers pattern**: Implement agents as provider classes (e.g., `CopywritingAgentProvider`, `AuditAgentProvider`) that encapsulate agent creation, tool registration, and configuration
- **Orchestration services**: Create orchestrator services that coordinate multiple agents with sequential workflows (dependent execution) and concurrent workflows (parallel execution)

**Dependency Injection:**
- **DI registration**: Register `IChatClient` as singleton via `.AsIChatClient()`. Register agent services as scoped for per-request isolation
- Implement Microsoft Agent Framework skills as injectable services (singleton or scoped)
- Use `IHostedService` or `BackgroundService` for long-running agent workers and background processing

**Storage and Memory:**
- Persist agent memory/state in Cosmos DB using the native Azure Cosmos DB SDK with encryption at rest
- Record conversations, decisions, and tool invocations for review—pipe to data lake storage with retention policies

**Quality and Governance:**
- Define clear SLAs for latency, accuracy, and guardrail enforcement
- Build regression suites that run in CI using offline evaluation datasets
- Provide SDK shims so frontend clients can call agent endpoints asynchronously (REST, streaming, or SignalR) with graceful fallback modes

#### 7. Observability & Resilience

- OpenTelemetry is configured automatically via Aspire `ServiceDefaults` (logs, metrics, traces exported via OTLP)
- Use ASP.NET Core health checks (`MapHealthChecks()`) and expose via Aspire dashboard
- Implement resilience with Polly (circuit breakers, retries, timeouts) via `Microsoft.Extensions.Http.Resilience`
- Return Problem Details (RFC 7807) for errors using `Results.Problem()` in Minimal APIs
- Use structured logging with `ILogger<T>` and log scopes for correlation

#### 8. Testing Strategy

- **Test Framework**: xUnit as preferred test framework. NUnit and MSTest are acceptable alternatives
- **Integration Tests**: Use `WebApplicationFactory<TProgram>` for integration tests, testing full HTTP pipelines
- **Assertions**: FluentAssertions for readable test assertions
- **Coverage Areas**:
  - Unit tests (services, domain logic)
  - Integration tests (API endpoints, database)
  - Contract tests (OpenAPI validation)
- Use Testcontainers for integration tests requiring databases or message brokers
- Load testing with k6, JMeter, or NBomber for high-throughput endpoints and SignalR hubs
- Keep coverage ≥ 85%

#### 9. Code Quality

**Pre-commit hooks** running:
- **dotnet format or CSharpier**: Consistent code formatting
- **Roslyn analyzers**: Enable StyleCop, SonarAnalyzer.CSharp, Microsoft.CodeAnalysis.NetAnalyzers
- **Commit message lint**: Enforce Conventional Commits format

**Configuration:**
- **EditorConfig**: Use `.editorconfig` for consistent coding standards across IDEs
- **Automated via IDE integration**: Configure Visual Studio, VS Code, or Rider
- **CI enforcement**: Run `dotnet format --verify-no-changes` in CI pipeline
- **Static analysis**: Configure analyzers in `Directory.Build.props`:
  ```xml
  <PropertyGroup>
    <AnalysisLevel>latest</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  ```

#### 10. Best Practices

- **Latest NuGet packages**: Always use latest stable NuGet packages unless there's a specific compatibility constraint
- **Central Package Management**: Use `Directory.Packages.props` for centralized NuGet package version management
- **Dependency updates**: Update dependencies regularly via Renovate bot with grouped update strategy
- **Cloud-native APIs**: High-performance APIs with built-in DI, middleware pipeline, and async-first patterns
- **Strong typing**: Record types and nullable reference types throughout
- **Build enforcement**: Fail builds on nullable warnings
- **Secrets management**: .NET User Secrets (`dotnet user-secrets`) for local development, Azure Key Vault for cloud environments
- **API documentation**: Generate OpenAPI/Swagger docs via Swashbuckle or NSwag with XML documentation comments

### Frontend Development (Next.js + React + TypeScript)

#### 1. Scaffolding & Folder Structure

- `app/` for routes (App Router)
- `components/` for shared UI components
- `features/<domain>/` for spec-driven modules
- `hooks/` for custom React hooks
- `lib/` for utility functions and shared logic
- `styles/` for global styles and themes
- Keep server actions isolated under `app/(server-actions)/`

#### 2. Spec-to-UI Generation

- Consume OpenAPI/JSON Schema to auto-generate forms with libraries like React Hook Form + Zod
- Leverage design tokens (Style Dictionary) to ensure theme consistency
- Document UI variants in Storybook with MDX stories

#### 3. State & Data Management

- Use React Server Components for data-fetch-heavy views
- Fall back to client components when interactivity is required
- Adopt TanStack Query for remote data caching and optimistic updates
- Use Zustand or Jotai for local ephemeral state
- Consume backend SDK via React Query to ensure cache consistency and retries

#### 4. Performance & Accessibility

**Performance Budgets:**
- Largest Contentful Paint (LCP) < 2.5s
- Cumulative Layout Shift (CLS) < 0.1
- Enforce via Lighthouse CI

**Accessibility:**
- Mandate WCAG 2.2 AA compliance
- Integrate axe-core tests in CI

#### 5. Testing Strategy

- **Unit tests:** Vitest per component/hook
- **Interaction tests:** React Testing Library + user-event
- **Visual regression:** Storybook stories double as visual regression baselines (Chromatic or Loki)
- **E2E tests:** Playwright suite tied to critical flows:
  - Authentication
  - Agent interaction
  - Realtime session setup
- Keep coverage ≥ 85%

#### 6. Type Safety

- **Full TypeScript coverage**: Every file must have proper TypeScript types
- **Strict configuration**: Enable strict mode in `tsconfig.json`
  - `strict: true`
  - `noUncheckedIndexedAccess: true`
  - `noImplicitOverride: true`
- **Fail builds on type regressions**: Run `tsc --noEmit` in CI pipeline
- **Generated types**: Use generated types from OpenAPI schemas for API contracts
- **No `any` types**: Avoid using `any`; use `unknown` with proper type guards instead
- **Type-safe API clients**: Generate TypeScript SDK from OpenAPI spec into `/packages/sdk`

#### 7. Code Quality

**Pre-commit hooks** running:
- **ESLint (Flat config)**: Use modern flat config format (`eslint.config.js`)
- **Prettier**: Consistent code formatting across the codebase
- **Stylelint**: CSS/SCSS linting for style files
- **Commit message lint**: Enforce Conventional Commits format

**Automation:**
- **Automated via IDE integration**: Configure VS Code, WebStorm, or other IDEs
- **CI enforcement**: Run all linters in CI pipeline before tests
- **TypeScript strict checks**: Enable `tsc --noEmit` in pre-commit and CI

#### 8. Architecture Patterns

- Embrace Server Components + Client Components split
- Server Components for data fetching and SEO-critical content
- Client Components for interactive features and real-time updates
- Support alternative adapters (e.g., Nuxt/Vue) only with explicit approval

#### 9. Realtime Communication

- Consume SignalR endpoints for real-time bidirectional communication
- Implement graceful degradation and reconnection logic
- Handle WebSocket fallbacks appropriately

#### 10. Best Practices

- **Latest npm packages**: Always use latest stable npm packages unless there's a specific compatibility constraint
- **Package manager**: Use pnpm with workspace mode for deterministic installs
- **Dependency updates**: Update dependencies regularly via Renovate bot with grouped update strategy
- **Lock files**: Always commit `pnpm-lock.yaml` for reproducible builds
- Keep components modular and self-contained
- Follow specification-driven development
- Generate SDK clients from OpenAPI specs into `/packages/sdk`
- Use design tokens for consistent theming

### Full-Stack Patterns

#### 1. API Contract Flow

1. Define backend API endpoints using ASP.NET Core Minimal APIs with record types
2. Generate OpenAPI specification via Swashbuckle or NSwag
3. Publish OpenAPI spec to `/packages/sdk`
4. Generate TypeScript SDK from OpenAPI spec
5. Frontend consumes SDK via TanStack Query

#### 2. Type Safety End-to-End

- **Backend**: Record types, nullable reference types, fail builds on nullable warnings
- **Contract**: OpenAPI schema with validation
- **Frontend**: TypeScript strict mode, generated types from OpenAPI, fail builds on type errors

#### 3. Real-Time Communication

- **Backend**: SignalR hubs with strongly-typed contracts
- **Frontend**: SignalR client with TypeScript types
- Automatic reconnection and backpressure handling
- WebSocket with Server-Sent Events and Long Polling fallbacks

## Agent-First Delivery

- Host agent brains and tools within Azure AI Foundry, storing prompt assets, evaluation suites, and safety rules under version control.
- Wrap each agent in the Microsoft Agent Framework lifecycle: planners, skills/tools, memory, and telemetry middleware.
- **AgentHost architecture**: Create a dedicated `{appname}.AgentHost` project for distributed agent execution, orchestration, and background processing separate from the API layer.
- **Agent providers pattern**: Implement agents as provider classes (e.g., `CopywritingAgentProvider`, `AuditAgentProvider`) that encapsulate agent creation, tool registration, and configuration.
- **Orchestration services**: Create orchestrator services that coordinate multiple agents with sequential workflows (dependent execution) and concurrent workflows (parallel execution).
- Define clear SLAs for latency, accuracy, and guardrail enforcement. Build regression suites that run in CI using offline evaluation datasets.
- Provide SDK shims so frontend clients can call agent endpoints asynchronously (REST, streaming, or SignalR) with graceful fallback modes.
- Record conversations, decisions, and tool invocations for review—pipe to data lake storage with retention policies.

## Shared Engineering Systems

### Code Quality

**Pre-commit hooks** running:
- **Backend**: dotnet format or CSharpier, Roslyn analyzers, commit message lint
- **Frontend**: ESLint (Flat config), Prettier, Stylelint, commit message lint
- **Conventional Commits**: Enforce across all commits

### Documentation

**MkDocs Framework (Mandatory):**

All project documentation **MUST** be written in Markdown and organized using **MkDocs** with Material theme.

**Required Folder Structure:**
```
/
├── docs/                          # MkDocs documentation root
│   ├── index.md                   # Landing page (required)
│   ├── getting-started/
│   │   ├── installation.md
│   │   ├── quick-start.md
│   │   └── configuration.md
│   ├── architecture/
│   │   ├── overview.md
│   │   ├── system-design.md
│   │   └── data-flow.md
│   ├── api/
│   │   ├── rest-api.md
│   │   └── websocket-api.md
│   ├── guides/
│   │   ├── development.md
│   │   ├── deployment.md
│   │   └── troubleshooting.md
│   └── reference/
│       ├── configuration.md
│       └── environment-variables.md
├── specs/                         # Product specifications
│   ├── adr/                       # Architecture Decision Records (MADRs)
│   ├── journal/                   # Engineering journal entries
│   └── product-specs/             # Product requirement documents
├── mkdocs.yml                     # MkDocs configuration (required)
└── README.md                      # Project overview (GitHub/repo view)
```

**Required MkDocs Configuration:**
- Create `mkdocs.yml` at repository root
- Use Material theme with navigation, search, and code copy features
- Configure plugins: search, git-revision-date-localized, minify
- Enable markdown extensions: admonition, pymdownx.details, pymdownx.superfences, pymdownx.tabbed, pymdownx.highlight, tables, toc

**Documentation Standards:**
- All documentation MUST be in Markdown (`.md` files)
- Use clear, descriptive filenames in kebab-case
- Use proper heading hierarchy (H1 → H2 → H3, no skipping levels)
- Include frontmatter metadata when needed

**Required Core Documents:**
1. `docs/index.md`: Landing page with project overview
2. `docs/getting-started/installation.md`: Setup instructions
3. `docs/getting-started/quick-start.md`: 5-minute tutorial
4. `docs/architecture/overview.md`: High-level architecture
5. `docs/guides/development.md`: Developer workflow
6. `docs/guides/deployment.md`: Deployment procedures

**Local Development:**
```bash
# Serve documentation locally
mkdocs serve

# Build static site
mkdocs build

# Build with strict mode
mkdocs build --strict
```

**CI/CD Integration:**
- Set up GitHub Actions workflow to deploy to GitHub Pages or Azure Static Web Apps
- Build runs on every push to docs/ or mkdocs.yml
- Enforce `mkdocs build --strict` passes in CI

**Architecture Decision Records (ADRs/MADRs):**
- Store in `/specs/adr/` directory
- Use sequential numbering: `0001-decision-title.md`, `0002-next-decision.md`
- Follow MADR format: Status, Context, Decision Drivers, Considered Options, Decision Outcome, Consequences
- **Create MADRs for task implementations**: When implementing a task, create a MADR documenting architectural decisions, alternatives considered, and rationale

**Documentation Updates:**
- **Update documentation in place**: Do NOT create separate summary documents
- Always update existing documentation files in `/docs` directly with new information, decisions, and learnings
- Documentation changes must be part of the same PR as code changes
- Keep living product specs in `/specs`
- Provide runbooks for on-call, including agent triage and incident response
- Share learning through weekly engineering journal entries in `/specs/journal`

### Secrets Management

- Use secure secret management for local development and cloud environments
- **Backend**: .NET User Secrets (`dotnet user-secrets`) for local development
- **Frontend**: `.env` for local development only
- **Never commit plain-text secrets**: No secrets in configuration files beyond placeholders
- **Cloud**: Azure Key Vault or GitHub OIDC → Cloud secret store for production

### Version Control

- Comprehensive `.gitignore` must cover build artifacts, dependencies, environment files, IDE files, and secrets
- Never commit build outputs, dependencies, or sensitive data
- **Lock files**: Always commit lock files for reproducible builds
  - Backend: NuGet lock files (when used)
  - Frontend: `pnpm-lock.yaml`

### Dependency Governance

- Automated dependency updates with grouped update strategy
- Centralized package management:
  - Backend: `Directory.Packages.props` for NuGet
  - Frontend: pnpm workspace mode
- Weekly security scan using GitHub Dependabot + Snyk

### Release Management

- Semantic versioning and automated changelog generation from Conventional Commits
- Use conventional commit format for all commits
- Automated release notes from commit history

## CI/CD Pipeline Expectations

### 1. Workflow Stages

**Pipeline Flow:**
```
lint → type-check → restore → build → unit-tests → integration-tests → security-scan → publish → deploy
```

**Backend Pipeline:**
- Use `dotnet` CLI for restore, build, test, and publish operations
- Run `dotnet format --verify-no-changes` for linting
- Run `tsc --noEmit` equivalent (C# type checking via build)
- Build the AppHost project which orchestrates all dependencies
- Run unit tests with coverage reports
- Run integration tests with Testcontainers

**Frontend Pipeline:**
- Use pnpm for package management
- Run ESLint, Prettier, Stylelint for linting
- Run `tsc --noEmit` for type checking
- Run Vitest for unit tests
- Run Playwright for E2E tests
- Build Next.js application

**Shared:**
- Reuse workflow templates from `/infra/github`
- Rely on monorepo tooling (Nx/Turborepo) for caching
- Run security scans (CodeQL, dependency review)

### 2. Preview Environments

**Frontend:**
- Preview deployments or staging slots per PR
- Deploy to Vercel, Azure Static Web Apps, or similar

**Backend:**
- Deploy ephemeral backend services to cloud platform using appropriate deployment tools per PR
- Use deployment manifests to generate infrastructure as code
- Deploy to Azure Container Apps with unique URLs per PR

### 3. Deployment

**Recommended:**
- Deploy to serverless/container platforms with automatic manifest generation
- Use `azd up` for Azure Container Apps deployment
- Generate deployment manifests using `aspire publish`

**Alternatives:**
- Deploy to managed app services, serverless functions, or container orchestration platforms

**Deployment Strategy:**
- Blue/green or canary deployments with automated health checks and rollback automation
- Capture SBOM (Software Bill of Materials) for each release

**Production Deployment:**
- Aspire is for local development only—**do not deploy AppHost to production**
- Deploy services independently to Azure Container Apps, Azure App Service, or Kubernetes

## Security & Compliance

- **Zero Trust**: Enforce MFA, least privilege, and just-in-time access for all cloud resources
- **Secure Code Scans**: Perform secure code scans (CodeQL, Trivy, Bandit) each PR
- **Threat Modeling**: Threat models accompany new features with STRIDE checklist
- **Data Classification**: Data classification drives storage, encryption, and retention policies. PII must remain within approved regions
- **WebRTC Security**: Handle WebRTC media via SRTP with DTLS; sanitize TURN credentials and rotate frequently
- **Secrets**: No plain-text secrets in configuration files. Use Azure Key Vault or GitHub OIDC for production
- **Dependency Hygiene**: Weekly security scans, automated dependency updates, vulnerability remediation

## Documentation & Knowledge Sharing

- **MkDocs**: All documentation must use MkDocs with Material theme
- Keep living product specs in `/specs`
- **Update documentation in place**: Do NOT create separate summary documents. Always update existing documentation files in `/docs` directly
- **Create MADRs for task implementations**: When implementing a task, create a Markdown Architectural Decision Record (MADR) in `/specs/adr` documenting the architectural decisions, alternatives considered, and rationale. Use sequential numbering (e.g., `0001-use-cosmos-db.md`)
- Synchronize with design docs (Figma) and API definitions
- Provide runbooks for on-call, including agent triage, incident response, and WebRTC signaling debugging
- Share learning through weekly engineering journal entries collected in `/specs/journal`
- Documentation changes must be part of the same PR as code changes

## Quality Gates Checklist

- ✅ **Linters and formatters** appropriate to the technology stack
  - Backend: dotnet format or CSharpier, Roslyn analyzers
  - Frontend: ESLint (Flat config), Prettier, Stylelint
- ✅ **Type checks** with strict type safety enforcement
  - Backend: Nullable reference types enabled, fail builds on nullable warnings
  - Frontend: TypeScript strict mode, fail builds on type errors
- ✅ **Static analysis** and code quality tools
  - Backend: StyleCop, SonarAnalyzer.CSharp, Microsoft.CodeAnalysis.NetAnalyzers
  - Frontend: ESLint with recommended rules
- ✅ **Tests** (unit, integration, e2e, contract)
  - Backend: xUnit/NUnit/MSTest, FluentAssertions, WebApplicationFactory, Testcontainers
  - Frontend: Vitest, React Testing Library, Playwright
  - Coverage ≥ 85%
- ✅ **Security scans** (CodeQL, dependency review, language-specific security analyzers)
- ✅ **Observability smoke tests** (health endpoints, telemetry assertions)
- ✅ **Documentation updates** (README, changelog, ADRs, code documentation, MkDocs)
- ✅ **MADR created** for significant architectural or implementation decisions
- ✅ **Latest stable package versions used** (unless specific version constraints exist)
  - Backend: Latest NuGet packages
  - Frontend: Latest npm packages
- ✅ **MkDocs build passes**: Run `mkdocs build --strict` successfully

## Change Management

- Work on feature branches. PRs must reference Jira/Azure Boards tickets and include test evidence.
- Require two approvals for code touching security, auth, or agent orchestration logic.
- Keep PRs < 500 LOC; split larger features into incremental, reviewable units.
- Merge only when CI is green and quality gates are satisfied. Use squash merges by default.
- **Conventional Commits**: All commits must follow Conventional Commits format
- **Documentation**: All PRs affecting functionality must include documentation updates
- **Code Review**: Reviewers must verify documentation completeness and accuracy
