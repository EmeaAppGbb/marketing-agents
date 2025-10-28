# Delivery Phases & Milestones

Phased plan aligning FRDs to iterative, high-impact delivery with clear success metrics and exit criteria.

## Phase 0: Foundations (Week 1–2)
| Scope | FRDs | Objectives | Success Metrics |
|-------|------|-----------|-----------------|
| Persistence scaffolding (data entities) | Persistence Model | Enable artifact/version storage | CRUD prototype operational; snapshot <750ms |
| Orchestration skeleton | Campaign Orchestration | Basic fan-out stubs & run ID | Single run returns placeholder outputs |
| Risk taxonomy adoption | Common Risk Taxonomy | Standardize risk language | Risks logged for orchestration & persistence |

Exit Criteria:
- Run ID created & stored.
- Entity model drafted + reviewed.
- Initial risk log entries documented.

## Phase 1: Core Generation (Week 3–4)
| Scope | FRDs | Objectives | Success Metrics |
|-------|------|-----------|-----------------|
| Copywriting implementation | Copy Agent | Produce headline/body/CTAs | ≥3 headlines & body variants usable |
| Short social copy integration | Short Social Copy | Platform variants generated | ≥3 variants/platform; char count flags working |
| Visual concept generation | Visual Poster | Provide ≥2 distinct concepts | Alt text present & palette respected |

Exit Criteria:
- All generation agents return structured artifacts.
- Latency baseline collected (mean + P95).
- Diversity heuristic logged for copy.

## Phase 2: Governance & Visibility (Week 5–6)
| Scope | FRDs | Objectives | Success Metrics |
|-------|------|-----------|-----------------|
| Audit agent MVP | Audit & Compliance | Flag & recommend improvements | Report structure stable; ≤12s latency |
| Real-time streaming channel | Real-Time Streaming | Show status progression | Events render <2s; reconnect snapshot works |
| Review dashboard basic UI | Review Dashboard | View artifacts + audit | Approve/regenerate actions persist |

Exit Criteria:
- Audit JSON outputs consumed by dashboard.
- Regeneration triggers new version with audit re-run.
- Streaming reliability >95% event delivery.

## Phase 3: Iteration & Quality Improvement (Week 7–8)
| Scope | FRDs | Objectives | Success Metrics |
|-------|------|-----------|-----------------|
| Feedback loop mechanics | Iteration Feedback | Capture + apply feedback | Iteration logs stored; auto audit re-run |
| Enhanced orchestration modes | Orchestration | Manual parallel/sequential toggle | Toggle functional; sequential latency tracked |
| Stabilization & tuning | All | Reduce friction & noise | ≥80% artifacts approved without 2nd regen |

Exit Criteria:
- Feedback + regeneration cycle stable.
- Audit improvement delta tracked.
- Sequential mode latency comparison recorded.

## Phase 4: Optimization & Deferred Enhancements (Week 9–10)
| Scope | Deferred Items | Objectives | Success Metrics |
|-------|----------------|-----------|-----------------|
| Bulk approval | Review Dashboard | Accelerate finalization | Bulk approve path available post clean audit |
| Emoji guidance & prompts | Short Copy / Iteration | Improve engagement clarity | Emoji guideline surfaced; structured prompts added |
| Override rationale logging | Audit & Compliance | Governance transparency | Overrides create rationale entries |

Exit Criteria:
- Bulk approval feature adoption measured.
- Override rationales stored & retrievable.
- Feedback suggestions reduce ambiguous free-text (>20% shift).

## Phase 5: Strategic Expansion (Post-MVP)
| Candidate | Trigger | Notes |
|-----------|--------|-------|
| Multilingual support | User locale demand >20% | Requires expanded model eval |
| Engagement prediction | Historical performance data available | Data collection pipeline first |
| Adaptive orchestration | Latency telemetry stable | Needs heuristic/ML routing design |
| Accessibility scoring | Qualitative audit stable | Criteria formalization + scoring model |
| Collaboration roles | Multi-user requests | Adds role & ownership metadata |

## Cross-Phase Governance
| Governance Item | Frequency | Owner |
|-----------------|----------|-------|
| Risk review (Medium+) | Weekly | Product + Eng |
| Metrics dashboard review | Weekly | Engineering |
| Open question backlog refinement | Bi-weekly | Product |
| ADR need assessment | Sprint planning | Tech Lead |

## Success KPIs (MVP)
| KPI | Target |
|-----|--------|
| Campaign end-to-end time (mean) | < 60s |
| Audit latency (mean) | < 10s |
| Approval w/o regen rate | ≥ 60% initial, aiming 80% |
| Iteration count median | ≤ 3 per campaign |
| Event delivery reliability | ≥ 95% |
| Data snapshot latency | ≤ 500ms |

## Rollback Criteria
| Condition | Action |
|-----------|--------|
| Audit latency >2x target for 3 days | Profile & reduce scope (disable non-core checks) |
| Streaming reliability <90% | Fallback to periodic polling |
| Revision loops >8 median | Introduce iteration guidance modal |

## Reporting Cadence
- Sprint Review: KPI snapshot + risk changes.
- Monthly: Roadmap adjustments (Phase shifts).
- Post-MVP: Evaluate Phase 5 triggers based on adoption & feedback.

## Approval Checklist
- [ ] Phases validated by stakeholders
- [ ] KPIs agreed
- [ ] Exit criteria accepted
- [ ] Rollback plan acknowledged
