# ADR 0005: Use Next.js App Router with TanStack Query

**Status**: Accepted

**Date**: 30 October 2025

## Context

The frontend needs to provide:
- Campaign creation form with validation
- Campaign display screen with three artifact sections
- Audit results display
- Loading states during generation
- Error handling and retry functionality
- Campaign list view

Users need a responsive, type-safe, performant web application.

## Decision Drivers

- AGENTS.md mandates: "Next.js 14 (App Router) + React 18 + TypeScript"
- Need for Server Components (data fetching) and Client Components (interactivity)
- Requirement for type-safe API consumption via generated SDK
- State management: "TanStack Query for remote data, Zustand or Jotai for local state"
- TypeScript strict mode with fail-on-errors
- Performance budgets: LCP < 2.5s, CLS < 0.1

## Considered Options

1. **Next.js 14 App Router + TanStack Query** (mandated)
2. Next.js Pages Router (deprecated approach)
3. Separate React SPA with client-side routing

## Decision Outcome

**Chosen: Next.js 14 App Router with TanStack Query**

### Folder Structure
```
/Web
├── app/
│   ├── page.tsx                    # Home/campaign list (Server Component)
│   ├── layout.tsx                  # Root layout
│   ├── campaigns/
│   │   ├── new/
│   │   │   └── page.tsx           # Campaign creation form (Client Component)
│   │   └── [id]/
│   │       └── page.tsx           # Campaign display (Server + Client Components)
├── components/
│   ├── campaign-form.tsx          # Form component
│   ├── artifact-display.tsx       # Artifact sections
│   └── audit-results.tsx          # Audit status and violations
├── hooks/
│   ├── use-campaign.ts            # TanStack Query hook for campaigns
│   └── use-generate.ts            # Mutation for generation
├── lib/
│   ├── api-client.ts              # Generated SDK wrapper
│   └── utils.ts                   # Utility functions
└── styles/
    └── globals.css
```

### Implementation Pattern

**Server Component (Data Fetching):**
```typescript
// app/campaigns/[id]/page.tsx
export default async function CampaignPage({ params }: { params: { id: string } }) {
  const campaign = await apiClient.getCampaign(params.id);
  
  return (
    <div>
      <CampaignBrief brief={campaign.brief} />
      <ArtifactDisplay artifacts={campaign.artifacts} />
      <AuditResults audit={campaign.audit} />
    </div>
  );
}
```

**Client Component (Interactivity):**
```typescript
'use client';

import { useMutation, useQueryClient } from '@tanstack/react-query';

export function RegenerateButton({ campaignId }: { campaignId: string }) {
  const queryClient = useQueryClient();
  
  const { mutate, isPending } = useMutation({
    mutationFn: () => apiClient.generateArtifacts(campaignId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['campaign', campaignId] });
    },
  });
  
  return (
    <button onClick={() => mutate()} disabled={isPending}>
      {isPending ? 'Regenerating...' : 'Regenerate Campaign'}
    </button>
  );
}
```

**TanStack Query Configuration:**
```typescript
// app/providers.tsx
'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      retry: 1,
    },
  },
});

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
}
```

### Rationale

- **AGENTS.md compliance**: Explicitly mandated approach
- **Server Components**: Optimal for campaign display (SEO, performance)
- **Client Components**: Required for forms, retry buttons, loading states
- **TanStack Query**: Cache consistency, automatic retries, optimistic updates
- **Type safety**: Generated TypeScript SDK from OpenAPI ensures end-to-end type safety

## Consequences

### Positive
- Automatic code splitting and optimization
- Server Components reduce client bundle size
- TanStack Query handles loading/error states automatically
- Type-safe API consumption via generated SDK
- Built-in caching and cache invalidation

### Negative
- Learning curve for Server/Client Component boundaries
- Need to mark interactive components with 'use client'
- Query client setup requires provider wrapper

### Implementation Notes

**TypeScript Configuration:**
```json
{
  "compilerOptions": {
    "strict": true,
    "noUncheckedIndexedAccess": true,
    "noImplicitOverride": true
  }
}
```

**SDK Generation:**
```bash
# Generate TypeScript SDK from OpenAPI spec
npx openapi-generator-cli generate \
  -i http://localhost:5000/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o packages/sdk
```

**Package Manager:**
- Use `pnpm` as mandated by AGENTS.md
- Enable workspace mode for monorepo

**State Management:**
- **TanStack Query**: All server state (campaigns, artifacts, audit results)
- **Zustand or Jotai**: Local ephemeral state if needed (form state handled by React Hook Form)

**Form Handling:**
```typescript
// Use React Hook Form + Zod for form validation
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const campaignSchema = z.object({
  theme: z.string().min(3),
  targetAudience: z.string().min(10),
  productDetails: z.string().min(10),
});

type CampaignForm = z.infer<typeof campaignSchema>;
```

**Performance:**
- Monitor with Lighthouse CI
- Enforce LCP < 2.5s, CLS < 0.1
- Use Image optimization for poster thumbnails (future)
- Implement code splitting for heavy components
