// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json.Serialization;

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents an orchestration run tracking agent execution.
/// </summary>
public record OrchestrationRun
{
    /// <summary>
    /// Gets the unique identifier for the orchestration run.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets the campaign ID (partition key).
    /// </summary>
    public required string CampaignId { get; init; }

    /// <summary>
    /// Gets the execution mode.
    /// </summary>
    public required ExecutionMode ExecutionMode { get; init; }

    /// <summary>
    /// Gets the run status.
    /// </summary>
    public required OrchestrationRunStatus Status { get; init; }

    /// <summary>
    /// Gets the run start timestamp.
    /// </summary>
    public required DateTimeOffset StartedAt { get; init; }

    /// <summary>
    /// Gets the run completion timestamp.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Gets the artifact version IDs generated in this run.
    /// </summary>
    public required string[] ArtifactVersionIds { get; init; }

    /// <summary>
    /// Gets the error message if the run failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the partition key for Cosmos DB (using campaign ID).
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey => CampaignId;
}
