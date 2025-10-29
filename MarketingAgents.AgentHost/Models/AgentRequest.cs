// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models;

/// <summary>
/// Base request model for agent execution.
/// </summary>
public record AgentRequest
{
    /// <summary>
    /// Gets the correlation ID for tracking the request across agents.
    /// </summary>
    public required string CorrelationId { get; init; }

    /// <summary>
    /// Gets the user input or prompt for the agent.
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets optional metadata for the request.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Gets the cancellation token for the operation.
    /// </summary>
    public CancellationToken CancellationToken { get; init; } = default;
}
