# Development Workflow

This guide describes the recommended development workflow for contributing to the Marketing Agents Platform.

## Overview

The development workflow follows these phases:

1. **Plan** - Review specifications and create tasks
2. **Develop** - Implement features following coding standards
3. **Test** - Write and run comprehensive tests
4. **Review** - Submit PR for code review
5. **Deploy** - Merge and deploy to staging/production

## Branch Strategy

### Main Branches

- `main` - Production-ready code, protected branch
- `develop` - Integration branch for features (if using GitFlow)

### Feature Branches

Create feature branches from `main` (or `develop`):

```bash
git checkout main
git pull origin main
git checkout -b feature/campaign-iteration-feedback
```

**Branch Naming Convention**:
- `feature/<feature-name>` - New features
- `fix/<bug-description>` - Bug fixes
- `docs/<doc-update>` - Documentation updates
- `refactor/<refactor-description>` - Code refactoring
- `test/<test-description>` - Test additions/improvements

### Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types**:
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation changes
- `style` - Code style changes (formatting, no logic changes)
- `refactor` - Code refactoring
- `test` - Adding or updating tests
- `chore` - Maintenance tasks

**Examples**:
```bash
git commit -m "feat(agents): add copywriting agent with retry logic"
git commit -m "fix(api): resolve null reference in campaign controller"
git commit -m "docs: update installation guide with Cosmos DB setup"
git commit -m "test(agents): add integration tests for audit agent"
```

## Development Cycle

### 1. Pick a Task

Review tasks in the project board or issues:

```bash
# Assign yourself to a GitHub issue
# Move issue to "In Progress" column
```

### 2. Create Feature Branch

```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

### 3. Implement Feature

Follow the team coding standards (see repository root AGENTS.md):

#### Backend (.NET)
```bash
# Navigate to project
cd MarketingAgents.Api

# Run the service with hot reload
dotnet watch

# In another terminal, run tests
dotnet test
```

#### Frontend (Next.js)
```bash
# Navigate to web project
cd MarketingAgents.Web

# Install dependencies
pnpm install

# Run dev server
pnpm dev

# In another terminal, run tests
pnpm test
```

### 4. Write Tests

#### Backend Tests (xUnit)
```csharp
public class CampaignServiceTests
{
    [Fact]
    public async Task CreateCampaign_ValidBrief_ReturnsCampaign()
    {
        // Arrange
        var service = CreateService();
        var brief = new CampaignBrief { Theme = "Test" };

        // Act
        var result = await service.CreateCampaignAsync(brief);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CampaignStatus.Created);
    }
}
```

#### Frontend Tests (Vitest + React Testing Library)
```typescript
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { CampaignCard } from './CampaignCard';

describe('CampaignCard', () => {
  it('renders campaign theme', () => {
    const campaign = { id: '1', theme: 'Summer Launch', status: 'Created' };
    render(<CampaignCard campaign={campaign} />);
    
    expect(screen.getByText('Summer Launch')).toBeInTheDocument();
  });
});
```

### 5. Run Quality Checks

Before committing, run all quality checks:

#### Backend
```bash
# Format code
dotnet format

# Run linters (Roslyn analyzers run during build)
dotnet build

# Run all tests
dotnet test

# Check code coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### Frontend
```bash
# Lint code
pnpm lint

# Format code
pnpm format

# Type check
pnpm type-check

# Run unit tests
pnpm test

# Run E2E tests
pnpm test:e2e
```

### 6. Commit Changes

```bash
# Stage changes
git add .

# Commit with conventional commit message
git commit -m "feat(api): add campaign revision endpoint"

# Pre-commit hooks will run automatically (formatting, linting)
```

### 7. Push and Create PR

```bash
# Push feature branch
git push origin feature/your-feature-name

# Create Pull Request on GitHub
# Use PR template and fill in details
```

## Pull Request Process

### PR Template

Pull requests should include:

```markdown
## Description
Brief description of changes

## Related Issue
Closes #123

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] E2E tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows team coding standards
- [ ] Tests pass locally
- [ ] Documentation updated
- [ ] MADR created (if architectural decision)
- [ ] No merge conflicts
```

### Code Review Checklist

Reviewers should verify:

- ✅ **Functionality** - Code solves the stated problem
- ✅ **Tests** - Comprehensive test coverage (≥85%)
- ✅ **Standards** - Follows AGENTS.md coding standards
- ✅ **Documentation** - Inline comments and docs updated
- ✅ **MADR** - Architectural decisions documented
- ✅ **Performance** - No obvious performance issues
- ✅ **Security** - No security vulnerabilities
- ✅ **Type Safety** - Strong typing enforced

### Merge Requirements

PRs must meet these criteria before merging:

