# Architecture Decision Records (ADRs)

This page provides an index of all architectural decisions made for the Marketing Agents Platform. Each decision is documented using the **Markdown Architectural Decision Record (MADR)** format.

## What are ADRs?

Architecture Decision Records (ADRs) capture important architectural decisions made during the project lifecycle. They document:

- **Context**: What factors influenced the decision?
- **Decision**: What was decided?
- **Consequences**: What are the positive and negative outcomes?
- **Alternatives**: What other options were considered?

## Active ADRs

### [ADR 0001: Backend Scaffolding Architecture - .NET 9 with Aspire Orchestration](#adr-0001)

**Status**: ✅ Accepted  
**Date**: 2025-01-XX

**Summary**: Chose a monorepo architecture with .NET Aspire orchestration over single monolithic project or multi-repo microservices.

**Full ADR**: See `/specs/adr/0001-backend-scaffolding-architecture.md` in the repository

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

## Upcoming ADRs

The following ADRs are planned for future implementation tasks:

### ADR 0002: Database Persistence Strategy (Azure Cosmos DB)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/004-task-campaign-persistence-model.md` in the repository

Decision on database choice, schema design, and data access patterns.

### ADR 0003: Real-Time Communication Architecture (SignalR)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/012-task-realtime-signalr-backend.md` in the repository

Decision on real-time communication protocol (SignalR vs WebSockets vs SSE).

### ADR 0004: Agent Orchestration Patterns (Microsoft Agent Framework)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/010-task-campaign-orchestration.md` in the repository

Decision on agent coordination patterns (sequential vs concurrent vs event-driven).

### ADR 0005: Deployment Strategy (Azure Container Apps vs Kubernetes)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/020-task-cicd-deployment.md` in the repository

Decision on production deployment platform and infrastructure as code approach.

### ADR 0006: Frontend State Management
**Status**: 🚧 Planned  
**Related Task**: Task 002 (Frontend Scaffolding)

Decision on state management approach (TanStack Query + Zustand vs Redux vs Jotai).

### ADR 0007: API Contract Strategy (OpenAPI)
**Status**: 🚧 Planned  
**Related Task**: See `/specs/tasks/013-task-campaign-api-endpoints.md` in the repository

Decision on API schema generation, SDK client generation, and contract testing approach.

### ADR 0008: Campaign Iteration Feedback Loop
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
