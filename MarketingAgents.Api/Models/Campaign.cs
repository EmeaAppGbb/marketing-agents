// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents a marketing campaign with all associated metadata.
/// </summary>
public record Campaign
{
    /// <summary>
    /// Gets the unique identifier for the campaign.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the campaign name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the campaign brief with all input requirements.
    /// </summary>
    public required CampaignBrief Brief { get; init; }

    /// <summary>
    /// Gets the campaign creation timestamp.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the campaign last update timestamp.
    /// </summary>
    public required DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Gets the current campaign status.
    /// </summary>
    public required CampaignStatus Status { get; init; }

    /// <summary>
    /// Gets the mapping of artifact types to their active version IDs.
    /// </summary>
    public Dictionary<ArtifactType, string>? ActiveVersionIds { get; init; }

    /// <summary>
    /// Gets a value indicating whether the campaign has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => Id;
}
