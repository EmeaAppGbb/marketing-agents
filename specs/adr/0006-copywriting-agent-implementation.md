# Copywriting Agent Implementation with Agent Framework and AIFunctionFactory Tools

* Status: accepted
* Deciders: Development Team
* Date: 2025-10-29

## Context and Problem Statement

The marketing agents application requires a specialized copywriting agent capable of generating high-quality marketing copy with multiple headline variants, body copy in different length tiers, and diverse CTA suggestions. The agent must validate outputs against best practices (headline length 50-60 characters, tone alignment, semantic diversity) and support iterative refinement based on feedback. How should we implement this agent to ensure reliability, maintainability, and adherence to the Microsoft Agent Framework patterns?

## Decision Drivers

* **Type Safety**: Need strongly-typed request/response models with C# record types and nullable reference types
* **Tool Integration**: Require multiple validation and analysis capabilities (headline length, tone alignment, diversity checking)
* **Framework Alignment**: Must follow Microsoft Agent Framework patterns using ChatClientAgent and AIFunctionFactory
* **Retry Logic**: Need robust error handling with exponential backoff for LLM failures
* **JSON Structured Output**: LLM must return predictable, parseable JSON for reliable processing
* **Testability**: Implementation must support ≥85% code coverage with unit and integration tests
* **Extensibility**: Design should allow adding new tools or modifying existing ones without breaking changes

## Considered Options

1. **Direct IChatClient Usage with Manual JSON Parsing**
2. **ChatClientAgent without Tools (Prompt Engineering Only)**
3. **Full BaseAgentProvider Pattern with AIFunctionFactory Tools (Chosen)**

## Decision Outcome

Chosen option: "Full BaseAgentProvider Pattern with AIFunctionFactory Tools", because it provides the best combination of framework alignment, type safety, tool integration, and extensibility while following established patterns from existing agents (AuditAgent).

### Positive Consequences

* Strong typing throughout the entire request/response pipeline with C# records
* Declarative tool registration using AIFunctionFactory.Create() with Description attributes
* Built-in retry logic and telemetry inherited from BaseAgentProvider<T>
* Tools are independently testable static methods with 90%+ coverage
* JSON schema is explicitly defined in agent instructions for predictable LLM behavior
* Service layer separates orchestration concerns from agent implementation
* Easy to add new tools by creating static methods and registering via AIFunctionFactory

### Negative Consequences

* Requires additional abstraction layers (Provider, Service) vs. direct IChatClient usage
* Tool invocations by LLM are not guaranteed—depends on model behavior
* JSON parsing requires ExtractJsonFromContent helper to handle markdown code blocks
* Current test coverage for service/provider layers is below 85% (11.1% and 32.4% respectively)
* More complex initial setup compared to prompt-only approach

## Pros and Cons of the Options

### Option 1: Direct IChatClient Usage with Manual JSON Parsing

Direct consumption of IChatClient in the service layer with custom JSON parsing logic.

* Good, because simplest implementation with fewest abstractions
* Good, because fastest to implement for prototypes
* Good, because no framework dependencies beyond IChatClient
* Bad, because no standardized retry logic or telemetry
* Bad, because violates established BaseAgentProvider<T> pattern used by other agents
* Bad, because tools cannot be easily registered or discovered
* Bad, because requires duplicating error handling and validation logic
* Bad, because harder to test—cannot easily mock agent behavior
* Bad, because doesn't leverage Agent Framework's tool calling capabilities

### Option 2: ChatClientAgent without Tools (Prompt Engineering Only)

Use ChatClientAgent wrapped around IChatClient, rely entirely on prompt engineering for validation logic.

* Good, because simpler than tool-based approach
* Good, because leverages ChatClientAgent for better agent lifecycle management
* Good, because prompt-based validation is faster (no tool round-trips)
* Good, because fewer moving parts to test and maintain
* Bad, because validation logic is opaque—embedded in prompts rather than code
* Bad, because no reusability of validation functions across agents
* Bad, because harder to enforce deterministic validation rules
* Bad, because cannot independently test validation logic
* Bad, because LLM must "learn" validation rules from examples rather than execute them
* Bad, because doesn't demonstrate Agent Framework's tool capabilities

### Option 3: Full BaseAgentProvider Pattern with AIFunctionFactory Tools (Chosen)

Extend BaseAgentProvider<CopywritingResponse> with three registered tools via AIFunctionFactory.

* Good, because follows established pattern from AuditAgent and other framework agents
* Good, because tools are independently testable static methods with 90%+ coverage
* Good, because declarative tool registration with Description attributes for LLM discovery
* Good, because inherits retry logic, telemetry, and error handling from base provider
* Good, because strongly-typed request/response models using C# records
* Good, because tools can be reused across multiple agents if needed
* Good, because validation logic is explicit, maintainable code rather than prompt magic
* Good, because LLM can choose to invoke tools based on context
* Good, because service layer cleanly separates orchestration from agent implementation
* Good, because extensible—new tools can be added by implementing static methods
* Bad, because requires more initial setup with provider, service, and tool classes
* Bad, because tool invocations add latency (LLM must decide to call, then process results)
* Bad, because LLM tool usage is non-deterministic (model may not always call expected tools)
* Bad, because current service/provider test coverage is below target (11.1% and 32.4%)
* Bad, because JSON parsing requires helper method to strip markdown code blocks

## Implementation Details

### Components Implemented

1. **Models**:
   - `CopywritingRequest`: Campaign brief, tone guidelines, length preferences, revision feedback
   - `CopywritingResponse`: Headlines[], BodyCopy variants (Short/Medium/Long), CTAs[]

2. **Tools** (registered via AIFunctionFactory):
   - `ValidateHeadlineLengthTool`: Validates 50-60 character best practice (83.3% coverage)
   - `AnalyzeToneAlignmentTool`: Keyword-based tone analysis (96.8% coverage)
   - `CheckSemanticDiversityTool`: Bigram similarity heuristic (94% coverage)

3. **Provider**:
   - `CopywritingAgentProvider`: Extends BaseAgentProvider<CopywritingResponse>
   - System prompt defines comprehensive JSON output schema
   - ProcessResponseAsync parses and validates JSON structure
   - Tool registration via AIFunctionFactory.Create()

4. **Service**:
   - `CopywritingService`: Implements ICopywritingService
   - BuildPrompt constructs context from request fields
   - ExecuteWithRetryAsync implements 3-attempt exponential backoff
   - ValidateResponseCompleteness ensures all required fields present

### Testing Strategy

- **Tool tests**: 26 unit tests covering all 3 tools with various edge cases
- **Provider tests**: Constructor validation, agent properties, GetAgent singleton behavior
- **Service tests**: Constructor validation, null checks, feedback requirement validation
- **Coverage**: Tools at 90%+, but service (11.1%) and provider (32.4%) need integration tests

### Future Improvements

1. Add integration tests that mock IChatClient to return realistic LLM responses
2. Implement E2E tests using actual Azure OpenAI endpoints with test campaign briefs
3. Add performance benchmarks for tool execution and overall generation time
4. Consider caching frequent tool results (e.g., tone keyword dictionaries)
5. Implement streaming responses for long-form body copy generation
6. Add metrics collection for tool invocation frequency and success rates

## Links

* Related to [0005-agent-framework-infrastructure.md](0005-agent-framework-infrastructure.md) - Infrastructure decision
* Implements Task [006-task-copywriting-agent.md](../tasks/006-task-copywriting-agent.md)
* Follows patterns from AuditAgent in MarketingAgents.AgentHost/Providers
