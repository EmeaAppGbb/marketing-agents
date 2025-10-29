// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for campaign operations.
/// </summary>
public interface ICampaignRepository
{
    /// <summary>
    /// Creates a new campaign.
    /// </summary>
    /// <param name="campaign">The campaign to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created campaign.</returns>
    Task<Campaign> CreateAsync(Campaign campaign, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a campaign by its ID.
    /// </summary>
    /// <param name="id">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The campaign if found; otherwise, null.</returns>
    Task<Campaign?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing campaign.
    /// </summary>
    /// <param name="campaign">The campaign to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated campaign.</returns>
    Task<Campaign> UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a campaign by marking it as deleted.
    /// </summary>
    /// <param name="id">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a complete campaign snapshot including active versions and audit statuses.
    /// </summary>
    /// <param name="id">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The campaign snapshot if found; otherwise, null.</returns>
    Task<CampaignSnapshot?> GetCampaignSnapshotAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a complete campaign snapshot with related data.
/// </summary>
public record CampaignSnapshot
{
    /// <summary>
    /// Gets the campaign.
    /// </summary>
    public required Campaign Campaign { get; init; }

    /// <summary>
    /// Gets the active artifact versions.
    /// </summary>
    public required Dictionary<ArtifactType, ArtifactVersion> ActiveVersions { get; init; }

    /// <summary>
    /// Gets the audit reports for active versions.
    /// </summary>
    public required Dictionary<string, AuditReport> AuditReports { get; init; }
}
