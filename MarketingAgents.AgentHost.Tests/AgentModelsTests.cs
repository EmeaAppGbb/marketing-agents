using MarketingAgents.AgentHost.Models;

namespace MarketingAgents.AgentHost.Tests.Models;

public class AgentModelsTests
{
    [Fact]
    public void AgentRequest_CanBeCreated()
    {
        var request = new AgentRequest
        {
            CorrelationId = "test-123",
            Prompt = "Test prompt",
            Metadata = new Dictionary<string, object> { ["key"] = "value" },
        };

        request.CorrelationId.Should().Be("test-123");
        request.Prompt.Should().Be("Test prompt");
        request.Metadata.Should().ContainKey("key");
    }

    [Fact]
    public void AgentResult_CreateSuccess_ReturnsSuccessResult()
    {
        var result = AgentResult<string>.CreateSuccess("data", new Dictionary<string, object> { ["test"] = true });

        result.Success.Should().BeTrue();
        result.Data.Should().Be("data");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void AgentResult_CreateFailure_ReturnsFailureResult()
    {
        var result = AgentResult<string>.CreateFailure("error");

        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().Be("error");
    }
}
