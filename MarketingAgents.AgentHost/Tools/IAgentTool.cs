// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Interface for agent tools that can be called by agents.
/// </summary>
public interface IAgentTool
{
    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of what the tool does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the tool with the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameters for the tool execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tool execution result.</returns>
    Task<ToolResult<object>> ExecuteAsync(
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);
}
