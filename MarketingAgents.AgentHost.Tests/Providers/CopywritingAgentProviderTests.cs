// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Models.Configuration;
using MarketingAgents.AgentHost.Providers;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketingAgents.AgentHost.Tests.Providers;

/// <summary>
/// Unit tests for CopywritingAgentProvider.
/// </summary>
public sealed class CopywritingAgentProviderTests
{
    private readonly Mock<IChatClient> _mockChatClient;
    private readonly Mock<ILogger<CopywritingAgentProvider>> _mockLogger;
    private readonly IOptions<AgentConfiguration> _configuration;

    public CopywritingAgentProviderTests()
    {
        _mockChatClient = new Mock<IChatClient>();
        _mockLogger = new Mock<ILogger<CopywritingAgentProvider>>();
        _configuration = Options.Create(new AgentConfiguration
        {
            DefaultTemperature = 0.7,
            MaxTokens = 4000,
        });
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Assert
        provider.Should().NotBeNull();
        provider.AgentName.Should().Be("CopywritingAgent");
        provider.AgentDescription.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void AgentName_ReturnsCorrectValue()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var name = provider.AgentName;

        // Assert
        name.Should().Be("CopywritingAgent");
    }

    [Fact]
    public void AgentDescription_ReturnsCorrectValue()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var description = provider.AgentDescription;

        // Assert
        description.Should().Contain("headline");
        description.Should().Contain("body copy");
        description.Should().Contain("CTA");
    }

    [Fact]
    public void AgentInstructions_ContainsRequiredElements()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var instructions = provider.AgentInstructions;

        // Assert
        instructions.Should().Contain("expert copywriter");
        instructions.Should().Contain("3 distinct headline");
        instructions.Should().Contain("50-100 words");
        instructions.Should().Contain("100-200 words");
        instructions.Should().Contain("200-400 words");
        instructions.Should().Contain("3 CTA");
        instructions.Should().Contain("JSON");
    }

    [Fact]
    public void GetAgent_ReturnsChatClientAgent()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var agent = provider.GetAgent();

        // Assert
        agent.Should().NotBeNull();
        agent.Should().BeOfType<ChatClientAgent>();
    }

    [Fact]
    public void GetAgent_CalledMultipleTimes_ReturnsSameInstance()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var agent1 = provider.GetAgent();
        var agent2 = provider.GetAgent();

        // Assert
        agent1.Should().BeSameAs(agent2);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(_mockChatClient.Object, _mockLogger.Object, _configuration);

        // Act
        var act = async () => await provider.ExecuteAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
