# Agent Framework Infrastructure

## Overview

The Marketing Agents application uses Microsoft Agent Framework to power AI-driven marketing campaign generation. This document describes the core infrastructure components that enable agent orchestration, execution, and resilience.

## Architecture

### Provider Pattern

All agents follow a consistent provider pattern with three key layers:

1. **IAgentProvider<T>** - Generic interface for all agent types
2. **BaseAgentProvider<T>** - Abstract base class with common functionality
3. **Specific Providers** - Concrete implementations (e.g., CopywritingAgentProvider, AuditAgentProvider)

This architecture enables:

- **Extensibility**: New agents can be added without modifying orchestration code
- **Type Safety**: Generic type parameters ensure type-safe results
- **Testability**: Easy to mock `IAgentProvider<T>` for unit testing
- **Consistency**: All agents inherit common patterns (retry logic, telemetry, error handling)

### Configuration Chain

The agent framework uses Aspire's Azure AI Foundry integration:

```
AppHost: AddAzureAIFoundry + AddDeployment → AgentHost: AddAzureChatCompletionsClient → IChatClient → ChatClientAgent → AIAgent
```

1. **AddAzureAIFoundry**: Aspire hosting integration registers Azure AI Foundry resource
2. **AddDeployment**: Configures specific model deployment (e.g., "gpt-4o")
3. **AddAzureChatCompletionsClient**: Client integration consumes the deployment via `WithReference`
3. **AddChatClient**: Registers `IChatClient` for the specified deployment (e.g., "gpt-4o")
4. **IChatClient**: Standardized abstraction from `Microsoft.Extensions.AI`
5. **ChatClientAgent**: Microsoft Agent Framework wrapper for agent lifecycle
6. **AIAgent**: Base agent runtime with tools, memory, and orchestration

## Core Components

### IAgentProvider<T>

Generic interface that all agent providers must implement:

```csharp
public interface IAgentProvider<T>
{
    ChatClientAgent GetAgent();
    Task<AgentResult<T>> ExecuteAsync(AgentRequest request, CancellationToken cancellationToken);
    string AgentName { get; }
    string AgentDescription { get; }
}
```

**Methods:**

- `GetAgent()`: Returns a configured `ChatClientAgent` instance
- `ExecuteAsync()`: Executes the agent with a given request and returns typed result

**Properties:**

- `AgentName`: Unique identifier for the agent (e.g., "CopywritingAgent")
- `AgentDescription`: Human-readable description of agent capabilities

### BaseAgentProvider<T>

Abstract base class providing:

- **Retry Logic**: Polly-based exponential backoff for rate limiting (429) and transient failures
- **Telemetry**: OpenTelemetry traces and metrics for all agent executions
- **Error Handling**: Standardized exception handling and result wrapping
- **Agent Creation**: ChatClientAgent initialization with proper configuration

**Protected Methods:**

- `ProcessResponseAsync()`: Override to transform agent response into typed result
- `GetTools()`: Override to register agent-specific tools (AIFunctions)

### AgentOrchestrator

Coordinates multiple agents with flexible execution patterns:

- **Sequential Execution**: Agents run one after another (dependencies between agents)
- **Parallel Execution**: Agents run concurrently (independent operations)
- **Partial Failure Handling**: Continue orchestration even if some agents fail
- **Real-time Events**: Emit lifecycle events for UI status updates

### RetryPolicyService

Resilience patterns for Azure AI Foundry operations:

- **Exponential Backoff**: Configurable base delay and multiplier
- **Rate Limiting**: Handles HTTP 429 (Too Many Requests)
- **Transient Failures**: Retries on HTTP 503 (Service Unavailable), timeouts, cancellations
- **Max Retries**: Configurable maximum retry attempts (default: 5)

## Configuration

### appsettings.json

```json
{
  "Agent": {
    "DefaultTemperature": 0.7,
    "MaxTokens": 4000,
    "MaxRetryAttempts": 5,
    "InitialRetryDelayMs": 1000,
    "RetryBackoffMultiplier": 2.0,
    "ExecutionTimeoutSeconds": 120,
    "EnableDetailedTelemetry": true
  },
  "OpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com",
    "PrimaryChatModel": "gpt-4o",
    "LightweightChatModel": "gpt-4o-mini",
    "ApiVersion": "2024-08-06",
    "MaxTokensPerMinute": 150000,
    "MaxRequestsPerMinute": 1000
  },
  "Orchestration": {
    "DefaultExecutionMode": "Parallel",
    "MaxConcurrentExecutions": 10,
    "MaxWaitTimeSeconds": 300,
    "ContinueOnPartialFailure": true,
    "EnableRealtimeEvents": true
  }
}
```

