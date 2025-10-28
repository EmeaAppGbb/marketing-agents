# Task 020: CI/CD Pipeline & Deployment Configuration

## Description
Set up comprehensive CI/CD pipeline using GitHub Actions with automated testing, linting, building, security scanning, and deployment workflows. Configure deployment manifests for Azure Container Apps using Aspire publish.

## Dependencies
- All previous tasks (full application stack)

## Technical Requirements

### GitHub Actions Workflow Structure
Create reusable workflow templates in `.github/workflows/`:

**Main Workflows:**
1. `ci.yml` - Continuous Integration (PR validation)
2. `cd.yml` - Continuous Deployment (main branch)
3. `pr-preview.yml` - Preview deployment for PRs
4. `security-scan.yml` - Security scanning
5. `dependency-update.yml` - Automated dependency updates

### CI Pipeline (PR Validation)
**`.github/workflows/ci.yml`:**

**Stages:**
1. **Lint Stage**
   - Backend: `dotnet format --verify-no-changes`
   - Frontend: ESLint, Prettier, Stylelint
   - Fail fast on linting errors

2. **Type Check Stage**
   - Backend: C# compilation with nullable warnings as errors
   - Frontend: `tsc --noEmit`

3. **Restore & Build Stage**
   - Backend: `dotnet restore` + `dotnet build`
   - Frontend: `pnpm install` + `pnpm build`
   - Use caching for dependencies

4. **Unit Tests Stage**
   - Backend: `dotnet test` with coverage
   - Frontend: Vitest with coverage
   - Fail if coverage <85%
   - Generate coverage reports

5. **Integration Tests Stage**
   - Backend: Integration tests with Testcontainers
   - Frontend: Component integration tests

6. **E2E Tests Stage**
   - Playwright tests across browsers
   - Parallel execution

7. **Security Scan Stage**
   - CodeQL analysis
   - Dependency vulnerability scanning
   - SAST (Static Application Security Testing)

**Example CI Workflow:**
```yaml
name: CI

on:
  pull_request:
    branches: [main, develop]
  push:
    branches: [main, develop]

jobs:
  lint:
    name: Lint Code
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
      
      - uses: pnpm/action-setup@v2
        with:
          version: 8
      
      - name: Restore .NET tools
        run: dotnet tool restore
      
      - name: Check .NET formatting
        run: dotnet format --verify-no-changes
      
      - name: Install frontend dependencies
        run: pnpm install --frozen-lockfile
      
      - name: Lint frontend
        run: pnpm lint

  type-check:
    name: Type Check
    runs-on: ubuntu-latest
    needs: lint
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Build .NET projects
        run: dotnet build --configuration Release --no-restore
      
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
      
      - uses: pnpm/action-setup@v2
      
      - name: Install dependencies
        run: pnpm install --frozen-lockfile
      
      - name: Type check frontend
        run: pnpm type-check

  test-backend:
    name: Backend Tests
    runs-on: ubuntu-latest
    needs: type-check
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Run unit tests
        run: dotnet test --configuration Release --no-restore --collect:"XPlat Code Coverage" --results-directory ./coverage
      
      - name: Upload coverage
        uses: codecov/codecov-action@v4
        with:
          files: ./coverage/**/coverage.cobertura.xml
          flags: backend

  test-frontend:
    name: Frontend Tests
    runs-on: ubuntu-latest
    needs: type-check
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
      
      - uses: pnpm/action-setup@v2
      
      - name: Install dependencies
        run: pnpm install --frozen-lockfile
      
      - name: Run unit tests
        run: pnpm test:coverage
      
      - name: Upload coverage
        uses: codecov/codecov-action@v4
        with:
          files: ./coverage/coverage-final.json
          flags: frontend

  e2e-tests:
    name: E2E Tests
    runs-on: ubuntu-latest
    needs: [test-backend, test-frontend]
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'
      
      - uses: pnpm/action-setup@v2
      
      - name: Install dependencies
        run: pnpm install --frozen-lockfile
      
      - name: Install Playwright
        run: pnpm exec playwright install --with-deps
      
      - name: Run E2E tests
        run: pnpm test:e2e
      
      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report
          path: playwright-report/

  security-scan:
    name: Security Scan
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Run CodeQL
        uses: github/codeql-action/init@v3
        with:
          languages: csharp, javascript
      
      - name: Autobuild
        uses: github/codeql-action/autobuild@v3
      
      - name: Perform CodeQL Analysis
        uses: github/codeql-action/analyze@v3
```

### CD Pipeline (Deployment)
**`.github/workflows/cd.yml`:**

**Deployment Stages:**
1. **Build & Publish**
   - Build backend services
   - Build frontend application
   - Generate deployment manifests using `aspire publish`
   - Create container images (if needed)
   - Tag images with version/commit SHA

2. **Deploy to Staging**
   - Deploy to Azure Container Apps (staging)
   - Run smoke tests
   - Health check validation

3. **Deploy to Production**
   - Manual approval required
   - Blue-green or canary deployment
   - Automated health checks
   - Rollback capability

