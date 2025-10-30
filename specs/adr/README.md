# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records documenting the key architectural decisions for the marketing agents application.

## Purpose

ADRs capture essential architectural decisions with just enough context for technical planning and implementation. They serve as:
- **Reference documentation** for developers during task planning
- **Living documents** that can be updated as implementation details emerge
- **Decision history** showing why choices were made

## ADR List

| ID | Title | Status | Date |
|----|-------|--------|------|
| [0001](0001-use-dotnet-aspire-monorepo.md) | Use .NET Aspire with Monorepo Architecture | Accepted | 2025-10-30 |
| [0002](0002-use-microsoft-agent-framework.md) | Use Microsoft Agent Framework for AI Agents | Accepted | 2025-10-30 |
| [0003](0003-use-cosmos-db-without-ef.md) | Use Azure Cosmos DB with Native SDK (No Entity Framework) | Accepted | 2025-10-30 |
| [0004](0004-use-minimal-apis-with-records.md) | Use ASP.NET Core Minimal APIs with Record Types | Accepted | 2025-10-30 |
| [0005](0005-use-nextjs-app-router-with-tanstack-query.md) | Use Next.js App Router with TanStack Query | Accepted | 2025-10-30 |
| [0006](0006-sequential-agent-execution-pattern.md) | Sequential Agent Execution with Retry Pattern | Accepted | 2025-10-30 |
| [0007](0007-openapi-sdk-generation.md) | OpenAPI-Driven SDK Generation for Type Safety | Accepted | 2025-10-30 |

## Key Decisions Summary

### Project Structure (ADR-0001)
- **Monorepo** with .NET Aspire orchestration
- Projects: AppHost, Api, Web, ServiceDefaults, AgentHost
- Aspire for local dev only; Azure Container Apps for production

### AI Agents (ADR-0002)
- **Microsoft Agent Framework** (`Microsoft.Agents.AI`)
- ChatClientAgent pattern for all agents
- AIAgent.RunAsync() execution pattern
- Agent providers pattern with orchestration services

### Data Persistence (ADR-0003)
- **Azure Cosmos DB** with native SDK
- **No Entity Framework** (explicitly forbidden)
- Repository pattern with async methods
- In-memory storage for development, Cosmos DB for production

### Backend API (ADR-0004)
- **ASP.NET Core Minimal APIs**
- **Record types** for DTOs
- Nullable reference types enabled
- OpenAPI/Swagger for documentation

### Frontend (ADR-0005)
- **Next.js 14 App Router**
- Server Components + Client Components
- **TanStack Query** for server state
- TypeScript strict mode

### Agent Orchestration (ADR-0006)
- **Sequential execution**: Copy → ShortCopy → Poster → Audit
- **Retry pattern** with compliance feedback (max 5 attempts)
- Feedback from failed audits incorporated into retry prompts

### Type Safety (ADR-0007)
- **OpenAPI → TypeScript SDK** generation
- End-to-end type safety: Backend → Contract → Frontend
- Fail builds on type regressions

## MADR Format

All ADRs follow the [Markdown Architectural Decision Records (MADR)](https://adr.github.io/madr/) format:

1. **Status**: Accepted, Proposed, Deprecated, Superseded
2. **Context**: Problem description and background
3. **Decision Drivers**: Key factors influencing the decision
4. **Considered Options**: Alternatives evaluated
5. **Decision Outcome**: Chosen option and rationale
6. **Consequences**: Positive, negative, and implementation notes

## Creating New ADRs

When creating a new ADR:
1. Use sequential numbering: `000X-decision-title.md`
2. Follow MADR format
3. Keep it focused on one decision
4. Include just enough context for planning
5. Reference PRD, FRDs, and AGENTS.md as appropriate
6. Update this README with the new ADR in the table above

## Updating ADRs

ADRs are living documents:
- Update during implementation if details emerge
- Add "Updated" section with date and changes
- Never delete content; mark as deprecated if superseded
- Create new ADR if decision changes significantly
