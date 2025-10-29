// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models.Configuration;

/// <summary>
/// Configuration settings for agent orchestration patterns.
/// </summary>
public sealed record OrchestrationConfiguration
{
    /// <summary>
    /// Gets or initializes the default execution mode (Parallel or Sequential).
    /// </summary>
    public ExecutionMode DefaultExecutionMode { get; init; } = ExecutionMode.Parallel;

    /// <summary>
    /// Gets or initializes the maximum concurrent agent executions.
    /// Only applies to parallel execution mode.
    /// </summary>
    public int MaxConcurrentExecutions { get; init; } = 10;

    /// <summary>
    /// Gets or initializes the maximum wait time for agent completion in seconds.
    /// </summary>
    public int MaxWaitTimeSeconds { get; init; } = 300;

    /// <summary>
    /// Gets a value indicating whether to continue on partial failures.
    /// When true, orchestration continues even if some agents fail.
    /// </summary>
    public bool ContinueOnPartialFailure { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to enable real-time status events.
    /// </summary>
    public bool EnableRealtimeEvents { get; init; } = true;
}

/// <summary>
/// Defines the execution mode for agent orchestration.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// Execute agents sequentially, one after another.
    /// </summary>
    Sequential,

    /// <summary>
    /// Execute agents in parallel concurrently.
    /// </summary>
    Parallel,
}
