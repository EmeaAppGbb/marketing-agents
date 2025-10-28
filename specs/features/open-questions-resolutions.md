# Open Questions – Draft Resolutions

Proposed answers to open questions raised across all FRDs. Mark any item needing revision with a follow-up comment; unresolved items will roll into an ADR if architectural.

## Campaign Orchestration
| Question | Proposed Resolution | Rationale |
|----------|--------------------|-----------|
| Sequential mode selectable or adaptive? | MVP: user-selectable toggle; adaptive deferred | Simple to implement; data needed for adaptive decision |
| Automatic retries or manual only? | MVP: manual retries for failed agents | Avoid hidden extra cost; transparency first |
| Priority weighting (copy first)? | Defer; rely on parallel for speed | No strong dependency; avoid complexity |
| Store intermediate states? | Store final bundle + transient in logs | Minimize persistence load early |

## Copywriting Agent
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Multilingual support early? | Phase 2 enhancement (post-MVP) | Core value MVP is workflow speed, not language breadth |
| CTA character limit enforcement? | Add soft warning if > recommended (not hard fail) | Prevent friction while guiding quality |
| Sentiment score per variant? | Defer; gather user need feedback | Avoid premature metrics noise |

## Short Social Copy Agent
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Emoji usage guidelines now? | Provide basic positive/neutral guidance; advanced later | Lightweight user value without complexity |
| Engagement prediction support? | Defer to Phase 3 (requires data) | Needs historical data; outside MVP scope |
| User selects reference long-form variant? | Default: use primary body copy; allow selection later | Simplifies initial UX |

## Visual Poster Concept Agent
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Tag concept with channel? | Add optional channel tag field (future) | Useful later; not essential MVP |
| Accessibility scoring now? | Defer; rely on audit qualitative check | Avoid half-baked scoring metric |
| Imagery style references? | Include simple style descriptors (e.g., "illustrative", "photographic") MVP | Low effort, high clarity |

## Audit & Compliance Agent
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Region-specific compliance early? | Defer; start with generic baseline | Complexity high, ROI unclear early |
| Confidence scores per flag? | Defer; add after calibration cycle | Need empirical tuning first |
| Manual override of fail status? | MVP: allow override with rationale logging | Supports workflow continuity |

## Real-Time Streaming
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Partial textual streaming? | Defer; use coarse artifact readiness events | Avoid UI complexity pre-validation |
| Persist event history or ephemeral? | Ephemeral + summary snapshot only | Minimizes storage; can add history later |
| Progress percentage estimation? | Defer; risk of misleading accuracy | Need reliable stage timing data first |

## Review Dashboard
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Inline editing or only regenerate? | MVP: regenerate only | Keeps agent logic central; reduces UX scope |
| Export (PDF/CSV) in MVP? | Defer; Phase 2 after feedback | Focus on core iteration cycle first |
| Bulk approve all option? | Add if all artifacts pass audit; disabled otherwise | Encourages compliance adherence |

## Iteration & Feedback Loop
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Iteration cap MVP? | Soft warning after 5 iterations; no hard cap | Allows flexibility while nudging closure |
| System-suggested feedback prompts? | Add basic suggestions (tone, length, clarity) MVP | Low-cost UX improvement |
| Improvement trend visualization? | Defer to analytics Phase 2 | Needs metric stability first |

## Persistence Model
| Question | Resolution | Rationale |
|----------|-----------|-----------|
| Export/backup endpoints MVP? | Defer; rely on raw data access | Reduces initial surface area |
| Soft delete retention duration? | 30 days default | Common operational baseline |
| Multi-user collaboration extension soon? | Phase 2; add ownership + role metadata | MVP single-user focus |

## Cross-Cutting Follow-Up Actions
| Item | Action | Target Phase |
|------|--------|--------------|
| Adaptive orchestration | Collect latency metrics | Phase 2 decision |
| Multilingual support | Gather user locale demand | Phase 2 backlog triage |
| Engagement prediction | Instrument performance + gather dataset | Phase 3 |
| Accessibility scoring | Define criteria with audit logs | Phase 2 |
| Trend visualization | Stabilize iteration & audit metrics | Phase 2 |

## Decisions Requiring Future ADRs
| Topic | Trigger Condition |
|-------|------------------|
| Introduce multilingual pipeline | When ≥20% users request additional languages |
| Adaptive orchestration algorithm | After stable latency telemetry (≥2 weeks) |
| Collaboration model (roles) | When multi-user requests appear in feedback |
| Compliance regionalization | Market expansion or regulatory requirement |

## Approval Checklist
- [ ] Product sign-off on deferred items list
- [ ] Engineering acknowledges feasibility of accepted resolutions
- [ ] Risks for deferred items evaluated in risk taxonomy
