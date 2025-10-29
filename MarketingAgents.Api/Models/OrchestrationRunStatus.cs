// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the status of an orchestration run.
/// </summary>
public enum OrchestrationRunStatus
{
    /// <summary>
    /// Run is queued and waiting to execute.
    /// </summary>
    Queued,

    /// <summary>
    /// Run is currently executing.
    /// </summary>
    Running,

    /// <summary>
    /// Run has completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Run has failed with errors.
    /// </summary>
    Failed,

    /// <summary>
    /// Run has been cancelled.
    /// </summary>
    Cancelled,
}