### Dependency Injection

**AppHost Configuration:**

```csharp
// Add Azure AI Foundry resource via Aspire's Azure AI Foundry integration
var foundry = builder.AddAzureAIFoundry("foundry");
var chatDeployment = foundry.AddDeployment("chat", "gpt-4o", "1", "Microsoft");

builder.AddProject<Projects.MarketingAgents_AgentHost>("agenthost")
    .WithReference(openai);
```

**AgentHost Service Registration:**

```csharp
// Add Aspire service defaults (telemetry, service discovery)
builder.AddServiceDefaults();

// Configure Azure OpenAI via Aspire client integration
// Automatically consumes the "openai" resource from AppHost
builder.AddAzureOpenAIClient("openai")
    .AddChatClient("gpt-4o");

// Register configuration
builder.Services.Configure<AgentConfiguration>(
    builder.Configuration.GetSection("Agent"));
builder.Services.Configure<OpenAIConfiguration>(
    builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<OrchestrationConfiguration>(
    builder.Configuration.GetSection("Orchestration"));

// Register core services
builder.Services.AddSingleton<RetryPolicyService>();
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

// Register agent providers (to be added in future tasks)
// builder.Services.AddScoped<IAgentProvider<CopyArtifact>, CopywritingAgentProvider>();
// builder.Services.AddScoped<IAgentProvider<AuditResult>, AuditAgentProvider>();
```

## Creating a New Agent

### Step 1: Define Result Type

```csharp
public record CopyArtifact
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public List<string> Keywords { get; init; } = [];
}
```

### Step 2: Implement Provider

```csharp
public class CopywritingAgentProvider : BaseAgentProvider<CopyArtifact>
{
    public CopywritingAgentProvider(
        IChatClient chatClient,
        ILogger<CopywritingAgentProvider> logger,
        IOptions<AgentConfiguration> agentConfig)
        : base(chatClient, logger, agentConfig.Value)
    {
    }

    public override string AgentName => "CopywritingAgent";

    public override string AgentDescription =>
        "Creates compelling marketing copy with headlines, body text, and keywords";

    protected override async Task<CopyArtifact> ProcessResponseAsync(
        AgentRunResponse response,
        CancellationToken cancellationToken)
    {
        // Parse agent response and return typed result
        var lastMessage = response.Messages.Last();
        var content = lastMessage.Text ?? string.Empty;

        // Example parsing logic (simplified)
        return new CopyArtifact
        {
            Title = "Extracted title",
            Body = content,
            Keywords = ["ai", "marketing"],
        };
    }

    protected override IEnumerable<AIFunction> GetTools()
    {
        // Define agent-specific tools
        return
        [
            AIFunctionFactory.Create(ValidateKeywords),
            AIFunctionFactory.Create(OptimizeHeadline),
        ];
    }

    private bool ValidateKeywords(string[] keywords)
    {
        // Tool implementation
        return keywords.Length >= 3 && keywords.Length <= 10;
    }

    private string OptimizeHeadline(string headline)
    {
        // Tool implementation
        return headline.Length <= 60 ? headline : headline[..60];
    }
}
```

### Step 3: Register in DI

```csharp
builder.Services.AddScoped<IAgentProvider<CopyArtifact>, CopywritingAgentProvider>();
```

### Step 4: Use in Orchestration

```csharp
public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly IAgentProvider<CopyArtifact> _copyAgent;

    public async Task<OrchestrationResult> OrchestrateCampaignAsync(
        CampaignBrief brief,
        CancellationToken cancellationToken)
    {
        var request = new AgentRequest
        {
            CorrelationId = Guid.NewGuid().ToString(),
            Prompt = $"Create copy for: {brief.ProductName}",
        };

        var copyResult = await _copyAgent.ExecuteAsync(request, cancellationToken);

        if (!copyResult.Success)
        {
            // Handle failure
        }

        // Use copyResult.Data (CopyArtifact)
    }
}
```

