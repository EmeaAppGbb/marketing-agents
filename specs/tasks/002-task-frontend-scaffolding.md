# Task 002: Frontend Scaffolding - Next.js App Structure

## Description
Set up the frontend application using Next.js 14 with App Router, React 18, and TypeScript. Configure the development environment with proper tooling, state management, and testing infrastructure following the AGENTS.md specifications.

## Dependencies
- Task 001: Backend Scaffolding (for API endpoint integration)

## Technical Requirements

### Project Creation
- Create Next.js 14 application using App Router: `MarketingAgents.Web`
- Configure TypeScript with strict mode enabled
- Set up pnpm workspace mode for package management
- Install and configure latest stable npm packages

### TypeScript Configuration
Create `tsconfig.json` with strict settings:
- `strict: true`
- `noUncheckedIndexedAccess: true`
- `noImplicitOverride: true`
- Configure path aliases for cleaner imports

### Folder Structure
```
app/                      # Next.js App Router routes
├── (server-actions)/     # Server actions isolation
├── layout.tsx           # Root layout
├── page.tsx             # Landing page
components/              # Shared UI components
features/                # Feature-specific modules
├── campaign/            # Campaign-related components
├── review/              # Review dashboard components
hooks/                   # Custom React hooks
lib/                     # Utility functions and shared logic
├── api/                 # API client configuration
├── validation/          # Zod schemas
styles/                  # Global styles and themes
public/                  # Static assets
```

### State Management Setup
- Install and configure TanStack Query for remote data caching
- Set up Zustand or Jotai for local ephemeral state
- Configure query client with proper defaults (staleTime, cacheTime, retries)
- Create API client hooks using TanStack Query

### UI Framework & Styling
- Install and configure a UI component library (Shadcn/ui recommended)
- Set up Tailwind CSS for styling
- Configure design tokens using CSS variables
- Create theme configuration (colors, typography, spacing)
- Set up global styles and CSS reset

### Code Quality Tools
- Configure ESLint with flat config format (`eslint.config.js`)
- Set up Prettier for code formatting
- Configure Stylelint for CSS/SCSS linting
- Set up pre-commit hooks with Husky
- Configure lint-staged for automated formatting

### Form Handling
- Install React Hook Form for form management
- Set up Zod for schema validation
- Create reusable form components and validation schemas

### Testing Infrastructure
- Configure Vitest for unit testing
- Set up React Testing Library for component testing
- Install Playwright for E2E testing
- Create test utilities and setup files
- Configure test coverage reporting (≥85% target)

### Build Configuration
- Configure Next.js for optimal performance
- Set up environment variable handling
- Configure bundle analyzer for optimization
- Set up source maps for debugging

### Development Tools
- Configure VS Code settings and extensions recommendations
- Set up debugging configuration
- Create npm scripts for common tasks (dev, build, test, lint)

## Acceptance Criteria
- [ ] Next.js application running successfully on localhost
- [ ] TypeScript strict mode enabled and no type errors
- [ ] All folder structure created per specification
- [ ] TanStack Query configured and query client provider set up
- [ ] UI component library installed and theme configured
- [ ] ESLint, Prettier, and Stylelint running without errors
- [ ] Pre-commit hooks executing successfully
- [ ] Sample component with unit test passing
- [ ] E2E test framework configured with sample test
- [ ] Application builds successfully with no warnings
- [ ] Development documentation in README.md

## Testing Requirements
- [ ] Unit test suite configured with Vitest
- [ ] Sample component test using React Testing Library
- [ ] E2E test configured with Playwright
- [ ] Test coverage reporting enabled
- [ ] All tests passing with ≥85% coverage baseline
- [ ] Integration with CI pipeline for automated testing

## Non-Functional Requirements
- Initial page load (LCP) < 2.5s
- First Contentful Paint < 1.5s
- TypeScript compilation time < 5s for incremental builds
- Hot module replacement working smoothly

## Out of Scope
- Actual feature components (covered in feature tasks)
- SignalR real-time integration (covered in realtime task)
- Authentication UI (not in MVP)
- Production deployment configuration

## Notes
- Follow AGENTS.md frontend development guidelines
- Use latest stable versions of all npm packages
- Ensure pnpm-lock.yaml is committed
- Configure for development with production-ready patterns
- Integrate with Aspire AppHost for local development orchestration
