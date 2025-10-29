# REST API Documentation

This document describes the REST API endpoints for the Marketing Agents Platform.

## Base URL

- **Local Development**: `http://localhost:5001`
- **Production**: `https://api.marketing-agents.azurecontainerapps.io` (example)

## Authentication

All API endpoints require authentication using Azure AD / Microsoft Entra ID bearer tokens.

```http
Authorization: Bearer <access_token>
```

## API Endpoints

### Health Check

#### GET /health

Check the health status of the API service.

**Response**: `200 OK`
```json
{
  "status": "Healthy",
  "checks": {
    "cosmosdb": "Healthy",
    "redis": "Healthy",
    "agenthostagent": "Healthy"
  }
}
```

---

### Campaigns

#### POST /api/campaigns

Create a new marketing campaign.

**Request Body**:
```json
{
  "theme": "Summer Product Launch",
  "product": "EcoBottle - Sustainable Water Bottle",
  "targetAudience": "Environmentally conscious millennials",
  "tone": "Inspirational and eco-friendly",
  "constraints": ["Must include recycling message", "Avoid plastic-related claims"]
}
```

**Response**: `201 Created`
```json
{
  "id": "camp_abc123",
  "status": "Created",
  "createdAt": "2025-10-29T10:00:00Z",
  "brief": {
    "theme": "Summer Product Launch",
    "product": "EcoBottle - Sustainable Water Bottle",
    "targetAudience": "Environmentally conscious millennials",
    "tone": "Inspirational and eco-friendly",
    "constraints": ["Must include recycling message", "Avoid plastic-related claims"]
  }
}
```

**Errors**:
- `400 Bad Request` - Invalid campaign brief
- `401 Unauthorized` - Missing or invalid authentication
- `429 Too Many Requests` - Rate limit exceeded

---

#### GET /api/campaigns/{id}

Retrieve campaign details.

**Parameters**:
- `id` (path, required) - Campaign ID

**Response**: `200 OK`
```json
{
  "id": "camp_abc123",
  "status": "Completed",
  "createdAt": "2025-10-29T10:00:00Z",
  "completedAt": "2025-10-29T10:05:30Z",
  "brief": {
    "theme": "Summer Product Launch",
    "product": "EcoBottle - Sustainable Water Bottle",
    "targetAudience": "Environmentally conscious millennials",
    "tone": "Inspirational and eco-friendly",
    "constraints": ["Must include recycling message"]
  },
  "artifacts": [
    {
      "type": "Copy",
      "status": "Approved",
      "createdAt": "2025-10-29T10:02:00Z"
    },
    {
      "type": "ShortCopy",
      "status": "Approved",
      "createdAt": "2025-10-29T10:03:00Z"
    },
    {
      "type": "Visual",
      "status": "Approved",
      "createdAt": "2025-10-29T10:04:00Z"
    }
  ],
  "complianceStatus": "Approved",
  "complianceScore": 95
}
```

**Errors**:
- `404 Not Found` - Campaign not found

---

#### GET /api/campaigns

List all campaigns for the authenticated user.

**Query Parameters**:
- `page` (optional, default: 1) - Page number
- `pageSize` (optional, default: 20, max: 100) - Items per page
- `status` (optional) - Filter by status: `Created`, `InProgress`, `Completed`, `Failed`

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "camp_abc123",
      "theme": "Summer Product Launch",
      "status": "Completed",
      "createdAt": "2025-10-29T10:00:00Z",
      "completedAt": "2025-10-29T10:05:30Z"
    },
    {
      "id": "camp_def456",
      "theme": "Holiday Sale Campaign",
      "status": "InProgress",
      "createdAt": "2025-10-29T11:00:00Z"
    }
  ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

---

#### POST /api/campaigns/{id}/generate

Trigger artifact generation for a campaign.

**Parameters**:
- `id` (path, required) - Campaign ID

**Response**: `202 Accepted`
```json
{
  "campaignId": "camp_abc123",
  "status": "InProgress",
  "estimatedCompletionTime": "2025-10-29T10:05:00Z"
}
```

**Errors**:
- `404 Not Found` - Campaign not found
- `409 Conflict` - Campaign already in progress or completed

---

#### GET /api/campaigns/{id}/artifacts

Retrieve all artifacts for a campaign.

**Parameters**:
- `id` (path, required) - Campaign ID
- `type` (query, optional) - Filter by artifact type: `Copy`, `ShortCopy`, `Visual`

