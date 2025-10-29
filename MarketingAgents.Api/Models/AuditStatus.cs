// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the overall audit status.
/// </summary>
public enum AuditStatus
{
    /// <summary>
    /// Audit passed all checks.
    /// </summary>
    Pass,

    /// <summary>
    /// Audit passed with conditions or warnings.
    /// </summary>
    Conditional,

    /// <summary>
    /// Audit failed compliance checks.
    /// </summary>
    Fail,
}