**Aspire Deployment:**
```yaml
deploy-staging:
  name: Deploy to Staging
  runs-on: ubuntu-latest
  needs: [test-backend, test-frontend, e2e-tests]
  if: github.ref == 'refs/heads/main'
  environment:
    name: staging
    url: https://staging.marketingagents.example.com
  steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
    
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Generate deployment manifests
      run: dotnet run --project MarketingAgents.AppHost -- publish --output-path ./deploy
    
    - name: Deploy to Azure Container Apps
      run: |
        az containerapp update \
          --name marketing-agents-api \
          --resource-group marketing-agents-staging \
          --image ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

### Deployment Manifest Generation
Use Aspire publish to generate deployment manifests:

**Generate Manifests:**
```bash
dotnet run --project MarketingAgents.AppHost -- publish --output-path ./deploy
```

**Generated Artifacts:**
- Bicep templates for Azure infrastructure
- Container app configurations
- Service configurations
- Environment variable templates

### Environment Configuration
Set up environment-specific configurations:

**Environments:**
1. **Development** - Local development
2. **Staging** - Pre-production testing
3. **Production** - Live environment

**GitHub Secrets:**
- `AZURE_CREDENTIALS` - Azure service principal
- `OPENAI_API_KEY` - Azure OpenAI key
- `COSMOS_CONNECTION_STRING` - Cosmos DB connection
- `SIGNALR_CONNECTION_STRING` - Azure SignalR Service

**Environment Variables:**
- Database connection strings
- API keys and secrets
- Feature flags
- Logging levels
- CORS origins

### Preview Deployments
**`.github/workflows/pr-preview.yml`:**

**PR Preview Features:**
- Deploy ephemeral environment per PR
- Unique URL per PR
- Automatic cleanup on PR close
- Comment on PR with preview URL

```yaml
name: PR Preview

on:
  pull_request:
    types: [opened, synchronize, reopened, closed]

jobs:
  deploy-preview:
    if: github.event.action != 'closed'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      # Build and deploy to unique preview environment
      - name: Deploy preview
        run: |
          export PR_NUMBER=${{ github.event.pull_request.number }}
          # Deploy to preview-pr-${PR_NUMBER}.example.com
      
      - name: Comment PR
        uses: actions/github-script@v7
        with:
          script: |
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: '🚀 Preview deployed to https://preview-pr-${{ github.event.pull_request.number }}.example.com'
            })

  cleanup-preview:
    if: github.event.action == 'closed'
    runs-on: ubuntu-latest
    steps:
      - name: Delete preview environment
        run: |
          export PR_NUMBER=${{ github.event.pull_request.number }}
          # Cleanup preview-pr-${PR_NUMBER} resources
```

### Caching Strategy
Optimize build times with caching:

**Cache Targets:**
- NuGet packages
- npm/pnpm modules
- .NET build artifacts
- Docker layers (if applicable)

**Cache Configuration:**
```yaml
- name: Cache NuGet packages
  uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json') }}

- name: Cache pnpm store
  uses: actions/cache@v4
  with:
    path: ~/.pnpm-store
    key: ${{ runner.os }}-pnpm-${{ hashFiles('**/pnpm-lock.yaml') }}
```

### Quality Gates
Enforce quality standards:

**Quality Checks:**
- Code coverage ≥85%
- No critical security vulnerabilities
- All tests passing
- No linting errors
- Documentation updated
- MkDocs build passes

**Branch Protection Rules:**
- Require PR reviews (≥2 approvers)
- Require status checks to pass
- Require up-to-date branches
- No direct commits to main

### Monitoring & Alerts
Set up deployment monitoring:

**Post-Deployment:**
- Health check validation
- Smoke test execution
- Performance baseline checks
- Error rate monitoring
- Alert on deployment failures

### Rollback Strategy
Implement rollback mechanisms:

**Rollback Triggers:**
- Health check failures
- Error rate spike
- Manual rollback request

**Rollback Process:**
- Revert to previous container image tag
- Restore previous configuration
- Validate rollback success

### Documentation
Create deployment documentation:

**Required Documentation:**
- Deployment runbook in MkDocs
- Environment setup guide
- Secret management guide
- Rollback procedures
- Troubleshooting guide

## Acceptance Criteria
- [ ] CI pipeline configured with all stages (lint, type-check, test, security)
- [ ] CD pipeline configured with staging and production deployment
- [ ] PR preview deployments working
- [ ] Aspire publish generating deployment manifests
- [ ] All quality gates enforced (coverage ≥85%)
- [ ] Branch protection rules configured
- [ ] GitHub secrets configured for all environments
- [ ] Caching configured for optimal build times
- [ ] Security scanning integrated (CodeQL, dependency scan)
- [ ] Deployment documentation in MkDocs
- [ ] Health checks post-deployment
- [ ] Rollback procedure documented and tested

## Testing Requirements
- [ ] Test CI pipeline with sample PR
- [ ] Test CD pipeline deployment to staging
- [ ] Test preview deployment creation and cleanup
- [ ] Test rollback procedure
- [ ] Verify quality gate enforcement
- [ ] Verify security scanning catches vulnerabilities
- [ ] Test caching effectiveness (build time reduction)

## Non-Functional Requirements
- CI pipeline execution time <10 minutes
- CD pipeline execution time <15 minutes
- Build time improvement ≥50% with caching
- Deployment reliability ≥99%
- Rollback time <5 minutes

## Out of Scope
- Multi-region deployment (future)
- Canary deployment strategy (future, use blue-green for MVP)
- Automated performance testing in pipeline
- Infrastructure as Code for all resources (use Aspire-generated manifests)

## Notes
- Follow AGENTS.md CI/CD pipeline expectations
- Use `dotnet` CLI for all backend operations
- Use pnpm for all frontend operations
- Leverage Aspire publish for deployment manifest generation
- Deploy to Azure Container Apps (not AppHost itself)
- Document all deployment procedures in MkDocs
- Test rollback procedure regularly
- Monitor deployment success rates
- Create MADR for deployment strategy decisions
- Keep secrets secure (never commit, use GitHub Secrets)
- Implement proper secret rotation procedures
