# Feature: Campaign Review & Audit Dashboard

Unified interface feature spec for viewing all generated artifacts, their audit results, and performing approvals or targeted regenerations.

## PRD Traceability
- Requirements: [REQ-3]
- User Stories: Visibility, validation, approval.

## Objectives
- Centralize artifact visibility + quality signals
- Streamline approve / regenerate decisions
- Maintain version history context

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Artifact bundle | Persistence / orchestration | Mandatory |
| Audit report | Audit Agent | Mandatory |
| Version history | Persistence | Optional |
| User actions (approve/regenerate) | UI | Mandatory |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Approval states per artifact | Persistence | Drives campaign status |
| Regeneration requests | Orchestration / agent | Scoped by type |
| Feedback submissions | Iteration loop | Free text + tags (future parsing) |
| Version comparison metadata | UI | Timestamps + status (MVP) |

## Functional Requirements
1. Display artifacts grouped by type with audit annotations.
2. Selective regeneration of individual artifact types.
3. Approval state transitions (pending → approved → revised).
4. Show ≥ last 3 versions metadata per artifact.
5. Capture structured feedback tags + free-text rationale.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Initial load (cached) | < 2s |
| Action latency (approve/regenerate) | < 1s ack |
| Version history fetch | ≤ 500ms |

## Acceptance Criteria
- All artifacts visible post orchestration completion.
- Audit flags show severity tiers visually.
- Regeneration creates new version ID; history updates.
- Approval persists across sessions.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Regeneration while audit pending | Queue audit after new artifact ready |
| Missing audit report | Placeholder; allow manual trigger |
| Rapid multiple regenerations | Queue sequential; surface latest status |

## Metrics
- Time from generation complete to approval
- Regeneration frequency per artifact type
- Audit flag resolution rate (flags cleared after iteration)

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| UI overload (too many variants) | Collapse secondary variants |
| Confusion revise vs regenerate | Clear labels + tooltips |
| Version clutter | Limit display; expandable history |

## Open Questions
1. Inline editing or only regenerate?  
2. Export (PDF/CSV) in MVP?  
3. Bulk approve all artifacts option?  

## Dependencies
- Orchestration
- Audit & Compliance Agent
- Persistence Model
- Iteration Feedback Loop

## Out-of-Scope (MVP)
- Textual diff visualization
- Image previews (future generation)

## Approval Checklist
- [ ] Grouping strategy approved
- [ ] Regeneration flow confirmed
- [ ] History depth finalized
- [ ] Feedback capture fields validated
