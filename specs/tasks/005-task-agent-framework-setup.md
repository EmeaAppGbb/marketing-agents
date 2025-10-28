# Task 005: Agent Framework Integration & Base Agent Setup

## Description
Set up the Microsoft Agent Framework infrastructure, configure Azure OpenAI integration via Aspire, and create the base agent provider pattern that will be used by all specialized agents (Copy, Short Copy, Visual, Audit).

## Dependencies
- Task 001: Backend Scaffolding
- Task 004: Campaign Data Model & Cosmos DB Persistence

## Technical Requirements

### NuGet Package Installation
Install latest stable versions of:
- Microsoft.Agents.AI
- Microsoft.Agents.AI.OpenAI
- Microsoft.Extensions.AI
- Microsoft.Extensions.AI.OpenAI
- Azure.AI.OpenAI

### Azure OpenAI Configuration via Aspire
- Configure Aspire integration for Azure OpenAI using `.AddChatClient()`
- Set up local development configuration with User Secrets
- Configure chat completion options (temperature, max tokens, etc.)
- Register `IChatClient` as singleton in DI container via `.AsIChatClient()`
- Support multiple model configurations (GPT-4 for complex tasks, GPT-3.5 for simple tasks)

### Base Agent Provider Pattern
Create abstract base class `BaseAgentProvider`:
- Constructor accepting `IChatClient` dependency
- Abstract method `CreateAgent()` returning `ChatClientAgent`
- Helper methods for tool registration using `AIFunctionFactory`
- Shared configuration for retry logic and timeout handling
- Logging and telemetry integration
- Error handling patterns

### Agent Provider Interface
Define `IAgentProvider<T>` interface:
- `ChatClientAgent GetAgent()`
- `Task<T> ExecuteAsync(AgentRequest request, CancellationToken cancellationToken)`
- Properties for agent name, description, and instructions

### Tool Infrastructure
Create base classes for agent tools:
- `IAgentTool` interface with Execute method
- Tool descriptor attributes for documentation
- Parameter validation using Data Annotations
- Tool result wrapper types
- Error handling and logging for tool execution

### Agent Orchestration Service Foundation
Create `IAgentOrchestrator` interface:
- `Task<OrchestrationResult> OrchestrateCampaignAsync(CampaignBrief brief, ExecutionMode mode, CancellationToken cancellationToken)`
- Support for parallel and sequential execution modes
- Event emission for lifecycle state changes
- Cancellation token propagation
- Error aggregation and partial failure handling

Implement base `AgentOrchestrator` service:
- Inject all agent providers via DI
- Implement parallel fan-out pattern using Task.WhenAll
- Implement sequential execution with dependency handling
- Emit status events for real-time updates
- Aggregate results into unified artifact bundle

### Retry and Resilience Patterns
- Implement retry logic with exponential backoff for agent calls
- Configure circuit breaker for Azure OpenAI rate limits
- Handle rate limiting (429) and timeout errors gracefully
- Maximum 5 retry attempts with configurable backoff
- Log all retry attempts for observability

### Telemetry and Observability
- Structured logging for all agent operations
- OpenTelemetry traces for agent execution spans
- Custom metrics for:
  - Agent execution duration
  - Token usage per agent
  - Retry counts
  - Success/failure rates
- Correlation IDs for end-to-end tracing

### Configuration Management
Create configuration models:
- `AgentConfiguration` record for shared agent settings
- `OpenAIConfiguration` record for model settings
- `OrchestrationConfiguration` record for execution modes
- Bind configuration from appsettings.json and User Secrets

## Acceptance Criteria
- [ ] All required NuGet packages installed (latest stable versions)
- [ ] Azure OpenAI client configured via Aspire integration
- [ ] `IChatClient` registered and injectable via DI
- [ ] Base agent provider abstract class created and documented
- [ ] Agent orchestrator interface and base implementation created
- [ ] Tool infrastructure interfaces and base classes created
- [ ] Retry logic with exponential backoff implemented (max 5 attempts)
- [ ] Telemetry and logging integrated with OpenTelemetry
- [ ] Configuration models created and bound from settings
- [ ] All components registered in DI container

## Testing Requirements
- [ ] Unit tests for base agent provider patterns (≥85% coverage)
- [ ] Unit tests for retry logic and error handling
- [ ] Unit tests for orchestration patterns (parallel/sequential)
- [ ] Integration tests with mock IChatClient
- [ ] Test cancellation token propagation
- [ ] Test partial failure scenarios
- [ ] Test telemetry emission (traces, metrics, logs)
- [ ] Test configuration binding and validation
- [ ] Load test for rate limit handling

## Non-Functional Requirements
- Agent initialization time < 500ms
- Support for 10+ concurrent agent executions
- Graceful degradation under rate limiting
- Zero message loss in event emission
- Comprehensive error messages for debugging

## Out of Scope
- Specific agent implementations (Copy, Short Copy, Visual, Audit) - covered in separate tasks
- Agent memory/state persistence beyond conversation context
- Multi-turn conversation support (MVP is single-turn)
- Fine-tuned model deployment

## Notes
- Follow AGENTS.md agent integration guidelines strictly
- Use AI Toolkit best practices tool before implementation
- Query Microsoft Docs MCP for latest Agent Framework samples
- All agents must use `ChatClientAgent` wrapper, never `IChatClient` directly
- Use `AIAgent.RunAsync()` pattern for agent execution
- Document agent construction chain in code comments: Azure OpenAI Client → ChatClient → .AsIChatClient() → ChatClientAgent → AIAgent
- Create MADR for agent architecture decisions
