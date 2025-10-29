// Copyright (c) Marketing Agents. All rights reserved.

using System.Text;
using Microsoft.Extensions.Logging;
using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Providers;

namespace MarketingAgents.AgentHost.Services;

/// <summary>
/// Service for copywriting operations using the Copywriting Agent.
/// </summary>
public sealed class CopywritingService : ICopywritingService
{
    private const int MaxRetryAttempts = 3;

    private readonly CopywritingAgentProvider _agentProvider;
    private readonly ILogger<CopywritingService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CopywritingService"/> class.
    /// </summary>
    /// <param name="agentProvider">The copywriting agent provider.</param>
    /// <param name="logger">The logger instance.</param>
    public CopywritingService(
        CopywritingAgentProvider agentProvider,
        ILogger<CopywritingService> logger)
    {
        _agentProvider = agentProvider ?? throw new ArgumentNullException(nameof(agentProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<CopywritingResponse> GenerateAsync(
        CopywritingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Generating copywriting content for campaign brief");

        var prompt = BuildPrompt(request, isRegeneration: false);

        return await ExecuteWithRetryAsync(prompt, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CopywritingResponse> RegenerateAsync(
        CopywritingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.RevisionFeedback))
        {
            throw new ArgumentException("RevisionFeedback is required for regeneration", nameof(request));
        }

        _logger.LogInformation("Regenerating copywriting content with revision feedback");

        var prompt = BuildPrompt(request, isRegeneration: true);

        return await ExecuteWithRetryAsync(prompt, cancellationToken);
    }

    /// <summary>
    /// Builds the prompt for the agent from the request.
    /// </summary>
    private static string BuildPrompt(CopywritingRequest request, bool isRegeneration)
    {
        var promptBuilder = new StringBuilder();

        promptBuilder.AppendLine("# Campaign Brief");
        promptBuilder.AppendLine(request.CampaignBrief);
        promptBuilder.AppendLine();

        if (request.ToneGuidelines is { Length: > 0 })
        {
            promptBuilder.AppendLine("# Tone Guidelines");
            promptBuilder.AppendLine($"Target tones: {string.Join(", ", request.ToneGuidelines)}");
            promptBuilder.AppendLine();
        }

        if (request.LengthPreferences is { Length: > 0 })
        {
            promptBuilder.AppendLine("# Length Preferences");
            promptBuilder.AppendLine($"Generate body copy for: {string.Join(", ", request.LengthPreferences)}");
            promptBuilder.AppendLine();
        }

        if (isRegeneration && !string.IsNullOrWhiteSpace(request.RevisionFeedback))
        {
            promptBuilder.AppendLine("# Revision Feedback");
            promptBuilder.AppendLine(request.RevisionFeedback);
            promptBuilder.AppendLine();

            if (request.PreviousVersion is not null)
            {
                promptBuilder.AppendLine("# Previous Version (for context)");
                promptBuilder.AppendLine($"Headlines: {string.Join(", ", request.PreviousVersion.Headlines)}");
                if (!string.IsNullOrWhiteSpace(request.PreviousVersion.BodyCopyMedium))
                {
                    promptBuilder.AppendLine($"Body Copy Sample: {request.PreviousVersion.BodyCopyMedium[..Math.Min(200, request.PreviousVersion.BodyCopyMedium.Length)]}...");
                }

                promptBuilder.AppendLine();
            }
        }

        promptBuilder.AppendLine("# Task");
        promptBuilder.AppendLine(isRegeneration
            ? "Please regenerate the campaign copy incorporating the revision feedback above."
            : "Please generate compelling campaign copy following all guidelines.");

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Validates that the response contains all required sections.
    /// </summary>
    /// <param name="response">The response to validate.</param>
    private static void ValidateResponseCompleteness(CopywritingResponse response)
    {
        var missingFields = new List<string>();

        if (response.Headlines.Length < 3)
        {
            missingFields.Add($"Headlines (has {response.Headlines.Length}, needs 3+)");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyShort))
        {
            missingFields.Add("BodyCopyShort");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyMedium))
        {
            missingFields.Add("BodyCopyMedium");
        }

        if (string.IsNullOrWhiteSpace(response.BodyCopyLong))
        {
            missingFields.Add("BodyCopyLong");
        }

        if (response.CTAs.Length < 3)
        {
            missingFields.Add($"CTAs (has {response.CTAs.Length}, needs 3+)");
        }

        if (missingFields.Count > 0)
        {
            throw new InvalidOperationException(
                $"Response is incomplete. Missing or insufficient: {string.Join(", ", missingFields)}");
        }
    }

    /// <summary>
    /// Executes the agent with retry logic for incomplete responses.
    /// </summary>
    private async Task<CopywritingResponse> ExecuteWithRetryAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt < MaxRetryAttempts)
        {
            attempt++;

            try
            {
                _logger.LogDebug("Executing copywriting agent (attempt {Attempt}/{MaxAttempts})", attempt, MaxRetryAttempts);

                var agentRequest = new AgentRequest
                {
                    Prompt = prompt,
                    CorrelationId = Guid.NewGuid().ToString(),
                };

                var result = await _agentProvider.ExecuteAsync(agentRequest, cancellationToken);

                if (!result.Success || result.Data is null)
                {
                    throw new InvalidOperationException(
                        $"Agent execution failed: {result.ErrorMessage ?? "Unknown error"}");
                }

                // Validate response completeness
                ValidateResponseCompleteness(result.Data!);

                _logger.LogInformation(
                    "Copywriting agent completed successfully on attempt {Attempt}",
                    attempt);

                return result.Data;
            }
            catch (InvalidOperationException ex) when (attempt < MaxRetryAttempts)
            {
                lastException = ex;
                _logger.LogWarning(
                    ex,
                    "Copywriting agent attempt {Attempt} failed: {Message}. Retrying...",
                    attempt,
                    ex.Message);

                // Add exponential backoff
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)), cancellationToken);
            }
        }

        throw new InvalidOperationException(
            $"Failed to generate complete copywriting response after {MaxRetryAttempts} attempts",
            lastException);
    }
}
