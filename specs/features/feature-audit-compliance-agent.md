# Feature: Audit & Compliance Agent

Evaluates generated campaign artifacts (copy, social posts, visual concepts) for compliance, tone consistency, accessibility, and risk flags. Produces structured recommendations and scores.

## PRD Traceability
- Requirements: [REQ-2]
- User Stories: Auditor validation story.

## Objectives
- Actionable remediation guidance
- Baseline marketing compliance enforcement
- Increase confidence prior to approval

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Artifact bundle | Orchestration | Mandatory |
| Compliance checklist categories | Config | Initial ≥5 |
| Brand tone descriptors | User / profile | Optional |
| Revision history | Persistence | Optional |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Audit report JSON (scores + flags) | Dashboard | Machine-readable |
| Flagged items list with rationale | Dashboard | Linked per artifact |
| Revision recommendations | Iteration loop | Structured guidance |
| Overall compliance status | Dashboard | Pass / conditional / fail |

## Functional Requirements
1. Per-artifact evaluation across categories (tone, clarity, compliance, accessibility*, risk). *Accessibility covers alt text + basic copy.
2. Overall status derived from thresholds.
3. Recommendation objects tied to flagged items (imperative phrasing).
4. Latency ≤10s per full bundle baseline.
5. Auto re-run post any artifact regeneration.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Full bundle latency | ≤ 10s baseline |
| Recommendation usefulness (qualitative) | Majority rated useful |
| False positive rate | < 15% (tuning) |

## Acceptance Criteria
- JSON report includes category scores.
- Recommendations reference artifact section or ID.
- Re-run attaches to new version IDs.
- Conditional pass state supported.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Missing alt text | Accessibility flag |
| Extremely short variant | Adjust clarity scoring heuristics |
| Missing compliance config | Load default baseline categories |

## Metrics
- Avg audit latency
- Flag count distribution
- Recommendation acceptance rate
- Re-audit frequency

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Over-flagging lowers trust | Calibrate + override option (future) |
| Under-flagging permits low quality | Feedback loop tuning |
| Ambiguous scoring semantics | Provide legend & docs |

## Open Questions
1. Region-specific compliance early?  
2. Confidence scores per flag?  
3. Manual override of fail allowed?  

## Dependencies
- Generation agents outputs
- Persistence (version linkage)
- Review Dashboard

## Out-of-Scope (MVP)
- Industry-specific taxonomies
- External regulatory APIs

## Approval Checklist
- [ ] Category list finalized
- [ ] Scoring logic defined
- [ ] Recommendation format accepted
- [ ] Re-run behavior confirmed
