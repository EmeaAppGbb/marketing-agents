// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the campaign brief containing all input requirements.
/// </summary>
public record CampaignBrief
{
    /// <summary>
    /// Gets the campaign objective or goal.
    /// </summary>
    public required string Objective { get; init; }

    /// <summary>
    /// Gets the target audience description.
    /// </summary>
    public required string TargetAudience { get; init; }

    /// <summary>
    /// Gets the product or service details.
    /// </summary>
    public required string ProductDetails { get; init; }

    /// <summary>
    /// Gets optional tone and style guidelines.
    /// </summary>
    public string[]? ToneGuidelines { get; init; }

    /// <summary>
    /// Gets optional brand color palette (hex values).
    /// </summary>
    public string[]? BrandPalette { get; init; }

    /// <summary>
    /// Gets optional campaign constraints.
    /// </summary>
    public CampaignConstraints? Constraints { get; init; }
}
