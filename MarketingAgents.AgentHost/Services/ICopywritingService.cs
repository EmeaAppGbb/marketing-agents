// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Models;

namespace MarketingAgents.AgentHost.Services;

/// <summary>
/// Service interface for copywriting operations.
/// </summary>
public interface ICopywritingService
{
    /// <summary>
    /// Generates new copywriting content based on the campaign brief.
    /// </summary>
    /// <param name="request">The copywriting request containing campaign brief and preferences.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated copywriting response.</returns>
    Task<CopywritingResponse> GenerateAsync(CopywritingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Regenerates copywriting content incorporating revision feedback.
    /// </summary>
    /// <param name="request">The copywriting request with revision feedback and previous version.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The regenerated copywriting response.</returns>
    Task<CopywritingResponse> RegenerateAsync(CopywritingRequest request, CancellationToken cancellationToken = default);
}
