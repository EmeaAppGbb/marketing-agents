// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models.Configuration;

/// <summary>
/// Shared configuration settings for all agents.
/// </summary>
public sealed record AgentConfiguration
{
    /// <summary>
    /// Gets or initializes the default temperature for agent responses (0.0-2.0).
    /// Lower values make output more deterministic.
    /// </summary>
    public double DefaultTemperature { get; init; } = 0.7;

    /// <summary>
    /// Gets or initializes the maximum tokens for agent responses.
    /// </summary>
    public int MaxTokens { get; init; } = 4000;

    /// <summary>
    /// Gets or initializes the maximum retry attempts for agent operations.
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 5;

    /// <summary>
    /// Gets or initializes the initial retry delay in milliseconds.
    /// </summary>
    public int InitialRetryDelayMs { get; init; } = 1000;

    /// <summary>
    /// Gets or initializes the retry backoff multiplier (exponential backoff).
    /// </summary>
    public double RetryBackoffMultiplier { get; init; } = 2.0;

    /// <summary>
    /// Gets or initializes the timeout for agent execution in seconds.
    /// </summary>
    public int ExecutionTimeoutSeconds { get; init; } = 120;

    /// <summary>
    /// Gets or initializes a value indicating whether to enable detailed telemetry.
    /// </summary>
    public bool EnableDetailedTelemetry { get; init; } = true;
}
