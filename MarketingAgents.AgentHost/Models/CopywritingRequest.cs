// Copyright (c) Marketing Agents. All rights reserved.

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly

namespace MarketingAgents.AgentHost.Models;

/// <summary>
/// Request model for the Copywriting Agent.
/// </summary>
public sealed record CopywritingRequest
{
    /// <summary>
    /// Gets the campaign brief describing the product, target audience, objectives, and context.
    /// </summary>
    public required string CampaignBrief { get; init; }

    /// <summary>
    /// Gets the tone guidelines for content generation (e.g., "professional", "playful", "empathetic").
    /// </summary>
    public string[]? ToneGuidelines { get; init; }

    /// <summary>
    /// Gets the preferred body copy length tiers to generate.
    /// </summary>
    public LengthTier[]? LengthPreferences { get; init; }

    /// <summary>
    /// Gets the feedback from previous iteration for regeneration.
    /// </summary>
    public string? RevisionFeedback { get; init; }

    /// <summary>
    /// Gets the previous version of the response for context during regeneration.
    /// </summary>
    public CopywritingResponse? PreviousVersion { get; init; }
}

/// <summary>
/// Enumeration for body copy length preferences.
/// </summary>
public enum LengthTier
{
    /// <summary>
    /// Short body copy (50-100 words).
    /// </summary>
    Short,

    /// <summary>
    /// Medium body copy (100-200 words).
    /// </summary>
    Medium,

    /// <summary>
    /// Long body copy (200-400 words).
    /// </summary>
    Long,
}
