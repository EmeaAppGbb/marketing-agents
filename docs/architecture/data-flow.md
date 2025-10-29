# Data Flow

This document describes the detailed data flows within the Marketing Agents Platform, covering campaign generation, artifact streaming, compliance auditing, and feedback iterations.

## Campaign Generation Data Flow

### Overview

When a user creates a new campaign, the system orchestrates multiple AI agents to generate a complete set of marketing artifacts. This flow demonstrates the sequential and concurrent execution patterns.

### Detailed Flow

```mermaid
flowchart TD
    Start([User Creates Campaign]) --> ValidateInput[Validate Campaign Brief]
    ValidateInput --> SaveCampaign[Save Campaign to Cosmos DB]
    SaveCampaign --> NotifyStart[Broadcast CampaignStarted via SignalR]
    NotifyStart --> BuildContext[Build Campaign Context]
    
    BuildContext --> ParallelGen{Concurrent Generation}
    
    ParallelGen --> CopyAgent[Copywriting Agent]
    ParallelGen --> ShortCopyAgent[Short Copy Agent]
    ParallelGen --> VisualAgent[Visual Poster Agent]
    
    CopyAgent --> SaveCopy[Save Copy Artifact]
    ShortCopyAgent --> SaveShort[Save Short Copy Artifact]
    VisualAgent --> SaveVisual[Save Visual Artifact]
    
    SaveCopy --> StreamCopy[Stream Copy to Client]
    SaveShort --> StreamShort[Stream Short Copy to Client]
    SaveVisual --> StreamVisual[Stream Visual to Client]
    
    StreamCopy --> WaitAll[Wait for All Artifacts]
    StreamShort --> WaitAll
    StreamVisual --> WaitAll
    
    WaitAll --> AuditAgent[Audit Compliance Agent]
    AuditAgent --> AuditResults{Compliant?}
    
    AuditResults -->|Yes| ApprovedFlow[Mark Approved]
    AuditResults -->|No| RejectionFlow[Generate Feedback]
    
    ApprovedFlow --> SaveApproval[Save Compliance Approval]
    RejectionFlow --> SaveFeedback[Save Rejection Feedback]
    
    SaveApproval --> NotifyComplete[Broadcast CampaignCompleted]
    SaveFeedback --> CheckRetries{Retries < 5?}
    
    CheckRetries -->|Yes| RetryWithFeedback[Retry with Feedback]
    CheckRetries -->|No| NotifyFailed[Notify Generation Failed]
    
    RetryWithFeedback --> BuildContext
    NotifyComplete --> End([Campaign Ready])
    NotifyFailed --> End
```

### Data Transformations

#### 1. Campaign Brief → Agent Context
**Input** (from user):
```json
{
  "theme": "Summer Product Launch",
  "product": "EcoBottle - Sustainable Water Bottle",
  "targetAudience": "Environmentally conscious millennials",
  "tone": "Inspirational and eco-friendly",
  "constraints": ["Must include recycling message", "Avoid plastic-related claims"]
}
```

**Transformation** (to agent context):
```csharp
var context = $@"
Campaign Theme: {brief.Theme}
Product: {brief.Product}
Target Audience: {brief.TargetAudience}
Tone: {brief.Tone}
Constraints: {string.Join(", ", brief.Constraints)}
Previous Feedback: {rejectionFeedback ?? "None"}
";
```

#### 2. Agent Response → Artifact Model
**Agent Output**:
```json
{
  "content": "Discover the EcoBottle: your daily hydration companion...",
  "metadata": {
    "wordCount": 650,
    "tone": "inspirational",
    "keywords": ["sustainable", "eco-friendly", "recycling"]
  }
}
```

**Artifact Model**:
```csharp
public record CampaignArtifact
{
    public string Id { get; init; }
    public string CampaignId { get; init; }
    public ArtifactType Type { get; init; } // Copy, ShortCopy, Visual
    public string Content { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public string AgentName { get; init; }
}
```

#### 3. Compliance Check → Audit Result
**Audit Agent Input**:
```json
{
  "artifacts": [
    {"type": "Copy", "content": "..."},
    {"type": "ShortCopy", "content": "..."},
    {"type": "Visual", "content": "..."}
  ],
  "brandGuidelines": {...},
  "regulatoryRules": {...}
}
```

**Audit Result**:
```csharp
public record ComplianceAuditResult
{
    public bool IsCompliant { get; init; }
    public int ComplianceScore { get; init; } // 0-100
    public List<ComplianceIssue> Issues { get; init; }
    public string Feedback { get; init; }
}

public record ComplianceIssue
{
    public string ArtifactType { get; init; }
    public string IssueDescription { get; init; }
    public SeverityLevel Severity { get; init; }
}
```

## Real-Time Artifact Streaming

### SignalR Event Flow

