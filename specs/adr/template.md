# MADR Template

Use this template for creating new Architecture Decision Records (MADRs).

## File Naming Convention

Use sequential numbering: `0001-decision-title.md`, `0002-next-decision.md`

## Template

```markdown
# [short title of solved problem and solution]

* Status: [proposed | accepted | rejected | deprecated | superseded by [ADR-0005](0005-example.md)]
* Deciders: [list everyone involved in the decision]
* Date: [YYYY-MM-DD when the decision was last updated]

## Context and Problem Statement

[Describe the context and problem statement, e.g., in free form using two to three sentences. You may want to articulate the problem in form of a question.]

## Decision Drivers

* [driver 1, e.g., a force, facing concern, ...]
* [driver 2, e.g., a force, facing concern, ...]
* [...]

## Considered Options

* [option 1]
* [option 2]
* [option 3]

## Decision Outcome

Chosen option: "[option 1]", because [justification. e.g., only option that meets k.o. criterion decision driver | resolves force | ... | comes out best (see below)].

### Positive Consequences

* [e.g., improvement of quality attribute satisfaction, follow-up decisions required, ...]
* [...]

### Negative Consequences

* [e.g., compromising quality attribute, follow-up decisions required, ...]
* [...]

## Pros and Cons of the Options

### [option 1]

[example | description | pointer to more information | ...]

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* [...]

### [option 2]

[example | description | pointer to more information | ...]

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* [...]

### [option 3]

[example | description | pointer to more information | ...]

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* [...]

## Links

* [Link type] [Link to ADR]
* [...]
```

## Example Usage

See `0001-use-aspire-for-orchestration.md` for a complete example.

## Guidelines

1. **Be specific**: Describe the exact problem and context
2. **Present 3+ options**: Always consider multiple alternatives
3. **Document trade-offs**: List pros and cons for each option
4. **Explain the decision**: Clearly state why the chosen option was selected
5. **Keep it concise**: Aim for 1-2 pages maximum
6. **Update status**: Mark as accepted, deprecated, or superseded when relevant

## When to Create an MADR

Create an MADR when:

- Making architectural decisions that impact multiple components
- Choosing between different technology stacks or frameworks
- Deciding on significant design patterns or approaches
- Implementing features that require architectural trade-offs
- Making decisions that will affect future development

## References

- [MADR Documentation](https://adr.github.io/madr/)
- [Architecture Decision Records](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
