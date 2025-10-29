// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Result wrapper for tool execution.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed record ToolResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the tool execution succeeded.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the result data if successful.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the error message if the execution failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the exception if the execution failed.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Creates a successful tool result.
    /// </summary>
    /// <param name="data">The result data.</param>
    /// <returns>A successful tool result.</returns>
    public static ToolResult<T> CreateSuccess(T data)
    {
        return new ToolResult<T>
        {
            Success = true,
            Data = data,
        };
    }

    /// <summary>
    /// Creates a failed tool result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed tool result.</returns>
    public static ToolResult<T> CreateFailure(string errorMessage, Exception? exception = null)
    {
        return new ToolResult<T>
        {
            Success = false,
            ErrorMessage = errorMessage,
            Exception = exception,
        };
    }
}
