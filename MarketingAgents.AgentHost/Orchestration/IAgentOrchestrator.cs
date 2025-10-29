// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Models.Configuration;

namespace MarketingAgents.AgentHost.Orchestration;

/// <summary>
/// Interface for agent orchestration service.
/// </summary>
public interface IAgentOrchestrator
{
    /// <summary>
    /// Orchestrates campaign generation across all agents.
    /// </summary>
    /// <param name="brief">The campaign brief.</param>
    /// <param name="mode">The execution mode (parallel or sequential).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The orchestration result with all generated artifacts.</returns>
    Task<OrchestrationResult> OrchestrateCampaignAsync(
        CampaignBrief brief,
        ExecutionMode mode,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Campaign brief for agent orchestration.
/// </summary>
public sealed record CampaignBrief
{
    /// <summary>
    /// Gets the unique identifier for this campaign brief.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the campaign objective or theme.
    /// </summary>
    public required string Objective { get; init; }

    /// <summary>
    /// Gets the target audience description.
    /// </summary>
    public required string TargetAudience { get; init; }

    /// <summary>
    /// Gets the product or service being promoted.
    /// </summary>
    public required string Product { get; init; }

    /// <summary>
    /// Gets the desired tone (e.g., professional, casual, humorous).
    /// </summary>
    public required string Tone { get; init; }

    /// <summary>
    /// Gets optional brand guidelines.
    /// </summary>
    public string? BrandGuidelines { get; init; }

    /// <summary>
    /// Gets optional prohibited terms or constraints.
    /// </summary>
    public List<string>? ProhibitedTerms { get; init; }

    /// <summary>
    /// Gets optional previous iteration feedback for refinement.
    /// </summary>
    public string? RevisionFeedback { get; init; }
}
