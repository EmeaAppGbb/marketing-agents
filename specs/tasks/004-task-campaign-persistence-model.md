# Task 004: Campaign Data Model & Cosmos DB Persistence

## Description
Implement the campaign data model and Cosmos DB persistence layer using the native Azure Cosmos DB SDK. Define all entities, repositories, and database initialization logic to support campaigns, artifacts, versions, audit reports, and iteration metadata.

## Dependencies
- Task 001: Backend Scaffolding

## Technical Requirements

### Cosmos DB Configuration
- Configure Aspire integration for Cosmos DB using `.AddAzureCosmosClient()`
- Set up local Cosmos DB Emulator for development via `.RunAsEmulator()`
- Configure automatic database and container creation in development mode
- Define partition key strategy for optimal query performance
- Set up connection string management via User Secrets

### Data Models (Use Record Types)
Create the following domain entities as C# records:

**Campaign Record**
- Id (string, globally unique)
- Name (string, required)
- Brief (CampaignBrief record type)
- CreatedAt (DateTimeOffset)
- UpdatedAt (DateTimeOffset)
- Status (enum: Draft, Generating, Generated, Auditing, Completed, Cancelled)
- ActiveVersionIds (Dictionary<ArtifactType, string>)

**CampaignBrief Record**
- Objective (string, required)
- TargetAudience (string, required)
- ProductDetails (string, required)
- ToneGuidelines (string[], optional)
- BrandPalette (string[], optional hex values)
- Constraints (CampaignConstraints record, optional)

**Artifact Record**
- Id (string, globally unique)
- CampaignId (string, required)
- Type (enum: Copy, ShortCopy, VisualConcept)
- CreatedAt (DateTimeOffset)
- VersionIds (string[] list of version IDs)

**ArtifactVersion Record**
- Id (string, globally unique)
- ArtifactId (string, required)
- CampaignId (string, required, partition key)
- VersionNumber (int)
- Content (string, JSON serialized)
- CreatedAt (DateTimeOffset)
- Status (enum: Pending, Generated, Audited, Approved, Archived)
- AuditReportId (string, nullable)

**AuditReport Record**
- Id (string, globally unique)
- VersionId (string, required)
- CampaignId (string, required, partition key)
- OverallStatus (enum: Pass, Conditional, Fail)
- CategoryScores (Dictionary<string, AuditScore>)
- FlaggedItems (FlaggedItem[])
- Recommendations (Recommendation[])
- CreatedAt (DateTimeOffset)

**IterationLog Record**
- Id (string, globally unique)
- CampaignId (string, required, partition key)
- ArtifactId (string, required)
- OldVersionId (string, required)
- NewVersionId (string, required)
- FeedbackText (string)
- FeedbackTags (string[], optional)
- CreatedAt (DateTimeOffset)

**OrchestrationRun Record**
- Id (string, globally unique)
- CampaignId (string, required, partition key)
- ExecutionMode (enum: Parallel, Sequential)
- Status (enum: Queued, Running, Completed, Failed, Cancelled)
- StartedAt (DateTimeOffset)
- CompletedAt (DateTimeOffset, nullable)
- ArtifactVersionIds (string[])
- ErrorMessage (string, nullable)

### Repository Pattern
Implement repository interfaces and implementations:

**ICampaignRepository**
- CreateAsync(Campaign campaign)
- GetByIdAsync(string id)
- UpdateAsync(Campaign campaign)
- DeleteAsync(string id) - soft delete
- GetCampaignSnapshotAsync(string id) - active versions + audit statuses

**IArtifactRepository**
- CreateAsync(Artifact artifact)
- GetByIdAsync(string id)
- GetByCampaignIdAsync(string campaignId)
- GetVersionHistoryAsync(string artifactId)

**IVersionRepository**
- CreateAsync(ArtifactVersion version)
- GetByIdAsync(string versionId)
- GetLatestByArtifactIdAsync(string artifactId)
- ArchiveAsync(string versionId)

**IAuditReportRepository**
- CreateAsync(AuditReport report)
- GetByVersionIdAsync(string versionId)
- GetByCampaignIdAsync(string campaignId)

**IIterationLogRepository**
- CreateAsync(IterationLog log)
- GetByCampaignIdAsync(string campaignId)
- GetByArtifactIdAsync(string artifactId)

**IOrchestrationRunRepository**
- CreateAsync(OrchestrationRun run)
- GetByIdAsync(string runId)
- UpdateAsync(OrchestrationRun run)
- GetByCampaignIdAsync(string campaignId)

### Database Initialization
- Create database initialization service
- Implement auto-creation of database and containers in development
- Define container configurations (partition keys, indexing policies)
- Set up TTL policies for archived versions (future consideration)

### Error Handling
- Implement retry policies for transient failures using Polly
- Handle Cosmos DB specific exceptions (rate limiting, conflicts)
- Ensure referential integrity through transactional operations where possible

### Performance Optimizations
- Configure indexing policies for common query patterns
- Implement caching strategy for frequently accessed campaigns
- Use point reads where possible (partition key + id)
- Implement pagination for list operations

## Acceptance Criteria
- [ ] All data models defined as C# records with nullable reference types
- [ ] All repository interfaces and implementations created
- [ ] Cosmos DB client configured via Aspire integration
- [ ] Local Cosmos DB Emulator working with automatic database/container creation
- [ ] Campaign snapshot retrieval returns complete data in ≤500ms
- [ ] No orphaned audit reports (referential integrity maintained)
- [ ] Soft delete functionality working for campaigns and versions
- [ ] Version history retrieval working correctly
- [ ] All repositories registered in DI container

## Testing Requirements
- [ ] Unit tests for all repository methods (≥85% coverage)
- [ ] Integration tests using Testcontainers for Cosmos DB
- [ ] Test data model serialization/deserialization
- [ ] Test referential integrity constraints
- [ ] Test pagination functionality
- [ ] Test soft delete and archival operations
- [ ] Test concurrent update scenarios
- [ ] Test performance of snapshot retrieval (≤500ms target)
- [ ] Test error handling and retry logic

## Non-Functional Requirements
- Snapshot retrieval latency ≤500ms (baseline target)
- Write operation reliability ≥99%
- Support for campaigns with up to 100 artifact versions
- Zero orphan audit reports through proper transaction handling

## Out of Scope
- Full-text search across versions (future enhancement)
- Automatic archival scheduling (future enhancement)
- Multi-region replication configuration
- Production-scale performance tuning

## Notes
- Follow AGENTS.md: Do NOT use Entity Framework, use native Cosmos DB SDK only
- Use record types for all data models
- Implement append-only semantics for versions and iteration logs
- Ensure all DateTime values use DateTimeOffset for consistency
- Use CampaignId as partition key for optimal co-location of related data
- Document partition key strategy in ADR
