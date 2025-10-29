// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models.Configuration;

/// <summary>
/// Configuration settings for Azure OpenAI models.
/// </summary>
public sealed record OpenAIConfiguration
{
    /// <summary>
    /// Gets or initializes the Azure OpenAI endpoint URL.
    /// </summary>
    public string? Endpoint { get; init; }

    /// <summary>
    /// Gets or initializes the deployment name for the primary chat model (e.g., gpt-4o).
    /// </summary>
    public string PrimaryChatModel { get; init; } = "gpt-4o";

    /// <summary>
    /// Gets or initializes the deployment name for the lightweight chat model (e.g., gpt-4o-mini).
    /// </summary>
    public string LightweightChatModel { get; init; } = "gpt-4o-mini";

    /// <summary>
    /// Gets or initializes the API version for Azure OpenAI.
    /// </summary>
    public string ApiVersion { get; init; } = "2024-08-06";

    /// <summary>
    /// Gets or initializes the maximum tokens per minute rate limit.
    /// Used for circuit breaker configuration.
    /// </summary>
    public int MaxTokensPerMinute { get; init; } = 150000;

    /// <summary>
    /// Gets or initializes the maximum requests per minute rate limit.
    /// Used for circuit breaker configuration.
    /// </summary>
    public int MaxRequestsPerMinute { get; init; } = 1000;
}
