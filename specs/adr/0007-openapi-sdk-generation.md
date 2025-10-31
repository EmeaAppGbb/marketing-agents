# ADR 0007: OpenAPI-Driven SDK Generation for Type Safety

**Status**: Accepted

**Date**: 30 October 2025

## Context

The frontend needs to consume backend API endpoints in a type-safe manner. AGENTS.md mandates end-to-end type safety:
- **Backend**: Record types, nullable reference types, fail builds on nullable warnings
- **Contract**: OpenAPI schema with validation
- **Frontend**: TypeScript strict mode, generated types from OpenAPI, fail builds on type errors

## Decision Drivers

- AGENTS.md requires: "API contracts defined via backend endpoints → OpenAPI spec published → clients generated into `/packages/sdk`"
- Need for automatic type synchronization between backend and frontend
- Requirement for fail-fast on type mismatches during build
- Developer experience: IntelliSense and compile-time errors for API calls

## Considered Options

1. **OpenAPI spec generation + TypeScript SDK generation** (mandated)
2. Manual TypeScript interfaces (error-prone, no sync)
3. GraphQL schema (different paradigm, not in canonical stack)

## Decision Outcome

**Chosen: OpenAPI-driven TypeScript SDK generation**

### Workflow

```
Backend Minimal APIs (with XML comments)
    ↓
Swashbuckle/NSwag generates OpenAPI spec
    ↓
OpenAPI spec published at /swagger/v1/swagger.json
    ↓
OpenAPI Generator creates TypeScript SDK
    ↓
TypeScript SDK published to /packages/sdk
    ↓
Frontend imports and uses generated SDK via TanStack Query
```

### Implementation Pattern

**Backend: OpenAPI Generation**
```csharp
// Program.cs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Agents API",
        Version = "v1",
        Description = "AI-powered marketing campaign generation"
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Enable XML documentation
// In .csproj: <GenerateDocumentationFile>true</GenerateDocumentationFile>
```

**SDK Generation Script**
```bash
#!/bin/bash
# scripts/generate-sdk.sh

# Start the API to generate OpenAPI spec
dotnet run --project MarketingAgents.Api &
API_PID=$!

# Wait for API to be ready
sleep 5

# Generate TypeScript SDK
npx openapi-generator-cli generate \
  -i http://localhost:5000/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o packages/sdk \
  --additional-properties=supportsES6=true,npmName=@marketing-agents/sdk,typescriptThreePlus=true

# Kill the API
kill $API_PID

# Install SDK dependencies
cd packages/sdk
pnpm install
pnpm build
```

**Frontend: SDK Consumption**
```typescript
// lib/api-client.ts
import { Configuration, CampaignsApi } from '@marketing-agents/sdk';

const config = new Configuration({
  basePath: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000',
});

export const campaignsApi = new CampaignsApi(config);
```

**TanStack Query Integration**
```typescript
// hooks/use-campaign.ts
import { useQuery } from '@tanstack/react-query';
import { campaignsApi } from '@/lib/api-client';

export function useCampaign(id: string) {
  return useQuery({
    queryKey: ['campaign', id],
    queryFn: () => campaignsApi.getCampaign({ id }),
  });
}
```

### Rationale

- **AGENTS.md compliance**: Explicitly required workflow
- **Type safety**: Backend types → OpenAPI schema → TypeScript types
- **Single source of truth**: Backend API definitions drive frontend types
- **Fail-fast**: Build breaks if backend changes break frontend contracts
- **Developer experience**: IntelliSense, autocomplete, compile-time errors

## Consequences

### Positive
- Automatic type synchronization on API changes
- Frontend developers get immediate feedback on API contract changes
- Eliminates manual type definition duplication
- API documentation automatically generated from code
- Reduces runtime errors from API mismatches

### Negative
- SDK regeneration required when API changes
- Generated code may be verbose
- Build process more complex (must run backend to generate spec)

### Implementation Notes

**CI/CD Integration:**
```yaml
# .github/workflows/build.yml
- name: Generate OpenAPI Spec
  run: |
    dotnet run --project src/MarketingAgents.Api &
    sleep 5
    curl http://localhost:5000/swagger/v1/swagger.json > openapi.json
    kill %1

- name: Generate TypeScript SDK
  run: |
    npx openapi-generator-cli generate \
      -i openapi.json \
      -g typescript-fetch \
      -o packages/sdk

- name: Build Frontend
  run: |
    cd src/MarketingAgents.Web
    pnpm install
    pnpm build
```

**Package Structure:**
```
/packages/sdk/
├── package.json
├── tsconfig.json
├── src/
│   ├── apis/
│   │   └── CampaignsApi.ts
│   ├── models/
│   │   ├── Campaign.ts
│   │   ├── CreateCampaignRequest.ts
│   │   └── CampaignResponse.ts
│   └── index.ts
└── README.md
```

**Version Control:**
- Commit generated SDK to version control
- Ensures reproducible builds
- Allows PR reviewers to see contract changes

**Alternative Generators:**
- Consider NSwag.CodeGeneration if Swashbuckle doesn't meet needs
- Evaluate Kiota (Microsoft's OpenAPI client generator) as alternative to openapi-generator-cli

**Type Validation:**
```json
// tsconfig.json
{
  "compilerOptions": {
    "strict": true,
    "noEmit": true  // Type-check without emitting
  }
}
```

**Build Command:**
```bash
# Frontend build fails if types don't match
pnpm run type-check  # Runs: tsc --noEmit
```
