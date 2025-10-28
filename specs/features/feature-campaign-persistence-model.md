# Feature: Campaign Data Model & Persistence

Defines logical storage representation for campaigns, artifacts, versions, audit reports, and iteration metadata ensuring traceability and efficient retrieval.

## PRD Traceability
- Supports [REQ-1], [REQ-2], [REQ-3]
- User Stories: All (persistent access & review)

## Objectives
- Consistent IDs and linkage across artifacts/versions
- Fast retrieval for dashboard & streaming reconciliation
- Support audit lineage & iteration tracking

## Conceptual Entities
| Entity | Purpose |
|--------|---------|
| Campaign | Root container linking artifacts & runs |
| Artifact | Generated item (type-tagged) |
| Artifact Version | Specific iteration of an artifact |
| Audit Report | Evaluation tied to version |
| Iteration Log | Feedback + rationale metadata |
| Orchestration Run | Execution context spanning multiple artifacts |

## Relationships
- Campaign 1..* Artifacts
- Artifact 1..* Versions
- Version 0..1 Audit Report
- Campaign 1..* Orchestration Runs
- Orchestration Run 1..* Artifact Versions (initial set)
- Version 0..* Iteration Logs

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Generated artifacts | Agents | Stored as versions |
| Audit results | Audit Agent | Linked to version |
| Feedback events | Iteration loop | Logged |
| Lifecycle state | Orchestration | Optional snapshot |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Campaign snapshot | Dashboard | Active versions + audit statuses |
| Version history listing | Dashboard | Metadata only (MVP) |
| Audit lineage data | Audit UI | Trace evaluation evolution |
| Iteration metrics summary | Analytics | Future trend |

## Functional Requirements
1. Assign globally unique IDs (campaign, artifact, version).
2. Maintain audit report → version linkage integrity.
3. Retrieve full campaign state within performance target.
4. Append-only semantics for versions & logs (no mutation).
5. Support soft archive state for old versions.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Snapshot retrieval | ≤ 500ms baseline |
| Write reliability | ≥ 99% |
| Orphan prevention | 0 orphan audit reports |

## Acceptance Criteria
- Retrieval returns active versions + audit statuses.
- Version list shows timestamp, iteration number, audit state.
- No audit report without valid version linkage.
- Archiving preserves audit + iteration history.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Frequent regeneration | Version list grows; all accessible |
| Audit fails | Version marked unaudited; retry possible |
| Campaign deletion | Soft delete (recoverable window) |

## Metrics
- Snapshot latency avg
- Versions per campaign distribution
- Audit reports per campaign
- Orphan prevention incidents (should be zero)

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Unbounded version growth | Retention + archival policy future |
| Complex lineage queries | Pre-computed snapshot / denormalized views |
| Orphaned audit reports | Transactional write sequencing |

## Open Questions
1. Export/backup endpoints needed in MVP?  
2. Soft delete retention duration?  
3. Multi-user collaboration extension soon?  

## Dependencies
- Generation agents
- Audit & Compliance Agent
- Iteration Feedback Loop
- Review Dashboard

## Out-of-Scope (MVP)
- Full-text search across versions
- Automatic archival scheduling

## Approval Checklist
- [ ] Entity list agreed
- [ ] Relationship model validated
- [ ] Performance target approved
- [ ] Versioning & audit linkage rules accepted
