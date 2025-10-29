// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents optional constraints for a campaign brief.
/// </summary>
public record CampaignConstraints
{
    /// <summary>
    /// Gets the maximum word count for copy artifacts.
    /// </summary>
    public int? MaxWordCount { get; init; }

    /// <summary>
    /// Gets the minimum word count for copy artifacts.
    /// </summary>
    public int? MinWordCount { get; init; }

    /// <summary>
    /// Gets restricted keywords that should not appear in content.
    /// </summary>
    public string[]? RestrictedKeywords { get; init; }

    /// <summary>
    /// Gets required keywords that must appear in content.
    /// </summary>
    public string[]? RequiredKeywords { get; init; }

    /// <summary>
    /// Gets the content rating or maturity level.
    /// </summary>
    public string? ContentRating { get; init; }
}
