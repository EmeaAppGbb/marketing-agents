// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents an audit recommendation.
/// </summary>
public record Recommendation
{
    /// <summary>
    /// Gets the recommendation category.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the recommendation text.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Gets the priority level.
    /// </summary>
    public required string Priority { get; init; }
}
