# Data Model & Persistence

This document describes the data model, entities, and persistence strategy for the Marketing Agents Platform.

## Overview

The platform uses **Azure Cosmos DB** for NoSQL data storage with the **native .NET SDK** (`Microsoft.Azure.Cosmos`). All data models are defined as immutable C# records with strong typing and nullable reference types enabled.

## Technology Stack

| Component | Technology | Version |
|-----------|----------|---------|
| Database | Azure Cosmos DB (NoSQL API) | - |
| SDK | Microsoft.Azure.Cosmos | 3.53.1 |
| Aspire Integration | Aspire.Microsoft.Azure.Cosmos | 9.5.1 |
| Serialization | System.Text.Json | Built-in |

## Database Architecture

### Database and Containers

**Database**: `marketingagents`

**Containers:**

| Container | Partition Key | Purpose |
|-----------|--------------|---------|
| `Campaigns` | `/partitionKey` (Campaign ID) | Root campaign entities |
| `Artifacts` | `/partitionKey` (Campaign ID) | Generated artifacts by type |
| `ArtifactVersions` | `/partitionKey` (Campaign ID) | Version history for artifacts |
| `AuditReports` | `/partitionKey` (Campaign ID) | Compliance audit results |
| `IterationLogs` | `/partitionKey` (Campaign ID) | Feedback and iteration tracking |
| `OrchestrationRuns` | `/partitionKey` (Campaign ID) | Orchestration execution records |

### Partition Key Strategy

**Strategy**: All entities use **Campaign ID** as the partition key.

**Rationale:**
- **Co-location**: Related data (campaign, artifacts, audits) stored in same logical partition
- **Query Efficiency**: Queries scoped to a campaign are fast (point reads)
- **Cost Optimization**: Cross-partition queries avoided for most operations
- **Performance Target**: Campaign snapshot retrieval ≤500ms

**Trade-offs:**
- Campaign size limited to 20 GB (well within requirements for MVP)
- Cross-campaign queries less efficient (not a primary use case)

## Entity Models

### Campaign

Root entity representing a marketing campaign.

```csharp
public record Campaign
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string Name { get; init; }
    public required CampaignBrief Brief { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required CampaignStatus Status { get; init; }
    public Dictionary<ArtifactType, string>? ActiveVersionIds { get; init; }
    public bool IsDeleted { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => Id;
}
```

**Status Values:**
- `Draft` - Initial state
- `Generating` - Artifacts being generated
- `Generated` - All artifacts created
- `Auditing` - Compliance check in progress
- `Completed` - Approved and ready
- `Cancelled` - Campaign cancelled

### CampaignBrief

Embedded entity containing campaign requirements.

```csharp
public record CampaignBrief
{
    public required string Objective { get; init; }
    public required string TargetAudience { get; init; }
    public required string ProductDetails { get; init; }
    public string[]? ToneGuidelines { get; init; }
    public string[]? BrandPalette { get; init; }
    public CampaignConstraints? Constraints { get; init; }
}
```

### Artifact

Represents a campaign artifact container.

```csharp
public record Artifact
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string CampaignId { get; init; }
    public required ArtifactType Type { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string[] VersionIds { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
```

**Artifact Types:**
- `Copy` - Long-form marketing copy
- `ShortCopy` - Social media posts
- `VisualConcept` - Poster/image concepts

### ArtifactVersion

Specific version of an artifact with content.

```csharp
public record ArtifactVersion
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string ArtifactId { get; init; }
    public required string CampaignId { get; init; }
    public required int VersionNumber { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required ArtifactVersionStatus Status { get; init; }
    public string? AuditReportId { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
```

**Status Values:**
- `Pending` - Created but not generated
- `Generated` - Content generated
- `Audited` - Compliance check completed
- `Approved` - Passed audit
- `Archived` - Old version

### AuditReport

Compliance audit results for a version.

```csharp
public record AuditReport
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string VersionId { get; init; }
    public required string CampaignId { get; init; }
    public required AuditStatus OverallStatus { get; init; }
    public required Dictionary<string, AuditScore> CategoryScores { get; init; }
    public required FlaggedItem[] FlaggedItems { get; init; }
    public required Recommendation[] Recommendations { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
```

**Audit Status:**
- `Pass` - Compliant, no issues
- `Conditional` - Minor issues, can proceed
- `Fail` - Major issues, must revise

### IterationLog

Tracks feedback and version changes.

```csharp
public record IterationLog
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string CampaignId { get; init; }
    public required string ArtifactId { get; init; }
    public required string OldVersionId { get; init; }
    public required string NewVersionId { get; init; }
    public required string FeedbackText { get; init; }
    public string[]? FeedbackTags { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
```

### OrchestrationRun

Execution metadata for agent orchestration.

```csharp
public record OrchestrationRun
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    public required string CampaignId { get; init; }
    public required ExecutionMode ExecutionMode { get; init; }
    public required OrchestrationRunStatus Status { get; init; }
    public required DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public required string[] ArtifactVersionIds { get; init; }
    public string? ErrorMessage { get; init; }
    
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
```

**Execution Modes:**
- `Parallel` - Agents run concurrently
- `Sequential` - Agents run one after another

**Run Status:**
- `Queued` - Waiting to execute
- `Running` - In progress
- `Completed` - Finished successfully
- `Failed` - Error occurred
- `Cancelled` - User cancelled

## Repository Pattern

### Interfaces

All repositories follow async patterns and use dependency injection.

```csharp
public interface ICampaignRepository
{
    Task<Campaign> CreateAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task<Campaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Campaign> UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<CampaignSnapshot?> GetCampaignSnapshotAsync(string id, CancellationToken cancellationToken = default);
}
```

### Campaign Snapshot

Optimized query for dashboard rendering.