- ✅ **CI Passes** - All GitHub Actions workflows pass
- ✅ **Code Coverage** - Coverage ≥85%
- ✅ **Approvals** - At least 2 approvals (1 for docs-only)
- ✅ **No Conflicts** - Merge conflicts resolved
- ✅ **Documentation** - Docs updated in same PR

### Merge Strategy

Use **Squash and Merge** by default:

```bash
# This creates a single commit on main with a clean history
# Commit message should follow conventional commits format
```

## Local Development Tips

### Running Specific Services

```bash
# Run only API service
dotnet run --project MarketingAgents.Api

# Run only AgentHost service
dotnet run --project MarketingAgents.AgentHost

# Run Aspire dashboard (all services)
dotnet run --project MarketingAgents.AppHost
```

### Debugging

#### Visual Studio / Rider
1. Set breakpoints in code
2. Press **F5** to start debugging
3. Interact with API to trigger breakpoints

#### VS Code
1. Set breakpoints in code
2. Open **Run and Debug** view (Cmd+Shift+D)
3. Select **.NET Aspire** configuration
4. Press **F5** to start debugging

### Database Queries

```bash
# Access local Cosmos DB Emulator
open https://localhost:8081/_explorer/index.html

# Access Redis CLI
docker exec -it <redis-container-id> redis-cli
```

### Viewing Logs

```bash
# View logs from all services (Aspire Dashboard)
# Navigate to http://localhost:15888 (Aspire dashboard)

# View logs from specific service
docker logs <container-id>

# Follow logs in real-time
docker logs -f <container-id>
```

## Troubleshooting Common Issues

### Build Failures

**Issue**: `dotnet build` fails with missing package errors

**Solution**:
```bash
dotnet clean
dotnet restore
dotnet build
```

---

**Issue**: TypeScript build fails with type errors

**Solution**:
```bash
# Delete node_modules and lock file
rm -rf node_modules pnpm-lock.yaml

# Reinstall dependencies
pnpm install

# Run type check
pnpm type-check
```

### Test Failures

**Issue**: Integration tests fail due to database connection

**Solution**:
```bash
# Ensure Docker is running
docker ps

# Restart Cosmos DB emulator
docker restart <cosmosdb-container-id>

# Verify connection string in appsettings.Development.json
```

---

**Issue**: E2E tests fail with timeout

**Solution**:
```bash
# Increase timeout in playwright.config.ts
export default defineConfig({
  timeout: 60000, // Increase to 60 seconds
});

# Run Playwright in headed mode to debug
pnpm test:e2e --headed
```

### Git Issues

**Issue**: Merge conflicts on pull from main

**Solution**:
```bash
git checkout feature/your-feature
git fetch origin
git rebase origin/main

# Resolve conflicts in files
# After resolving:
git add .
git rebase --continue

# Force push (rebase rewrites history)
git push --force-with-lease origin feature/your-feature
```

---

**Issue**: Pre-commit hooks failing

**Solution**:
```bash
# Run formatters manually
dotnet format                    # Backend
pnpm format                      # Frontend

# Commit again
git commit -m "your message"
```

## Best Practices

### Code Quality

1. **Write self-documenting code** - Clear variable names, logical structure
2. **Add comments for "why" not "what"** - Explain reasoning, not mechanics
3. **Keep functions small** - Single Responsibility Principle
4. **Use strongly-typed interfaces** - Avoid `any` in TypeScript, avoid `dynamic` in C#
5. **Handle errors gracefully** - Use try-catch, return Problem Details for APIs

### Testing

1. **Test behavior, not implementation** - Focus on outcomes
2. **Use descriptive test names** - `CreateCampaign_ValidBrief_ReturnsCampaign`
3. **Arrange-Act-Assert pattern** - Structure tests clearly
4. **Avoid test interdependencies** - Tests should run independently
5. **Mock external dependencies** - Use test doubles for databases, APIs

### Documentation

1. **Update docs in same PR** - Don't defer documentation
2. **Use MkDocs format** - Write in Markdown, follow structure
3. **Include code examples** - Show practical usage
4. **Create MADRs for decisions** - Document architectural choices
5. **Keep README.md updated** - Ensure setup instructions work

### Performance

1. **Async all the way** - Use async/await for I/O operations
2. **Avoid blocking calls** - No `.Result` or `.Wait()` in C#
3. **Use caching wisely** - Cache expensive operations, invalidate properly
4. **Optimize database queries** - Use indexes, partition keys
5. **Profile before optimizing** - Measure first, optimize second

## Next Steps

- [Deployment Guide](deployment.md) - Deploy to Azure
- [Troubleshooting Guide](troubleshooting.md) - Common issues and solutions
- [Architecture Overview](../architecture/overview.md) - System design
