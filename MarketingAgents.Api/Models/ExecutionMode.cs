// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the execution mode for orchestration runs.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// Execute agents in parallel.
    /// </summary>
    Parallel,

    /// <summary>
    /// Execute agents sequentially.
    /// </summary>
    Sequential,
}
