// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents a flagged item from the audit.
/// </summary>
public record FlaggedItem
{
    /// <summary>
    /// Gets the category of the flagged item.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets the severity level.
    /// </summary>
    public required string Severity { get; init; }

    /// <summary>
    /// Gets the description of the issue.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the location or context where the issue was found.
    /// </summary>
    public string? Location { get; init; }
}
