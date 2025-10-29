// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for artifact operations.
/// </summary>
public interface IArtifactRepository
{
    /// <summary>
    /// Creates a new artifact.
    /// </summary>
    /// <param name="artifact">The artifact to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created artifact.</returns>
    Task<Artifact> CreateAsync(Artifact artifact, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an artifact by its ID.
    /// </summary>
    /// <param name="id">The artifact ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The artifact if found; otherwise, null.</returns>
    Task<Artifact?> GetByIdAsync(string id, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all artifacts for a campaign.
    /// </summary>
    /// <param name="campaignId">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of artifacts.</returns>
    Task<List<Artifact>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the version history for an artifact.
    /// </summary>
    /// <param name="artifactId">The artifact ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of version metadata.</returns>
    Task<List<ArtifactVersion>> GetVersionHistoryAsync(string artifactId, string campaignId, CancellationToken cancellationToken = default);
}
