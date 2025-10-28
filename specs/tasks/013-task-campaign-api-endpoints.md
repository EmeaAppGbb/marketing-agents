# Task 013: Campaign API Endpoints (Minimal APIs)

## Description
Implement RESTful API endpoints using ASP.NET Core Minimal APIs for campaign management, artifact retrieval, approval workflows, and regeneration requests. Generate OpenAPI specification for frontend SDK generation.

## Dependencies
- Task 001: Backend Scaffolding
- Task 004: Campaign Data Model & Cosmos DB Persistence
- Task 010: Campaign Orchestration Implementation
- Task 011: Campaign Iteration & Feedback Loop Implementation

## Technical Requirements

### Endpoint Organization
Create endpoint groups in `Endpoints/` folder:
- `CampaignEndpoints.cs` - Campaign CRUD operations
- `OrchestrationEndpoints.cs` - Campaign generation and orchestration
- `ArtifactEndpoints.cs` - Artifact retrieval and version history
- `AuditEndpoints.cs` - Audit report retrieval
- `IterationEndpoints.cs` - Feedback submission and regeneration
- `HealthEndpoints.cs` - Health checks and readiness

### Campaign Endpoints
**POST /api/campaigns**
- Create new campaign (brief only, no generation)
- Request: CreateCampaignRequest
- Response: CampaignResponse (201 Created)
- Validate campaign brief completeness

**GET /api/campaigns/{id}**
- Retrieve campaign snapshot with active versions
- Response: CampaignSnapshotResponse (200 OK)
- Include audit statuses
- Return 404 if not found

**GET /api/campaigns**
- List campaigns with pagination
- Query parameters: page, pageSize, sortBy, sortOrder
- Response: PagedResponse<CampaignSummary> (200 OK)

**DELETE /api/campaigns/{id}**
- Soft delete campaign
- Response: 204 No Content
- Preserve data for retention period

### Orchestration Endpoints
**POST /api/campaigns/{id}/generate**
- Trigger campaign generation (orchestration)
- Request: GenerateCampaignRequest (executionMode, selectedPlatforms)
- Response: OrchestrationResponse with runId (202 Accepted)
- Return 409 Conflict if already generating

**POST /api/campaigns/{id}/cancel**
- Cancel ongoing orchestration
- Response: 200 OK with cancellation acknowledgement
- Return 400 Bad Request if not cancellable state

**GET /api/campaigns/{id}/runs**
- List orchestration runs for campaign
- Response: PagedResponse<OrchestrationRunSummary>

**GET /api/campaigns/{id}/runs/{runId}**
- Get specific orchestration run details
- Response: OrchestrationRunDetails (200 OK)
- Include execution metrics

### Artifact Endpoints
**GET /api/campaigns/{id}/artifacts**
- Get all active artifacts for campaign
- Response: ArtifactBundleResponse (200 OK)

**GET /api/campaigns/{id}/artifacts/{artifactType}**
- Get specific artifact type with active version
- artifactType: copy, shortcopy, visualconcept
- Response: ArtifactResponse (200 OK)

**GET /api/campaigns/{id}/artifacts/{artifactType}/versions**
- Get version history for artifact type
- Response: PagedResponse<ArtifactVersionSummary>

**GET /api/campaigns/{id}/artifacts/{artifactType}/versions/{versionId}**
- Get specific artifact version
- Response: ArtifactVersionResponse (200 OK)

**POST /api/campaigns/{id}/artifacts/{artifactType}/approve**
- Approve artifact (set status to Approved)
- Response: 200 OK
- Update version status

### Audit Endpoints
**GET /api/campaigns/{id}/audit**
- Get latest audit report for campaign
- Response: AuditReportResponse (200 OK)

**GET /api/campaigns/{id}/audit/history**
- Get audit history across iterations
- Response: PagedResponse<AuditReportSummary>

**GET /api/campaigns/{id}/artifacts/{artifactType}/audit**
- Get audit report for specific artifact version
- Response: AuditReportResponse (200 OK)

### Iteration Endpoints
**POST /api/campaigns/{id}/artifacts/{artifactType}/regenerate**
- Submit feedback and regenerate artifact
- Request: RegenerateArtifactRequest (feedback, tags, platform)
- Response: IterationResponse with new versionId (202 Accepted)

**GET /api/campaigns/{id}/artifacts/{artifactType}/iterations**
- Get iteration history for artifact
- Response: IterationHistoryResponse

