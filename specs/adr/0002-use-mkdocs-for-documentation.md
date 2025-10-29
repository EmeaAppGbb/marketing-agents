# Use MkDocs with Material Theme for Documentation

* Status: accepted
* Deciders: Development Team, Documentation Team
* Date: 2025-10-29

## Context and Problem Statement

The Marketing Agents Platform requires comprehensive documentation for developers, operators, and users. The documentation must be versioned, searchable, easy to maintain, and deployable to GitHub Pages or similar hosting.

How should we structure and manage project documentation to ensure it's maintainable, accessible, and stays in sync with code?

## Decision Drivers

* Developer experience - documentation should be easy to write and maintain
* Markdown support - developers prefer Markdown for technical documentation
* Version control integration - documentation should live alongside code
* Search functionality - users need to quickly find information
* Deployment simplicity - automatic deployment to GitHub Pages
* Theme customization - professional appearance with good mobile support
* Plugin ecosystem - extensibility for features like git revision dates, code highlighting
* Standards compliance - follow industry best practices for documentation

## Considered Options

* Option 1: MkDocs with Material theme
* Option 2: Docusaurus
* Option 3: GitHub Wiki

## Decision Outcome

Chosen option: "MkDocs with Material theme", because it provides the best balance of simplicity, features, and Python ecosystem integration. It's lightweight, fast, and has excellent Material theme with built-in search, code highlighting, and mobile responsiveness.

### Positive Consequences

* **Simple Markdown-based**: All documentation in Markdown, familiar to developers
* **Material theme**: Professional, modern theme with excellent UX
* **Fast builds**: Static site generation in < 3 seconds
* **Plugin ecosystem**: Rich plugin support (git-revision-date, minify, etc.)
* **GitHub Pages integration**: Automatic deployment via GitHub Actions
* **Search**: Client-side search with instant results
* **Mobile responsive**: Works well on all devices
* **Low maintenance**: Static site with no backend dependencies
* **Python integration**: Natural fit for .NET projects using Python for docs tooling

### Negative Consequences

* **Python dependency**: Requires Python and pip for local development
* **Limited interactivity**: Static site, no dynamic features (acceptable for docs)
* **Custom plugins**: Less plugin availability compared to larger ecosystems like Hugo
* **YAML configuration**: mkdocs.yml can become large for complex navigation

## Pros and Cons of the Options

### Option 1: MkDocs with Material theme

MkDocs is a fast, simple static site generator designed for building project documentation from Markdown files.

* Good, because Markdown-only documentation is simple and version-controllable
* Good, because Material theme provides professional appearance out-of-the-box
* Good, because build times are very fast (< 3 seconds for this project)
* Good, because plugins extend functionality (git-revision-date-localized, minify, etc.)
* Good, because client-side search works without backend infrastructure
* Good, because it's easy to deploy to GitHub Pages via GitHub Actions
* Good, because `mkdocs build --strict` enforces link validation and quality
* Good, because navigation is customizable via mkdocs.yml
* Good, because Material theme has dark mode, code copy buttons, and mobile support
* Bad, because Python is required for local development (additional toolchain)
* Bad, because navigation must be manually configured in mkdocs.yml
* Bad, because plugin ecosystem is smaller than Hugo or Gatsby

### Option 2: Docusaurus

Docusaurus is a React-based static site generator for documentation, developed by Meta.

* Good, because powerful React-based customization
* Good, because supports versioning natively for multiple doc versions
* Good, because large plugin ecosystem
* Good, because popular in open-source community
* Good, because supports blog functionality out-of-the-box
* Bad, because requires Node.js, adding another toolchain dependency
* Bad, because more complex setup and configuration (React, MDX)
* Bad, because slower build times compared to MkDocs
* Bad, because overkill for internal developer documentation
* Bad, because heavier runtime bundle size (client-side React app)

### Option 3: GitHub Wiki

GitHub Wiki is built-in wiki functionality for GitHub repositories.

* Good, because zero setup required
* Good, because integrated with GitHub repository
* Good, because supports Markdown
* Good, because free hosting
* Bad, because separate Git repository from code (hard to keep in sync)
* Bad, because limited customization and theming
* Bad, because no build-time validation (broken links not detected)
* Bad, because search is basic and limited
* Bad, because no support for plugins or extensions
* Bad, because wiki commits don't trigger code review process
* Bad, because difficult to enforce documentation standards

## Implementation Details

### Folder Structure

```
docs/
├── index.md
├── getting-started/
├── architecture/
├── api/
├── guides/
└── reference/

specs/
├── adr/              # Architecture Decision Records
├── journal/          # Engineering journal
└── product-specs/    # Product specifications
```

### Required Plugins

- **search**: Built-in client-side search
- **git-revision-date-localized**: Show last updated dates
- **minify**: Minify HTML, JS, CSS for production

### Markdown Extensions

- **pymdownx.superfences**: Code blocks with Mermaid diagram support
- **pymdownx.tabbed**: Tabbed content sections
- **pymdownx.highlight**: Syntax highlighting
- **admonition**: Callout boxes (warnings, tips, notes)
- **toc**: Table of contents with permalinks

### CI/CD Integration

GitHub Actions workflow automatically:
1. Builds documentation with `mkdocs build --strict`
2. Validates all links and references
3. Deploys to GitHub Pages on merge to main
4. Runs on every PR affecting docs/ or mkdocs.yml

### Documentation Standards

- All files in Markdown format (.md)
- Kebab-case filenames (e.g., `system-design.md`)
- Proper heading hierarchy (H1 → H2 → H3)
- Code examples with language identifiers
- Mermaid diagrams for architecture and flows

## Links

* [MkDocs Documentation](https://www.mkdocs.org/)
* [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/)
* [MADR Template](template.md)
* Related ADR: [0001-use-aspire-for-orchestration.md](0001-use-aspire-for-orchestration.md)
