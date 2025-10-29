# Engineering Journal

This directory contains weekly engineering journal entries documenting learning, decisions, and progress.

## Purpose

The engineering journal serves to:

- Document weekly progress and achievements
- Record lessons learned and insights gained
- Share knowledge across the team
- Track technical decisions and their outcomes
- Provide historical context for future team members

## Format

Create one entry per week using the filename format: `YYYY-MM-DD-week-summary.md`

Example: `2025-10-29-week-summary.md`

## Template

```markdown
# Week of [Date Range]

## Summary

Brief overview of the week's work (2-3 sentences).

## Accomplishments

- Feature or task completed
- Another accomplishment
- Major milestone reached

## Challenges

### [Challenge Title]

**Problem**: Description of the issue encountered

**Solution**: How it was resolved

**Learning**: Key takeaway or insight

## Technical Decisions

- Decision made and rationale
- Link to MADR if applicable

## Metrics

- Pull requests merged: X
- Issues closed: X
- Test coverage: X%
- Build time: X minutes

## Next Week

- Planned tasks
- Goals
- Areas of focus

## Team Kudos

Recognition for team members who made significant contributions.
```

## Example Entry

See below for a sample journal entry:

---

# Week of October 21-27, 2025

## Summary

Completed scaffolding for the API service and AgentHost service. Set up .NET Aspire orchestration and integrated Cosmos DB emulator for local development. Began implementing copywriting agent.

## Accomplishments

- ✅ Created MarketingAgents.Api project with health check endpoint
- ✅ Created MarketingAgents.AgentHost project with agent framework setup
- ✅ Configured .NET Aspire AppHost for service orchestration
- ✅ Integrated Cosmos DB emulator with automatic container creation
- ✅ Set up SignalR hub for real-time communication
- ✅ Wrote integration tests for API health check

## Challenges

### Cosmos DB Emulator SSL Certificate Issues

**Problem**: Local development failed with SSL certificate validation errors when connecting to Cosmos DB emulator.

**Solution**: Added certificate trust step to setup documentation and disabled SSL validation for local development only in `appsettings.Development.json`.

**Learning**: Always document one-time setup steps for new developers. Consider automation scripts for certificate trust.

### Agent Framework Tool Registration

**Problem**: Agent tools were not being discovered and invoked correctly.

**Solution**: Used `AIFunctionFactory.Create()` pattern with proper parameter descriptions and registered tools in agent constructor.

**Learning**: Tool descriptions are critical for agent decision-making. Clear, descriptive tool names and parameter docs improve agent performance.

## Technical Decisions

- Decided to use `ChatClientAgent` wrapper for all agents instead of direct `IChatClient` calls (improves testability and consistency)
- Created MADR 0001 documenting .NET Aspire decision

## Metrics

- Pull requests merged: 3
- Issues closed: 5
- Test coverage: 87%
- Aspire startup time: 12 seconds

## Next Week

- Complete copywriting agent with retry logic
- Implement short copy agent
- Add compliance agent scaffolding
- Write agent orchestration service
- Add more integration tests

## Team Kudos

- Great work by @johndoe on SignalR hub implementation
- Thanks to @janedoe for thorough code reviews

---

## Guidelines

1. **Be honest**: Document both successes and failures
2. **Share context**: Explain why decisions were made
3. **Be specific**: Include code snippets, metrics, or links when helpful
4. **Keep it concise**: Aim for 1-2 pages per week
5. **Write for the future**: Imagine a new team member reading this in 6 months

## Benefits

- **Knowledge sharing**: Team members learn from each other's experiences
- **Historical record**: Understand how the project evolved over time
- **Onboarding**: New team members can quickly get up to speed
- **Retrospectives**: Data-driven discussions about what works and what doesn't
- **Recognition**: Celebrate achievements and contributions

## Next Steps

Start creating weekly journal entries as you work on the project!
