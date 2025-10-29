// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Models;

/// <summary>
/// Generic result wrapper for agent operations.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed record AgentResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the result data if successful.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the exception details if the operation failed.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets metadata about the execution (e.g., token usage, duration).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="data">The result data.</param>
    /// <param name="metadata">Optional metadata.</param>
    /// <returns>A successful agent result.</returns>
    public static AgentResult<T> CreateSuccess(T data, Dictionary<string, object>? metadata = null)
    {
        return new AgentResult<T>
        {
            Success = true,
            Data = data,
            Metadata = metadata,
        };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed agent result.</returns>
    public static AgentResult<T> CreateFailure(string errorMessage, Exception? exception = null)
    {
        return new AgentResult<T>
        {
            Success = false,
            ErrorMessage = errorMessage,
            Exception = exception,
        };
    }
}
