// Copyright (c) Marketing Agents. All rights reserved.

using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MarketingAgents.AgentHost.Models.Configuration;
using Polly;
using Polly.Retry;

namespace MarketingAgents.AgentHost.Services;

/// <summary>
/// Service for managing retry policies with exponential backoff for agent operations.
/// Implements resilience patterns for Azure OpenAI rate limiting and transient failures.
/// </summary>
public sealed class RetryPolicyService
{
    private readonly ILogger<RetryPolicyService> _logger;
    private readonly AgentConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryPolicyService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configuration">The agent configuration.</param>
    public RetryPolicyService(
        ILogger<RetryPolicyService> logger,
        IOptions<AgentConfiguration> configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates a retry strategy for agent operations with exponential backoff.
    /// Handles rate limiting (429), timeout, and transient errors.
    /// </summary>
    /// <returns>A configured retry strategy options.</returns>
    public ResiliencePipeline<T> CreateRetryPipeline<T>()
    {
        return new ResiliencePipelineBuilder<T>()
            .AddRetry(new RetryStrategyOptions<T>
            {
                MaxRetryAttempts = _configuration.MaxRetryAttempts,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(_configuration.InitialRetryDelayMs),
                MaxDelay = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder<T>()
                    .Handle<HttpRequestException>(ex =>
                        ex.StatusCode == HttpStatusCode.TooManyRequests ||
                        ex.StatusCode == HttpStatusCode.ServiceUnavailable)
                    .Handle<TaskCanceledException>()
                    .Handle<TimeoutException>(),
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} after {Delay}ms. Reason: {Reason}",
                        args.AttemptNumber,
                        _configuration.MaxRetryAttempts,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? "Unknown");

                    return ValueTask.CompletedTask;
                },
            })
            .Build();
    }

    /// <summary>
    /// Executes an operation with retry policy and exponential backoff.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> ExecuteWithRetryAsync<T>(
        Func<CancellationToken, ValueTask<T>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var pipeline = CreateRetryPipeline<T>();

        return await pipeline.ExecuteAsync(
            async ct => await operation(ct),
            cancellationToken);
    }
}
