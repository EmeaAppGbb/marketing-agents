// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Models;
using MarketingAgents.AgentHost.Models.Configuration;
using MarketingAgents.AgentHost.Providers;
using MarketingAgents.AgentHost.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketingAgents.AgentHost.Tests.Services;

/// <summary>
/// Unit tests for CopywritingService.
/// </summary>
public sealed class CopywritingServiceTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var mockProvider = CreateMockProvider();
        var mockLogger = new Mock<ILogger<CopywritingService>>();

        // Act
        var service = new CopywritingService(mockProvider, mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullProvider_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CopywritingService>>();

        // Act
        var act = () => new CopywritingService(null!, mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("agentProvider");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var mockProvider = CreateMockProvider();

        // Act
        var act = () => new CopywritingService(mockProvider, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public async Task GenerateAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = async () => await service.GenerateAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RegenerateAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = async () => await service.RegenerateAsync(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RegenerateAsync_WithoutRevisionFeedback_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService();
        var request = new CopywritingRequest
        {
            CampaignBrief = "Test campaign",
        };

        // Act
        var act = async () => await service.RegenerateAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*RevisionFeedback*");
    }

    private static CopywritingService CreateService()
    {
        var mockProvider = CreateMockProvider();
        var mockLogger = new Mock<ILogger<CopywritingService>>();
        return new CopywritingService(mockProvider, mockLogger.Object);
    }

    private static CopywritingAgentProvider CreateMockProvider()
    {
        var mockChatClient = new Mock<IChatClient>();
        var mockLogger = new Mock<ILogger<CopywritingAgentProvider>>();
        var configuration = Options.Create(new AgentConfiguration
        {
            DefaultTemperature = 0.7,
            MaxTokens = 4000,
        });

        return new CopywritingAgentProvider(mockChatClient.Object, mockLogger.Object, configuration);
    }
}
