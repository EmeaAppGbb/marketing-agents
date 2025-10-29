using MarketingAgents.AgentHost.Models.Configuration;

namespace MarketingAgents.AgentHost.Tests.Models;

public class ConfigurationTests
{
    [Fact]
    public void AgentConfiguration_HasCorrectDefaults()
    {
        var config = new AgentConfiguration();

        config.DefaultTemperature.Should().Be(0.7);
        config.MaxTokens.Should().Be(4000);
        config.MaxRetryAttempts.Should().Be(5);
        config.InitialRetryDelayMs.Should().Be(1000);
        config.ExecutionTimeoutSeconds.Should().Be(120);
        config.EnableDetailedTelemetry.Should().BeTrue();
    }

    [Fact]
    public void OpenAIConfiguration_HasCorrectDefaults()
    {
        var config = new OpenAIConfiguration();

        config.PrimaryChatModel.Should().Be("gpt-4o");
        config.LightweightChatModel.Should().Be("gpt-4o-mini");
        config.ApiVersion.Should().Be("2024-08-06");
        config.MaxTokensPerMinute.Should().Be(150000);
        config.MaxRequestsPerMinute.Should().Be(1000);
    }

    [Fact]
    public void OrchestrationConfiguration_HasCorrectDefaults()
    {
        var config = new OrchestrationConfiguration();

        config.DefaultExecutionMode.Should().Be(ExecutionMode.Parallel);
        config.MaxConcurrentExecutions.Should().Be(10);
        config.MaxWaitTimeSeconds.Should().Be(300);
        config.ContinueOnPartialFailure.Should().BeTrue();
        config.EnableRealtimeEvents.Should().BeTrue();
    }
}
