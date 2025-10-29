// Copyright (c) Marketing Agents. All rights reserved.

using System.ComponentModel;

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Tool for validating headline character length against best practices.
/// </summary>
public static class ValidateHeadlineLengthTool
{
    /// <summary>
    /// Validates headline character count against best practices (50-60 characters).
    /// </summary>
    /// <param name="headline">The headline text to validate.</param>
    /// <returns>Validation result with recommendations.</returns>
    [Description("Validates headline character count against best practices (50-60 chars optimal)")]
    public static string ValidateHeadlineLength(
        [Description("The headline text to validate")] string headline)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headline);

        var length = headline.Length;

        return length switch
        {
            < 30 => $"Headline is too short ({length} chars). Consider expanding to 50-60 characters for better impact and SEO.",
            >= 30 and <= 45 => $"Headline is acceptable ({length} chars) but could be longer. Optimal range is 50-60 characters.",
            >= 46 and <= 60 => $"Headline length is optimal ({length} chars). Perfect for readability and engagement.",
            >= 61 and <= 70 => $"Headline is slightly long ({length} chars). Consider condensing to 50-60 characters for better readability.",
            _ => $"Headline is too long ({length} chars). Strongly recommend shortening to 50-60 characters to avoid truncation.",
        };
    }
}
