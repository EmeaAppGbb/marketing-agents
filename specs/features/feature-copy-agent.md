# Feature: Copywriting Agent Integration

Generates long-form campaign messaging (headlines, body variants, CTAs) aligned with the campaign brief and brand tone.

## PRD Traceability
- Requirements: [REQ-1]
- User Stories: Comprehensive campaign creation; artifact assessment.

## Objectives
- Multi-length narrative content
- Tone alignment & messaging cohesion
- Foundation for short copy alignment

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Campaign brief | Orchestration | Mandatory |
| Tone guidelines | User / brand profile | Optional |
| Length preferences | User selection | Optional (defaults all) |
| Revision feedback | Iteration loop | Optional after v1 |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| Headline options (≥3) | Dashboard | Unranked (MVP) |
| Body copy variants (short/medium/long) | Dashboard | Tagged by length |
| CTA suggestions (≥3) | Dashboard / audit | Action oriented |
| Tone adherence metadata | Audit Agent | Qualitative descriptor |

## Functional Requirements
1. Generate ≥3 distinct headline options.
2. Produce body copy in three length tiers unless constrained.
3. Provide ≥3 CTA suggestions tied to objective.
4. Regeneration preserves original brief + feedback context.
5. Include tone adherence summary per response.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Generation latency | < 20s baseline |
| Tone adherence (qualitative) | ≥ 80% perceived match |
| Regeneration completeness | 100% (no empty sections) |

## Acceptance Criteria
- Returns required sections in structured response.
- Regenerated output reflects prior feedback tokens.
- Headlines semantically diverse (not trivial synonyms).
- CTAs contextually relevant.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| Missing tone guidelines | Neutral professional tone |
| Niche product ambiguity | Avoid hallucinated features; clarify generically |
| Excessively long feedback | Summarize/truncate with notice |

## Metrics
- Avg generation time
- Regeneration count per campaign
- Headline diversity (n-gram overlap heuristic)
- Acceptance rate (approved without further regen)

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Repetitive variants | Diversity-oriented prompt adjustments |
| Tone drift on iterations | Reinject original tone descriptors |
| Ambiguous feedback | Future structured tag UI encouragement |

## Open Questions
1. Multilingual support early?  
2. CTA character limit enforcement needed?  
3. Sentiment score per variant helpful?  

## Dependencies
- Orchestration
- Iteration Feedback Loop
- Audit & Compliance Agent

## Out-of-Scope (MVP)
- Automatic sentiment optimization
- Real-time token streaming

## Approval Checklist
- [ ] Input schema confirmed
- [ ] Output sections defined
- [ ] Regeneration rules accepted
- [ ] Metrics validated
