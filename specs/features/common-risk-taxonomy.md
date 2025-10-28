# Common Risk Taxonomy

Standardized categories to use across FRDs for consistency in assessment and mitigation planning.

## Categories
| Category | Definition | Typical Indicators | Mitigation Patterns |
|----------|------------|--------------------|---------------------|
| Latency | Delays in generating or auditing artifacts beyond targets | Rising mean/95th percentile time | Adaptive strategies, parallel/sequential fallback, caching context |
| Quality Consistency | Divergence in tone, style, or messaging across artifacts | User dissatisfaction, high revision count | Shared normalized brief, reinforcement prompts, audit calibration |
| Compliance Accuracy | Over/under-flagging of issues | High false positive/negative rate | Threshold tuning, feedback loop, override workflow |
| Scalability | Resource contention with growing campaigns/users | Throughput drops, queue growth | Concurrency controls, batching, autoscaling (future) |
| Usability & Clarity | User confusion about states/actions | High help/support requests | Clear UI status taxonomy, action labeling, tooltips |
| Version Sprawl | Excessive artifact versions causing clutter | Large storage growth, slow retrieval | Retention policies, archiving strategy |
| Data Integrity | Orphaned or mismatched records (audit/version) | Integrity check failures | Transactional writes, referential integrity checks |
| Feature Creep | Scope expanding without validation | Unplanned backlog growth | Strict traceability, ADRs for major changes |
| Security & Privacy | Inadvertent sensitive data exposure or misuse | Logs contain PII, access anomalies | Sanitization, access control reviews, monitoring |
| Accessibility | Generated artifacts fail accessibility needs | Missing alt text, unclear structure | Accessibility checks, audit category emphasis |

## Severity Levels
| Level | Description | Action Guidance |
|-------|-------------|-----------------|
| Low | Minimal impact; monitor only | Track metrics; no immediate change |
| Medium | Noticeable degradation or risk | Add to iteration backlog; plan mitigation |
| High | Material user impact or failure risk | Immediate mitigation task + stakeholder alert |
| Critical | Blocks core workflows or compliance | Escalate, hotfix path, post-mortem |

## Risk Registration Template
Use this template when adding a new risk to a feature FRD:
```
| Risk ID | Category | Description | Likelihood | Impact | Severity | Mitigation | Owner | Review Date |
|---------|----------|-------------|-----------|--------|---------|-----------|-------|------------|
```

## Monitoring Metrics (Cross-Cutting)
| Metric | Purpose | Related Category |
|--------|---------|------------------|
| Mean + P95 orchestration time | Latency tracking | Latency |
| Revision count per campaign | Quality consistency signal | Quality Consistency |
| Audit false positive rate | Compliance tuning | Compliance Accuracy |
| Active versions per campaign | Version growth oversight | Version Sprawl |
| Snapshot retrieval latency | Data integrity/performance | Data Integrity |
| Help/support interactions tagged | Usability tracking | Usability & Clarity |

## Governance Workflow
1. Identify new risk during planning/review.
2. Add risk entry to FRD using template.
3. If High or Critical: create ADR summarizing decision or mitigation path.
4. Review all Medium+ risks weekly; update severity based on metrics.
5. Close risks only after mitigation deployed and metric stabilized.

## Escalation Triggers
| Trigger | Escalation Path |
|---------|-----------------|
| Critical latency breach (3 consecutive runs) | Product + Eng sync; capacity analysis |
| Compliance false negative event (missed major issue) | Immediate audit criteria review |
| Data integrity violation (orphan audit) | Suspend related operations; fix pipeline |

## Integration With PRD Changes
- On PRD expansion (new requirement), re-run risk assessment to add categories impacted.
- Traceability matrix updated to reflect added FRD risk landscape.

## Approval Checklist
- [ ] Taxonomy reviewed by Product & Engineering.
- [ ] Severity definitions accepted.
- [ ] Monitoring metrics wired into backlog or instrumentation plan.
