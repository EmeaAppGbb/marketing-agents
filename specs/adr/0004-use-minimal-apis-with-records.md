# ADR 0004: Use ASP.NET Core Minimal APIs with Record Types

**Status**: Accepted

**Date**: 30 October 2025

## Context

The backend API needs to expose endpoints for:
- Campaign creation (POST /api/campaigns)
- Campaign retrieval (GET /api/campaigns/{id})
- Artifact generation (POST /api/campaigns/{id}/generate)
- Audit execution (POST /api/campaigns/{id}/audit)
- Campaign listing (GET /api/campaigns)

All endpoints must be type-safe, performant, and follow AGENTS.md guidelines.

## Decision Drivers

- AGENTS.md mandates: ".NET 9 + ASP.NET Core (Minimal APIs)" with "Strong typing via record types and nullable reference types"
- Need for clean, concise API definitions without controller boilerplate
- Requirement for OpenAPI spec generation for frontend SDK
- Type safety end-to-end (backend → contract → frontend)
- Performance and minimal overhead

## Considered Options

1. **ASP.NET Core Minimal APIs with record types** (mandated)
2. MVC Controllers with classes
3. gRPC endpoints

## Decision Outcome

**Chosen: ASP.NET Core Minimal APIs with record types**

### Implementation Pattern

**Endpoint Organization:**
```
/Api
├── Program.cs                  # Application startup, middleware, DI
├── Endpoints/
│   ├── CampaignEndpoints.cs   # Campaign CRUD endpoints
│   ├── GenerationEndpoints.cs # Artifact generation endpoints
│   └── AuditEndpoints.cs      # Audit endpoints
├── Services/
│   ├── CampaignOrchestrator.cs
│   └── AgentService.cs
└── Models/
    ├── Requests/              # Request DTOs (records)
    ├── Responses/             # Response DTOs (records)
    └── Domain/                # Domain entities (records)
```

**Endpoint Example:**
```csharp
public static class CampaignEndpoints
{
    public static void MapCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/campaigns")
            .WithTags("Campaigns")
            .WithOpenApi();

        group.MapPost("/", CreateCampaign)
            .WithName("CreateCampaign")
            .Produces<CreateCampaignResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/{id}", GetCampaign)
            .WithName("GetCampaign")
            .Produces<CampaignResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateCampaign(
        CreateCampaignRequest request,
        ICampaignRepository repository,
        ILogger<CampaignEndpoints> logger)
    {
        // Implementation
    }
}
```

**Record Types for DTOs:**
```csharp
public record CreateCampaignRequest(
    string Theme,
    string TargetAudience,
    string ProductDetails
);

public record CreateCampaignResponse(
    string CampaignId,
    string Status,
    DateTimeOffset CreatedAt
);
```

### Rationale

- **AGENTS.md compliance**: Explicitly mandated approach
- **Type safety**: Record types provide immutability and value equality
- **Minimal boilerplate**: No controller classes, just endpoint mapping functions
- **OpenAPI generation**: Automatic via Swashbuckle/NSwag for SDK generation
- **Performance**: Minimal overhead compared to MVC controllers
- **Testability**: Easy to test with `WebApplicationFactory<Program>`

## Consequences

### Positive
- Clean, functional-style endpoint definitions
- Automatic OpenAPI/Swagger documentation
- Strong typing with nullable reference types enabled
- Easy to organize endpoints by feature (endpoint groups)
- Built-in support for problem details (RFC 7807) via `Results.Problem()`

### Negative
- Less familiar to developers coming from MVC background
- No built-in attribute routing (use endpoint groups instead)
- Requires discipline to keep endpoints organized

### Implementation Notes

**Nullable Reference Types:**
```xml
<!-- In .csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
  <WarningsAsErrors>nullable</WarningsAsErrors>
</PropertyGroup>
```

**Error Handling Pattern:**
```csharp
private static async Task<IResult> GetCampaign(
    string id,
    ICampaignRepository repository)
{
    var campaign = await repository.GetByIdAsync(id);
    
    return campaign is not null
        ? Results.Ok(campaign)
        : Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Campaign not found",
            detail: $"Campaign with ID '{id}' does not exist."
        );
}
```

**OpenAPI Configuration:**
```csharp
// In Program.cs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketing Agents API",
        Version = "v1"
    });
});

// After app.Build()
app.UseSwagger();
app.UseSwaggerUI();
```

**Validation:**
- Use Data Annotations on record types
- Enable automatic validation via `Results.ValidationProblem()`
- Consider FluentValidation for complex scenarios
