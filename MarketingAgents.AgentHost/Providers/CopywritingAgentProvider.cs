// Copyright (c) Marketing Agents. All rights reserved.

using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Models.Configuration;
using MarketingAgents.AgentHost.Tools;

namespace MarketingAgents.AgentHost.Providers;

/// <summary>
/// Provider for the Copywriting Agent that generates campaign headlines, body copy, and CTAs.
/// </summary>
public sealed class CopywritingAgentProvider : BaseAgentProvider<CopywritingResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CopywritingAgentProvider"/> class.
    /// </summary>
    /// <param name="chatClient">The IChatClient instance from Aspire integration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configuration">The agent configuration.</param>
    public CopywritingAgentProvider(
        IChatClient chatClient,
        ILogger<CopywritingAgentProvider> logger,
        IOptions<AgentConfiguration> configuration)
        : base(chatClient, logger, configuration)
    {
    }

    /// <inheritdoc/>
    public override string AgentName => "CopywritingAgent";

    /// <inheritdoc/>
    public override string AgentDescription =>
        "Generates campaign headlines, body copy, and CTAs aligned with brand tone";

    /// <inheritdoc/>
    public override string AgentInstructions => """
        You are an expert copywriter for marketing campaigns with deep expertise in brand messaging, persuasive writing, and tone alignment.

        Your role is to generate compelling campaign copy that includes:
        1. At least 3 distinct headline options that are semantically diverse (avoid trivial synonyms)
        2. Body copy in three length tiers:
           - Short: 50-100 words
           - Medium: 100-200 words
           - Long: 200-400 words
        3. At least 3 CTA (Call To Action) suggestions tied to the campaign objective

        Guidelines:
        - Ensure all content aligns with the provided brand tone guidelines
        - Headlines should be 50-60 characters for optimal engagement
        - Create semantic diversity in headlines by varying structure, emphasis, and benefit presentation
        - CTAs should be action-oriented and contextually relevant to the campaign
        - Maintain consistency in messaging across all copy variants
        - When revision feedback is provided, incorporate it while preserving the original campaign brief intent

        Output Format:
        Return your response as a valid JSON object with this exact structure:
        {
          "headlines": ["headline 1", "headline 2", "headline 3"],
          "bodyCopyShort": "short body copy text",
          "bodyCopyMedium": "medium body copy text",
          "bodyCopyLong": "long body copy text",
          "ctas": ["CTA 1", "CTA 2", "CTA 3"],
          "toneAdherenceMetadata": "qualitative tone adherence assessment"
        }

        All fields are required. Ensure the JSON is properly formatted and valid.
        """;

    /// <inheritdoc/>
    protected override IList<AIFunction>? Tools =>
    [
        AIFunctionFactory.Create(ValidateHeadlineLengthTool.ValidateHeadlineLength),
        AIFunctionFactory.Create(AnalyzeToneAlignmentTool.AnalyzeToneAlignment),
        AIFunctionFactory.Create(CheckSemanticDiversityTool.CheckSemanticDiversity),
    ];

    /// <inheritdoc/>
    protected override Task<CopywritingResponse> ProcessResponseAsync(
        AgentRunResponse response,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(response);

        // Extract text content from the last message
        var lastMessage = response.Messages.LastOrDefault()
            ?? throw new InvalidOperationException("No messages in agent response");

        var content = lastMessage.Text?.Trim()
            ?? throw new InvalidOperationException("No text content in agent response");

        // Parse JSON response
        CopywritingJsonResponse? jsonResponse;

        try
        {
            // Try to extract JSON from code blocks if present
            var jsonContent = ExtractJsonFromContent(content);
            jsonResponse = JsonSerializer.Deserialize<CopywritingJsonResponse>(jsonContent);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse agent response as JSON: {ex.Message}", ex);
        }

        if (jsonResponse is null)
        {
            throw new InvalidOperationException("Agent returned null response");
        }

        // Validate response completeness
        ValidateResponse(jsonResponse);

        // Map to response model
        return Task.FromResult(new CopywritingResponse
        {
            Headlines = jsonResponse.Headlines ?? [],
            BodyCopyShort = jsonResponse.BodyCopyShort,
            BodyCopyMedium = jsonResponse.BodyCopyMedium,
            BodyCopyLong = jsonResponse.BodyCopyLong,
            CTAs = jsonResponse.CTAs ?? [],
            ToneAdherenceMetadata = jsonResponse.ToneAdherenceMetadata,
            GeneratedAt = DateTimeOffset.UtcNow,
        });
    }

    /// <summary>
    /// Extracts JSON content from response, handling code block wrappers.
    /// </summary>
    /// <param name="content">The content to extract JSON from.</param>
    /// <returns>Extracted JSON string.</returns>
    private static string ExtractJsonFromContent(string content)
    {
        // Remove markdown code block markers if present
        var trimmed = content.Trim();
        if (trimmed.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            var startIndex = trimmed.IndexOf('\n') + 1;
            var endIndex = trimmed.LastIndexOf("```");
            if (endIndex > startIndex)
            {
                return trimmed[startIndex..endIndex].Trim();
            }
        }

        return trimmed;
    }

    /// <summary>
    /// Validates that the response contains all required sections.
    /// </summary>
    /// <param name="response">The JSON response to validate.</param>
    private static void ValidateResponse(CopywritingJsonResponse response)
    {
        var errors = new List<string>();

        if (response.Headlines is null || response.Headlines.Length < 3)
        {
            errors.Add("Response must contain at least 3 headlines");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyShort))
        {
            errors.Add("BodyCopyShort is missing");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyMedium))
        {
            errors.Add("BodyCopyMedium is missing");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyLong))
        {
            errors.Add("BodyCopyLong is missing");
        }

        if (response.CTAs is null || response.CTAs.Length < 3)
        {
            errors.Add("Response must contain at least 3 CTAs");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException($"Invalid agent response: {string.Join("; ", errors)}");
        }
    }

    /// <summary>
    /// Internal model for JSON deserialization.
    /// </summary>
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
    private sealed class CopywritingJsonResponse
    {
        public string[]? Headlines { get; set; }

        public string? BodyCopyShort { get; set; }

        public string? BodyCopyMedium { get; set; }

        public string? BodyCopyLong { get; set; }

        public string[]? CTAs { get; set; }

        public string? ToneAdherenceMetadata { get; set; }
    }
#pragma warning restore SA1011
#pragma warning restore S1144
#pragma warning restore S3459
}
