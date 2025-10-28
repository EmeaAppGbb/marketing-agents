# Task Index

This document provides an overview of all implementation tasks for the Marketing Agents application, organized by category and execution order.

## Scaffolding Tasks (Phase 0 - Week 1-2)

These foundational tasks must be completed first to establish the development environment and infrastructure.

### 001: Backend Scaffolding - Project Structure & Aspire Setup
**File:** `001-task-backend-scaffolding.md`
**Dependencies:** None
**Duration:** 3-5 days
**Description:** Create .NET 9 backend infrastructure with Aspire orchestration, configure service discovery, telemetry, and establish base project structure.

### 002: Frontend Scaffolding - Next.js App Structure
**File:** `002-task-frontend-scaffolding.md`
**Dependencies:** Task 001
**Duration:** 2-3 days
**Description:** Set up Next.js 14 frontend with TypeScript, state management, testing infrastructure, and development tooling.

### 003: Documentation Scaffolding - MkDocs Setup
**File:** `003-task-documentation-scaffolding.md`
**Dependencies:** None (parallel with 001, 002)
**Duration:** 1-2 days
**Description:** Configure MkDocs documentation system, create initial documentation structure, and set up CI/CD for documentation deployment.

## Backend Core Tasks (Phase 0-1 - Week 1-4)

Core backend functionality including data persistence, agent framework, and individual agent implementations.

### 004: Campaign Data Model & Cosmos DB Persistence
**File:** `004-task-campaign-persistence-model.md`
**Dependencies:** Task 001
**Duration:** 4-5 days
**Description:** Implement campaign data model using C# records, create Cosmos DB repositories with native SDK, and set up database initialization.

### 005: Agent Framework Integration & Base Agent Setup
**File:** `005-task-agent-framework-setup.md`
**Dependencies:** Task 001, 004
**Duration:** 3-4 days
**Description:** Configure Microsoft Agent Framework, integrate Azure OpenAI via Aspire, create base agent provider pattern, and establish orchestration foundation.

### 006: Copywriting Agent Implementation
**File:** `006-task-copywriting-agent.md`
**Dependencies:** Task 005
**Duration:** 4-5 days
**Description:** Implement copywriting agent for headlines, body copy, and CTAs with tool registration and prompt engineering.

### 007: Short Social Copy Agent Implementation
**File:** `007-task-short-copy-agent.md`
**Dependencies:** Task 005, 006
**Duration:** 4-5 days
**Description:** Implement short social copy agent for platform-specific variants with character counting, hashtag suggestions, and alignment validation.

### 008: Visual Poster Concept Agent Implementation
**File:** `008-task-visual-concept-agent.md`
**Dependencies:** Task 005, 006 (optional)
**Duration:** 3-4 days
**Description:** Implement visual concept agent for poster descriptions with mood, palette, layout, and accessibility-focused alt text.

### 009: Audit & Compliance Agent Implementation
**File:** `009-task-audit-compliance-agent.md`
**Dependencies:** Task 005, 006, 007, 008
**Duration:** 5-6 days
**Description:** Implement audit agent for compliance evaluation across multiple categories with scoring, flagging, and recommendations.

## Backend Orchestration & API (Phase 1-2 - Week 3-6)

Orchestration logic, iteration support, real-time communication, and API endpoints.

### 010: Campaign Orchestration Implementation
**File:** `010-task-campaign-orchestration.md`
**Dependencies:** Task 005, 006, 007, 008, 009, 004
**Duration:** 5-6 days
**Description:** Implement campaign orchestration service supporting parallel and sequential execution with lifecycle events and error handling.

### 011: Campaign Iteration & Feedback Loop Implementation
**File:** `011-task-iteration-feedback-loop.md`
**Dependencies:** Task 004, 006, 007, 008, 009, 010
**Duration:** 3-4 days
**Description:** Implement iteration service for artifact regeneration with feedback context enrichment and automatic audit re-runs.

### 012: Real-Time Streaming & SignalR Integration (Backend)
**File:** `012-task-realtime-signalr-backend.md`
**Dependencies:** Task 001, 010
**Duration:** 3-4 days
**Description:** Implement SignalR hub for real-time event broadcasting, snapshot recovery, and connection management.

### 013: Campaign API Endpoints (Minimal APIs)
**File:** `013-task-campaign-api-endpoints.md`
**Dependencies:** Task 001, 004, 010, 011
**Duration:** 4-5 days
**Description:** Implement RESTful API endpoints using Minimal APIs with OpenAPI generation for all campaign operations.

## Frontend Tasks (Phase 2-3 - Week 5-8)

Frontend components for campaign creation, review, and management.

### 014: Frontend API Client & SDK Generation
**File:** `014-task-frontend-api-client-sdk.md`
**Dependencies:** Task 002, 013
**Duration:** 2-3 days
**Description:** Generate TypeScript SDK from OpenAPI spec, configure TanStack Query, and create React Query hooks for all API operations.

