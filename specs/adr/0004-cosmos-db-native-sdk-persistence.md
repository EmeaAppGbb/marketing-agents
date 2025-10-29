# 0004: Campaign Data Persistence with Cosmos DB and Native SDK

Date: 2025-10-29

## Status

Accepted

## Context

The Marketing Agents application requires a robust data persistence layer to store campaigns, artifacts, versions, audit reports, iteration logs, and orchestration runs. The system needs to support:

- Fast retrieval of campaign snapshots (≤500ms target)
- Efficient querying by campaign ID (partition key)
- Append-only semantics for versions and iteration logs
- Referential integrity between entities
- Automatic database initialization in development
- Integration with .NET Aspire for local development orchestration

## Decision Drivers

- **Performance**: Need sub-second query performance for dashboard rendering
- **Type Safety**: Strong typing with C# records and nullable reference types
- **Aspire Integration**: Must integrate seamlessly with .NET Aspire orchestration
- **Developer Experience**: Auto-initialization in development, minimal configuration
- **Cost Efficiency**: Partition key strategy to optimize query performance and costs
- **Data Consistency**: No orphaned audit reports, clear entity relationships

## Options Considered

### Option 1: Entity Framework Core with Cosmos DB Provider

**Pros:**
- Familiar ORM patterns for .NET developers
- LINQ query support
- Built-in change tracking
- Code-first migrations

**Cons:**
- Adds abstraction layer overhead
- Less control over Cosmos DB-specific optimizations
- **Violates AGENTS.md requirement**: "Do NOT use Entity Framework"
- Potential performance overhead for simple CRUD operations
- Limited support for Cosmos DB-specific features (partition keys, TTL, etc.)

### Option 2: Dapper with Cosmos DB SQL API

**Pros:**
- Lightweight micro-ORM
- More performant than full EF Core
- SQL query flexibility
- Minimal overhead

**Cons:**
- Cosmos DB SQL API has limitations vs. native SDK
- Still adds an abstraction layer
- Less Cosmos DB-specific feature support
- Manual mapping required
- No native Aspire integration for Dapper + Cosmos DB

### Option 3: Native Azure Cosmos DB SDK (CHOSEN)

**Pros:**
- **Follows AGENTS.md requirement**: "Use native Azure Cosmos DB SDK (Microsoft.Azure.Cosmos) directly"
- Direct access to all Cosmos DB features
- Optimal performance with no ORM overhead
- Full control over partition keys, indexing policies, and query optimization
- Native Aspire integration via `Aspire.Microsoft.Azure.Cosmos`
- Excellent async/await support
- Point reads with partition key are fastest possible
- Strong typing with record types
- Built-in retry policies and resilience with Polly

**Cons:**
- More verbose repository implementations
- Manual entity mapping (mitigated by record types)
- Requires explicit partition key management
- No automatic change tracking (intentional for our use case)

## Decision

We will use **Option 3: Native Azure Cosmos DB SDK** for the following reasons:

1. **Compliance with Architecture Standards**: Explicitly required by AGENTS.md
2. **Performance**: Direct SDK calls provide optimal performance for our ≤500ms snapshot retrieval target
3. **Aspire Integration**: `Aspire.Microsoft.Azure.Cosmos` provides seamless integration with .NET Aspire orchestration
4. **Type Safety**: Works perfectly with C# 11+ record types and nullable reference types
5. **Cosmos DB Features**: Full access to partition keys, indexing policies, TTL, and other Cosmos DB-specific features
6. **Control**: Complete control over query optimization and performance tuning

## Implementation Details

### Data Model
- All entities defined as C# records with required properties
- Partition key strategy: Use `CampaignId` as partition key for co-location of related data
- Enums for status fields (CampaignStatus, ArtifactVersionStatus, etc.)
- JSON serialization with `System.Text.Json.Serialization` attributes

### Repository Pattern
- Interface-based repositories for testability (`ICampaignRepository`, `IArtifactRepository`, etc.)
- Scoped lifetime for repositories in DI container
- Async methods throughout using `async`/`await`
- Point reads using partition key + id for optimal performance
- Query methods using `QueryDefinition` with parameters

### Database Initialization
- `CosmosDbInitializationService` for automatic database and container creation in development
- Container configurations with partition key paths
- Manual throughput (400 RU/s) for development
- Indexing policy configuration for optimal query performance

### Aspire Integration
- AppHost configuration: `.AddAzureCosmosDB("cosmosdb").RunAsPreviewEmulator()`
- API project registration: `builder.AddAzureCosmosClient("cosmosdb")`
- Automatic connection string injection via Aspire service discovery
- Container references with `.WithReference(cosmosDb).WaitFor(cosmosDb)`

### Error Handling
- Cosmos DB exceptions handled with try-catch blocks
- `HttpStatusCode.NotFound` returns `null` for missing entities
- Retry policies via Aspire's built-in Polly integration

## Consequences

### Positive

- **High Performance**: Direct SDK usage provides fastest possible queries
- **Full Feature Access**: Can utilize all Cosmos DB capabilities
- **Type Safety**: Strong typing throughout with records and nullable reference types
- **Aspire Benefits**: Automatic local emulator setup, telemetry, service discovery
- **Clear Separation**: Repository pattern provides clean abstraction for testing
- **Future-Proof**: Easy to add Cosmos DB-specific features (TTL, change feed, etc.)

### Negative

- **More Code**: Repository implementations are more verbose than EF Core
- **Manual Mapping**: No automatic change tracking (mitigated by record immutability)
- **Partition Key Management**: Must explicitly manage partition keys (documented in code)

### Neutral

- **Learning Curve**: Developers must understand Cosmos DB concepts (partitioning, RU/s, etc.)
- **Testing**: Requires Testcontainers or emulator for integration tests

## Compliance Checklist

- ✅ Uses native Cosmos DB SDK as required by AGENTS.md
- ✅ All entities defined as C# records
- ✅ Nullable reference types enabled
- ✅ Async methods throughout
- ✅ Repository pattern for testability
- ✅ Aspire integration configured
- ✅ Auto-initialization in development
- ✅ Partition key strategy documented
- ✅ Performance targets considered (≤500ms)

## Related Decisions

- ADR-0001: Backend Scaffolding Architecture (Aspire orchestration)
- Task-001: Backend Scaffolding (project structure)
- Task-004: Campaign Persistence Model (this implementation)

## References

- [AGENTS.md - Backend Development Section](../../AGENTS.md)
- [Azure Cosmos DB .NET SDK Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-dotnet-v3)
- [.NET Aspire Azure Cosmos DB Integration](https://learn.microsoft.com/en-us/dotnet/aspire/database/azure-cosmos-db-integration)
- [Feature: Campaign Data Model & Persistence](../features/feature-campaign-persistence-model.md)
- [Task 004: Campaign Persistence Model](../tasks/004-task-campaign-persistence-model.md)
