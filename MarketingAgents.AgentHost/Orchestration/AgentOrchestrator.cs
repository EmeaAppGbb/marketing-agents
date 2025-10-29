// Copyright (c) Marketing Agents. All rights reserved.

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketingAgents.AgentHost.Models.Configuration;

namespace MarketingAgents.AgentHost.Orchestration;

/// <summary>
/// Base implementation of agent orchestration service.
/// Coordinates multiple agents with sequential and parallel execution patterns.
/// </summary>
public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly ILogger<AgentOrchestrator> _logger;
    private readonly OrchestrationConfiguration _configuration;
    private readonly List<OrchestrationEvent> _events = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentOrchestrator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configuration">The orchestration configuration.</param>
    public AgentOrchestrator(
        ILogger<AgentOrchestrator> logger,
        IOptions<OrchestrationConfiguration> configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc/>
    public async Task<OrchestrationResult> OrchestrateCampaignAsync(
        CampaignBrief brief,
        ExecutionMode mode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(brief);

        var runId = Guid.NewGuid().ToString();
        using var activity = Activity.Current?.Source.StartActivity("AgentOrchestrator.OrchestrateCampaign");
        activity?.SetTag("run.id", runId);
        activity?.SetTag("campaign.brief.id", brief.Id);
        activity?.SetTag("execution.mode", mode.ToString());

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Starting campaign orchestration for brief {BriefId} with mode {Mode}",
                brief.Id,
                mode);

            _events.Clear();
            EmitEvent("Orchestrator", "Started", $"Orchestration started for brief {brief.Id}");

            // Placeholder for actual agent execution
            // In future tasks, this will invoke all agent providers
            await Task.Delay(100, cancellationToken); // Simulate work

            stopwatch.Stop();

            var result = new OrchestrationResult
            {
                RunId = runId,
                CampaignBriefId = brief.Id,
                Success = true,
                ExecutionMode = mode.ToString(),
                DurationMs = stopwatch.ElapsedMilliseconds,
                Events = [.. _events],
                Metadata = new Dictionary<string, object>
                {
                    ["StartTime"] = DateTime.UtcNow.AddMilliseconds(-stopwatch.ElapsedMilliseconds),
                    ["EndTime"] = DateTime.UtcNow,
                },
            };

            EmitEvent("Orchestrator", "Completed", $"Orchestration completed in {stopwatch.ElapsedMilliseconds}ms");

            _logger.LogInformation(
                "Orchestration {RunId} completed successfully in {DurationMs}ms",
                runId,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                ex,
                "Orchestration {RunId} was cancelled after {DurationMs}ms",
                runId,
                stopwatch.ElapsedMilliseconds);

            EmitEvent("Orchestrator", "Cancelled", "Orchestration was cancelled");

            return new OrchestrationResult
            {
                RunId = runId,
                CampaignBriefId = brief.Id,
                Success = false,
                ExecutionMode = mode.ToString(),
                DurationMs = stopwatch.ElapsedMilliseconds,
                Events = [.. _events],
                Errors = new Dictionary<string, string>
                {
                    ["Orchestrator"] = "Operation was cancelled",
                },
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Orchestration {RunId} failed after {DurationMs}ms: {ErrorMessage}",
                runId,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            EmitEvent("Orchestrator", "Failed", $"Orchestration failed: {ex.Message}");

            return new OrchestrationResult
            {
                RunId = runId,
                CampaignBriefId = brief.Id,
                Success = false,
                ExecutionMode = mode.ToString(),
                DurationMs = stopwatch.ElapsedMilliseconds,
                Events = [.. _events],
                Errors = new Dictionary<string, string>
                {
                    ["Orchestrator"] = ex.Message,
                },
            };
        }
    }

    /// <summary>
    /// Emits an orchestration event for tracking.
    /// </summary>
    /// <param name="agentName">The name of the agent.</param>
    /// <param name="eventType">The type of event.</param>
    /// <param name="message">The event message.</param>
    protected void EmitEvent(string agentName, string eventType, string? message = null)
    {
        var evt = new OrchestrationEvent
        {
            Timestamp = DateTime.UtcNow,
            AgentName = agentName,
            EventType = eventType,
            Message = message,
        };

        _events.Add(evt);

        if (_configuration.EnableRealtimeEvents)
        {
            _logger.LogInformation(
                "Event: {AgentName} - {EventType} - {Message}",
                agentName,
                eventType,
                message);
        }
    }
}
