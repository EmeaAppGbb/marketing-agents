// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models;

/// <summary>
/// Response model from the Copywriting Agent containing generated campaign copy.
/// </summary>
public sealed record CopywritingResponse
{
    /// <summary>
    /// Gets the headline options (minimum 3 distinct variations).
    /// </summary>
    public required string[] Headlines { get; init; }

    /// <summary>
    /// Gets the short body copy (50-100 words).
    /// </summary>
    public string? BodyCopyShort { get; init; }

    /// <summary>
    /// Gets the medium body copy (100-200 words).
    /// </summary>
    public string? BodyCopyMedium { get; init; }

    /// <summary>
    /// Gets the long body copy (200-400 words).
    /// </summary>
    public string? BodyCopyLong { get; init; }

    /// <summary>
    /// Gets the CTA (Call To Action) suggestions (minimum 3 variations).
    /// </summary>
    public required string[] CTAs { get; init; }

    /// <summary>
    /// Gets the tone adherence metadata providing qualitative assessment.
    /// </summary>
    public string? ToneAdherenceMetadata { get; init; }

    /// <summary>
    /// Gets the timestamp when the response was generated.
    /// </summary>
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;
}
