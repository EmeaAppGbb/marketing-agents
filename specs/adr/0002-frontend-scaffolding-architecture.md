# ADR 0002: Frontend Scaffolding Architecture - Next.js, TanStack Query, and Zustand

## Status

Accepted

## Context

The Marketing Agents application requires a modern, performant frontend that integrates with the .NET backend API and supports real-time updates via SignalR. The frontend must provide an excellent developer experience, strong type safety, efficient state management, and comprehensive testing capabilities.

### Decision Drivers

- **Type Safety**: End-to-end type safety from API to UI components
- **Performance**: Fast initial load times, optimized bundle sizes, and smooth interactions
- **Developer Experience**: Modern tooling, hot module replacement, and clear patterns
- **State Management**: Clear separation between server state and local UI state
- **Testing**: High test coverage with unit, integration, and E2E tests
- **Maintainability**: Clear folder structure and modular architecture
- **Integration**: Seamless integration with .NET Aspire for local development

## Options Considered

### Option 1: Next.js 14 + TanStack Query + Zustand (Selected)

**Framework**: Next.js 14 with App Router  
**Server State**: TanStack Query (React Query)  
**Local State**: Zustand  
**Styling**: Tailwind CSS  
**Forms**: React Hook Form + Zod  
**Testing**: Vitest + React Testing Library + Playwright

**Pros**:
- ✅ Best-in-class TypeScript support with strict mode
- ✅ TanStack Query provides excellent server state management with caching, retries, and optimistic updates
- ✅ Zustand is lightweight (< 1KB) and provides simple, unopinionated local state management
- ✅ Next.js App Router enables Server Components for better performance and SEO
- ✅ Vitest is fast, compatible with Vite, and has excellent TypeScript support
- ✅ Wide community adoption and extensive documentation
- ✅ Easy integration with .NET Aspire via AddNpmApp
- ✅ Tailwind CSS provides utility-first styling with excellent DX

**Cons**:
- ⚠️ Learning curve for App Router patterns (Server vs Client Components)
- ⚠️ TanStack Query adds bundle size (~15KB)
- ⚠️ Requires understanding of multiple state management concepts

**Implementation Details**:
- TypeScript strict mode with `noUncheckedIndexedAccess` and `noImplicitOverride`
- TanStack Query for all API data fetching with query key factories
- Zustand stores for local UI state (sidebar, modals, etc.)
- Path aliases (`@/*`) for clean imports
- Pre-commit hooks with Husky + lint-staged
- ESLint flat config with TypeScript rules
- Vitest with 45% minimum coverage baseline (increasing to 85% as features are added)

### Option 2: Next.js + Redux Toolkit + RTK Query

**Framework**: Next.js 14 with App Router  
**Server State**: RTK Query  
**Local State**: Redux Toolkit  
**Styling**: Tailwind CSS  
**Forms**: React Hook Form + Zod  
**Testing**: Jest + React Testing Library + Playwright

**Pros**:
- ✅ Redux is well-known and has extensive ecosystem
- ✅ RTK Query provides excellent code generation from OpenAPI
- ✅ Single store for all state (server + local)
- ✅ Redux DevTools for debugging
- ✅ Strong TypeScript support

**Cons**:
- ❌ More boilerplate code required (actions, reducers, selectors)
- ❌ Larger bundle size (~30KB for Redux Toolkit)
- ❌ Higher complexity for simple use cases
- ❌ Overkill for local UI state management
- ❌ Jest is slower than Vitest
- ❌ Redux patterns can be verbose

**Why Not Selected**: While Redux Toolkit is powerful, it introduces unnecessary complexity for this application. The combination of TanStack Query + Zustand provides cleaner separation of concerns between server state and local state with less boilerplate.

### Option 3: Next.js + SWR + React Context

**Framework**: Next.js 14 with App Router  
**Server State**: SWR  
**Local State**: React Context + useReducer  
**Styling**: Tailwind CSS  
**Forms**: React Hook Form + Zod  
**Testing**: Vitest + React Testing Library + Playwright

**Pros**:
- ✅ Minimal dependencies (SWR is lightweight)
- ✅ Built-in React patterns (Context API)
- ✅ SWR has automatic revalidation features
- ✅ Simpler mental model

**Cons**:
- ❌ SWR lacks advanced features (mutations, optimistic updates not as robust)
- ❌ Context API causes unnecessary re-renders at scale
- ❌ No built-in devtools for debugging
- ❌ Manual query key management
- ❌ Context performance issues with frequent updates
- ❌ Less TypeScript inference compared to TanStack Query

**Why Not Selected**: While SWR is simpler, TanStack Query provides better TypeScript support, more robust mutation handling, and superior devtools. React Context doesn't scale well for complex local state compared to Zustand.

## Decision Outcome

**Chosen Option**: Next.js 14 + TanStack Query + Zustand

### Positive Consequences

- **Type Safety**: Full TypeScript strict mode with excellent type inference across the stack
- **Performance**: Server Components reduce client-side JavaScript; TanStack Query provides intelligent caching
- **Developer Experience**: Fast HMR with Vitest, excellent DevTools, minimal boilerplate
- **Scalability**: Clear separation of server state (TanStack Query) and local state (Zustand)
- **Testing**: Fast unit tests with Vitest, comprehensive E2E with Playwright
- **Maintainability**: Modular folder structure with features, components, and hooks clearly separated
- **Integration**: Seamless local development with .NET Aspire orchestration

### Negative Consequences

- **Learning Curve**: Team needs to learn App Router patterns and TanStack Query concepts
- **Migration Path**: Future migration to alternative state management requires refactoring
- **Coverage Baseline**: Starting with 45% coverage requires disciplined incremental improvement

### Mitigation Strategies

1. **Learning Curve**: Provide comprehensive code samples, establish clear patterns in codebase
2. **Migration Risk**: Abstract API calls behind custom hooks to isolate TanStack Query usage
3. **Coverage**: Set up automated coverage gates in CI, track coverage trends, enforce reviews

## Follow-Up Actions

1. ✅ Create sample components demonstrating patterns (CampaignCard completed)
2. ✅ Set up query key factories for type-safe cache invalidation
3. ⏳ Create Storybook for component documentation (future task)
4. ⏳ Implement SignalR integration with TanStack Query (future task)
5. ⏳ Add more comprehensive E2E tests for critical user flows (future task)
6. ⏳ Gradually increase coverage thresholds to 85% as features are implemented

## References

- [TanStack Query Documentation](https://tanstack.com/query/latest)
- [Zustand Documentation](https://docs.pmnd.rs/zustand/getting-started/introduction)
- [Next.js App Router](https://nextjs.org/docs/app)
- [Vitest Documentation](https://vitest.dev/)
- [AGENTS.md Frontend Guidelines](../../AGENTS.md#frontend-development-nextjs--react--typescript)
- [Microsoft Docs: Aspire Node.js Integration](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/build-aspire-apps-with-nodejs)
