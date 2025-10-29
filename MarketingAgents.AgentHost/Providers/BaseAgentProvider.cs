// Copyright (c) Marketing Agents. All rights reserved.

using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Models.Configuration;

namespace MarketingAgents.AgentHost.Providers;

/// <summary>
/// Abstract base class for all agent providers.
/// Implements common functionality for agent creation, execution, and error handling.
/// Configuration chain: Azure OpenAI Client → ChatClient → .AsIChatClient() → ChatClientAgent → AIAgent.
/// </summary>
/// <typeparam name="T">The type of data the agent returns.</typeparam>
public abstract class BaseAgentProvider<T> : IAgentProvider<T>
{
    private readonly IChatClient _chatClient;
    private readonly ILogger _logger;
    private readonly AgentConfiguration _configuration;
    private ChatClientAgent? _agent;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAgentProvider{T}"/> class.
    /// </summary>
    /// <param name="chatClient">The IChatClient instance from Aspire integration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configuration">The agent configuration.</param>
    protected BaseAgentProvider(
        IChatClient chatClient,
        ILogger logger,
        IOptions<AgentConfiguration> configuration)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc/>
    public abstract string AgentName { get; }

    /// <inheritdoc/>
    public abstract string AgentDescription { get; }

    /// <inheritdoc/>
    public abstract string AgentInstructions { get; }

    /// <summary>
    /// Gets the list of tools for this agent.
    /// Override in derived classes to provide agent-specific tools.
    /// </summary>
    protected virtual IList<AIFunction>? Tools => null;

    /// <summary>
    /// Gets the chat options for this agent.
    /// Override in derived classes to customize chat behavior.
    /// </summary>
    protected virtual ChatOptions? ChatOptions => new()
    {
        Temperature = (float)_configuration.DefaultTemperature,
        MaxOutputTokens = _configuration.MaxTokens,
    };

    /// <inheritdoc/>
    public ChatClientAgent GetAgent()
    {
        if (_agent is not null)
        {
            return _agent;
        }

        // Create agent using ChatClientAgent wrapper pattern
        // Configuration chain: Azure OpenAI Client → ChatClient → .AsIChatClient() → ChatClientAgent
        var options = new ChatClientAgentOptions
        {
            Name = AgentName,
            Instructions = AgentInstructions,
            ChatOptions = ChatOptions,
        };

        _agent = new ChatClientAgent(_chatClient, options);

        _logger.LogInformation(
            "Created agent '{AgentName}' with description: {AgentDescription}",
            AgentName,
            AgentDescription);

        return _agent;
    }

    /// <inheritdoc/>
    public async Task<AgentResult<T>> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var activity = Activity.Current?.Source.StartActivity($"{AgentName}.Execute");
        activity?.SetTag("agent.name", AgentName);
        activity?.SetTag("correlation.id", request.CorrelationId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Executing agent '{AgentName}' for correlation ID: {CorrelationId}",
                AgentName,
                request.CorrelationId);

            var agent = GetAgent();

            // Create chat messages for the request
            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, request.Prompt),
            };

            // Use AIAgent.RunAsync() pattern with messages collection as per best practices
            var response = await agent.RunAsync(messages, cancellationToken: cancellationToken);

            stopwatch.Stop();

            var result = await ProcessResponseAsync(response, cancellationToken);

            var metadata = new Dictionary<string, object>
            {
                ["ExecutionDurationMs"] = stopwatch.ElapsedMilliseconds,
                ["AgentName"] = AgentName,
                ["CorrelationId"] = request.CorrelationId,
            };

            _logger.LogInformation(
                "Agent '{AgentName}' completed successfully in {DurationMs}ms",
                AgentName,
                stopwatch.ElapsedMilliseconds);

            activity?.SetTag("execution.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("execution.success", true);

            return AgentResult<T>.CreateSuccess(result, metadata);
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                ex,
                "Agent '{AgentName}' was cancelled after {DurationMs}ms",
                AgentName,
                stopwatch.ElapsedMilliseconds);

            activity?.SetTag("execution.cancelled", true);

            return AgentResult<T>.CreateFailure("Operation was cancelled", ex);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Agent '{AgentName}' failed after {DurationMs}ms: {ErrorMessage}",
                AgentName,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            activity?.SetTag("execution.error", ex.Message);
            activity?.SetTag("execution.success", false);

            return AgentResult<T>.CreateFailure(ex.Message, ex);
        }
    }

    /// <summary>
    /// Processes the agent response and converts it to the expected type.
    /// Override in derived classes to implement type-specific processing.
    /// </summary>
    /// <param name="response">The agent run response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The processed result of type T.</returns>
    protected abstract Task<T> ProcessResponseAsync(
        AgentRunResponse response,
        CancellationToken cancellationToken);
}
