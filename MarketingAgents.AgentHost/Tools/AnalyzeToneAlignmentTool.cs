// Copyright (c) Marketing Agents. All rights reserved.

using System.ComponentModel;

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Tool for analyzing tone alignment of generated copy against provided guidelines.
/// </summary>
public static class AnalyzeToneAlignmentTool
{
    // Tone keyword indicators mapped to tone types
    private static readonly Dictionary<string, string[]> ToneIndicators = new(StringComparer.OrdinalIgnoreCase)
    {
        ["professional"] = ["expertise", "solution", "deliver", "optimize", "enhance", "strategic"],
        ["playful"] = ["fun", "enjoy", "love", "exciting", "amazing", "awesome"],
        ["empathetic"] = ["understand", "care", "help", "support", "together", "you"],
        ["urgent"] = ["now", "today", "limited", "hurry", "don't miss", "act fast"],
        ["innovative"] = ["new", "cutting-edge", "revolutionary", "breakthrough", "advanced", "modern"],
        ["trustworthy"] = ["proven", "reliable", "trusted", "secure", "guarantee", "certified"],
    };

    /// <summary>
    /// Analyzes generated copy against tone guidelines to assess tone adherence.
    /// </summary>
    /// <param name="copyText">The copy text to analyze.</param>
    /// <param name="targetTone">The target tone to match (e.g., professional, playful).</param>
    /// <returns>Tone adherence assessment with score and qualitative feedback.</returns>
    [Description("Analyzes copy against tone guidelines and returns tone adherence score")]
    public static string AnalyzeToneAlignment(
        [Description("The copy text to analyze")] string copyText,
        [Description("The target tone to match (e.g., professional, playful, empathetic)")] string targetTone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(copyText);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetTone);

        var lowerText = copyText.ToLowerInvariant();
        var normalizedTone = targetTone.ToLowerInvariant().Trim();

        if (!ToneIndicators.TryGetValue(normalizedTone, out var indicators))
        {
            return $"Unknown tone '{targetTone}'. Supported tones: {string.Join(", ", ToneIndicators.Keys)}";
        }

        var matchCount = indicators.Count(indicator => lowerText.Contains(indicator, StringComparison.OrdinalIgnoreCase));
        var totalIndicators = indicators.Length;
        var adherencePercentage = (matchCount / (double)totalIndicators) * 100;

        var assessment = adherencePercentage switch
        {
            >= 70 => "Excellent",
            >= 50 => "Good",
            >= 30 => "Moderate",
            _ => "Weak",
        };

        var matchedKeywords = indicators.Where(i => lowerText.Contains(i, StringComparison.OrdinalIgnoreCase)).ToList();

        return $"Tone Adherence: {assessment} ({adherencePercentage:F1}%). " +
               $"Matched {matchCount}/{totalIndicators} tone indicators for '{targetTone}'. " +
               $"Matched keywords: {(matchedKeywords.Count > 0 ? string.Join(", ", matchedKeywords) : "none")}.";
    }
}
