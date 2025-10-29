// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for audit report operations.
/// </summary>
public interface IAuditReportRepository
{
    /// <summary>
    /// Creates a new audit report.
    /// </summary>
    /// <param name="report">The audit report to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created audit report.</returns>
    Task<AuditReport> CreateAsync(AuditReport report, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an audit report by version ID.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The audit report if found; otherwise, null.</returns>
    Task<AuditReport?> GetByVersionIdAsync(string versionId, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all audit reports for a campaign.
    /// </summary>
    /// <param name="campaignId">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of audit reports.</returns>
    Task<List<AuditReport>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
}
