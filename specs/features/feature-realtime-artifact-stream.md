# Feature: Real-Time Artifact Streaming & Status Updates

Surfaces progressive status updates and artifact availability to the UI, enabling transparency during campaign generation.

## PRD Traceability
- Requirements: [REQ-3]
- User Stories: Visibility & assessment.

## Objectives
- Timely feedback on agent progress
- Reduced perceived latency
- Enable cancellation capability

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Agent lifecycle events | Orchestration / agents | Mandatory |
| Artifact readiness notifications | Agents | Trigger UI updates |
| Cancellation signal | User action | Optional |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Ordered event feed | Frontend | Sequence IDs for ordering |
| Artifact ready events | Dashboard | Link to artifact ID |
| Cancellation acknowledgement | Frontend | Terminal state |

## Functional Requirements
1. Emit events for transitions: queued, running, completed, audited (+ partial optional future).
2. Provide sequence IDs for ordering / replay.
3. Emit artifact-specific readiness events.
4. Broadcast cancellation state changes.
5. Reconnection delivers snapshot summary (authoritative state).

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Delivery latency | ≤ 2s to render |
| Missed event rate | < 1% |
| Reconnection restoration | < 3s |

## Acceptance Criteria
- User sees progress without manual refresh.
- Cancellation halts future events except final acknowledgement.
- Reconnect yields current snapshot.
- Event schema consistent across agent types.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Out-of-order events | Client reorders via sequence IDs |
| Lost connection mid-run | Snapshot replay on reconnect |
| Rapid transitions | Coalesce non-critical or emit all with order preserved |

## Metrics
- Avg emission→render latency
- Reconnection frequency
- Cancellation usage rate
- Events per campaign

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Event storms overwhelm UI | Throttle/batch low-priority events |
| Ordering confusion | Sequence IDs + client merge logic |
| Dropped events hide progress | Health checks + snapshot replay |

## Open Questions
1. Partial textual streaming now or later?  
2. Persist event history or ephemeral?  
3. Progress percentage estimation needed?  

## Dependencies
- Orchestration
- Generation agents
- Dashboard frontend

## Out-of-Scope (MVP)
- Token-level streaming
- Historical analytics of events

## Approval Checklist
- [ ] Event schema finalized
- [ ] Transition taxonomy approved
- [ ] Reconnection behavior defined
- [ ] Cancellation semantics confirmed
