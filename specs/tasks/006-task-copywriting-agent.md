# Task 006: Copywriting Agent Implementation

## Description
Implement the Copywriting Agent that generates long-form campaign messaging including headlines, body copy variants, and CTAs. The agent should produce diverse, tone-aligned content with support for feedback-driven regeneration.

## Dependencies
- Task 005: Agent Framework Integration & Base Agent Setup

## Technical Requirements

### Agent Provider Implementation
Create `CopywritingAgentProvider` class extending `BaseAgentProvider`:
- Inherit from base agent provider pattern
- Constructor accepting `IChatClient` dependency
- Override `CreateAgent()` to return configured `ChatClientAgent`
- Agent name: "CopywritingAgent"
- Agent description: "Generates campaign headlines, body copy, and CTAs aligned with brand tone"

### Agent Instructions (System Prompt)
Craft comprehensive instructions covering:
- Role as expert copywriter for marketing campaigns
- Requirement to produce ≥3 distinct headline options
- Body copy in three length tiers (short: 50-100 words, medium: 100-200 words, long: 200-400 words)
- ≥3 CTA suggestions tied to campaign objective
- Tone adherence based on provided guidelines
- Semantic diversity requirement (avoid trivial synonyms)
- Output structure as JSON with clearly defined sections

### Agent Tools (using AIFunctionFactory)
Implement minimum 3 tools:

**Tool 1: ValidateHeadlineLength**
- Validates headline character count against best practices (50-60 chars)
- Returns validation result with recommendations

**Tool 2: AnalyzeToneAlignment**
- Analyzes generated copy against tone guidelines
- Returns tone adherence score and qualitative assessment

**Tool 3: CheckSemanticDiversity**
- Analyzes headline variants for semantic similarity
- Uses n-gram overlap heuristic
- Returns diversity score and suggestions

Additional tools as needed for:
- CTA action verb strength analysis
- Readability scoring
- Keyword density checking

### Request/Response Models
Create strongly-typed models as C# records:

**CopywritingRequest**
- CampaignBrief (required)
- ToneGuidelines (string[], optional)
- LengthPreferences (enum[], optional)
- RevisionFeedback (string, optional for regeneration)
- PreviousVersion (CopywritingResponse, optional)

**CopywritingResponse**
- Headlines (string[], minimum 3)
- BodyCopyShort (string)
- BodyCopyMedium (string)
- BodyCopyLong (string)
- CTAs (string[], minimum 3)
- ToneAdherenceMetadata (string, qualitative descriptor)
- GeneratedAt (DateTimeOffset)

### Service Integration
Create `ICopywritingService` interface:
- `Task<CopywritingResponse> GenerateAsync(CopywritingRequest request, CancellationToken cancellationToken)`
- `Task<CopywritingResponse> RegenerateAsync(CopywritingRequest request, CancellationToken cancellationToken)`

Implement `CopywritingService`:
- Inject `CopywritingAgentProvider`
- Build prompt from campaign brief + optional revision feedback
- Call `AIAgent.RunAsync()` with constructed prompt
- Parse agent response into `CopywritingResponse`
- Handle missing sections with defaults or errors
- Implement retry logic for incomplete responses (max 3 attempts)

### Prompt Engineering
- Construct comprehensive campaign brief string from input
- Include tone guidelines explicitly in prompt
- For regeneration: append feedback context to original brief
- Use structured output format specification in prompt
- Include examples of expected output quality

### Error Handling
- Validate response completeness (all required sections present)
- Handle partial responses gracefully
- Retry on parsing failures or missing sections
- Log all errors with sufficient context for debugging
- Return meaningful error messages to orchestrator

### Metrics and Telemetry
Instrument the following metrics:
- Generation latency (mean and P95)
- Regeneration count per campaign
- Headline diversity score distribution
- Tone adherence score distribution
- Tool invocation counts
- Success/failure rates

## Acceptance Criteria
- [ ] CopywritingAgentProvider implemented with ChatClientAgent wrapper
- [ ] Agent returns ≥3 distinct headlines for every request
- [ ] Body copy generated in all three length tiers unless constrained
- [ ] ≥3 contextually relevant CTAs generated
- [ ] Regeneration preserves original brief and applies feedback context
- [ ] Headlines are semantically diverse (diversity score >0.6)
- [ ] Tone adherence metadata included in response
- [ ] All required tools implemented using AIFunctionFactory
- [ ] Response parsing handles JSON structure correctly
- [ ] Generation latency baseline <20s measured

## Testing Requirements
- [ ] Unit tests for CopywritingService (≥85% coverage)
- [ ] Unit tests for all agent tools
- [ ] Integration tests with mock IChatClient
- [ ] Test with various campaign briefs and tone guidelines
- [ ] Test regeneration with feedback preserves context
- [ ] Test headline diversity calculation
- [ ] Test response parsing with valid and invalid JSON
- [ ] Test error handling for incomplete responses
- [ ] Test retry logic for transient failures
- [ ] Load test to verify 20s latency target

## Non-Functional Requirements
- Generation latency <20s (baseline target)
- Tone adherence ≥80% qualitative match
- Headline diversity score ≥0.6 (n-gram overlap metric)
- 100% regeneration completeness (no empty sections)
- Support for campaigns with 5+ iterations

## Out of Scope
- Multilingual support (future enhancement)
- Real-time token streaming (future enhancement)
- Automatic sentiment optimization
- CTA character limit enforcement (handled by Short Copy agent for social)
- Image or media suggestions

## Notes
- Follow AGENTS.md agent implementation patterns
- Use AI Toolkit best practices before implementation
- Query Microsoft Docs MCP for Agent Framework code samples
- Never use IChatClient directly - always wrap in ChatClientAgent
- Document prompt engineering decisions in code comments
- Create MADR if significant prompt strategy decisions are made
- Test with diverse campaign briefs to ensure robustness
- Consider edge cases: missing tone guidelines, niche products, extremely long feedback
