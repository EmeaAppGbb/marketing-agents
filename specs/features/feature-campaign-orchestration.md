# Feature: Campaign Orchestration Core

Central coordination layer that accepts a campaign brief and triggers all generation agents (copy, short social copy, visual poster concepts) then hands results to the audit/compliance agent.

## PRD Traceability
- Requirements: [REQ-1], indirectly supports [REQ-2], [REQ-3]
- User Stories: All

## Objectives
- Single entry point for campaign creation
- Parallel or sequential execution
- Consistent contextual inputs
- Foundation for iteration/versioning

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Campaign brief (objective, audience, product, tone) | User form | Mandatory |
| Optional constraints (length, prohibited terms) | User form | Optional |
| Brand guidelines (tone adjectives, palette descriptors) | User / library | Optional |
| Execution mode (parallel vs sequential) | System default / user toggle | Default parallel |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Unified artifact bundle (IDs + metadata) | Dashboard / audit | Generated copy/social/visual refs |
| Lifecycle events (status progression) | Realtime channel | Queued → Generating → Generated → Auditing → Completed |
| Result envelope (success / partial / failure reasons) | UI / logs | For messaging and retry |

## Dependencies
- Copywriting Agent
- Short Social Copy Agent
- Visual Poster Concept Agent
- Audit & Compliance Agent
- Persistence Model
- Realtime Streaming

## Functional Requirements
1. Fan out request to all generation agents.
2. Support parallel then batch audit vs sequential audit.
3. Aggregate outputs with normalized tags.
4. Emit lifecycle events per agent + overall state.
5. Error isolation for partial failures.
6. Support cancellation.

## Non-Functional Targets
| Attribute | Target | Notes |
|-----------|--------|-------|
| Time to first artifact | < 15s | Parallel path |
| Total orchestration time | < 60s | Baseline subject to tuning |
| Reliability | ≥ 95% successful runs | Rolling 30d |
| Observability coverage | 100% transitions logged | Traces + metrics |

## Acceptance Criteria
- Single action yields bundle with stable IDs.
- Partial failures surfaced with retry options.
- Events visible within ≤2s of state change.
- Cancellation halts pending executions and marks run cancelled.
- Audit triggers automatically post generation completion.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| One agent fails | Continue others; mark missing artifact |
| Audit unavailable | Mark audit pending; queue later run |
| Cancellation mid-run | Retain completed artifacts; mark cancelled |
| Duplicate brief submitted | Treat separately (MVP) |
| Oversized brief | Validate, truncate or reject with guidance |

## Metrics
- Mean orchestration duration
- Time to first artifact
- Failure rate per agent
- Cancellation rate
- Audit latency added

## Risks & Mitigations
| Risk | Mitigation |
|------|-----------|
| Latency spikes | Fallback to sequential mode threshold |
| Context inconsistency | Normalized brief schema injection |
| Partial failure confusion | Explicit status taxonomy + UI indications |
| Cost contention | Concurrency limits + rate controls |

## Open Questions
1. Sequential mode user-selectable or adaptive?  
2. Automatic retries vs manual only MVP?  
3. Priority weighting (copy first) needed?  
4. Store intermediate states or only final bundle?  

## Versioning
- Run ID links artifacts; iteration references prior run ID.

## Out-of-Scope (MVP)
- Formal SLA enforcement
- Dynamic agent routing rules

## Approval Checklist
- [ ] Inputs validated
- [ ] Strategies defined
- [ ] Error taxonomy drafted
- [ ] Metrics instrumentation planned
- [ ] Open questions tracked
