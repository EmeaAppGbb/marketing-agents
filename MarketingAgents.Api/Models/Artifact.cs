// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents a campaign artifact (copy, short copy, or visual concept).
/// </summary>
public record Artifact
{
    /// <summary>
    /// Gets the unique identifier for the artifact.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the campaign ID this artifact belongs to (partition key).
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// Gets the artifact type.
    /// </summary>
    public required ArtifactType Type { get; init; }

    /// <summary>
    /// Gets the artifact creation timestamp.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the list of version IDs for this artifact.
    /// </summary>
    public required string[] VersionIds { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