### 015: SignalR Real-Time Integration (Frontend)
**File:** `015-task-signalr-realtime-frontend.md`
**Dependencies:** Task 002, 012, 014
**Duration:** 3-4 days
**Description:** Integrate SignalR client for real-time updates with automatic reconnection, event ordering, and snapshot recovery.

### 016: Campaign Creation UI & Form
**File:** `016-task-campaign-creation-ui.md`
**Dependencies:** Task 002, 014
**Duration:** 4-5 days
**Description:** Build campaign creation form with validation, draft saving, and submission workflow.

### 017: Review Dashboard UI - Artifact Display & Audit Results
**File:** `017-task-review-dashboard-ui.md`
**Dependencies:** Task 002, 014, 015
**Duration:** 6-7 days
**Description:** Build comprehensive review dashboard displaying artifacts, audit results, version history, and approval workflow.

### 018: Campaign List & Navigation UI
**File:** `018-task-campaign-list-navigation.md`
**Dependencies:** Task 002, 014
**Duration:** 3-4 days
**Description:** Build campaign list page with filtering, sorting, pagination, and navigation.

## Quality Assurance & Deployment (Phase 3-4 - Week 7-10)

Testing and deployment infrastructure.

### 019: E2E Testing Suite Setup
**File:** `019-task-e2e-testing-suite.md`
**Dependencies:** Task 002, 016, 017, 018
**Duration:** 4-5 days
**Description:** Set up Playwright E2E testing with page object models, critical user flow tests, and CI integration.

### 020: CI/CD Pipeline & Deployment Configuration
**File:** `020-task-cicd-deployment.md`
**Dependencies:** All previous tasks
**Duration:** 5-6 days
**Description:** Configure GitHub Actions CI/CD with testing, security scanning, Aspire publish, and Azure Container Apps deployment.

## Task Dependencies Visualization

```
Scaffolding (Parallel):
001 (Backend) ─┐
002 (Frontend) ┼─→ Core Development
003 (Docs)     ┘

Backend Core (Sequential + Some Parallel):
001 → 004 → 005 ─┬→ 006 ─┐
                 ├→ 007 ─┤
                 └→ 008 ─┴→ 009

Backend Orchestration:
005, 006, 007, 008, 009, 004 → 010 → 011
                                010 → 012
                       004, 010, 011 → 013

Frontend (Some Parallel):
002, 013 → 014 ─┬→ 016
002, 012, 014 → 015 → 017
002, 014 → 018

Testing & Deployment:
002, 016, 017, 018 → 019
All tasks → 020
```

## Execution Strategy

### Phase 0: Foundations (Week 1-2)
- **Parallel:** Tasks 001, 002, 003
- **Then:** Task 004

### Phase 1: Core Generation (Week 3-4)
- Task 005 (foundation)
- **Parallel:** Tasks 006, 007, 008 (agents can be developed concurrently)
- Task 009 (depends on all agents)

### Phase 2: Orchestration & Backend API (Week 5-6)
- Task 010 (orchestration)
- **Parallel:** Tasks 011, 012
- Task 013 (API endpoints)

### Phase 3: Frontend Development (Week 7-8)
- Task 014 (SDK client)
- Task 015 (SignalR client)
- **Parallel:** Tasks 016, 018 (creation and list can be parallel)
- Task 017 (review dashboard, needs 015)

### Phase 4: Testing & Deployment (Week 9-10)
- Task 019 (E2E tests)
- Task 020 (CI/CD)

## Success Metrics by Phase

### Phase 0 (Foundations)
- Run ID created & stored
- Entity model drafted + reviewed
- Health checks passing
- Documentation site live

### Phase 1 (Core Generation)
- All generation agents return structured artifacts
- Latency baseline collected (mean + P95)
- ≥3 headlines & body variants usable per campaign
- ≥3 variants/platform for social copy
- ≥2 distinct visual concepts with alt text

### Phase 2 (Orchestration & API)
- Single action yields complete artifact bundle
- Audit JSON outputs consumed successfully
- Real-time events render <2s
- API endpoints documented via OpenAPI

### Phase 3 (Frontend)
- Campaign creation flow complete
- Artifacts + audit visible within dashboard
- Approve/regenerate actions persist
- Real-time status updates working

### Phase 4 (Testing & Deployment)
- E2E tests covering critical paths
- CI/CD pipeline deploying successfully
- Test coverage ≥85%
- Deployment to staging automated

## Notes

- Each task file contains detailed technical requirements, acceptance criteria, testing requirements, and notes
- Tasks are designed to be independently implementable with minimal cross-dependencies where possible
- Refer to individual task files for complete specifications
- All tasks follow AGENTS.md guidelines and standards
- Create MADRs (Markdown Architecture Decision Records) for significant architectural decisions during implementation
- Update documentation in MkDocs as part of each task completion
