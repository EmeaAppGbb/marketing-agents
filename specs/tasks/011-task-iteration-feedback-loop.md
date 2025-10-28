# Task 011: Campaign Iteration & Feedback Loop Implementation

## Description
Implement the iteration and feedback loop mechanism that captures user feedback, triggers selective artifact regeneration, and maintains version history with audit re-runs. Support both free-text feedback and structured tags.

## Dependencies
- Task 004: Campaign Data Model & Cosmos DB Persistence
- Task 006: Copywriting Agent Implementation
- Task 007: Short Social Copy Agent Implementation
- Task 008: Visual Poster Concept Agent Implementation
- Task 009: Audit & Compliance Agent Implementation
- Task 010: Campaign Orchestration Implementation

## Technical Requirements

### Feedback Models
Create strongly-typed models as C# records:

**FeedbackRequest**
- CampaignId (string, required)
- ArtifactType (enum: Copy, ShortCopy, VisualConcept, required)
- FeedbackText (string, required, minimum length validation)
- FeedbackTags (FeedbackTag[], optional)
- CurrentVersionId (string, required)

**FeedbackTag** (enum, extensible)
- TooLong, TooShort, ChangeTone, AddDetail, Simplify, 
- MoreCreative, MoreConservative, ChangeWording, Other

**IterationRequest**
- CampaignId (string, required)
- ArtifactType (enum, required)
- Feedback (FeedbackRequest, required)
- TargetPlatform (Platform?, optional for ShortCopy selective regen)

**IterationResult**
- CampaignId (string)
- NewVersionId (string)
- IterationNumber (int)
- NewAuditReport (AuditReport)
- PreviousVersionId (string)
- IterationLogId (string)
- Timestamp (DateTimeOffset)

### Service Implementation
Create `IIterationService` interface:
- `Task<IterationResult> RegenerateArtifactAsync(IterationRequest request, CancellationToken cancellationToken)`
- `Task<IterationHistory> GetIterationHistoryAsync(string campaignId, ArtifactType artifactType)`

Implement `IterationService`:

**Constructor Dependencies:**
- ICopywritingService
- IShortCopyService
- IVisualConceptService
- IAuditService
- ICampaignRepository
- IVersionRepository
- IIterationLogRepository
- IAuditReportRepository
- ILogger<IterationService>

**Regeneration Flow:**
1. Validate feedback request (non-empty text, valid artifact type)
2. Retrieve campaign and current artifact version from database
3. Build enriched regeneration context:
   - Original campaign brief
   - Previous artifact content
   - User feedback text
   - Feedback tags (converted to natural language)
   - Latest audit findings (for improvement guidance)
4. Call appropriate agent service regeneration method
5. Create new ArtifactVersion record with incremented version number
6. Trigger audit re-run with new artifact
7. Create IterationLog record with feedback and version IDs
8. Update Campaign active version ID for this artifact type
9. Return IterationResult

**Context Enrichment:**
- Construct comprehensive prompt context
- Include original campaign brief in full
- Reference previous version content
- Append feedback text verbatim
- Translate feedback tags to natural language hints
- Include relevant audit findings from previous version

**Iteration History:**
Create `IterationHistory` record:
- CampaignId (string)
- ArtifactType (enum)
- IterationLogs (IterationLog[], ordered by timestamp)
- CurrentVersionId (string)
- TotalIterations (int)

Implement `GetIterationHistoryAsync`:
1. Query all iteration logs for campaign + artifact type
2. Order by timestamp descending
3. Include feedback summary and version metadata
4. Return structured history

### Feedback Validation
- Validate feedback text minimum length (10 characters)
- Validate artifact type is valid
- Validate current version ID exists in database
- Validate conflicting tags (warn but allow)
- Truncate excessively long feedback (>2000 chars) with notice

### Audit Re-run Logic
After regeneration:
1. Build audit request with:
   - All current campaign artifacts (new version for regenerated, existing for others)
   - Campaign brief
   - Revision history (previous audit reports)
2. Call IAuditService.ReauditAsync()
3. Save new AuditReport linked to new version ID
4. Compare audit scores to track improvement

### Version Management
- Increment version number automatically
- Maintain link between old and new versions via IterationLog
- Keep all previous versions (append-only)
- Mark previous version as Archived status
- Update active version pointer in Campaign record

### Metrics and Telemetry
Instrument the following metrics:
- Iterations per campaign (distribution)
- Average time between iterations
- Audit improvement delta (before/after scores)
- Feedback tag usage frequency
- Regeneration success rate
- Feedback text length distribution
- Time from feedback submission to regeneration completion

### Error Handling
- Handle empty feedback with validation error
- Retry regeneration on transient failures (max 3 attempts)
- Handle audit failure gracefully (mark audit pending)
- Validate version ID exists before regeneration
- Log all errors with full context

## Acceptance Criteria
- [ ] IterationService implemented with regeneration flow
- [ ] Regeneration preserves original campaign brief
- [ ] Feedback context correctly passed to agent services
- [ ] Audit automatically re-runs after regeneration
- [ ] IterationLog records created with complete metadata
- [ ] Version number increments correctly
- [ ] Active version ID updated in Campaign record
- [ ] Previous version marked as Archived
- [ ] Iteration history retrieval works correctly
- [ ] Feedback validation prevents empty submissions
- [ ] Submission to regeneration start latency <2s

## Testing Requirements
- [ ] Unit tests for IterationService (≥85% coverage)
- [ ] Test regeneration flow for all artifact types
- [ ] Test context enrichment with feedback and tags
- [ ] Test audit re-run integration
- [ ] Test version number incrementation
- [ ] Test iteration history retrieval
- [ ] Test feedback validation rules
- [ ] Test conflicting feedback tags handling
- [ ] Test with various feedback lengths
- [ ] Integration tests with real agent services
- [ ] Test concurrent iteration requests (should be serialized)

## Non-Functional Requirements
- Submission to regeneration start latency <2s
- Audit re-run trigger reliability 100%
- Feedback data loss 0%
- Support for 10+ iterations per artifact
- Iteration history retrieval <500ms

## Out of Scope
- Iteration cap enforcement (future soft cap)
- System-suggested feedback prompts (future enhancement)
- Semantic diff visualization (future)
- Automatic audit improvement tracking dashboard (future)
- Bulk regeneration of multiple artifacts
- Iteration rollback to previous version

## Notes
- Follow AGENTS.md iteration feedback patterns
- Ensure feedback context is comprehensive for best results
- Document context enrichment strategy in code comments
- Consider edge cases: empty feedback, conflicting tags, rapid successive iterations
- Create MADR for feedback handling decisions
- Test with diverse feedback scenarios
- Future: provide UI feedback tag suggestions based on audit findings
- Future: track improvement trends over iterations
- Ensure regeneration is atomic (version creation + audit + log)
