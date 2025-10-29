// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Orchestration;

/// <summary>
/// Result of orchestrated campaign generation.
/// </summary>
public sealed record OrchestrationResult
{
    /// <summary>
    /// Gets the unique run identifier for this orchestration.
    /// </summary>
    public required string RunId { get; init; }

    /// <summary>
    /// Gets the campaign brief ID that was orchestrated.
    /// </summary>
    public required string CampaignBriefId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the orchestration succeeded.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the generated artifacts by agent name.
    /// </summary>
    public Dictionary<string, object>? Artifacts { get; init; }

    /// <summary>
    /// Gets the lifecycle events emitted during orchestration.
    /// </summary>
    public List<OrchestrationEvent>? Events { get; init; }

    /// <summary>
    /// Gets error information if the orchestration failed or partially failed.
    /// </summary>
    public Dictionary<string, string>? Errors { get; init; }

    /// <summary>
    /// Gets metadata about the orchestration execution.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Gets the execution mode used (Parallel or Sequential).
    /// </summary>
    public required string ExecutionMode { get; init; }

    /// <summary>
    /// Gets the total execution duration in milliseconds.
    /// </summary>
    public long DurationMs { get; init; }
}

/// <summary>
/// Event emitted during orchestration lifecycle.
/// </summary>
public sealed record OrchestrationEvent
{
    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the agent name that emitted the event.
    /// </summary>
    public required string AgentName { get; init; }

    /// <summary>
    /// Gets the event type (e.g., Queued, Generating, Generated, Auditing, Completed, Failed).
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// Gets optional event message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets optional event metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}
