# ADR-0005: Agent Framework Infrastructure Architecture

## Status

Accepted

## Context

We need to establish the foundational infrastructure for the Microsoft Agent Framework integration to support multiple specialized marketing agents (Copy, Short Copy, Visual, Audit). The architecture must:

- Support both parallel and sequential agent execution patterns
- Enable retry logic with exponential backoff for rate limiting
- Provide comprehensive telemetry and observability
- Allow for easy extension with new agent types
- Integrate seamlessly with Azure AI Foundry via Aspire
- Follow clean architecture and SOLID principles

Three architectural approaches were considered for organizing the agent infrastructure.

## Decision Drivers

- **Scalability**: Must support adding new agent types with minimal changes
- **Maintainability**: Clear separation of concerns and testability
- **Performance**: Support for concurrent agent execution
- **Reliability**: Built-in resilience patterns (retries, circuit breakers)
- **Observability**: Comprehensive telemetry and logging
- **Developer Experience**: Simple, intuitive API for agent creation

## Considered Options

### Option 1: Direct IChatClient Usage (Rejected)

**Description**: Each specialized agent directly uses `IChatClient` from Microsoft.Extensions.AI without abstraction.

**Pros**:
- Simplest initial implementation
- Minimal indirection
- Direct access to all chat client features

**Cons**:
- Violates DRY principle - retry logic, logging, telemetry duplicated across agents
- No standardized error handling
- Difficult to enforce best practices (e.g., always use ChatClientAgent wrapper)
- Hard to test agents in isolation
- No centralized orchestration
- Configuration scattered across agent implementations

**Evaluation**: Rejected due to poor scalability and maintainability.

---

### Option 2: Service Layer with Dependency Injection (Rejected)

**Description**: Create agent services directly in DI container without provider abstraction. Each agent is a service implementing a specific interface (e.g., `ICopywritingAgent`, `IAuditAgent`).

**Structure**:
```
Services/
  CopywritingAgentService.cs (implements ICopywritingAgent)
  AuditAgentService.cs (implements IAuditAgent)
  AgentOrchestrator.cs (depends on specific agent interfaces)
```

**Pros**:
- Standard .NET DI patterns
- Direct service injection
- Clear interface contracts

**Cons**:
- Orchestrator becomes tightly coupled to specific agent interfaces
- Adding new agents requires modifying orchestrator
- No common base functionality for retry logic, telemetry
- Difficult to implement generic orchestration patterns (parallel/sequential)
- Each agent interface needs custom definition
- Testing orchestration requires mocking all agent interfaces

**Evaluation**: Rejected due to tight coupling and poor extensibility.

---

### Option 3: Provider Pattern with Base Abstraction (Accepted) ✅

**Description**: Implement a provider pattern with:
- Generic `IAgentProvider<T>` interface for all agent types
- Abstract `BaseAgentProvider<T>` class with common functionality
- Specific provider implementations (e.g., `CopywritingAgentProvider`, `AuditAgentProvider`)
- Generic orchestrator that works with `IAgentProvider<T>`
- Separation of tool infrastructure and agent logic

**Structure**:
```
Providers/
  IAgentProvider<T>.cs (generic interface)
  BaseAgentProvider<T>.cs (common functionality: retry, telemetry, error handling)
  CopywritingAgentProvider.cs (implements IAgentProvider<CopyArtifact>)
  AuditAgentProvider.cs (implements IAgentProvider<AuditResult>)

Orchestration/
  IAgentOrchestrator.cs
  AgentOrchestrator.cs (generic orchestration using IAgentProvider<T>)

Tools/
  IAgentTool.cs
  ToolResult<T>.cs

Models/
  AgentRequest.cs
  AgentResult<T>.cs

Services/
  RetryPolicyService.cs (Polly-based resilience)
```

**Pros**:
- **Extensibility**: New agents added by implementing `BaseAgentProvider<T>` - no orchestrator changes
- **DRY**: Common functionality (retry, telemetry, error handling) in base class
- **Type Safety**: Generic type parameter ensures type-safe results
- **Testability**: Easy to mock `IAgentProvider<T>` for testing orchestration
- **Flexibility**: Orchestrator can work with any agent type via interface
- **Best Practices Enforcement**: Base class ensures all agents follow pattern (ChatClientAgent wrapper, AIAgent.RunAsync())
- **Separation of Concerns**: Tools, providers, orchestration are separate
- **Observability**: Centralized telemetry and logging in base provider

**Cons**:
- More initial boilerplate code
- Slightly more complex than direct service approach
- Requires understanding of provider pattern

**Evaluation**: **Accepted** as it provides the best balance of extensibility, maintainability, and adherence to SOLID principles.

## Decision Outcome

We will implement **Option 3: Provider Pattern with Base Abstraction**.

### Implementation Details:

1. **Generic Provider Interface**:
   ```csharp
   public interface IAgentProvider<T>
   {
       ChatClientAgent GetAgent();
       Task<AgentResult<T>> ExecuteAsync(AgentRequest request, CancellationToken cancellationToken);
       string AgentName { get; }
       string AgentDescription { get; }
   }
   ```

2. **Base Provider with Common Functionality**:
   - Retry logic with exponential backoff (Polly v8)
   - OpenTelemetry instrumentation (traces, metrics)
   - Structured logging
   - Error handling and result wrapping
   - ChatClientAgent creation with proper configuration

3. **Configuration Chain**:
   ```
   AppHost: AddAzureAIFoundry + AddDeployment → AgentHost: AddAzureChatCompletionsClient → IChatClient → ChatClientAgent → AIAgent
   ```

4. **Orchestration**:
   - Generic `AgentOrchestrator` works with any `IAgentProvider<T>`
   - Supports parallel and sequential execution modes
   - Emits lifecycle events for real-time monitoring
   - Handles partial failures gracefully

5. **Resilience**:
   - `RetryPolicyService` using Polly v8 `ResiliencePipeline`
   - Exponential backoff (configurable base delay and multiplier)
   - Max 5 retry attempts (configurable)
   - Handles rate limiting (429), timeouts, transient failures

6. **Aspire Integration**:
   - AppHost configures Azure AI Foundry resource using `builder.AddAzureAIFoundry("foundry").AddDeployment("chat", "gpt-4o", "1", "Microsoft")`
   - AgentHost consumes via `builder.AddAzureChatCompletionsClient("chat")`
   - Automatic service discovery and configuration injection via `WithReference`

## Consequences

### Positive:
- ✅ New agent types can be added without modifying orchestration code
- ✅ Common functionality (retry, telemetry, logging) is centralized and reusable
- ✅ Type-safe agent results via generic type parameter
- ✅ Easy to test agents and orchestration in isolation
- ✅ Enforces Microsoft Agent Framework best practices in base class
- ✅ Supports both parallel and sequential orchestration patterns
- ✅ Built-in resilience and observability

### Negative:
- ⚠️ Requires developers to understand provider pattern
- ⚠️ More files and classes than simpler approaches
- ⚠️ Abstract base class may become complex if too much functionality is added

### Mitigation:
- Provide clear documentation and examples for creating new agent providers
- Keep `BaseAgentProvider` focused on essential common functionality
- Consider extracting specialized concerns (e.g., memory, context) to separate mixins/decorators if needed

## Links

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/en-us/agent-framework/)
- [Aspire Azure AI Foundry Integration](https://learn.microsoft.com/en-us/dotnet/aspire/azureai/azureai-foundry-integration)
- [Polly Resilience Framework](https://www.pollydocs.org/)
- Task 005: Agent Framework Integration & Base Agent Setup
