// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents an audit score for a specific category.
/// </summary>
public record AuditScore
{
    /// <summary>
    /// Gets the category name.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the score value (0-100).
    /// </summary>
    public required int Score { get; init; }

    /// <summary>
    /// Gets the status for this category.
    /// </summary>
    public required AuditStatus Status { get; init; }

    /// <summary>
    /// Gets additional notes or comments.
    /// </summary>
    public string? Notes { get; init; }
}
