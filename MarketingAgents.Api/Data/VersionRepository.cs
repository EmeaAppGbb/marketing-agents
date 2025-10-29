// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the version repository.
/// </summary>
public class VersionRepository : IVersionRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    public VersionRepository(CosmosClient cosmosClient, string databaseName)
    {
        _container = cosmosClient.GetContainer(databaseName, "ArtifactVersions");
    }

    /// <inheritdoc/>
    public async Task<ArtifactVersion> CreateAsync(ArtifactVersion version, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            version,
            new PartitionKey(version.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<ArtifactVersion?> GetByIdAsync(string versionId, string campaignId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<ArtifactVersion>(
                versionId,
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
    public async Task<ArtifactVersion?> GetLatestByArtifactIdAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT TOP 1 * FROM c WHERE c.ArtifactId = @artifactId ORDER BY c.VersionNumber DESC")
            .WithParameter("@artifactId", artifactId);

        using var feed = _container.GetItemQueryIterator<ArtifactVersion>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(campaignId),
            });

        if (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync(cancellationToken);
            return response.FirstOrDefault();
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task ArchiveAsync(string versionId, string campaignId, CancellationToken cancellationToken = default)
    {
        var version = await GetByIdAsync(versionId, campaignId, cancellationToken);
        if (version is null)
        {
            return;
        }

        var updatedVersion = version with
        {
            Status = ArtifactVersionStatus.Archived,
        };

        await UpdateAsync(updatedVersion, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ArtifactVersion> UpdateAsync(ArtifactVersion version, CancellationToken cancellationToken = default)
    {
        var response = await _container.ReplaceItemAsync(
            version,
            version.Id,
            new PartitionKey(version.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }
}
