# Task 009: Audit & Compliance Agent Implementation

## Description
Implement the Audit & Compliance Agent that evaluates all generated campaign artifacts for compliance, tone consistency, accessibility, and risk flags. The agent produces structured recommendations and scores to guide approval decisions.

## Dependencies
- Task 005: Agent Framework Integration & Base Agent Setup
- Task 006: Copywriting Agent Implementation
- Task 007: Short Social Copy Agent Implementation
- Task 008: Visual Poster Concept Agent Implementation

## Technical Requirements

### Agent Provider Implementation
Create `AuditAgentProvider` class extending `BaseAgentProvider`:
- Inherit from base agent provider pattern
- Constructor accepting `IChatClient` dependency
- Override `CreateAgent()` to return configured `ChatClientAgent`
- Agent name: "AuditComplianceAgent"
- Agent description: "Evaluates campaign artifacts for compliance, tone, accessibility, and quality"

### Agent Instructions (System Prompt)
Craft comprehensive instructions covering:
- Role as compliance and quality assurance expert
- Evaluation across multiple categories (tone, clarity, compliance, accessibility, risk)
- Structured scoring methodology (0-100 scale per category)
- Identification of specific issues with rationale
- Actionable remediation recommendations
- Overall compliance status determination logic
- Output structure as JSON audit report

### Compliance Categories Configuration
Define evaluation categories (extensible via configuration):
1. **Tone Consistency**: Alignment with brand tone guidelines
2. **Clarity**: Message clarity and readability
3. **Compliance**: Regulatory and marketing standards
4. **Accessibility**: Alt text quality, readability, inclusive language
5. **Risk**: Prohibited content, controversial topics, false claims

Each category has:
- Scoring criteria (0-100)
- Threshold levels (Pass >80, Conditional 60-80, Fail <60)
- Weighted importance (configurable)

### Agent Tools (using AIFunctionFactory)
Implement minimum 3 tools:

**Tool 1: AnalyzeToneConsistency**
- Compares artifacts against tone descriptors
- Identifies tone drift across artifacts
- Returns consistency score and specific drift examples

**Tool 2: ValidateAccessibility**
- Checks alt text presence and completeness
- Analyzes readability scores
- Validates inclusive language usage
- Returns accessibility score with improvement areas

**Tool 3: IdentifyComplianceRisks**
- Scans for prohibited terms and patterns
- Checks regulatory compliance markers
- Identifies potential false claims
- Returns risk flags with severity levels

Additional tools as needed for:
- Readability scoring (Flesch-Kincaid)
- Keyword density analysis
- Brand guideline validation
- Legal disclaimer requirements

### Request/Response Models
Create strongly-typed models as C# records:

**AuditRequest**
- CampaignBrief (required, for context)
- CopywritingArtifact (CopywritingResponse, required)
- ShortCopyArtifact (ShortCopyResponse, required)
- VisualConceptArtifact (VisualConceptResponse, required)
- BrandToneDescriptors (string[], optional)
- ComplianceChecklist (string[], optional categories)
- RevisionHistory (AuditReport[], optional for improvement tracking)

**CategoryScore**
- Category (string)
- Score (int, 0-100)
- Status (enum: Pass, Conditional, Fail)
- Weight (decimal, 0-1)

**FlaggedItem**
- ArtifactType (enum: Copy, ShortCopy, VisualConcept)
- ArtifactSection (string, specific location)
- IssueDescription (string, clear explanation)
- Severity (enum: Low, Medium, High, Critical)
- Category (string, which evaluation category)

**Recommendation**
- ArtifactType (enum)
- ArtifactSection (string)
- Recommendation (string, imperative phrasing)
- Priority (enum: Low, Medium, High)
- RelatedFlag (string, reference to FlaggedItem ID)

**AuditReport**
- Id (string, globally unique)
- VersionIds (Dictionary<ArtifactType, string>)
- OverallStatus (enum: Pass, Conditional, Fail)
- CategoryScores (CategoryScore[])
- FlaggedItems (FlaggedItem[])
- Recommendations (Recommendation[])
- ComplianceSummary (string)
- CreatedAt (DateTimeOffset)

### Service Integration
Create `IAuditService` interface:
- `Task<AuditReport> AuditCampaignAsync(AuditRequest request, CancellationToken cancellationToken)`
- `Task<AuditReport> ReauditAsync(AuditRequest request, CancellationToken cancellationToken)`

