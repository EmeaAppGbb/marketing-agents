// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for iteration log operations.
/// </summary>
public interface IIterationLogRepository
{
    /// <summary>
    /// Creates a new iteration log entry.
    /// </summary>
    /// <param name="log">The iteration log to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created iteration log.</returns>
    Task<IterationLog> CreateAsync(IterationLog log, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all iteration logs for a campaign.
    /// </summary>
    /// <param name="campaignId">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of iteration logs.</returns>
    Task<List<IterationLog>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all iteration logs for an artifact.
    /// </summary>
    /// <param name="artifactId">The artifact ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of iteration logs.</returns>
    Task<List<IterationLog>> GetByArtifactIdAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default);
}
