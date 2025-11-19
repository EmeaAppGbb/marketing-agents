# ADR 0002: Use Microsoft Agent Framework for AI Agents

**Status**: Accepted

**Date**: 30 October 2025

## Context

The application requires four specialized AI agents:
1. **Copy Agent** - Generates full marketing copy (200-800 words)
2. **Short Copy Agent** - Generates social media posts (3-5 posts, 50-280 chars each)
3. **Poster Agent** - Generates visual poster concepts and descriptions
4. **Audit Agent** - Validates compliance and quality of generated content

All agents must generate content based on campaign briefs and the audit agent must provide feedback for regeneration.

## Decision Drivers

- AGENTS.md explicitly mandates: "Do NOT implement custom agent orchestration logic—use Microsoft Agent Framework"
- Need for structured agent execution with tools/capabilities
- Requirement for retry logic with compliance feedback
- Need for agent telemetry and observability
- Integration with Azure OpenAI and Azure AI Foundry

## Considered Options

1. **Microsoft Agent Framework** (`Microsoft.Agents.AI` + `Microsoft.Extensions.AI`)
2. Semantic Kernel
3. LangChain .NET
4. Custom LLM wrapper implementation

## Decision Outcome

**Chosen: Microsoft Agent Framework**

### Implementation Pattern

**Agent Construction:**
```csharp
// Use ChatClientAgent for all agents
var agent = new ChatClientAgent(
    chatClient: chatClient,
    instructions: "Agent-specific instructions...",
    name: "CopyAgent"
);
```

**Agent Execution:**
```csharp
// Use AIAgent.RunAsync() pattern
var result = await agent.RunAsync(campaignBriefPrompt);
```

**Agent Architecture:**
- **Agent Providers**: Create provider classes (e.g., `CopyAgentProvider`, `AuditAgentProvider`) that encapsulate agent creation and tool registration
- **Orchestration Services**: Create orchestrator services that coordinate multiple agents with sequential execution
- **Dependency Injection**: Register `IChatClient` as singleton, agent providers as scoped

### Rationale

- **Compliance with AGENTS.md**: Explicitly required by project guidelines
- **ChatClientAgent pattern**: Provides proper agent abstraction with instructions, name, and tools
- **AIAgent.RunAsync()**: Correct execution pattern (never use `IChatClient` directly for agent operations)
- **Tools via AIFunctionFactory**: Enables agent capabilities with clear descriptions and parameter documentation
- **Built-in telemetry**: Integrates with OpenTelemetry via ServiceDefaults

## Consequences

### Positive
- Full compliance with project guidelines
- Proper agent abstraction with ChatClientAgent
- Easy tool registration via AIFunctionFactory
- Built-in support for retry loops with feedback
- Integration with Azure AI Foundry for prompt management

### Negative
- Dependency on Microsoft ecosystem (not portable to other LLM frameworks)
- Learning curve for Microsoft Agent Framework patterns

### Implementation Notes

**Required NuGet Packages:**
- `Microsoft.Agents.AI`
- `Microsoft.Agents.AI.OpenAI`
- `Microsoft.Extensions.AI`
- `Microsoft.Extensions.AI.OpenAI`

**Configuration Chain:**
```
Azure OpenAI Client → ChatClient → .AsIChatClient() → ChatClientAgent → AIAgent
```

**Agent Orchestration:**
- Implement sequential execution for initial version (Copy → ShortCopy → Poster → Audit)
- Build comprehensive campaign brief strings with theme, audience, product details
- For retries: append audit feedback to prompt for next attempt
- Track retry attempt count (max 5 attempts recommended)

**Agent Host Architecture:**
- Create `MarketingAgents.AgentHost` project for agent execution
- Separate from API layer for distributed processing capability
- Implement orchestration services for sequential and future parallel execution
