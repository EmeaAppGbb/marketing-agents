// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the artifact repository.
/// </summary>
public class ArtifactRepository : IArtifactRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArtifactRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    public ArtifactRepository(CosmosClient cosmosClient, string databaseName)
    {
        _container = cosmosClient.GetContainer(databaseName, "Artifacts");
    }

    /// <inheritdoc/>
    public async Task<Artifact> CreateAsync(Artifact artifact, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            artifact,
            new PartitionKey(artifact.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<Artifact?> GetByIdAsync(string id, string campaignId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<Artifact>(
                id,
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
    public async Task<List<Artifact>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.CampaignId = @campaignId")
            .WithParameter("@campaignId", campaignId);

        var results = new List<Artifact>();
        using var feed = _container.GetItemQueryIterator<Artifact>(
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
    public async Task<List<ArtifactVersion>> GetVersionHistoryAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.ArtifactId = @artifactId ORDER BY c.VersionNumber DESC")
            .WithParameter("@artifactId", artifactId);

        var results = new List<ArtifactVersion>();
        using var feed = _container.GetItemQueryIterator<ArtifactVersion>(
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