## Resilience Patterns

### Retry with Exponential Backoff

All agent operations automatically retry on transient failures:

- **Initial Delay**: 1000ms (configurable via `InitialRetryDelayMs`)
- **Backoff Multiplier**: 2.0 (doubles delay each retry)
- **Max Delay**: 30 seconds (capped to prevent excessive waits)
- **Max Retries**: 5 attempts (configurable via `MaxRetryAttempts`)

### Circuit Breaker (Future)

Planned circuit breaker integration for:

- Tracking failure rates per agent
- Opening circuit after threshold failures
- Half-open state for gradual recovery

### Timeout Handling

Each agent execution has configurable timeout:

- **Default**: 120 seconds (`ExecutionTimeoutSeconds`)
- **Graceful Cancellation**: CancellationToken propagation
- **Timeout Exception**: Handled by retry policy

## Telemetry

### OpenTelemetry Integration

All agents automatically emit:

- **Traces**: Distributed tracing across agent workflows
- **Metrics**: Execution duration, token usage, success/failure rates
- **Logs**: Structured logs with correlation IDs

### Viewing Telemetry

**Aspire Dashboard (Local Development):**

1. Run the AppHost project
2. Open the Aspire dashboard URL (shown in console)
3. Navigate to "Traces" or "Metrics" tab
4. Filter by service name: "agenthost"

**Azure Monitor (Production):**

1. Configure Application Insights connection string in appsettings
2. View traces and metrics in Azure Portal
3. Create custom dashboards and alerts

## Testing

### Unit Testing Agents

```csharp
public class CopywritingAgentProviderTests
{
    private readonly Mock<IChatClient> _mockChatClient;
    private readonly Mock<ILogger<CopywritingAgentProvider>> _mockLogger;
    private readonly AgentConfiguration _config;

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccessResult()
    {
        // Arrange
        var provider = new CopywritingAgentProvider(
            _mockChatClient.Object,
            _mockLogger.Object,
            Options.Create(_config));

        var request = new AgentRequest
        {
            CorrelationId = "test-123",
            Prompt = "Create campaign copy",
        };

        _mockChatClient.Setup(x => x.CompleteAsync(...))
            .ReturnsAsync(new ChatCompletion([...]));

        // Act
        var result = await provider.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
```

### Integration Testing Orchestration

```csharp
public class AgentOrchestratorTests
{
    private readonly WebApplicationFactory<Program> _factory;

    [Fact]
    public async Task OrchestrateCampaignAsync_ExecutesAllAgents()
    {
        // Arrange
        var client = _factory.CreateClient();
        var brief = new CampaignBrief { ... };

        // Act
        var response = await client.PostAsJsonAsync("/api/campaigns", brief);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Best Practices

1. **Always use IChatClient abstraction**: Never directly instantiate Azure AI clients
2. **Wrap agents in ChatClientAgent**: Use `ChatClientAgent` wrapper for Microsoft Agent Framework integration
3. **Implement retry logic**: Leverage `RetryPolicyService` for all Azure AI calls
4. **Define clear tools**: Each agent should have 3+ domain-specific tools
5. **Type-safe results**: Use generic type parameters for compile-time safety
6. **Structured logging**: Include correlation IDs in all log messages
7. **Test in isolation**: Mock `IChatClient` to avoid live API calls in tests

## Troubleshooting

### Rate Limiting (429)

**Symptom**: `HttpRequestException` with status code 429

**Solution**:
- Increase `InitialRetryDelayMs` in configuration
- Reduce `MaxConcurrentExecutions` for parallel orchestration
- Upgrade Azure AI deployment tier for higher rate limits

### Agent Timeout

**Symptom**: `TaskCanceledException` after 120 seconds

**Solution**:
- Increase `ExecutionTimeoutSeconds` in configuration
- Optimize agent prompts to reduce token usage
- Split complex tasks across multiple agents

### Failed Dependency Injection

**Symptom**: `InvalidOperationException` when resolving `IChatClient`

**Solution**:
- Verify AppHost correctly configures `AddConnectionString("openai")`
- Ensure AgentHost calls `builder.AddAzureChatCompletionsClient("chat")`
- Check connection string format in appsettings or environment variables

## Related Documentation

- [System Architecture](overview.md)
- [Configuration Reference](../reference/configuration.md)
