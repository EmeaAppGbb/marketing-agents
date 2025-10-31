# ADR 0006: Sequential Agent Execution with Retry Pattern

**Status**: Accepted

**Date**: 30 October 2025

## Context

The system must orchestrate four AI agents for campaign generation:
1. Copy Agent (generates marketing copy)
2. Short Copy Agent (generates social media posts)
3. Poster Agent (generates visual concepts)
4. Audit Agent (validates compliance)

Per FRD-002, agents must execute sequentially in initial implementation. When audit fails, the system must support regeneration with compliance feedback incorporated into retry attempts.

## Decision Drivers

- FRD-002 specifies sequential execution: Copy → ShortCopy → Poster → Audit
- Need for retry logic when compliance audit fails (up to 5 attempts per FRD-003)
- Requirement to pass audit feedback to regeneration attempts
- Execution time targets: ≤30s per agent, ≤120s total
- AGENTS.md requires orchestration patterns with sequential and concurrent support

## Considered Options

1. **Sequential execution with retry loop** (required for MVP)
2. Parallel agent execution (future enhancement)
3. Event-driven asynchronous execution

## Decision Outcome

**Chosen: Sequential execution with compliance-aware retry pattern**

### Orchestration Flow

```
User submits campaign brief
    ↓
Create campaign in database (status: "created")
    ↓
Update status to "generating"
    ↓
Execute Copy Agent → store artifact
    ↓
Execute Short Copy Agent → store artifact
    ↓
Execute Poster Agent → store artifact
    ↓
Execute Audit Agent → store audit result
    ↓
If audit PASSED:
  Update status to "completed"
  Return campaign to user
    ↓
If audit FAILED and retries < 5:
  Construct feedback prompt with audit violations
  Re-execute Copy Agent (with feedback)
  Re-execute Short Copy Agent (with feedback)
  Re-execute Poster Agent (with feedback)
  Re-execute Audit Agent
  Repeat until passed or max retries reached
    ↓
If max retries reached:
  Update status to "failed"
  Return campaign with failures to user
```

### Implementation Pattern

**Orchestrator Service:**
```csharp
public class CampaignOrchestrator
{
    private readonly ICopyAgentProvider _copyAgent;
    private readonly IShortCopyAgentProvider _shortCopyAgent;
    private readonly IPosterAgentProvider _posterAgent;
    private readonly IAuditAgentProvider _auditAgent;
    private readonly ICampaignRepository _repository;
    private const int MaxRetries = 5;

    public async Task<Campaign> GenerateCampaignAsync(string campaignId)
    {
        var campaign = await _repository.GetByIdAsync(campaignId);
        await _repository.UpdateStatusAsync(campaignId, "generating");

        int retryCount = 0;
        AuditResult? auditResult = null;

        do
        {
            // Build prompt with optional feedback from previous audit
            var prompt = BuildCampaignPrompt(campaign.Brief, auditResult?.Feedback);

            // Execute content generation agents sequentially
            var copy = await _copyAgent.GenerateAsync(prompt);
            var shortCopy = await _shortCopyAgent.GenerateAsync(prompt);
            var poster = await _posterAgent.GenerateAsync(prompt);

            // Store artifacts
            await _repository.StoreArtifactsAsync(campaignId, new Artifacts
            {
                Copy = copy,
                ShortCopy = shortCopy,
                Poster = poster
            });

            // Execute audit agent
            auditResult = await _auditAgent.AuditAsync(copy, shortCopy, poster);
            await _repository.StoreAuditResultAsync(campaignId, auditResult);

            retryCount++;

        } while (auditResult.Status == "failed" && retryCount < MaxRetries);

        var finalStatus = auditResult.Status == "passed" ? "completed" : "failed";
        await _repository.UpdateStatusAsync(campaignId, finalStatus);

        return await _repository.GetByIdAsync(campaignId);
    }

    private string BuildCampaignPrompt(CampaignBrief brief, string? feedback = null)
    {
        var basePrompt = $@"
            Theme: {brief.Theme}
            Target Audience: {brief.TargetAudience}
            Product Details: {brief.ProductDetails}
        ";

        if (!string.IsNullOrEmpty(feedback))
        {
            basePrompt += $@"
            
            IMPORTANT - Previous attempt had compliance issues. Address this feedback:
            {feedback}
            ";
        }

        return basePrompt;
    }
}
```

### Rationale

- **Simplicity**: Sequential execution is straightforward to implement and debug
- **Compliance focus**: Retry loop ensures content meets standards before delivery
- **Feedback integration**: Failed audits inform next generation attempt
- **Time budget**: 3 agents × 30s + audit = ~90-120s total (within requirements)
- **Clear failure handling**: Max retries prevents infinite loops

## Consequences

### Positive
- Predictable execution order and timing
- Easy to reason about state transitions
- Audit feedback directly improves next attempt
- Simple error handling and logging
- Meets all FRD-002 and FRD-003 requirements

### Negative
- Slower than parallel execution (but acceptable for MVP per FRD-002)
- All agents re-execute on retry (even if only one had issues)
- Network/LLM latency adds up sequentially

### Future Enhancements
- **Parallel agent execution**: Run Copy, ShortCopy, and Poster agents concurrently to reduce total time to ~30-40s
- **Selective retry**: Only re-run agents with violations
- **Streaming results**: Return artifacts as they complete rather than waiting for all
- **Exponential backoff**: Add delays between retries to handle rate limiting

### Implementation Notes

**Error Handling:**
- Each agent execution wrapped in try-catch
- Agent failures logged with campaign ID and context
- Partial results stored even if some agents fail
- Return 207 Multi-Status for partial success

**Telemetry:**
- Log each agent execution start/completion
- Record execution times per agent
- Track retry attempt numbers
- Monitor pass/fail rates

**Configuration:**
```csharp
public class CampaignGenerationOptions
{
    public int MaxRetries { get; set; } = 5;
    public TimeSpan AgentTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan TotalTimeout { get; set; } = TimeSpan.FromSeconds(120);
}
```

**Agent Provider Pattern:**
- Each agent encapsulated in provider class
- Providers registered as scoped services in DI
- Providers create ChatClientAgent instances with appropriate instructions
- Orchestrator depends on provider interfaces for testability
