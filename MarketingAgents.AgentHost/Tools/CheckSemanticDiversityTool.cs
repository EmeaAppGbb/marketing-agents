// Copyright (c) Marketing Agents. All rights reserved.

using System.ComponentModel;

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Tool for checking semantic diversity among headline variants.
/// </summary>
public static class CheckSemanticDiversityTool
{
    /// <summary>
    /// Analyzes headline variants for semantic similarity using n-gram overlap heuristic.
    /// </summary>
    /// <param name="headlines">Comma-separated list of headlines to analyze.</param>
    /// <returns>Diversity score and suggestions for improvement.</returns>
    [Description("Analyzes headline variants for semantic diversity using n-gram overlap")]
    public static string CheckSemanticDiversity(
        [Description("Comma-separated list of headlines to analyze")] string headlines)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(headlines);

        var headlineList = headlines.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (headlineList.Length < 2)
        {
            return "Need at least 2 headlines to check diversity.";
        }

        var totalComparisons = 0;
        var totalSimilarity = 0.0;

        for (var i = 0; i < headlineList.Length - 1; i++)
        {
            for (var j = i + 1; j < headlineList.Length; j++)
            {
                var similarity = CalculateBigramSimilarity(headlineList[i], headlineList[j]);
                totalSimilarity += similarity;
                totalComparisons++;
            }
        }

        var averageSimilarity = totalComparisons > 0 ? totalSimilarity / totalComparisons : 0;
        var diversityScore = 1.0 - averageSimilarity; // Higher diversity = lower similarity

        var assessment = diversityScore switch
        {
            >= 0.7 => "Excellent diversity",
            >= 0.5 => "Good diversity",
            >= 0.3 => "Moderate diversity",
            _ => "Low diversity - headlines are too similar",
        };

        var recommendation = diversityScore < 0.6
            ? " Consider using different word choices, varying sentence structures, or emphasizing different benefits."
            : string.Empty;

        return $"Diversity Score: {diversityScore:F2} ({assessment}).{recommendation}";
    }

    /// <summary>
    /// Calculates bigram similarity between two strings.
    /// </summary>
    private static double CalculateBigramSimilarity(string text1, string text2)
    {
        var bigrams1 = GetBigrams(text1.ToLowerInvariant());
        var bigrams2 = GetBigrams(text2.ToLowerInvariant());

        if (bigrams1.Count == 0 || bigrams2.Count == 0)
        {
            return 0;
        }

        var intersection = bigrams1.Intersect(bigrams2).Count();
        var union = bigrams1.Union(bigrams2).Count();

        return union > 0 ? intersection / (double)union : 0;
    }

    /// <summary>
    /// Extracts bigrams from a string.
    /// </summary>
    private static HashSet<string> GetBigrams(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var bigrams = new HashSet<string>();

        for (var i = 0; i < words.Length - 1; i++)
        {
            bigrams.Add($"{words[i]} {words[i + 1]}");
        }

        return bigrams;
    }
}
