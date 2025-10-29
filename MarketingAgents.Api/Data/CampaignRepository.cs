// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using MarketingAgents.Api.Models;
using Microsoft.Azure.Cosmos;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Cosmos DB implementation of the campaign repository.
/// </summary>
public class CampaignRepository : ICampaignRepository
{
    private readonly Container _container;
    private readonly IVersionRepository _versionRepository;
    private readonly IAuditReportRepository _auditReportRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CampaignRepository"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="versionRepository">The version repository.</param>
    /// <param name="auditReportRepository">The audit report repository.</param>
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

    /// <inheritdoc/>
    public async Task<Campaign> CreateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        var response = await _container.CreateItemAsync(
            campaign,
            new PartitionKey(campaign.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task<Campaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<Campaign>(
                id,
                new PartitionKey(id),
                cancellationToken: cancellationToken);

            return response.Resource.IsDeleted ? null : response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<Campaign> UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        var response = await _container.ReplaceItemAsync(
            campaign,
            campaign.Id,
            new PartitionKey(campaign.PartitionKey),
            cancellationToken: cancellationToken);

        return response.Resource;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await GetByIdAsync(id, cancellationToken);
        if (campaign is null)
        {
            return;
        }

        var updatedCampaign = campaign with
        {
            IsDeleted = true,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        await UpdateAsync(updatedCampaign, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CampaignSnapshot?> GetCampaignSnapshotAsync(string id, CancellationToken cancellationToken = default)
    {
        var campaign = await GetByIdAsync(id, cancellationToken);
        if (campaign is null)
        {
            return null;
        }

        var activeVersions = new Dictionary<ArtifactType, ArtifactVersion>();
        var auditReports = new Dictionary<string, AuditReport>();

        if (campaign.ActiveVersionIds is not null)
        {
            foreach (var (artifactType, versionId) in campaign.ActiveVersionIds)
            {
                var version = await _versionRepository.GetByIdAsync(versionId, campaign.Id, cancellationToken);
                if (version is not null)
                {
                    activeVersions[artifactType] = version;

                    if (version.AuditReportId is not null)
                    {
                        var auditReport = await _auditReportRepository.GetByVersionIdAsync(
                            version.Id,
                            campaign.Id,
                            cancellationToken);

                        if (auditReport is not null)
                        {
                            auditReports[version.Id] = auditReport;
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
}
