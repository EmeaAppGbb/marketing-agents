# Task 019: E2E Testing Suite Setup

## Description
Set up comprehensive end-to-end testing infrastructure using Playwright to test critical user workflows across the entire application stack.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 016: Campaign Creation UI & Form
- Task 017: Review Dashboard UI
- Task 018: Campaign List & Navigation UI

## Technical Requirements

### Playwright Configuration
Install and configure Playwright:
- Install `@playwright/test` package
- Initialize Playwright with `npx playwright install`
- Configure for multiple browsers (Chromium, Firefox, WebKit)
- Set up test environments (local, CI)

**`playwright.config.ts`:**
```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [
    ['html'],
    ['junit', { outputFile: 'test-results/junit.xml' }],
    ['list'],
  ],
  use: {
    baseURL: process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:3000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
    {
      name: 'mobile-chrome',
      use: { ...devices['Pixel 5'] },
    },
  ],
  webServer: {
    command: 'pnpm dev',
    url: 'http://localhost:3000',
    reuseExistingServer: !process.env.CI,
  },
});
```

### Test Structure
Organize tests by feature:
```
e2e/
├── fixtures/               # Test fixtures and helpers
│   ├── campaignData.ts
│   └── testHelpers.ts
├── pages/                  # Page Object Models
│   ├── CampaignListPage.ts
│   ├── CreateCampaignPage.ts
│   └── CampaignDetailPage.ts
├── tests/                  # Test specs
│   ├── campaign-creation.spec.ts
│   ├── campaign-review.spec.ts
│   ├── campaign-iteration.spec.ts
│   ├── campaign-approval.spec.ts
│   └── realtime-updates.spec.ts
└── utils/                  # Utilities
    ├── mockData.ts
    └── assertions.ts
```

### Page Object Models
Create reusable page objects:

**`e2e/pages/CreateCampaignPage.ts`:**
```typescript
import { Page, Locator } from '@playwright/test';

export class CreateCampaignPage {
  readonly page: Page;
  readonly nameInput: Locator;
  readonly objectiveTextarea: Locator;
  readonly targetAudienceTextarea: Locator;
  readonly productNameInput: Locator;
  readonly productDetailsTextarea: Locator;
  readonly toneSelectorChips: Locator;
  readonly platformCheckboxes: Locator;
  readonly submitButton: Locator;
  readonly saveDraftButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.nameInput = page.getByLabel('Campaign Name');
    this.objectiveTextarea = page.getByLabel('Campaign Objective');
    this.targetAudienceTextarea = page.getByLabel('Target Audience');
    this.productNameInput = page.getByLabel('Product Name');
    this.productDetailsTextarea = page.getByLabel('Product Details');
    this.toneSelectorChips = page.getByRole('group', { name: 'Tone Guidelines' });
    this.platformCheckboxes = page.getByRole('group', { name: 'Platform Selection' });
    this.submitButton = page.getByRole('button', { name: 'Create Campaign' });
    this.saveDraftButton = page.getByRole('button', { name: 'Save Draft' });
  }

  async goto() {
    await this.page.goto('/campaigns/new');
  }

  async fillBasicInfo(name: string, objective: string, audience: string) {
    await this.nameInput.fill(name);
    await this.objectiveTextarea.fill(objective);
    await this.targetAudienceTextarea.fill(audience);
  }

  async fillProductInfo(productName: string, details: string) {
    await this.productNameInput.fill(productName);
    await this.productDetailsTextarea.fill(details);
  }

  async selectTone(tone: string) {
    await this.toneSelectorChips.getByText(tone).click();
  }

  async selectPlatform(platform: string) {
    await this.platformCheckboxes.getByLabel(platform).check();
  }

  async submit() {
    await this.submitButton.click();
  }
}
```

### Critical Test Scenarios

**Test 1: Campaign Creation Flow**
```typescript
test('should create campaign and trigger generation', async ({ page }) => {
  const createPage = new CreateCampaignPage(page);
  await createPage.goto();

  await createPage.fillBasicInfo(
    'Summer Sale Campaign',
    'Promote summer sale with 30% discount',
    'Young professionals aged 25-35'
  );
  await createPage.fillProductInfo(
    'Premium Sneakers',
    'High-quality athletic sneakers with advanced cushioning'
  );
  await createPage.selectTone('Professional');
  await createPage.selectTone('Energetic');
  await createPage.selectPlatform('Instagram');
  await createPage.selectPlatform('Twitter');

  await createPage.submit();

  // Should navigate to campaign detail page
  await expect(page).toHaveURL(/\/campaigns\/[a-f0-9-]+$/);
  
  // Should show generating status
  await expect(page.getByText(/generating/i)).toBeVisible();
});
```

**Test 2: Real-Time Updates**
```typescript
test('should display real-time generation updates', async ({ page }) => {
  // Navigate to campaign detail page
  await page.goto('/campaigns/test-campaign-id');

  // Should show initial status
  await expect(page.getByText('Generating copywriting...')).toBeVisible();

  // Wait for copywriting complete event
  await expect(page.getByText('Copy').locator('..').getByText('✓')).toBeVisible({ timeout: 30000 });

  // Should show next status
  await expect(page.getByText('Generating social media copy...')).toBeVisible();
});
```

