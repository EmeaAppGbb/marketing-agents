# Task 007: Short Social Copy Agent Implementation

## Description
Implement the Short Social Copy Agent that generates platform-specific micro copy aligned with campaign messaging. The agent produces multiple variants per platform with character count awareness, hashtag suggestions, and alignment metadata.

## Dependencies
- Task 005: Agent Framework Integration & Base Agent Setup
- Task 006: Copywriting Agent Implementation (for alignment with long-form copy)

## Technical Requirements

### Agent Provider Implementation
Create `ShortCopyAgentProvider` class extending `BaseAgentProvider`:
- Inherit from base agent provider pattern
- Constructor accepting `IChatClient` dependency
- Override `CreateAgent()` to return configured `ChatClientAgent`
- Agent name: "ShortSocialCopyAgent"
- Agent description: "Generates platform-specific social media copy with hashtag suggestions"

### Agent Instructions (System Prompt)
Craft comprehensive instructions covering:
- Role as social media copywriting expert
- Platform-specific character limits and best practices
- Requirement to produce ≥3 variants per selected platform
- Alignment with main campaign messaging
- Hashtag relevance and banned hashtag avoidance
- Over-limit flagging (do not auto-truncate)
- Emoji usage guidelines (optional, configurable)
- Output structure as JSON with platform-grouped variants

### Platform Configuration
Define platform specifications as configuration:
- Twitter/X: 280 characters
- Facebook: 63,206 characters (recommend <500 for engagement)
- Instagram: 2,200 characters (recommend <125 for captions)
- LinkedIn: 3,000 characters (recommend <150)
- Threads: 500 characters
- Configurable via appsettings with extensibility for new platforms

### Agent Tools (using AIFunctionFactory)
Implement minimum 3 tools:

**Tool 1: ValidateCharacterCount**
- Validates text against platform-specific limits
- Returns over-limit flag and character count
- Suggests truncation points if over limit

**Tool 2: AnalyzeHashtagRelevance**
- Evaluates hashtag relevance to campaign brief
- Checks against banned hashtag list
- Returns relevance score and alternative suggestions

**Tool 3: CheckMessageAlignment**
- Compares short copy with main campaign headlines/messaging
- Returns alignment score and consistency notes
- Identifies tone or message drift

Additional tools as needed for:
- Emoji appropriateness scoring
- Engagement prediction hints (future)
- Platform-specific best practice validation

### Request/Response Models
Create strongly-typed models as C# records:

**ShortCopyRequest**
- CampaignBrief (required)
- LongFormCopyReference (CopywritingResponse, required for alignment)
- SelectedPlatforms (Platform[], required, minimum 1)
- HashtagRules (HashtagConstraints, optional)
- RevisionFeedback (string, optional)
- PreviousVersion (ShortCopyResponse, optional)
- TargetPlatform (Platform?, optional for selective regeneration)

**Platform** (enum)
- Twitter, Facebook, Instagram, LinkedIn, Threads

**HashtagConstraints**
- MaxHashtags (int, optional)
- BannedHashtags (string[], optional)
- RequiredHashtags (string[], optional)

**PlatformVariants**
- Platform (Platform enum)
- Variants (string[], minimum 3)
- CharacterCounts (int[])
- OverLimitFlags (bool[])
- Hashtags (string[])
- AlignmentNote (string)

**ShortCopyResponse**
- PlatformVariants (PlatformVariants[], one per selected platform)
- GeneratedAt (DateTimeOffset)

### Service Integration
Create `IShortCopyService` interface:
- `Task<ShortCopyResponse> GenerateAsync(ShortCopyRequest request, CancellationToken cancellationToken)`
- `Task<ShortCopyResponse> RegenerateAsync(ShortCopyRequest request, CancellationToken cancellationToken)`
- `Task<PlatformVariants> RegeneratePlatformAsync(ShortCopyRequest request, Platform platform, CancellationToken cancellationToken)`

Implement `ShortCopyService`:
- Inject `ShortCopyAgentProvider`
- Build prompt including campaign brief + long-form copy reference + platform specs
- For selective regeneration: scope prompt to single platform
- Call `AIAgent.RunAsync()` with constructed prompt
- Parse agent response into `ShortCopyResponse`
- Validate all variants and perform character counting
- Flag over-limit posts without truncation
- Implement retry logic for incomplete responses (max 3 attempts)

### Prompt Engineering
- Construct context from campaign brief + long-form copy snippets
- Include explicit platform character limits in prompt
- For regeneration: include feedback and preserve platform scope
- Specify hashtag constraints clearly
- Reference main message/headlines for alignment guidance
- Use structured output format specification

### Validation and Processing
- Validate at least one platform selected
- Calculate character counts for all variants
- Flag over-limit variants
- Validate hashtag suggestions against banned list
- Check all required platforms have ≥3 variants
- Ensure alignment notes present for each platform

### Error Handling
- Handle unsupported platforms gracefully with warnings
- Validate response structure and completeness
- Retry on parsing failures
- Log banned hashtag violations
- Return meaningful errors to orchestrator

### Metrics and Telemetry
Instrument the following metrics:
- Generation latency for all platforms (mean and P95)
- Regeneration frequency per platform
- Over-limit post rate
- Hashtag usage distribution
- Variant count per platform
- Alignment quality proxy scores
- Tool invocation counts

## Acceptance Criteria
- [ ] ShortCopyAgentProvider implemented with ChatClientAgent wrapper
- [ ] Agent returns ≥3 variants for each selected platform
- [ ] Character counts calculated and over-limit posts flagged
- [ ] Hashtag suggestions respect banned list and max constraints
- [ ] Alignment notes present for each platform bundle
- [ ] Selective regeneration preserves other platforms unchanged
- [ ] Platform configuration extensible for new platforms
- [ ] Generation latency baseline <25s for all platforms
- [ ] Over-limit post rate ≤10% initially

## Testing Requirements
- [ ] Unit tests for ShortCopyService (≥85% coverage)
- [ ] Unit tests for all agent tools
- [ ] Integration tests with mock IChatClient
- [ ] Test all supported platforms with various briefs
- [ ] Test selective platform regeneration
- [ ] Test hashtag validation and filtering
- [ ] Test character count calculation accuracy
- [ ] Test alignment score calculation
- [ ] Test with missing/invalid platform selections
- [ ] Test banned hashtag detection
- [ ] Load test to verify 25s latency target

## Non-Functional Requirements
- Generation latency <25s for all platforms (baseline)
- Hashtag relevance ≥70% qualitative assessment
- Over-limit posts ≤10% initially
- Alignment with long-form copy maintained
- Support for 5+ platforms simultaneously

## Out of Scope
- Scheduling or publishing integration
- Engagement prediction modeling (future)
- Automatic emoji insertion based on sentiment
- Platform API integration for validation
- Image or media attachment handling

## Notes
- Follow AGENTS.md agent implementation patterns
- Use AI Toolkit best practices before implementation
- Query Microsoft Docs MCP for Agent Framework samples
- Never use IChatClient directly - always wrap in ChatClientAgent
- Platform limits may change - keep configuration flexible
- Consider emojis increase character count differently across platforms
- Test with edge cases: no platforms selected, conflicting feedback
- Document hashtag strategy in code comments
- Create MADR if significant platform strategy decisions made
