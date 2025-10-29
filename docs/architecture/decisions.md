# Architecture Decision Records (ADRs)

This page provides an index of all architectural decisions made for the Marketing Agents Platform. Each decision is documented using the **Markdown Architectural Decision Record (MADR)** format.

## What are ADRs?

Architecture Decision Records (ADRs) capture important architectural decisions made during the project lifecycle. They document:

- **Context**: What factors influenced the decision?
- **Decision**: What was decided?
- **Consequences**: What are the positive and negative outcomes?
- **Alternatives**: What other options were considered?

## Active ADRs

### ADR 0001: Use .NET Aspire for Local Development Orchestration

**Status**: ✅ Accepted  
**Date**: 2025-10-29

**Summary**: Chose .NET Aspire for local development orchestration over Docker Compose or manual service startup.

**Full ADR**: See `/specs/adr/0001-use-aspire-for-orchestration.md` in the repository

**Key Decision**: Use .NET Aspire for local development orchestration with separate Api and AgentHost services.

**Rationale**:
- Automatic service discovery eliminates manual configuration
- Built-in OpenTelemetry provides observability from day one
- Separation of API layer from long-running agent workloads
- Local development parity with cloud deployment
- Future-proof for adding additional services

**Trade-offs**:
- ✅ Developer productivity gains with one-command startup
- ✅ Independent scaling of API vs Agent workloads
- ⚠️ Learning curve for developers unfamiliar with Aspire
- ⚠️ Deployment requires `azd` or `aspire publish`

**Alternatives Considered**:
1. **Single Monolithic Project**: Simpler but couples API with agent execution
2. **Multi-Repo Microservices**: Maximum isolation but complex local development
3. **Monorepo with Aspire** ✅: Best balance of productivity and separation

---

### ADR 0002: Use MkDocs with Material Theme for Documentation

**Status**: ✅ Accepted  
**Date**: 2025-10-29

**Summary**: Chose MkDocs with Material theme for project documentation over Docusaurus or GitHub Wiki.

**Full ADR**: See `/specs/adr/0002-use-mkdocs-for-documentation.md` in the repository

**Key Decision**: Use MkDocs with Material theme for all project documentation.

**Rationale**:
- Markdown-based documentation lives alongside code
- Fast build times (< 3 seconds) with `mkdocs build --strict`
- Professional Material theme with built-in search and dark mode
- Simple GitHub Pages deployment via GitHub Actions
- Excellent plugin ecosystem (git-revision-date, minify, Mermaid diagrams)

**Trade-offs**:
- ✅ Simple Markdown workflow familiar to developers
- ✅ Static site with no backend dependencies
- ✅ Client-side search with instant results
- ⚠️ Requires Python for local documentation development
- ⚠️ Less interactive than React-based solutions like Docusaurus

**Alternatives Considered**:
1. **Docusaurus**: React-based with powerful customization but more complex
2. **GitHub Wiki**: Zero setup but limited features and separate repository
3. **MkDocs with Material** ✅: Best balance of simplicity and features

---

## Upcoming ADRs

The following ADRs are planned for future implementation tasks:

### ADR 0003: Database Persistence Strategy (Azure Cosmos DB)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/004-task-campaign-persistence-model.md` in the repository

Decision on database choice, schema design, and data access patterns.

### ADR 0004: Real-Time Communication Architecture (SignalR)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/012-task-realtime-signalr-backend.md` in the repository

Decision on real-time communication protocol (SignalR vs WebSockets vs SSE).

### ADR 0005: Agent Orchestration Patterns (Microsoft Agent Framework)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/010-task-campaign-orchestration.md` in the repository

Decision on agent coordination patterns (sequential vs concurrent vs event-driven).

### ADR 0006: Deployment Strategy (Azure Container Apps vs Kubernetes)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/020-task-cicd-deployment.md` in the repository

Decision on production deployment platform and infrastructure as code approach.

### ADR 0007: Frontend State Management
**Status**: 🚧 Planned  
**Related Task**: Task 002 (Frontend Scaffolding)

Decision on state management approach (TanStack Query + Zustand vs Redux vs Jotai).

### ADR 0008: API Contract Strategy (OpenAPI)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/013-task-campaign-api-endpoints.md` in the repository

Decision on API schema generation, SDK client generation, and contract testing approach.

### ADR 0009: Campaign Iteration Feedback Loop
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/011-task-iteration-feedback-loop.md` in the repository

Decision on retry mechanisms, feedback propagation, and convergence criteria.

---

## How to Create an ADR

When making a significant architectural decision:

1. **Create a new MADR file** in `/specs/adr/`:
   ```bash
   # Use sequential numbering
   touch specs/adr/XXXX-decision-title.md
   ```

2. **Follow the MADR template**:
   ```markdown
   # ADR XXXX: [Title]
   
   ## Status
   [Proposed | Accepted | Deprecated | Superseded]
   
   ## Context
   [What is the issue we're facing? What factors are driving this decision?]
   
   ## Decision Drivers
   [What requirements or constraints influence this decision?]
   
   ## Considered Options
   [What alternatives did we evaluate?]
   
   ## Decision Outcome
   [What did we decide and why?]
   
   ## Consequences
   ### Positive
   [Benefits of this decision]
   
   ### Negative
   [Drawbacks or risks]
   
   ## References
   [Links to related docs, specs, or external resources]
   ```

3. **Link the ADR** to related tasks in `/specs/tasks/`

4. **Update this index page** with a summary of the new ADR

## ADR Lifecycle

- **Proposed**: Decision is under consideration
- **Accepted**: Decision is approved and implemented
- **Deprecated**: Decision is no longer recommended but may still be in use
- **Superseded**: Decision replaced by a newer ADR (link to replacement)

## Resources

- [MADR Template](https://adr.github.io/madr/)
- [ADR GitHub Organization](https://adr.github.io/)
- [Documenting Architecture Decisions (Michael Nygard)](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