### Request/Response Models
Create DTOs as C# records:

**CreateCampaignRequest**
- Name (string, required)
- Brief (CampaignBriefDto, required)

**GenerateCampaignRequest**
- ExecutionMode (string: "parallel" | "sequential")
- SelectedPlatforms (string[], for short copy)

**RegenerateArtifactRequest**
- FeedbackText (string, required)
- FeedbackTags (string[], optional)
- TargetPlatform (string, optional for short copy)

**CampaignResponse**
- Id (string)
- Name (string)
- Brief (CampaignBriefDto)
- Status (string)
- CreatedAt (DateTimeOffset)
- UpdatedAt (DateTimeOffset)

**CampaignSnapshotResponse**
- Campaign (CampaignResponse)
- ActiveArtifacts (ArtifactBundleDto)
- LatestAudit (AuditReportDto, nullable)

**PagedResponse<T>**
- Items (T[])
- TotalCount (int)
- Page (int)
- PageSize (int)
- HasNextPage (bool)

### Validation
Use FluentValidation or Data Annotations:
- Campaign brief required fields
- Feedback text minimum length (10 chars)
- Selected platforms valid values
- Pagination parameters (page ≥ 1, pageSize ≤ 100)
- Artifact type valid enum values

### Error Handling
Return Problem Details (RFC 7807):
- 400 Bad Request: Validation errors
- 404 Not Found: Resource not found
- 409 Conflict: State conflict (e.g., already generating)
- 422 Unprocessable Entity: Business logic errors
- 500 Internal Server Error: Unexpected errors

Use `Results.Problem()` for consistent error responses

### OpenAPI Configuration
Configure Swashbuckle/NSwag:
- Generate OpenAPI 3.0 specification
- Include XML documentation comments
- Define request/response examples
- Tag endpoints by feature area
- Include authentication scheme (future)
- Export spec to `/packages/sdk/openapi.json`

**XML Documentation:**
- Document all endpoints with summary and remarks
- Document request/response types
- Include example values
- Document possible status codes

### Content Negotiation
- Support application/json (default)
- Return UTF-8 encoding
- Support compression (gzip, brotli)

### Rate Limiting
Configure basic rate limiting:
- Per-IP limits: 100 requests/minute (configurable)
- Per-campaign generation: 1 concurrent request
- Use ASP.NET Core rate limiting middleware

### Observability
- Log all requests with correlation IDs
- Emit metrics for endpoint latency
- Track endpoint usage frequency
- OpenTelemetry spans per request
- Structured logging with request context

## Acceptance Criteria
- [ ] All endpoint groups implemented using Minimal APIs
- [ ] Campaign CRUD operations working
- [ ] Orchestration trigger and cancellation working
- [ ] Artifact retrieval with version history
- [ ] Audit report retrieval endpoints
- [ ] Regeneration endpoints with feedback
- [ ] OpenAPI specification generated and exported
- [ ] Validation working with clear error messages
- [ ] Problem Details format for all errors
- [ ] Health check endpoints responding
- [ ] Rate limiting configured
- [ ] All endpoints documented with XML comments

## Testing Requirements
- [ ] Unit tests for endpoint logic (≥85% coverage)
- [ ] Integration tests for all endpoints using WebApplicationFactory
- [ ] Test request validation for all endpoints
- [ ] Test error handling and Problem Details responses
- [ ] Test pagination functionality
- [ ] Test concurrent orchestration conflict detection
- [ ] Contract tests validating OpenAPI specification
- [ ] Test rate limiting behavior
- [ ] Load test critical endpoints (campaign generation)

## Non-Functional Requirements
- Endpoint response time <200ms for GET requests
- Orchestration trigger response (202) <500ms
- OpenAPI spec generation <5s on startup
- Support 50+ concurrent requests
- Rate limiting accuracy ≥95%

## Out of Scope
- Authentication and authorization (future)
- Webhook notifications (future)
- Batch operations (future)
- Export to PDF/CSV (future)
- API versioning (v1 implicit for MVP)

## Notes
- Follow AGENTS.md Minimal APIs patterns
- Use record types for all DTOs
- Generate OpenAPI spec to `/packages/sdk/openapi.json` for frontend SDK
- Document all endpoints thoroughly
- Use route parameter constraints where appropriate
- Consider endpoint grouping with `MapGroup()`
- Return 202 Accepted for async operations
- Use appropriate HTTP status codes consistently
- Create MADR for API design decisions
- Future: add API versioning strategy
