// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the iteration log repository.
/// </summary>
public class IterationLogRepository : IIterationLogRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="IterationLogRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    public IterationLogRepository(CosmosClient cosmosClient, string databaseName)
    {
        _container = cosmosClient.GetContainer(databaseName, "IterationLogs");
    }

    /// <inheritdoc/>
    public async Task<IterationLog> CreateAsync(IterationLog log, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            log,
            new PartitionKey(log.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<List<IterationLog>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.CampaignId = @campaignId ORDER BY c.CreatedAt DESC")
            .WithParameter("@campaignId", campaignId);

        var results = new List<IterationLog>();
        using var feed = _container.GetItemQueryIterator<IterationLog>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(campaignId),
            });

        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<IterationLog>> GetByArtifactIdAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.ArtifactId = @artifactId ORDER BY c.CreatedAt DESC")
            .WithParameter("@artifactId", artifactId);

        var results = new List<IterationLog>();
        using var feed = _container.GetItemQueryIterator<IterationLog>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(campaignId),
            });

        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }
}