Implement `AuditService`:
- Inject `AuditAgentProvider`
- Build comprehensive audit prompt with all artifacts
- Include evaluation criteria and category definitions
- Call `AIAgent.RunAsync()` with constructed prompt
- Parse agent response into `AuditReport`
- Calculate weighted overall score from category scores
- Determine overall status from thresholds
- Validate all required sections present
- Implement retry logic for incomplete reports (max 3 attempts)

### Prompt Engineering
- Construct artifact bundle summary for evaluation
- Include explicit evaluation criteria for each category
- Reference brand tone guidelines if provided
- Specify scoring methodology clearly
- Request structured recommendations tied to specific issues
- Use examples to demonstrate desired output format

### Scoring and Status Determination
**Category Scoring:**
- Each category scored 0-100
- Apply category-specific criteria from configuration

**Overall Status Logic:**
- Calculate weighted average of category scores
- Pass: Overall score >80 AND no Critical flagged items
- Conditional: Overall score 60-80 OR has High severity items
- Fail: Overall score <60 OR has Critical items

**Flagged Item Severity:**
- Low: Minor style or preference issues
- Medium: Noticeable quality or compliance concerns
- High: Significant compliance or quality issues
- Critical: Blocking issues requiring immediate remediation

### Validation and Processing
- Validate all artifact types evaluated
- Ensure category scores within valid range (0-100)
- Validate recommendations reference valid artifact sections
- Check flagged items have valid severity levels
- Ensure overall status matches calculated scores
- Validate compliance summary is non-empty

### Error Handling
- Handle missing artifacts gracefully
- Retry on incomplete evaluation results
- Log all evaluation errors with context
- Return meaningful errors to orchestrator
- Handle edge cases (empty artifacts, missing tone guidelines)

### Metrics and Telemetry
Instrument the following metrics:
- Audit latency (mean and P95)
- Category score distributions
- Overall status distribution (Pass/Conditional/Fail rates)
- Flag count per severity level
- Recommendation acceptance rate (future tracking)
- Re-audit frequency
- Tool invocation counts
- False positive rate estimates (from user feedback)

## Acceptance Criteria
- [ ] AuditAgentProvider implemented with ChatClientAgent wrapper
- [ ] Audit report includes all category scores (≥5 categories)
- [ ] Overall status correctly derived from scores and thresholds
- [ ] Flagged items reference specific artifact sections
- [ ] Recommendations use imperative phrasing and link to flags
- [ ] Accessibility category validates alt text presence/quality
- [ ] Re-audit attaches to new version IDs correctly
- [ ] Audit latency baseline ≤10s measured
- [ ] All tools implemented using AIFunctionFactory

## Testing Requirements
- [ ] Unit tests for AuditService (≥85% coverage)
- [ ] Unit tests for all agent tools
- [ ] Unit tests for scoring and status determination logic
- [ ] Integration tests with mock IChatClient and sample artifacts
- [ ] Test with various artifact quality levels
- [ ] Test category score threshold logic
- [ ] Test flagged item severity handling
- [ ] Test recommendation generation and linking
- [ ] Test re-audit with revision history
- [ ] Test missing/incomplete artifacts handling
- [ ] Load test to verify 10s latency target

## Non-Functional Requirements
- Full bundle audit latency ≤10s (baseline target)
- Recommendation usefulness >70% qualitative rating (user feedback)
- False positive rate <15% (calibration target)
- Support for 5+ evaluation categories
- Extensible category configuration

## Out of Scope
- Region-specific compliance rules (future enhancement)
- External regulatory API integration (future)
- Automated legal disclaimer generation
- Industry-specific taxonomies (future)
- Confidence scores per flag (future)
- Manual override workflow (future)

## Notes
- Follow AGENTS.md agent implementation patterns
- Use AI Toolkit best practices before implementation
- Query Microsoft Docs MCP for Agent Framework samples
- Never use IChatClient directly - always wrap in ChatClientAgent
- Calibration of thresholds may be needed after initial testing
- Document evaluation criteria in code comments and configuration
- Consider edge cases: missing alt text, extremely short copy, conflicting tone
- Create MADR for compliance category and scoring decisions
- Future: support manual override with rationale logging
- Ensure recommendations are actionable and specific
