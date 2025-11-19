# ADR 0003: Use Azure Cosmos DB with Native SDK (No Entity Framework)

**Status**: Accepted

**Date**: 30 October 2025

## Context

The application needs to store:
- Campaign briefs (theme, target audience, product details)
- Generated artifacts (copy, social media posts, poster concepts)
- Audit results (compliance status, violations, warnings)
- Metadata (execution times, retry counts, errors)

Per FRD-005, initial implementation may use in-memory storage with planned migration to persistent database.

## Decision Drivers

- AGENTS.md explicitly states: "Do NOT use Entity Framework" and "Use native Azure Cosmos DB SDK (`Microsoft.Azure.Cosmos`) directly"
- Need for flexible schema to accommodate evolving artifact structures
- Requirement for fast read/write operations (≤500ms per FRD-005)
- Need for auto-initialization in development mode
- Future scalability to handle thousands of campaigns per user

## Considered Options

1. **Azure Cosmos DB with native SDK** (mandated by AGENTS.md)
2. Entity Framework Core with Cosmos DB provider (explicitly forbidden)
3. In-memory only (insufficient for production)
4. SQL Database with Entity Framework (wrong tool for document storage)

## Decision Outcome

**Chosen: Azure Cosmos DB with native SDK (`Microsoft.Azure.Cosmos`)**

### Implementation Approach

**Development Phase:**
- Start with in-memory storage for rapid development
- Use repository pattern with async methods for CRUD operations
- Design repository interfaces to match future Cosmos DB SDK

**Production Phase:**
- Migrate to Azure Cosmos DB with native SDK
- Use Aspire integration: `builder.AddAzureCosmosClient()`
- Auto-initialize database and containers in development via startup logic

### Data Model
```csharp
public record Campaign
{
    [JsonPropertyName("id")]
    public string Id { get; init; } // UUID

    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; init; } // Could be userId in future

    [JsonPropertyName("brief")]
    public CampaignBrief Brief { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } // created, generating, completed, failed

    [JsonPropertyName("artifacts")]
    public Artifacts? Artifacts { get; init; }

    [JsonPropertyName("audit")]
    public AuditResult? Audit { get; init; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; init; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; init; }
}
```

### Rationale

- **AGENTS.md compliance**: Explicitly required, Entity Framework forbidden
- **Document model fit**: Campaign data is naturally hierarchical and document-like
- **Performance**: Native SDK provides optimal performance for document operations
- **Schema flexibility**: Can evolve artifact structures without migrations
- **Global distribution**: Cosmos DB supports multi-region if needed in future

## Consequences

### Positive
- Direct control over queries and performance
- No ORM abstraction overhead
- Flexible schema evolution
- Built-in partitioning and scaling
- Aspire integration simplifies connection management

### Negative
- More boilerplate code than Entity Framework (but this is intentional per guidelines)
- Manual query construction (but provides more control)
- Learning curve for developers unfamiliar with Cosmos DB SDK

### Implementation Notes

**Repository Pattern:**
```csharp
public interface ICampaignRepository
{
    Task<Campaign> CreateAsync(Campaign campaign);
    Task<Campaign?> GetByIdAsync(string id);
    Task<IEnumerable<CampaignSummary>> ListAsync();
    Task UpdateAsync(Campaign campaign);
}
```

**Aspire Configuration:**
```csharp
// In AppHost/Program.cs
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator(); // For local development

// In Api/Program.cs
builder.AddAzureCosmosClient("cosmos");
```

**Auto-initialization (Development):**
```csharp
// Create database and containers on startup in development mode
if (builder.Environment.IsDevelopment())
{
    var cosmosClient = builder.Services.BuildServiceProvider()
        .GetRequiredService<CosmosClient>();
    
    await cosmosClient.CreateDatabaseIfNotExistsAsync("MarketingAgents");
    var database = cosmosClient.GetDatabase("MarketingAgents");
    await database.CreateContainerIfNotExistsAsync("Campaigns", "/partitionKey");
}
```

**Performance Targets (from FRD-005):**
- Create: ≤1 second
- Read: ≤500ms
- Update: ≤500ms
