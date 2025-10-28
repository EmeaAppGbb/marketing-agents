# Feature: Campaign Iteration & Feedback Loop

Defines how user feedback guides regeneration of specific artifacts and how context is preserved across iterations.

## PRD Traceability
- Success Criteria: Quality improvement & speed.
- User Stories: Refinement (implicit in campaign creation).

## Objectives
- Structured + free-form feedback capture
- Preserve rationale across versions
- Automatic audit re-run after regeneration

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Feedback (free text) | User | Mandatory |
| Structured tags (tone, length, urgency) | User selection | Optional |
| Previous artifact versions | Persistence | Context |
| Latest audit findings | Audit Agent | Improvement guidance |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Regeneration request w/ enriched context | Orchestration | Triggers agent run |
| Diff rationale log (metadata) | Persistence | Traceability |
| Updated artifact version + audit report | Dashboard | Replaces prior active version |

## Functional Requirements
1. Capture free text + optional structured tags per artifact.
2. Attach feedback to regeneration context payload.
3. Log iteration event (timestamp, artifact ID, old/new version IDs, feedback summary).
4. Auto-trigger audit re-run after new artifact completion.
5. Maintain iteration count surfaced to UI.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Submission→regen start latency | < 2s |
| Audit re-run trigger reliability | 100% |
| Feedback data loss | 0% |

## Acceptance Criteria
- Regeneration retains original brief while applying feedback.
- Iteration history lists rationale per version.
- Latest audit always tied to current active version.
- No orphaned versions lacking audit path (unless audit intentionally skipped).

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Empty feedback submitted | Validation error |
| Excessively long feedback | Truncate + user notice |
| Conflicting tags ("shorter" + "add detail") | Warning; proceed including both |

## Metrics
- Iterations per campaign
- Avg time between iterations
- Audit improvement score delta
- Feedback tag usage frequency

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Ambiguous feedback reduces impact | Encourage structured tags (UI) |
| Infinite iteration loop | Optional soft cap or guidance |
| Version sprawl | Archive/collapse older versions future |

## Open Questions
1. Enforce iteration cap MVP?  
2. Provide system-suggested feedback prompts?  
3. Improvement trend visualization needed early?  

## Dependencies
- Orchestration
- Audit & Compliance Agent
- Persistence Model
- Review Dashboard

## Out-of-Scope (MVP)
- Automated suggestion generation from audit results
- Semantic diff visualization

## Approval Checklist
- [ ] Feedback schema accepted
- [ ] Iteration logging defined
- [ ] Audit trigger chain confirmed
- [ ] Metrics list validated
