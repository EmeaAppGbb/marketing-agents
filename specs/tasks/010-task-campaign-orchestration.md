# Task 010: Campaign Orchestration Implementation

## Description
Implement the Campaign Orchestration service that coordinates all generation agents (Copy, Short Copy, Visual Concept) and the Audit agent. Support both parallel and sequential execution modes, emit lifecycle events, and handle partial failures gracefully.

## Dependencies
- Task 005: Agent Framework Integration & Base Agent Setup
- Task 006: Copywriting Agent Implementation
- Task 007: Short Social Copy Agent Implementation
- Task 008: Visual Poster Concept Agent Implementation
- Task 009: Audit & Compliance Agent Implementation
- Task 004: Campaign Data Model & Cosmos DB Persistence

## Technical Requirements

### Orchestration Models
Create strongly-typed models as C# records:

**OrchestrationRequest**
- CampaignBrief (required)
- ExecutionMode (enum: Parallel, Sequential, default Parallel)
- SelectedPlatforms (Platform[], for short copy)
- CancellationToken (for cancellation support)

**OrchestrationResult**
- RunId (string, globally unique)
- CampaignId (string)
- Status (enum: Success, PartialSuccess, Failed, Cancelled)
- ArtifactBundle (GeneratedArtifactBundle)
- AuditReport (AuditReport, if completed)
- Errors (OrchestrationError[], if any)
- ExecutionMetrics (OrchestrationMetrics)

**GeneratedArtifactBundle**
- Copy (CopywritingResponse)
- ShortCopy (ShortCopyResponse)
- VisualConcept (VisualConceptResponse)
- GeneratedAt (DateTimeOffset)

**OrchestrationError**
- AgentType (enum: Copy, ShortCopy, VisualConcept, Audit)
- ErrorMessage (string)
- ErrorCode (string)
- Timestamp (DateTimeOffset)

**OrchestrationMetrics**
- TotalDuration (TimeSpan)
- TimeToFirstArtifact (TimeSpan)
- GenerationPhaseTime (TimeSpan)
- AuditPhaseTime (TimeSpan)
- AgentDurations (Dictionary<string, TimeSpan>)

### Lifecycle Event System
Define lifecycle states and events:

**OrchestrationState** (enum)
- Queued, GeneratingCopy, GeneratingShortCopy, GeneratingVisual, 
- GenerationComplete, Auditing, Completed, PartiallyCompleted, Failed, Cancelled

**OrchestrationEvent**
- EventId (string, unique)
- RunId (string)
- State (OrchestrationState)
- AgentType (string, optional)
- Message (string)
- Timestamp (DateTimeOffset)
- Metadata (Dictionary<string, object>, optional)

Create `IOrchestrationEventPublisher` interface:
- `Task PublishAsync(OrchestrationEvent evt)`
- Implementation can use in-memory queue, SignalR hub, or message queue

### Service Implementation
Enhance `IAgentOrchestrator` interface:
- `Task<OrchestrationResult> OrchestrateCampaignAsync(OrchestrationRequest request)`
- `Task CancelOrchestrationAsync(string runId)`

Implement `CampaignOrchestrator` service:

**Constructor Dependencies:**
- ICopywritingService
- IShortCopyService
- IVisualConceptService
- IAuditService
- ICampaignRepository
- IOrchestrationRunRepository
- IOrchestrationEventPublisher
- ILogger<CampaignOrchestrator>

**Parallel Execution Mode:**
1. Create OrchestrationRun record and save to database
2. Emit Queued event
3. Fan out to all three generation agents using Task.WhenAll
4. Emit state change events as each agent completes
5. Collect all results (handle partial failures)
6. Emit GenerationComplete event
7. Trigger audit agent with all artifacts
8. Emit Auditing event
9. Persist artifacts and audit report to database
10. Emit Completed event
11. Return OrchestrationResult

**Sequential Execution Mode:**
1. Create OrchestrationRun record
2. Emit Queued event
3. Execute Copy agent → emit event → audit
4. Execute Short Copy agent → emit event → audit
5. Execute Visual Concept agent → emit event → audit
6. Aggregate results
7. Emit Completed event
8. Return OrchestrationResult

**Error Handling:**
- Isolate errors per agent (don't fail entire orchestration)
- Continue with successful agents in parallel mode
- Mark partial success if ≥1 agent succeeded
- Store error details in OrchestrationError records
- Log all errors with full context

**Cancellation Support:**
1. Accept CancellationToken in request
2. Propagate token to all agent calls
3. Check token between agent executions
4. On cancellation: halt pending agents, emit Cancelled event
5. Return partial results collected so far
6. Update OrchestrationRun status to Cancelled

### Artifact Persistence
After generation and audit:
1. Create Campaign record in database
2. Create Artifact records for each artifact type
3. Create ArtifactVersion records with generated content
4. Create AuditReport record linked to version IDs
5. Update Campaign with active version IDs
6. All writes in transactional sequence

### Metrics Collection
Collect and emit metrics:
- Total orchestration duration (start to finish)
- Time to first artifact (parallel mode)
- Individual agent execution durations
- Audit execution duration
- Database write durations
- Event emission counts
- Success/failure rates

### Validation
- Validate campaign brief completeness before starting
- Validate selected platforms for short copy
- Validate execution mode is supported
- Pre-flight checks for service availability

## Acceptance Criteria
- [ ] CampaignOrchestrator service implemented with both execution modes
- [ ] Parallel mode executes all agents concurrently using Task.WhenAll
- [ ] Sequential mode executes agents one-by-one with per-agent audit
- [ ] Lifecycle events emitted for all state transitions (≤2s latency)
- [ ] Partial failures handled gracefully (mark partial success)
- [ ] Cancellation halts pending executions and returns partial results
- [ ] Audit triggers automatically after generation completion
- [ ] All artifacts and audit reports persisted to database
- [ ] OrchestrationRun record tracks execution history
- [ ] Total orchestration time <60s (baseline, parallel mode)
- [ ] Time to first artifact <15s (parallel mode)

## Testing Requirements
- [ ] Unit tests for CampaignOrchestrator (≥85% coverage)
- [ ] Test parallel execution mode with all agents
- [ ] Test sequential execution mode
- [ ] Test partial failure scenarios (1 agent fails)
- [ ] Test complete failure scenarios (all agents fail)
- [ ] Test cancellation at various stages
- [ ] Test event emission for all state transitions
- [ ] Test artifact persistence after successful generation
- [ ] Test database transaction integrity
- [ ] Integration tests with real agent services
- [ ] Load test to verify performance targets

## Non-Functional Requirements
- Total orchestration time (parallel) <60s baseline
- Time to first artifact (parallel) <15s
- Event delivery latency ≤2s
- Reliability ≥95% successful runs (30-day rolling)
- Support for concurrent campaign orchestrations (5+)
- Observability coverage: 100% state transitions logged with traces

## Out of Scope
- Automatic retry of failed agents (manual regeneration only in MVP)
- Dynamic mode switching based on performance
- Priority weighting (execute copy first)
- Formal SLA enforcement with penalties
- Rate limiting and quota management
- Batch campaign orchestration

## Notes
- Follow AGENTS.md orchestration patterns
- Use structured logging with correlation IDs for full traceability
- Emit OpenTelemetry spans for each agent execution
- Consider fallback to sequential on parallel mode timeout (future)
- Document execution mode trade-offs in code comments
- Create MADR for orchestration strategy decisions
- Test thoroughly with various failure scenarios
- Ensure cancellation is prompt and clean
- Keep event schema consistent and documented
