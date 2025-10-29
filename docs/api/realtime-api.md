# Realtime API Documentation

This document describes the SignalR WebSocket API for real-time communication in the Marketing Agents Platform.

## Overview

The Marketing Agents Platform uses ASP.NET Core SignalR to provide real-time bidirectional communication between the server and clients. This enables live updates as AI agents generate campaign artifacts.

## Connection

### Endpoint

**Local Development**: `ws://localhost:5001/hubs/campaign`  
**Production**: `wss://api.marketing-agents.azurecontainerapps.io/hubs/campaign`

### Authentication

Include the bearer token in the connection request:

#### JavaScript/TypeScript
```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5001/hubs/campaign', {
    accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Information)
  .build();

await connection.start();
```

#### C# Client
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5001/hubs/campaign", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(accessToken);
    })
    .WithAutomaticReconnect()
    .Build();

await connection.StartAsync();
```

## Hub Methods

### Client → Server Methods

Methods that clients can invoke on the server.

---

#### JoinCampaignGroup

Join a campaign group to receive real-time updates for a specific campaign.

**Parameters**:
- `campaignId` (string) - Campaign ID to subscribe to

**Usage**:
```typescript
await connection.invoke('JoinCampaignGroup', 'camp_abc123');
```

```csharp
await connection.InvokeAsync("JoinCampaignGroup", "camp_abc123");
```

---

#### LeaveCampaignGroup

Leave a campaign group to stop receiving updates.

**Parameters**:
- `campaignId` (string) - Campaign ID to unsubscribe from

**Usage**:
```typescript
await connection.invoke('LeaveCampaignGroup', 'camp_abc123');
```

```csharp
await connection.InvokeAsync("LeaveCampaignGroup", "camp_abc123");
```

---

#### RequestCampaignStatus

Request the current status of a campaign.

**Parameters**:
- `campaignId` (string) - Campaign ID

**Usage**:
```typescript
await connection.invoke('RequestCampaignStatus', 'camp_abc123');
```

**Response**: Triggers `OnCampaignStatusReceived` callback

---

### Server → Client Events

Events that the server sends to clients.

---

#### OnCampaignStarted

Fired when campaign generation begins.

**Callback Signature**:
```typescript
connection.on('OnCampaignStarted', (campaignId: string) => {
  console.log(`Campaign ${campaignId} generation started`);
});
```

```csharp
connection.On<string>("OnCampaignStarted", campaignId =>
{
    Console.WriteLine($"Campaign {campaignId} generation started");
});
```

---

#### OnArtifactGenerated

Fired when an AI agent generates a new artifact.

**Payload**:
```typescript
interface CampaignArtifact {
  id: string;
  campaignId: string;
  type: 'Copy' | 'ShortCopy' | 'Visual';
  content: string;
  metadata: Record<string, any>;
  createdAt: string;
  agentName: string;
}
```

**Callback Signature**:
```typescript
connection.on('OnArtifactGenerated', (artifact: CampaignArtifact) => {
  console.log(`New ${artifact.type} artifact:`, artifact.content);
  // Update UI with new artifact
});
```

```csharp
connection.On<CampaignArtifact>("OnArtifactGenerated", artifact =>
{
    Console.WriteLine($"New {artifact.Type} artifact: {artifact.Content}");
});
```

---

#### OnComplianceCheckCompleted

Fired when the audit agent completes compliance review.

**Payload**:
```typescript
interface ComplianceAuditResult {
  campaignId: string;
  isCompliant: boolean;
  complianceScore: number;
  issues: ComplianceIssue[];
  feedback: string;
  auditedAt: string;
}

interface ComplianceIssue {
  artifactType: string;
  issueDescription: string;
  severity: 'Low' | 'Medium' | 'High';
}
```

**Callback Signature**:
```typescript
connection.on('OnComplianceCheckCompleted', (result: ComplianceAuditResult) => {
  if (result.isCompliant) {
    console.log(`Campaign approved with score ${result.complianceScore}`);
  } else {
    console.log(`Compliance issues found:`, result.issues);
  }
});
```

```csharp
connection.On<ComplianceAuditResult>("OnComplianceCheckCompleted", result =>
{
    if (result.IsCompliant)
    {
        Console.WriteLine($"Campaign approved with score {result.ComplianceScore}");
    }
});
```

---

#### OnCampaignCompleted

Fired when all campaign artifacts are generated and approved.

**Callback Signature**:
```typescript
connection.on('OnCampaignCompleted', (campaignId: string) => {
  console.log(`Campaign ${campaignId} completed successfully`);
  // Navigate to review dashboard
});
```

```csharp
connection.On<string>("OnCampaignCompleted", campaignId =>
{
    Console.WriteLine($"Campaign {campaignId} completed successfully");
});
```

---

#### OnGenerationFailed

Fired when campaign generation fails after all retries.

**Payload**:
```typescript
interface GenerationError {
  campaignId: string;
  errorMessage: string;
  failedAt: string;
}
```

**Callback Signature**:
```typescript
connection.on('OnGenerationFailed', (error: GenerationError) => {
  console.error(`Campaign generation failed:`, error.errorMessage);
  // Show error notification to user
});
```

```csharp
connection.On<GenerationError>("OnGenerationFailed", error =>
{
    Console.WriteLine($"Campaign generation failed: {error.ErrorMessage}");
});
```

---

#### OnChunkReceived

Fired when streaming partial responses from agents (for long-form content).

**Payload**:
```typescript
interface ArtifactChunk {
  campaignId: string;
  artifactType: string;
  chunk: string;
  isComplete: boolean;
}
```

**Callback Signature**:
```typescript
connection.on('OnChunkReceived', (chunk: ArtifactChunk) => {
  // Append chunk to UI
  appendToArtifact(chunk.artifactType, chunk.chunk);
  
  if (chunk.isComplete) {
    finalizeArtifact(chunk.artifactType);
  }
});
```

---

## Complete Example

### TypeScript/React Example

```typescript
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useEffect, useState } from 'react';

