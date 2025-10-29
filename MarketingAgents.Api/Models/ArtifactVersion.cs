// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents a specific version of an artifact.
/// </summary>
public record ArtifactVersion
{
    /// <summary>
    /// Gets the unique identifier for the version.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the artifact ID this version belongs to.
    /// </summary>
    public required string ArtifactId { get; init; }

    /// <summary>
    /// Gets the campaign ID (partition key).
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// Gets the version number (1-based sequence).
    /// </summary>
    public required int VersionNumber { get; init; }

    /// <summary>
    /// Gets the artifact content (JSON serialized).
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets the version creation timestamp.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets the version status.
    /// </summary>
    public required ArtifactVersionStatus Status { get; init; }

    /// <summary>
    /// Gets the audit report ID if this version has been audited.
    /// </summary>
    public string? AuditReportId { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
