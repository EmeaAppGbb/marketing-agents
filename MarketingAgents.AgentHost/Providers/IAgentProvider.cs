// Copyright (c) Marketing Agents. All rights reserved.

using Microsoft.Agents.AI;

namespace MarketingAgents.AgentHost.Providers;

/// <summary>
/// Generic interface for agent providers.
/// </summary>
/// <typeparam name="T">The type of data the agent returns.</typeparam>
public interface IAgentProvider<T>
{
    /// <summary>
    /// Gets the agent instance.
    /// </summary>
    /// <returns>The configured ChatClientAgent.</returns>
    ChatClientAgent GetAgent();

    /// <summary>
    /// Executes the agent asynchronously.
    /// </summary>
    /// <param name="request">The agent request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The agent result.</returns>
    Task<Models.AgentResult<T>> ExecuteAsync(Models.AgentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the agent name.
    /// </summary>
    string AgentName { get; }

    /// <summary>
    /// Gets the agent description.
    /// </summary>
    string AgentDescription { get; }

    /// <summary>
    /// Gets the agent instructions.
    /// </summary>
    string AgentInstructions { get; }
}