**Response**: `200 OK`
```json
{
  "campaignId": "camp_abc123",
  "artifacts": [
    {
      "id": "art_copy_001",
      "type": "Copy",
      "content": "Discover the EcoBottle: your daily hydration companion that's as kind to the planet as it is to you...",
      "metadata": {
        "wordCount": 650,
        "tone": "inspirational",
        "keywords": ["sustainable", "eco-friendly", "recycling"]
      },
      "createdAt": "2025-10-29T10:02:00Z",
      "agentName": "CopywritingAgent"
    },
    {
      "id": "art_shortcopy_001",
      "type": "ShortCopy",
      "content": "🌿 Hydrate responsibly with EcoBottle. Your sustainable choice for a greener tomorrow. #EcoBottle #Sustainability",
      "metadata": {
        "characterCount": 115,
        "hashtags": ["#EcoBottle", "#Sustainability"]
      },
      "createdAt": "2025-10-29T10:03:00Z",
      "agentName": "ShortCopyAgent"
    },
    {
      "id": "art_visual_001",
      "type": "Visual",
      "content": "Visual design specification with color palette and layout...",
      "metadata": {
        "colors": ["#2E7D32", "#81C784", "#FFFFFF"],
        "layout": "hero-centered"
      },
      "createdAt": "2025-10-29T10:04:00Z",
      "agentName": "VisualPosterAgent"
    }
  ]
}
```

---

#### GET /api/campaigns/{id}/compliance

Retrieve compliance audit results for a campaign.

**Parameters**:
- `id` (path, required) - Campaign ID

**Response**: `200 OK`
```json
{
  "campaignId": "camp_abc123",
  "isCompliant": true,
  "complianceScore": 95,
  "auditedAt": "2025-10-29T10:05:00Z",
  "issues": [],
  "feedback": "Campaign meets all brand guidelines and regulatory requirements. Excellent adherence to sustainability messaging."
}
```

**Non-Compliant Example**:
```json
{
  "campaignId": "camp_def456",
  "isCompliant": false,
  "complianceScore": 62,
  "auditedAt": "2025-10-29T11:05:00Z",
  "issues": [
    {
      "artifactType": "Copy",
      "issueDescription": "Tone is too aggressive for target audience",
      "severity": "Medium"
    },
    {
      "artifactType": "ShortCopy",
      "issueDescription": "Missing required hashtag #BrandName",
      "severity": "High"
    }
  ],
  "feedback": "Please revise the copy to adopt a more friendly tone and ensure all short copy includes #BrandName."
}
```

---

#### POST /api/campaigns/{id}/revise

Request a revision of campaign artifacts with feedback.

**Parameters**:
- `id` (path, required) - Campaign ID

**Request Body**:
```json
{
  "feedback": "Make the tone more energetic and youthful. Emphasize the adventure aspect.",
  "specificChanges": [
    "Add references to outdoor activities",
    "Include more vibrant language",
    "Shorten the introduction"
  ]
}
```

**Response**: `202 Accepted`
```json
{
  "campaignId": "camp_abc123",
  "revisionNumber": 2,
  "status": "InProgress",
  "estimatedCompletionTime": "2025-10-29T11:10:00Z"
}
```

---

#### DELETE /api/campaigns/{id}

Delete a campaign and all associated artifacts.

**Parameters**:
- `id` (path, required) - Campaign ID

**Response**: `204 No Content`

**Errors**:
- `404 Not Found` - Campaign not found
- `409 Conflict` - Campaign cannot be deleted while in progress

---

## Error Responses

All error responses follow the RFC 7807 Problem Details format:

```json
{
  "type": "https://api.marketing-agents.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The 'theme' field is required.",
  "instance": "/api/campaigns",
  "errors": {
    "theme": ["The theme field is required."]
  }
}
```

### Common Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `202 Accepted` - Request accepted for processing
- `204 No Content` - Request successful, no content to return
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Missing or invalid authentication
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource conflict
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

## Rate Limiting

- **Rate Limit**: 100 requests per minute per authenticated user
- **Headers**:
  - `X-RateLimit-Limit`: Maximum requests allowed
  - `X-RateLimit-Remaining`: Requests remaining
  - `X-RateLimit-Reset`: Time when limit resets (Unix timestamp)

## Pagination

List endpoints support pagination with the following query parameters:

- `page` - Page number (1-based)
- `pageSize` - Items per page (max 100)

Response includes:
```json
{
  "items": [...],
  "totalCount": 150,
  "page": 2,
  "pageSize": 20,
  "totalPages": 8
}
```

## Next Steps

- [Realtime API Documentation](realtime-api.md) - SignalR WebSocket API
- [Quick Start Guide](../getting-started/quick-start.md) - Try the API
- [Development Workflow](../guides/development.md) - Build with the API