**Test 3: Artifact Review and Approval**
```typescript
test('should display artifacts and allow approval', async ({ page }) => {
  const detailPage = new CampaignDetailPage(page);
  await detailPage.goto('completed-campaign-id');

  // Should display copy artifacts
  await expect(detailPage.copySection).toBeVisible();
  await expect(detailPage.copySection.getByText(/headline/i)).toBeVisible();

  // Should display audit results
  await expect(detailPage.auditPanel).toBeVisible();
  await expect(detailPage.auditPanel.getByText(/overall status/i)).toBeVisible();

  // Should allow approval
  await detailPage.approveCopyArtifact();
  await expect(detailPage.copySection.getByText('Approved')).toBeVisible();
});
```

**Test 4: Regeneration with Feedback**
```typescript
test('should regenerate artifact with feedback', async ({ page }) => {
  const detailPage = new CampaignDetailPage(page);
  await detailPage.goto('completed-campaign-id');

  // Open regeneration form
  await detailPage.openRegenerationForm('copy');

  // Submit feedback
  await page.getByLabel('Feedback').fill('Make headlines more creative and engaging');
  await page.getByLabel('Too Conservative').check();
  await page.getByRole('button', { name: 'Regenerate' }).click();

  // Should show loading state
  await expect(page.getByText(/regenerating/i)).toBeVisible();

  // Should update to new version
  await expect(page.getByText(/version 2/i)).toBeVisible({ timeout: 30000 });
});
```

**Test 5: Campaign List and Navigation**
```typescript
test('should list campaigns and navigate to details', async ({ page }) => {
  const listPage = new CampaignListPage(page);
  await listPage.goto();

  // Should display campaign list
  await expect(page.getByRole('article')).toHaveCount(3); // Assuming 3 campaigns

  // Should filter by status
  await listPage.filterByStatus('Completed');
  await expect(page.getByRole('article')).toHaveCount(1);

  // Should navigate to campaign detail
  await page.getByRole('article').first().click();
  await expect(page).toHaveURL(/\/campaigns\/[a-f0-9-]+$/);
});
```

### Test Data Management
Create test fixtures:

**`e2e/fixtures/campaignData.ts`:**
```typescript
export const mockCampaignBrief = {
  name: 'Test Campaign',
  objective: 'Test marketing campaign for E2E testing purposes',
  targetAudience: 'Test audience aged 25-45',
  productName: 'Test Product',
  productDetails: 'This is a test product for automated testing',
  toneGuidelines: ['Professional', 'Friendly'],
  selectedPlatforms: ['Instagram', 'Twitter'],
};

export const mockCompletedCampaign = {
  id: 'test-campaign-id',
  name: 'Completed Test Campaign',
  status: 'Completed',
  artifacts: {
    copy: { /* ... */ },
    shortCopy: { /* ... */ },
    visualConcept: { /* ... */ },
  },
  audit: { /* ... */ },
};
```

### API Mocking
Set up API mocking for tests:
- Use Playwright's route interception
- Mock backend API responses
- Control timing for real-time event testing

**API Mock Example:**
```typescript
test.beforeEach(async ({ page }) => {
  // Mock campaign generation endpoint
  await page.route('**/api/campaigns/*/generate', async (route) => {
    await route.fulfill({
      status: 202,
      body: JSON.stringify({ runId: 'test-run-id' }),
    });
  });
});
```

### CI/CD Integration
Configure for GitHub Actions:

**`.github/workflows/e2e-tests.yml`:**
```yaml
name: E2E Tests

on:
  pull_request:
  push:
    branches: [main]

jobs:
  e2e-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
      - uses: pnpm/action-setup@v2
      
      - name: Install dependencies
        run: pnpm install
      
      - name: Install Playwright Browsers
        run: pnpm exec playwright install --with-deps
      
      - name: Run E2E tests
        run: pnpm test:e2e
        env:
          PLAYWRIGHT_BASE_URL: http://localhost:3000
      
      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
          retention-days: 30
```

## Acceptance Criteria
- [ ] Playwright configured for multiple browsers
- [ ] Page Object Models created for all main pages
- [ ] Critical user flow tests implemented (creation, review, approval, regeneration)
- [ ] Real-time update tests working
- [ ] Test data fixtures created
- [ ] API mocking set up for consistent tests
- [ ] CI/CD integration configured
- [ ] Test reports generated (HTML, JUnit)
- [ ] Screenshot and video capture on failure
- [ ] All tests passing consistently (≥95% pass rate)

## Testing Requirements
- [ ] At least 10 E2E test scenarios covering critical paths
- [ ] Tests for campaign creation flow
- [ ] Tests for real-time updates
- [ ] Tests for artifact review and approval
- [ ] Tests for regeneration with feedback
- [ ] Tests for campaign list and filtering
- [ ] Tests for responsive design (mobile, desktop)
- [ ] Tests for error handling scenarios
- [ ] Cross-browser testing (Chromium, Firefox, WebKit)
- [ ] Accessibility testing integration

## Non-Functional Requirements
- Test execution time <5 minutes for full suite
- Test reliability ≥95% pass rate
- Parallel test execution support
- Clear test failure reporting
- Screenshot/video capture on failure

## Out of Scope
- Load testing (separate task)
- Security testing (separate task)
- Visual regression testing (future)
- Performance profiling (future)

## Notes
- Follow Playwright best practices
- Use Page Object Model pattern consistently
- Keep tests independent and idempotent
- Use meaningful test descriptions
- Implement proper wait strategies (avoid hard waits)
- Test critical paths thoroughly
- Document test setup and execution
- Create helper functions for common operations
- Consider test data cleanup strategy
- Run tests in CI on every PR