```csharp
public record CampaignSnapshot
{
    public required Campaign Campaign { get; init; }
    public required Dictionary<ArtifactType, ArtifactVersion> ActiveVersions { get; init; }
    public required Dictionary<string, AuditReport> AuditReports { get; init; }
}
```

**Performance:**
- Target: ≤500ms retrieval time
- Uses point reads with partition key
- Minimal cross-document joins

### Implementation Example

```csharp
public class CampaignRepository : ICampaignRepository
{
    private readonly Container _container;
    private readonly IVersionRepository _versionRepository;
    private readonly IAuditReportRepository _auditReportRepository;

    public CampaignRepository(
        CosmosClient cosmosClient,
        string databaseName,
        IVersionRepository versionRepository,
        IAuditReportRepository auditReportRepository)
    {
        _container = cosmosClient.GetContainer(databaseName, "Campaigns");
        _versionRepository = versionRepository;
        _auditReportRepository = auditReportRepository;
    }

    public async Task<Campaign> CreateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            campaign,
            new PartitionKey(campaign.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }
    
    // Additional methods...
}
```

## Database Initialization

### Development Auto-Initialization

The `CosmosDbInitializationService` automatically creates database and containers in development.

```csharp
public class CosmosDbInitializationService
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Create database
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(
            _databaseName,
            cancellationToken: cancellationToken);

        var database = databaseResponse.Database;

        // Create containers with partition keys
        var containerConfigs = new[]
        {
            new ContainerConfig("Campaigns", "/partitionKey"),
            new ContainerConfig("Artifacts", "/partitionKey"),
            new ContainerConfig("ArtifactVersions", "/partitionKey"),
            new ContainerConfig("AuditReports", "/partitionKey"),
            new ContainerConfig("IterationLogs", "/partitionKey"),
            new ContainerConfig("OrchestrationRuns", "/partitionKey"),
        };

        foreach (var config in containerConfigs)
        {
            await CreateContainerIfNotExistsAsync(database, config, cancellationToken);
        }
    }
}
```

**Container Configuration:**
- **Throughput**: 400 RU/s (manual, development)
- **Indexing**: Automatic, consistent mode
- **Partition Key**: `/partitionKey` for all containers

### Aspire Integration

```csharp
// AppHost configuration
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsPreviewEmulator(emulator => emulator.WithDataExplorer())
    .AddCosmosDatabase("marketingagents");

// API registration
builder.AddAzureCosmosClient("cosmosdb");
```

## Query Patterns

### Point Read (Fastest)

```csharp
var campaign = await _container.ReadItemAsync<Campaign>(
    id: campaignId,
    partitionKey: new PartitionKey(campaignId));
```

**Performance:** Sub-millisecond latency

### Filtered Query (Within Partition)

```csharp
var query = new QueryDefinition("SELECT * FROM c WHERE c.CampaignId = @campaignId AND c.Type = @type")
    .WithParameter("@campaignId", campaignId)
    .WithParameter("@type", "Copy");

using var feed = _container.GetItemQueryIterator<Artifact>(
    query,
    requestOptions: new QueryRequestOptions
    {
        PartitionKey = new PartitionKey(campaignId),
    });

while (feed.HasMoreResults)
{
    var response = await feed.ReadNextAsync();
    foreach (var artifact in response)
    {
        // Process artifact
    }
}
```

### Soft Delete Pattern

```csharp
public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
{
    var campaign = await GetByIdAsync(id, cancellationToken);
    if (campaign is null) return;

    var updatedCampaign = campaign with
    {
        IsDeleted = true,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    await UpdateAsync(updatedCampaign, cancellationToken);
}
```

## Error Handling

### Cosmos DB Exceptions

```csharp
try
{
    var response = await _container.ReadItemAsync<Campaign>(
        id,
        new PartitionKey(id),
        cancellationToken: cancellationToken);

    return response.Resource;
}
catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    return null;
}
```

### Retry Policies

Aspire automatically configures Polly for resilience:
- Circuit breakers
- Exponential backoff retries
- Timeout policies

## Performance Considerations

### Optimization Strategies

1. **Use Partition Keys**: Always provide partition key for queries
2. **Point Reads**: Prefer `ReadItemAsync` over queries when possible
3. **Indexing**: Default automatic indexing is sufficient for MVP
4. **Batch Operations**: Use `TransactionalBatch` for atomic multi-item operations
5. **Pagination**: Implement continuation tokens for large result sets

### Performance Targets

| Operation | Target | Notes |
|-----------|--------|-------|
| Campaign Snapshot | ≤500ms | Including related entities |
| Point Read | <10ms | Single item by ID + partition key |
| Filtered Query | <100ms | Within single partition |
| Write Operation | <50ms | Single item create/update |

## Security & Compliance

### Data Encryption
- **At Rest**: Enabled by default in Cosmos DB
- **In Transit**: TLS 1.2+ required
- **Credentials**: Stored in Azure Key Vault (production)

### Access Control
- **Development**: Cosmos DB Emulator with fixed key
- **Production**: Managed Identity (Azure AD)
- **Least Privilege**: Repository pattern limits data access scope

## Future Enhancements

### Phase 2 Considerations

- **TTL (Time-to-Live)**: Automatic cleanup of archived versions
- **Change Feed**: Real-time event processing for analytics
- **Hierarchical Partition Keys**: Multi-level partitioning for scale
- **Materialized Views**: Pre-computed aggregations
- **Full-Text Search**: Azure Cognitive Search integration

## Related Documentation

- **ADR-0004: Cosmos DB Native SDK Persistence** - See [Architecture Decisions](decisions.md) for the full decision record
- [Data Flow](data-flow.md)
- [System Design](system-design.md)
- [API Documentation](../api/rest-api.md)
