// Copyright (c) Marketing Agents. All rights reserved.

namespace MarketingAgents.AgentHost.Tools;

/// <summary>
/// Attribute to document agent tool capabilities.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ToolDescriptorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolDescriptorAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the tool.</param>
    /// <param name="description">The description of the tool.</param>
    public ToolDescriptorAttribute(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of the tool.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets or sets the category of the tool.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets example usage of the tool.
    /// </summary>
    public string? Example { get; set; }
}