export function useCampaignHub(campaignId: string, accessToken: string) {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [artifacts, setArtifacts] = useState<CampaignArtifact[]>([]);
  const [complianceResult, setComplianceResult] = useState<ComplianceAuditResult | null>(null);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5001/hubs/campaign', {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff: 0s, 2s, 4s, 8s, 16s, 30s
          if (retryContext.previousRetryCount === 0) return 0;
          if (retryContext.previousRetryCount < 5) {
            return Math.pow(2, retryContext.previousRetryCount) * 1000;
          }
          return 30000;
        },
      })
      .configureLogging(LogLevel.Information)
      .build();

    // Set up event handlers
    newConnection.on('OnCampaignStarted', (id) => {
      console.log('Campaign started:', id);
    });

    newConnection.on('OnArtifactGenerated', (artifact: CampaignArtifact) => {
      setArtifacts((prev) => [...prev, artifact]);
    });

    newConnection.on('OnComplianceCheckCompleted', (result: ComplianceAuditResult) => {
      setComplianceResult(result);
    });

    newConnection.on('OnCampaignCompleted', (id) => {
      console.log('Campaign completed:', id);
    });

    newConnection.on('OnGenerationFailed', (error) => {
      console.error('Generation failed:', error);
    });

    // Start connection and join campaign group
    newConnection.start()
      .then(() => {
        console.log('Connected to SignalR hub');
        return newConnection.invoke('JoinCampaignGroup', campaignId);
      })
      .then(() => {
        console.log('Joined campaign group:', campaignId);
      })
      .catch((err) => {
        console.error('Connection error:', err);
      });

    setConnection(newConnection);

    // Cleanup
    return () => {
      if (newConnection.state === 'Connected') {
        newConnection.invoke('LeaveCampaignGroup', campaignId)
          .then(() => newConnection.stop())
          .catch((err) => console.error('Disconnection error:', err));
      }
    };
  }, [campaignId, accessToken]);

  return { connection, artifacts, complianceResult };
}
```

### C# Console Example

```csharp
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5001/hubs/campaign", options =>
    {
        options.AccessTokenProvider = () => Task.FromResult(accessToken);
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<string>("OnCampaignStarted", campaignId =>
{
    Console.WriteLine($"Campaign {campaignId} started");
});

connection.On<CampaignArtifact>("OnArtifactGenerated", artifact =>
{
    Console.WriteLine($"[{artifact.Type}] {artifact.Content}");
});

connection.On<ComplianceAuditResult>("OnComplianceCheckCompleted", result =>
{
    Console.WriteLine($"Compliance: {(result.IsCompliant ? "APPROVED" : "REJECTED")}");
    Console.WriteLine($"Score: {result.ComplianceScore}");
});

connection.On<string>("OnCampaignCompleted", campaignId =>
{
    Console.WriteLine($"Campaign {campaignId} completed!");
});

await connection.StartAsync();
await connection.InvokeAsync("JoinCampaignGroup", "camp_abc123");

Console.WriteLine("Listening for campaign updates. Press Enter to exit.");
Console.ReadLine();

await connection.InvokeAsync("LeaveCampaignGroup", "camp_abc123");
await connection.StopAsync();
```

## Connection Lifecycle

### Reconnection Strategy

SignalR automatically reconnects when the connection is lost:

1. **Immediate retry** (0ms delay)
2. **2 second delay**
3. **4 second delay**
4. **8 second delay**
5. **16 second delay**
6. **30 second delay** (continues indefinitely)

### Connection States

- `Disconnected` - Initial state, not connected
- `Connecting` - Attempting to connect
- `Connected` - Successfully connected
- `Reconnecting` - Connection lost, attempting to reconnect

### Handling Disconnections

```typescript
connection.onreconnecting((error) => {
  console.log('Connection lost. Reconnecting...', error);
  // Show reconnecting UI
});

connection.onreconnected((connectionId) => {
  console.log('Reconnected with ID:', connectionId);
  // Re-join campaign groups
  connection.invoke('JoinCampaignGroup', campaignId);
});

connection.onclose((error) => {
  console.error('Connection closed:', error);
  // Attempt manual reconnection or show error
});
```

## Error Handling

### Server Errors

When a hub method invocation fails:

```typescript
try {
  await connection.invoke('JoinCampaignGroup', campaignId);
} catch (error) {
  console.error('Failed to join campaign group:', error);
}
```

### Client Errors

Handle errors in event callbacks:

```typescript
connection.on('OnArtifactGenerated', (artifact) => {
  try {
    updateUI(artifact);
  } catch (error) {
    console.error('Failed to process artifact:', error);
  }
});
```

## Best Practices

1. **Always use automatic reconnection** - Enable `withAutomaticReconnect()` for resilience
2. **Clean up connections** - Invoke `LeaveCampaignGroup` and `stop()` when unmounting
3. **Handle all connection states** - Implement `onreconnecting`, `onreconnected`, `onclose`
4. **Use strongly-typed interfaces** - Define TypeScript interfaces for all payloads
5. **Implement exponential backoff** - Use increasing delays for reconnection attempts
6. **Join groups after reconnection** - Re-subscribe to campaign groups after reconnecting

## Next Steps

- [REST API Documentation](rest-api.md) - HTTP API reference
- [System Design](../architecture/system-design.md) - Architecture overview
- [Quick Start Guide](../getting-started/quick-start.md) - Get started
