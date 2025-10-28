# Feature: Short Social Copy Agent

Generates platform-specific micro copy aligned with campaign messaging, including character counts and hashtag suggestions.

## PRD Traceability
- Requirements: [REQ-1]
- User Stories: Artifact separation & assessment.

## Objectives
- Multiple variants per platform
- Alignment with long-form copy
- Platform constraint awareness

## Inputs
| Input | Source | Notes |
|-------|--------|-------|
| Campaign brief | Orchestration | Mandatory |
| Long-form copy reference | Copy Agent | Alignment |
| Platform list | User selection | Mandatory |
| Hashtag rules (max/banned) | User / config | Optional |
| Revision feedback | Iteration loop | Optional |

## Outputs
| Output | Consumer | Notes |
|--------|----------|-------|
| ≥3 variants per platform | Dashboard | Structured list |
| Character counts + limit flag | Dashboard / audit | Compliance |
| Hashtag suggestions | Dashboard | Avoid banned list |
| Alignment note | Audit Agent | Consistency check |

## Functional Requirements
1. Generate ≥3 variants per selected platform.
2. Flag over-character-limit posts (do not auto-truncate silently).
3. Suggest hashtags respecting banned/max constraints.
4. Provide alignment metadata referencing main message/headline.
5. Allow selective regeneration per single platform.

## Non-Functional Targets
| Attribute | Target |
|-----------|--------|
| Latency (all platforms) | < 25s baseline |
| Hashtag relevance (qualitative) | ≥ 70% |
| Over-limit posts (initial) | ≤ 10% |

## Acceptance Criteria
- Required variants produced for each platform.
- Over-limit items clearly flagged.
- Alignment note present for each platform bundle.
- Regeneration preserves platform scope, not affecting others.

## Edge Cases
| Scenario | Behavior |
|----------|----------|
| No platforms selected | Validation error pre-run |
| Unsupported platform | Ignore + warning |
| Banned hashtag generated | Flag + propose alternative |

## Metrics
- Variants per platform
- Regeneration frequency
- Hashtag usage distribution
- Alignment quality proxy (future classifier)

## Risks & Mitigation
| Risk | Mitigation |
|------|-----------|
| Hashtag spam | Cap + relevance filtering |
| Rapid platform policy changes | Config-driven constraints |
| Tone inconsistency | Inject shared context reliably |

## Open Questions
1. Emoji usage guidelines now or later?  
2. Engagement prediction support future?  
3. User chooses reference long-form variant explicitly?  

## Dependencies
- Copy Agent output
- Orchestration
- Audit & Compliance Agent

## Out-of-Scope (MVP)
- Scheduling integration
- Engagement prediction modeling

## Approval Checklist
- [ ] Platform schema
- [ ] Variant minimum confirmed
- [ ] Hashtag rules defined
- [ ] Regeneration behavior confirmed
