# Traceability Index

Maps PRD requirements and user stories to Feature Requirement Documents (FRDs) for transparency and scope control.

## Source References
- PRD Requirements: [REQ-1], [REQ-2], [REQ-3]
- User Stories:
  1. Generate a complete marketing campaign.
  2. View separate sections for copy, social posts, and poster visuals.
  3. AI auditor validates campaign artifacts.

## Matrix
| PRD Requirement | FRD Coverage | Notes |
|-----------------|-------------|-------|
| REQ-1 (Integrate multiple generation agents) | Orchestration, Copy Agent, Short Social Copy, Visual Poster, Persistence | Orchestration is backbone; persistence enables continuity |
| REQ-2 (Dedicated auditing agent) | Audit & Compliance Agent, Review Dashboard, Iteration Feedback | Dashboard surfaces audit; iteration acts on feedback |
| REQ-3 (Interactive UI with real-time & review) | Real-Time Streaming, Review Dashboard, Orchestration, Persistence | Streaming for progress; dashboard for assessment |

## User Stories Mapping
| User Story | FRDs | Coverage Rationale |
|------------|------|--------------------|
| Campaign generation (Story 1) | Orchestration, Copy, Short Copy, Visual Poster, Audit, Persistence | Full flow from brief to audited artifacts |
| Separate artifact sections (Story 2) | Short Copy, Visual Poster, Review Dashboard, Real-Time Streaming | Distinct grouping + progressive visibility |
| Auditor validation (Story 3) | Audit & Compliance, Review Dashboard, Iteration Feedback | Audit results drive revisions |

## Implicit Supporting Features
| Supporting Concern | FRD | Notes |
|--------------------|-----|------|
| Iteration & quality improvement | Iteration Feedback | Enhances acceptance and compliance |
| Data lineage & versioning | Persistence Model | Trace back any audit or artifact state |
| Status transparency | Real-Time Streaming | Reduces perceived latency |

## Coverage Assessment
- All PRD requirements have ≥2 FRDs contributing (reduces single-point failure risk).
- Each user story ties to at least one governance feature (audit or persistence) ensuring quality.

## Gaps & Watchpoints (None Critical)
| Potential Gap | Observation | Proposed Monitoring |
|---------------|------------|--------------------|
| Multilingual support unspecified | Open question in Copy Agent | Track as enhancement candidate |
| Export / external sharing | Raised in Review Dashboard open questions | Defer until core flows stable |
| Adaptive execution strategy | Open question in Orchestration | Evaluate after latency baselines gathered |

## Change Management
When PRD changes:
1. Update affected FRD(s) sections.
2. Reflect change here in matrix.
3. Log decision in ADR if architectural (e.g., add new agent type).

## Approval Checklist
- [ ] Stakeholders confirm all PRD items mapped.
- [ ] Product owner signs off on gap list.
- [ ] Engineering agrees monitoring items.