```mermaid
sequenceDiagram
    participant Client
    participant SignalRHub
    participant AgentHost
    participant Agent
    participant CosmosDB
    
    Client->>SignalRHub: Connect to hub
    SignalRHub-->>Client: ConnectionId assigned
    Client->>SignalRHub: JoinCampaignGroup(campaignId)
    
    Note over AgentHost: Agent generates artifact
    Agent->>CosmosDB: Save artifact
    Agent->>SignalRHub: SendArtifactToGroup(campaignId, artifact)
    SignalRHub->>Client: OnArtifactGenerated(artifact)
    Client->>Client: Display artifact in UI
    
    Note over AgentHost: Compliance check completes
    Agent->>CosmosDB: Save audit result
    Agent->>SignalRHub: SendComplianceResultToGroup(campaignId, result)
    SignalRHub->>Client: OnComplianceCheckCompleted(result)
    Client->>Client: Update UI with compliance status
```

### SignalR Hub Methods

#### Server → Client Events

```typescript
// Frontend TypeScript client interface
interface ICampaignHub {
  // Invoked by server
  onCampaignStarted(campaignId: string): void;
  onArtifactGenerated(artifact: CampaignArtifact): void;
  onComplianceCheckCompleted(result: ComplianceAuditResult): void;
  onCampaignCompleted(campaignId: string): void;
  onGenerationFailed(campaignId: string, error: string): void;
}
```

#### Client → Server Methods

```csharp
// Backend C# hub interface
public interface ICampaignHub
{
    Task JoinCampaignGroup(string campaignId);
    Task LeaveCampaignGroup(string campaignId);
    Task RequestCampaignStatus(string campaignId);
}
```

## Feedback Iteration Flow

### Revision Request Data Flow

```mermaid
flowchart TD
    UserReview([User Reviews Campaign]) --> Feedback{Satisfied?}
    
    Feedback -->|Yes| Approve[Approve Campaign]
    Feedback -->|No| ProvideNotes[Provide Revision Notes]
    
    ProvideNotes --> SaveNotes[Save Feedback to Cosmos DB]
    SaveNotes --> TriggerRevision[Trigger Revision Workflow]
    
    TriggerRevision --> LoadOriginal[Load Original Campaign]
    LoadOriginal --> BuildRevisionContext[Build Context with Feedback]
    BuildRevisionContext --> RegenerateArtifacts[Re-run Agent Orchestration]
    
    RegenerateArtifacts --> StreamUpdated[Stream Updated Artifacts]
    StreamUpdated --> AuditRevised[Audit Revised Campaign]
    AuditRevised --> NotifyRevisionComplete[Notify Revision Complete]
    
    Approve --> MarkFinal[Mark Campaign as Final]
    NotifyRevisionComplete --> UserReview
    MarkFinal --> End([Campaign Finalized])
```

### Revision Context Structure

```csharp
public record RevisionContext
{
    public string OriginalCampaignId { get; init; }
    public int RevisionNumber { get; init; }
    public string UserFeedback { get; init; }
    public List<string> SpecificChanges { get; init; }
    public List<CampaignArtifact> PreviousArtifacts { get; init; }
}
```

**Example Revision Prompt**:
```
Original Campaign Theme: Summer Product Launch
Original Copy: "Discover the EcoBottle: your daily hydration companion..."

User Feedback: "Make the tone more energetic and youthful. Emphasize the adventure aspect."

Specific Changes Requested:
- Add references to outdoor activities
- Include more vibrant language
- Shorten the introduction

Please regenerate the copy incorporating this feedback while maintaining brand consistency.
```

## Database Query Patterns

### Cosmos DB Queries

> **Note**: For complete data model details, entity schemas, and partition key strategy, see [Persistence Layer Documentation](persistence.md).

#### Retrieve Campaign Snapshot (Optimized for Dashboard)

```csharp
// Uses the repository pattern for aggregated view
var snapshot = await _campaignRepository.GetCampaignSnapshotAsync(campaignId);

// Returns:
// - Campaign entity
// - Active artifact versions (per type)
// - Audit reports for each version
// Target: ≤500ms retrieval time
```

**Implementation** (from `CampaignRepository`):
```csharp
public async Task<CampaignSnapshot?> GetCampaignSnapshotAsync(
    string id, 
    CancellationToken cancellationToken = default)
{
    // 1. Point read for campaign (fastest operation)
    var campaign = await GetByIdAsync(id, cancellationToken);
    if (campaign is null) return null;

    // 2. Fetch active versions in parallel
    var activeVersions = new Dictionary<ArtifactType, ArtifactVersion>();
    var auditReports = new Dictionary<string, AuditReport>();

    if (campaign.ActiveVersionIds is not null)
    {
        var versionTasks = campaign.ActiveVersionIds.Select(async kvp =>
        {
            var version = await _versionRepository.GetByIdAsync(
                kvp.Value, 
                campaign.Id, 
                cancellationToken);
            return (Type: kvp.Key, Version: version);
        });

        var versions = await Task.WhenAll(versionTasks);

        foreach (var (type, version) in versions)
        {
            if (version is not null)
            {
                activeVersions[type] = version;

                // 3. Fetch audit report if available
                if (version.AuditReportId is not null)
                {
                    var audit = await _auditReportRepository.GetByIdAsync(
                        version.AuditReportId, 
                        campaign.Id, 
                        cancellationToken);
                    
                    if (audit is not null)
                    {
                        auditReports[version.Id] = audit;
                    }
                }
            }
        }
    }

    return new CampaignSnapshot
    {
        Campaign = campaign,
        ActiveVersions = activeVersions,
        AuditReports = auditReports,
    };
}
```

