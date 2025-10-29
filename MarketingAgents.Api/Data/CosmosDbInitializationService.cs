// Copyright (c) Marketing Agents. All rights reserved.

using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Service to initialize Cosmos DB database and containers in development mode.
/// </summary>
public class CosmosDbInitializationService
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly ILogger<CosmosDbInitializationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbInitializationService"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="logger">The logger.</param>
    public CosmosDbInitializationService(
        CosmosClient cosmosClient,
        string databaseName,
        ILogger<CosmosDbInitializationService> logger)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the database and all required containers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing Cosmos DB database: {DatabaseName}", _databaseName);

        // Create database if it doesn't exist
        var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(
            _databaseName,
            cancellationToken: cancellationToken);

        var database = databaseResponse.Database;

        // Define container configurations
        var containerConfigs = new[]
        {
            new ContainerConfig("Campaigns", "/partitionKey"),
            new ContainerConfig("Artifacts", "/partitionKey"),
            new ContainerConfig("ArtifactVersions", "/partitionKey"),
            new ContainerConfig("AuditReports", "/partitionKey"),
            new ContainerConfig("IterationLogs", "/partitionKey"),
            new ContainerConfig("OrchestrationRuns", "/partitionKey"),
        };

        // Create containers
        foreach (var config in containerConfigs)
        {
            await CreateContainerIfNotExistsAsync(database, config, cancellationToken);
        }

        _logger.LogInformation("Cosmos DB initialization completed successfully");
    }

    private async Task CreateContainerIfNotExistsAsync(
        Database database,
        ContainerConfig config,
        CancellationToken cancellationToken)
    {
        var containerProperties = new ContainerProperties
        {
            Id = config.ContainerName,
            PartitionKeyPath = config.PartitionKeyPath,
        };

        // Configure indexing policy for optimal query performance
        containerProperties.IndexingPolicy = new IndexingPolicy
        {
            Automatic = true,
            IndexingMode = IndexingMode.Consistent,
        };

        _logger.LogInformation(
            "Creating container: {ContainerName} with partition key: {PartitionKey}",
            config.ContainerName,
            config.PartitionKeyPath);

        await database.CreateContainerIfNotExistsAsync(
            containerProperties,
            throughput: 400, // Manual throughput for dev
            cancellationToken: cancellationToken);
    }

    private record ContainerConfig(string ContainerName, string PartitionKeyPath);
}
