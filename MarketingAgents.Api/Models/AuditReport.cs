// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents an audit report for an artifact version.
/// </summary>
public record AuditReport
{
    /// <summary>
    /// Gets the unique identifier for the audit report.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the version ID this report audits.
    /// </summary>
    public required string VersionId { get; init; }

    /// <summary>
    /// Gets the campaign ID (partition key).
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// Gets the overall audit status.
    /// </summary>
    public required AuditStatus OverallStatus { get; init; }

    /// <summary>
    /// Gets the audit scores by category.
    /// </summary>
    public required Dictionary<string, AuditScore> CategoryScores { get; init; }

    /// <summary>
    /// Gets the list of flagged items.
    /// </summary>
    public required FlaggedItem[] FlaggedItems { get; init; }

    /// <summary>
    /// Gets the list of recommendations.
    /// </summary>
    public required Recommendation[] Recommendations { get; init; }

    /// <summary>
    /// Gets the audit creation timestamp.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
