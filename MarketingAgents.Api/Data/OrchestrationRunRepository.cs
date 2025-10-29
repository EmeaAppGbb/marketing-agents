// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the orchestration run repository.
/// </summary>
public class OrchestrationRunRepository : IOrchestrationRunRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrchestrationRunRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    public OrchestrationRunRepository(CosmosClient cosmosClient, string databaseName)
    {
        _container = cosmosClient.GetContainer(databaseName, "OrchestrationRuns");
    }

    /// <inheritdoc/>
    public async Task<OrchestrationRun> CreateAsync(OrchestrationRun run, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            run,
            new PartitionKey(run.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<OrchestrationRun?> GetByIdAsync(string runId, string campaignId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<OrchestrationRun>(
                runId,
                new PartitionKey(campaignId),
                cancellationToken: cancellationToken);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<OrchestrationRun> UpdateAsync(OrchestrationRun run, CancellationToken cancellationToken = default)
    {
        var response = await _container.ReplaceItemAsync(
            run,
            run.Id,
            new PartitionKey(run.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<List<OrchestrationRun>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.CampaignId = @campaignId ORDER BY c.StartedAt DESC")
            .WithParameter("@campaignId", campaignId);

        var results = new List<OrchestrationRun>();
        using var feed = _container.GetItemQueryIterator<OrchestrationRun>(
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
