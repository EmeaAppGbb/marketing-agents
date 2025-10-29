// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents an iteration log entry tracking feedback and version changes.
/// </summary>
public record IterationLog
{
    /// <summary>
    /// Gets the unique identifier for the iteration log.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the campaign ID (partition key).
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// Gets the artifact ID.
    /// </summary>
    public required string ArtifactId { get; init; }

    /// <summary>
    /// Gets the old version ID (source).
    /// </summary>
    public required string OldVersionId { get; init; }

    /// <summary>
    /// Gets the new version ID (result).
    /// </summary>
    public required string NewVersionId { get; init; }

    /// <summary>
    /// Gets the feedback text explaining the iteration.
    /// </summary>
    public required string FeedbackText { get; init; }

    /// <summary>
    /// Gets optional feedback tags for categorization.
    /// </summary>
    public string[]? FeedbackTags { get; init; }

    /// <summary>
    /// Gets the iteration creation timestamp.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
