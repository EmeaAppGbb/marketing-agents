// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.Api.Models;

/// <summary>
/// Represents the current status of a campaign.
/// </summary>
public enum CampaignStatus
{
    /// <summary>
    /// Campaign is in draft state.
    /// </summary>
    Draft,

    /// <summary>
    /// Campaign artifacts are being generated.
    /// </summary>
    Generating,

    /// <summary>
    /// Campaign artifacts have been generated.
    /// </summary>
    Generated,

    /// <summary>
    /// Campaign artifacts are being audited.
    /// </summary>
    Auditing,

    /// <summary>
    /// Campaign is completed with audited artifacts.
    /// </summary>
    Completed,

    /// <summary>
    /// Campaign has been cancelled.
    /// </summary>
    Cancelled,
}
