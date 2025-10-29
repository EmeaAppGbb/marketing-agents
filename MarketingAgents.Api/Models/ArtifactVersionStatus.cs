// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the status of an artifact version.
/// </summary>
public enum ArtifactVersionStatus
{
    /// <summary>
    /// Version is pending generation.
    /// </summary>
    Pending,

    /// <summary>
    /// Version has been generated.
    /// </summary>
    Generated,

    /// <summary>
    /// Version has been audited.
    /// </summary>
    Audited,

    /// <summary>
    /// Version has been approved.
    /// </summary>
    Approved,

    /// <summary>
    /// Version has been archived.
    /// </summary>
    Archived,
}
