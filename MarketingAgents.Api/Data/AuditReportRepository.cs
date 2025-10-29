// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the audit report repository.
/// </summary>
public class AuditReportRepository : IAuditReportRepository
{
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditReportRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    public AuditReportRepository(CosmosClient cosmosClient, string databaseName)
    {
        _container = cosmosClient.GetContainer(databaseName, "AuditReports");
    }

    /// <inheritdoc/>
    public async Task<AuditReport> CreateAsync(AuditReport report, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            report,
            new PartitionKey(report.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<AuditReport?> GetByVersionIdAsync(string versionId, string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.VersionId = @versionId")
            .WithParameter("@versionId", versionId);

        using var feed = _container.GetItemQueryIterator<AuditReport>(
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
    public async Task<List<AuditReport>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.CampaignId = @campaignId")
            .WithParameter("@campaignId", campaignId);

        var results = new List<AuditReport>();
        using var feed = _container.GetItemQueryIterator<AuditReport>(
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