#### Query Artifacts by Campaign and Type

```csharp
// Uses QueryDefinition with partition key for efficiency
var query = new QueryDefinition(
    "SELECT * FROM c WHERE c.campaignId = @campaignId AND c.type = @type ORDER BY c.createdAt DESC")
    .WithParameter("@campaignId", campaignId)
    .WithParameter("@type", "Copy");

using var feed = _container.GetItemQueryIterator<Artifact>(
    query,
    requestOptions: new QueryRequestOptions
    {
        PartitionKey = new PartitionKey(campaignId), // Scoped to single partition
    });

var artifacts = new List<Artifact>();
while (feed.HasMoreResults)
{
    var response = await feed.ReadNextAsync();
    artifacts.AddRange(response);
}
```

#### Retrieve All Versions for an Artifact

```csharp
// Implementation from ArtifactVersionRepository
public async Task<IReadOnlyList<ArtifactVersion>> GetByArtifactIdAsync(
    string artifactId, 
    string campaignId, 
    CancellationToken cancellationToken = default)
{
    var query = new QueryDefinition(
        "SELECT * FROM c WHERE c.artifactId = @artifactId ORDER BY c.versionNumber DESC")
        .WithParameter("@artifactId", artifactId);

    using var feed = _container.GetItemQueryIterator<ArtifactVersion>(
        query,
        requestOptions: new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(campaignId),
        });

    var versions = new List<ArtifactVersion>();
    while (feed.HasMoreResults)
    {
        var response = await feed.ReadNextAsync(cancellationToken);
        versions.AddRange(response);
    }

    return versions.AsReadOnly();
}
```

#### Point Read (Fastest Query Pattern)

```csharp
// Used throughout repositories for maximum performance
var response = await _container.ReadItemAsync<Campaign>(
    id: campaignId,
    partitionKey: new PartitionKey(campaignId),
    cancellationToken: cancellationToken);

return response.Resource;

// Performance: Sub-millisecond latency
// Cost: 1 RU per operation
```

### Redis Caching Patterns

#### Cache Agent Response
```csharp
var cacheKey = $"agent:{agentName}:response:{contextHash}";
var cachedResponse = await redis.StringGetAsync(cacheKey);

if (!cachedResponse.IsNullOrEmpty)
{
    return JsonSerializer.Deserialize<AgentResponse>(cachedResponse);
}

// Generate response via agent
var response = await agent.RunAsync(context);

// Cache for 1 hour
await redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromHours(1));

return response;
```

## Error Handling & Retry Logic

### Agent Execution Error Flow

```mermaid
flowchart TD
    Execute[Execute Agent] --> TryCatch{Exception?}
    
    TryCatch -->|No| Success[Return Result]
    TryCatch -->|Yes| CheckRetries{Retries < Max?}
    
    CheckRetries -->|Yes| LogRetry[Log Retry Attempt]
    CheckRetries -->|No| LogFailure[Log Permanent Failure]
    
    LogRetry --> ExponentialBackoff[Wait with Exponential Backoff]
    ExponentialBackoff --> Execute
    
    LogFailure --> NotifyUser[Notify User of Failure]
    NotifyUser --> Fallback{Fallback Available?}
    
    Fallback -->|Yes| UseFallback[Use Fallback Agent]
    Fallback -->|No| ReturnError[Return Error Response]
    
    UseFallback --> Success
    Success --> End([Complete])
    ReturnError --> End
```

### Retry Configuration

```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutException>()
    .WaitAndRetryAsync(
        retryCount: 5,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timespan, retryCount, context) =>
        {
            logger.LogWarning("Retry {RetryCount} after {Delay}s due to {Exception}",
                retryCount, timespan.TotalSeconds, exception.GetType().Name);
        });

await retryPolicy.ExecuteAsync(async () =>
{
    return await agent.RunAsync(prompt);
});
```

## Performance Optimization

### Concurrent Agent Execution

```csharp
// Execute independent agents in parallel
var copyTask = copyAgentProvider.GenerateCopyAsync(context);
var shortCopyTask = shortCopyAgentProvider.GenerateShortCopyAsync(context);
var visualTask = visualAgentProvider.GenerateVisualAsync(context);

// Wait for all to complete
await Task.WhenAll(copyTask, shortCopyTask, visualTask);

// Gather results
var artifacts = new List<CampaignArtifact>
{
    await copyTask,
    await shortCopyTask,
    await visualTask
};
```

### Streaming Large Responses

```csharp
// Stream agent response as it's generated
await foreach (var chunk in agent.StreamAsync(prompt))
{
    await signalRHub.Clients.Group(campaignId).SendAsync("OnChunkReceived", chunk);
}
```

## Next Steps

- [System Design](system-design.md) - Component architecture
- [Development Workflow](../guides/development.md) - Build features
- [API Documentation](../api/rest-api.md) - API reference
