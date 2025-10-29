// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Models;

namespace MarketingAgents.Api.Data;

/// <summary>
/// Repository interface for orchestration run operations.
/// </summary>
public interface IOrchestrationRunRepository
{
    /// <summary>
    /// Creates a new orchestration run.
    /// </summary>
    /// <param name="run">The orchestration run to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created orchestration run.</returns>
    Task<OrchestrationRun> CreateAsync(OrchestrationRun run, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an orchestration run by its ID.
    /// </summary>
    /// <param name="runId">The run ID.</param>
    /// <param name="campaignId">The campaign ID (partition key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The orchestration run if found; otherwise, null.</returns>
    Task<OrchestrationRun?> GetByIdAsync(string runId, string campaignId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing orchestration run.
    /// </summary>
    /// <param name="run">The orchestration run to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated orchestration run.</returns>
    Task<OrchestrationRun> UpdateAsync(OrchestrationRun run, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orchestration runs for a campaign.
    /// </summary>
    /// <param name="campaignId">The campaign ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of orchestration runs.</returns>
    Task<List<OrchestrationRun>> GetByCampaignIdAsync(string campaignId, CancellationToken cancellationToken = default);
}
