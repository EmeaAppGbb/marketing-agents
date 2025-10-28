# Task 008: Visual Poster Concept Agent Implementation

## Description
Implement the Visual Poster Concept Agent that generates structured conceptual descriptions for campaign poster ideas. The agent produces mood descriptors, palette suggestions, layout notes, and draft alt text without generating actual images.

## Dependencies
- Task 005: Agent Framework Integration & Base Agent Setup
- Task 006: Copywriting Agent Implementation (optional reference for messaging alignment)

## Technical Requirements

### Agent Provider Implementation
Create `VisualConceptAgentProvider` class extending `BaseAgentProvider`:
- Inherit from base agent provider pattern
- Constructor accepting `IChatClient` dependency
- Override `CreateAgent()` to return configured `ChatClientAgent`
- Agent name: "VisualConceptAgent"
- Agent description: "Generates conceptual descriptions for campaign visual posters with accessibility focus"

### Agent Instructions (System Prompt)
Craft comprehensive instructions covering:
- Role as visual creative director and accessibility expert
- Requirement to produce ≥2 clearly distinct concept directions
- Conceptual descriptions only (no actual image generation)
- Mood descriptors and thematic elements
- Color palette suggestions (prefer provided brand palette)
- Layout hierarchy notes (headline zone, focal area, CTA placement)
- Draft alt text for accessibility (subject + purpose + mood)
- Emphasis on accessibility when flag enabled
- Output structure as JSON with concept objects

### Agent Tools (using AIFunctionFactory)
Implement minimum 3 tools:

**Tool 1: ValidateColorPalette**
- Checks provided brand palette for accessibility (contrast ratios)
- Suggests complementary colors that maintain accessibility
- Returns validation result with WCAG compliance notes

**Tool 2: GenerateAltText**
- Creates descriptive alt text from concept elements
- Ensures subject + purpose + mood are present
- Validates alt text completeness and clarity
- Returns alt text with quality score

**Tool 3: AnalyzeConceptDistinctiveness**
- Compares multiple concepts for thematic differentiation
- Identifies overlaps and suggests diversification
- Returns distinctiveness score and recommendations

Additional tools as needed for:
- Layout hierarchy validation
- Mood consistency checking
- Brand guideline compliance

### Request/Response Models
Create strongly-typed models as C# records:

**VisualConceptRequest**
- CampaignBrief (required)
- BrandPalette (string[], optional hex values)
- BrandImageryDescriptors (string[], optional)
- AccessibilityEmphasisFlag (bool, default false)
- RevisionFeedback (string, optional)
- PreviousVersion (VisualConceptResponse, optional)

**ConceptDirection**
- ConceptName (string)
- ThematicDescription (string)
- MoodDescriptors (string[])
- ColorPalette (string[], hex values)
- LayoutNotes (LayoutHierarchy)
- AltText (string, required)
- AccessibilityNotes (string, optional)

**LayoutHierarchy**
- HeadlineZone (string, description of placement)
- FocalArea (string, main visual element description)
- CTAPlacement (string, call-to-action location)
- SecondaryElements (string[], optional)

**VisualConceptResponse**
- ConceptDirections (ConceptDirection[], minimum 2)
- PaletteReusedFromBrand (bool)
- GeneratedAt (DateTimeOffset)

### Service Integration
Create `IVisualConceptService` interface:
- `Task<VisualConceptResponse> GenerateAsync(VisualConceptRequest request, CancellationToken cancellationToken)`
- `Task<VisualConceptResponse> RegenerateAsync(VisualConceptRequest request, CancellationToken cancellationToken)`

Implement `VisualConceptService`:
- Inject `VisualConceptAgentProvider`
- Build prompt from campaign brief + brand palette + imagery descriptors
- Emphasize accessibility if flag enabled
- Call `AIAgent.RunAsync()` with constructed prompt
- Parse agent response into `VisualConceptResponse`
- Validate minimum 2 distinct concepts
- Ensure all concepts have complete alt text
- Implement retry logic for incomplete responses (max 3 attempts)

### Prompt Engineering
- Construct context from campaign brief and brand assets
- Include provided brand palette explicitly with instruction to prefer it
- For accessibility emphasis: strengthen alt text quality requirements
- For regeneration: include feedback to adjust concepts
- Specify layout hierarchy requirements clearly
- Use structured output format specification with examples

### Validation and Processing
- Validate ≥2 concept directions present
- Check alt text completeness for each concept (subject + purpose + mood)
- Validate color palette format (hex values)
- Ensure thematic distinctiveness between concepts
- Validate layout hierarchy has required zones
- Check palette reuse if brand colors provided

### Accessibility Validation
- Validate alt text includes subject, purpose, and mood
- Check color contrast if palette validation enabled
- Ensure layout notes consider accessibility
- Flag concepts with potential accessibility issues

### Error Handling
- Handle missing alt text as validation error
- Retry if concepts are too similar (distinctiveness score <0.5)
- Validate hex color format
- Log all errors with context
- Return meaningful errors to orchestrator

### Metrics and Telemetry
Instrument the following metrics:
- Generation latency (mean and P95)
- Concept regeneration count
- Palette reuse ratio (when brand palette provided)
- Alt text completeness heuristic (token count, keyword presence)
- Concept distinctiveness scores
- Tool invocation counts
- Accessibility flag usage rate

## Acceptance Criteria
- [ ] VisualConceptAgentProvider implemented with ChatClientAgent wrapper
- [ ] Agent returns ≥2 clearly distinct concept directions
- [ ] All concepts include complete alt text with subject + purpose + mood
- [ ] Palette suggestions respect provided brand palette when available
- [ ] Layout hierarchy notes present for all concepts
- [ ] Regeneration maintains consistency and applies feedback
- [ ] Concepts demonstrate thematic separation (distinctiveness score ≥0.5)
- [ ] Accessibility emphasis flag influences alt text quality
- [ ] Generation latency baseline <20s measured

## Testing Requirements
- [ ] Unit tests for VisualConceptService (≥85% coverage)
- [ ] Unit tests for all agent tools
- [ ] Integration tests with mock IChatClient
- [ ] Test with and without brand palette provided
- [ ] Test accessibility emphasis flag effects
- [ ] Test concept distinctiveness validation
- [ ] Test alt text completeness validation
- [ ] Test regeneration with feedback
- [ ] Test color palette format validation
- [ ] Test layout hierarchy validation
- [ ] Load test to verify 20s latency target

## Non-Functional Requirements
- Generation latency <20s (baseline target)
- Concept distinctiveness score ≥0.5 (thematic separation)
- Alt text completeness 100% (all concepts have valid alt text)
- Palette reuse when provided ≥80%
- Support for regeneration with 5+ iterations

## Out of Scope
- Actual image generation (future enhancement)
- Automated contrast ratio calculation and scoring
- Integration with design tools (Figma, Adobe)
- Multi-channel concept variations (print vs digital)
- Stock photo recommendations

## Notes
- Follow AGENTS.md agent implementation patterns
- Use AI Toolkit best practices before implementation
- Query Microsoft Docs MCP for Agent Framework samples
- Never use IChatClient directly - always wrap in ChatClientAgent
- Accessibility is a key focus - ensure alt text quality
- Document palette strategy in code comments
- Consider edge cases: no brand palette, conflicting descriptors, overly abstract briefs
- Create MADR if significant accessibility or palette decisions made
- Alt text should prioritize clarity over metaphor when accessibility flag enabled
