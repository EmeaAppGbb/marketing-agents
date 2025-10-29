import { test, expect } from '@playwright/test';

test.describe('Homepage', () => {
  test('should load successfully', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveTitle(/Marketing Agents/);
  });

  test('should display main content', async ({ page }) => {
    await page.goto('/');
    // Wait for the page to be fully loaded
    await page.waitForLoadState('networkidle');

    // Check that the page has loaded
    const body = page.locator('body');
    await expect(body).toBeVisible();
  });
});
