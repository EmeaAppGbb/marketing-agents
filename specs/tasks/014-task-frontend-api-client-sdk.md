# Task 014: Frontend API Client & SDK Generation

## Description
Generate TypeScript SDK from OpenAPI specification and set up API client infrastructure with TanStack Query for data fetching, caching, and state management.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 013: Campaign API Endpoints (for OpenAPI spec)

## Technical Requirements

### OpenAPI SDK Generation
Set up automatic SDK generation:
- Install `openapi-typescript` and `openapi-fetch` packages
- Create generation script in package.json
- Point to backend OpenAPI spec: `/packages/sdk/openapi.json`
- Generate TypeScript types and fetch client
- Output to `/packages/sdk/src/generated/`
- Include generation in build pipeline

**Generation Command:**
```bash
pnpm run generate:sdk
```

**Generated Artifacts:**
- TypeScript type definitions for all DTOs
- Type-safe fetch client functions
- Request/response types
- Enum definitions

### SDK Package Structure
Create `/packages/sdk/` workspace package:
```
packages/sdk/
├── package.json
├── src/
│   ├── generated/          # Auto-generated from OpenAPI
│   │   ├── types.ts
│   │   └── client.ts
│   ├── client.ts           # Configured API client
│   ├── hooks/              # React Query hooks
│   │   ├── useCampaigns.ts
│   │   ├── useOrchestration.ts
│   │   ├── useArtifacts.ts
│   │   ├── useAudit.ts
│   │   └── useIteration.ts
│   └── index.ts            # Public exports
└── tsconfig.json
```

### API Client Configuration
Create configured API client:

**`client.ts`:**
- Wrap generated fetch client with configuration
- Set base URL from environment variables
- Add request/response interceptors
- Configure timeout and retry policies
- Add correlation ID headers
- Handle authentication (future)

**Configuration:**
```typescript
const apiClient = createClient({
  baseUrl: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});
```

### TanStack Query Integration
Install and configure TanStack Query (React Query):
- Install `@tanstack/react-query`
- Install `@tanstack/react-query-devtools`
- Create QueryClient configuration
- Set up QueryClientProvider in app layout
- Configure default query options

**QueryClient Configuration:**
```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60 * 1000, // 1 minute
      cacheTime: 5 * 60 * 1000, // 5 minutes
      refetchOnWindowFocus: false,
      retry: 2,
    },
    mutations: {
      retry: 1,
    },
  },
});
```

### React Query Hooks
Create type-safe hooks for all API operations:

**Campaign Hooks (`useCampaigns.ts`):**
- `useCreateCampaign()` - mutation for creating campaigns
- `useCampaign(id)` - query for single campaign
- `useCampaigns(params)` - query for campaign list with pagination
- `useDeleteCampaign()` - mutation for deleting campaigns

**Orchestration Hooks (`useOrchestration.ts`):**
- `useGenerateCampaign()` - mutation for triggering generation
- `useCancelOrchestration()` - mutation for cancellation
- `useOrchestrationRuns(campaignId)` - query for run history
- `useOrchestrationRun(campaignId, runId)` - query for run details

**Artifact Hooks (`useArtifacts.ts`):**
- `useArtifactBundle(campaignId)` - query for all artifacts
- `useArtifact(campaignId, artifactType)` - query for specific artifact
- `useArtifactVersions(campaignId, artifactType)` - query for version history
- `useApproveArtifact()` - mutation for approval

**Audit Hooks (`useAudit.ts`):**
- `useLatestAudit(campaignId)` - query for latest audit report
- `useAuditHistory(campaignId)` - query for audit history
- `useArtifactAudit(campaignId, artifactType)` - query for artifact audit

**Iteration Hooks (`useIteration.ts`):**
- `useRegenerateArtifact()` - mutation for regeneration with feedback
- `useIterationHistory(campaignId, artifactType)` - query for iteration history

### Hook Implementation Pattern
Use consistent pattern for all hooks:

**Query Hook Example:**
```typescript
export function useCampaign(id: string) {
  return useQuery({
    queryKey: ['campaign', id],
    queryFn: async () => {
      const response = await apiClient.GET('/api/campaigns/{id}', {
        params: { path: { id } },
      });
      if (response.error) throw new Error(response.error);
      return response.data;
    },
    enabled: !!id,
  });
}
```

**Mutation Hook Example:**
```typescript
export function useCreateCampaign() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (request: CreateCampaignRequest) => {
      const response = await apiClient.POST('/api/campaigns', {
        body: request,
      });
      if (response.error) throw new Error(response.error);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['campaigns'] });
    },
  });
}
```

### Optimistic Updates
Implement optimistic updates for mutations:
- Artifact approval
- Campaign updates
- Regeneration requests

Use `onMutate`, `onError`, `onSettled` for rollback

### Cache Invalidation Strategy
Define cache invalidation rules:
- Campaign creation → invalidate campaigns list
- Artifact regeneration → invalidate artifact queries
- Approval → invalidate artifact and audit queries
- Orchestration completion → invalidate campaign snapshot

### Error Handling
Create error handling utilities:
- Type-safe error responses
- Error boundary integration
- Toast notification on errors
- Retry logic configuration

**Error Types:**
```typescript
type ApiError = {
  status: number;
  title: string;
  detail: string;
  errors?: Record<string, string[]>;
};
```

### Type Safety
Ensure full type safety:
- All hooks return typed data
- Mutations accept typed parameters
- Errors are typed
- No `any` types used
- Leverage generated OpenAPI types

### Environment Configuration
Set up environment variables:
- `NEXT_PUBLIC_API_URL` - API base URL
- `NEXT_PUBLIC_WS_URL` - WebSocket/SignalR URL (for next task)
- Different values for development, staging, production

**`.env.local`:**
```
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_WS_URL=http://localhost:5000/hubs/campaign
```

## Acceptance Criteria
- [ ] OpenAPI SDK generation working and integrated in build
- [ ] TypeScript types generated from OpenAPI spec
- [ ] API client configured with base URL and interceptors
- [ ] QueryClient configured with appropriate defaults
- [ ] All React Query hooks created for API operations
- [ ] Hooks follow consistent naming and implementation patterns
- [ ] Optimistic updates implemented for key mutations
- [ ] Cache invalidation strategy working correctly
- [ ] Error handling utilities created and working
- [ ] Full type safety across all hooks and client calls
- [ ] Environment variables configured
- [ ] React Query DevTools integrated in development

## Testing Requirements
- [ ] Unit tests for API client configuration
- [ ] Unit tests for custom hooks using Mock Service Worker (MSW)
- [ ] Test cache invalidation logic
- [ ] Test optimistic update scenarios
- [ ] Test error handling and retry logic
- [ ] Test type safety (TypeScript compilation)
- [ ] Integration tests with actual API endpoints
- [ ] Test environment variable configuration

## Non-Functional Requirements
- SDK generation time <10s
- Hook execution overhead <5ms
- Cache hit rate >80% for repeated queries
- Type coverage 100% (no `any` types)
- Bundle size impact <50KB (gzipped)

## Out of Scope
- GraphQL support (REST only in MVP)
- Authentication token management (future)
- Request queue management
- Offline support and sync
- Custom cache persistence beyond default

## Notes
- Follow AGENTS.md frontend development patterns
- Use latest stable versions of TanStack Query
- Regenerate SDK whenever OpenAPI spec changes
- Document hook usage in code comments
- Create custom hook wrappers for complex queries
- Use query keys consistently for cache management
- Test with React Query DevTools during development
- Consider pagination helpers for list queries
- Future: add authentication interceptor
- Ensure hooks are tree-shakeable for optimal bundle size
