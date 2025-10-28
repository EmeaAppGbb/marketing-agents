# Task 012: Real-Time Streaming & SignalR Integration (Backend)

## Description
Implement the real-time backend infrastructure using ASP.NET Core SignalR for streaming campaign orchestration events, artifact updates, and status changes to connected frontend clients. Support reconnection with snapshot recovery.

## Dependencies
- Task 001: Backend Scaffolding
- Task 010: Campaign Orchestration Implementation

## Technical Requirements

### SignalR Hub Implementation
Create `CampaignHub` class extending `Hub`:
- Hub methods for client subscriptions
- Type-safe hub interfaces
- Connection lifecycle management
- Group-based broadcasting per campaign

**Hub Methods:**
- `Task SubscribeToCampaign(string campaignId)`
- `Task UnsubscribeFromCampaign(string campaignId)`
- `Task GetCampaignSnapshot(string campaignId)` - for reconnection

**Client Interface (for strong typing):**
```csharp
public interface ICampaignClient
{
    Task ReceiveOrchestrationEvent(OrchestrationEvent evt);
    Task ReceiveArtifactUpdate(ArtifactUpdate update);
    Task ReceiveCampaignSnapshot(CampaignSnapshot snapshot);
    Task ReceiveCancellationAcknowledgement(string runId);
}
```

### Event Broadcasting Service
Implement `IOrchestrationEventPublisher` interface (from Task 010):

Create `SignalREventPublisher`:
- Constructor inject `IHubContext<CampaignHub, ICampaignClient>`
- Implement `PublishAsync` to broadcast events to subscribed clients
- Use campaign-based groups for targeted broadcasting
- Add sequence IDs to events for client-side ordering

**Event Publishing:**
```csharp
Task PublishAsync(OrchestrationEvent evt):
1. Add sequence ID to event
2. Determine target group (campaignId)
3. Broadcast to group via IHubContext
4. Log event publication
5. Emit telemetry
```

### Event Sequencing
Add sequence tracking:
- `IEventSequencer` interface
- In-memory or Redis-based sequence generator
- Monotonically increasing sequence per campaign
- Included in all events for client-side ordering

### Snapshot Recovery
Implement snapshot generation for reconnection:

**CampaignSnapshot Record:**
- CampaignId (string)
- CurrentState (OrchestrationState)
- CompletedArtifacts (ArtifactType[])
- PendingArtifacts (ArtifactType[])
- AuditStatus (enum: NotStarted, InProgress, Completed)
- LastEventSequence (long)
- Timestamp (DateTimeOffset)

**Snapshot Service:**
- `ICampaignSnapshotService` interface
- `Task<CampaignSnapshot> GetSnapshotAsync(string campaignId)`
- Query campaign and orchestration run status from database
- Build current state representation
- Return authoritative snapshot within <500ms

### Connection Management
Track connected clients:
- `IConnectionTracker` interface (optional for metrics)
- Track connections per campaign
- Handle connection/disconnection events
- Clean up on disconnect

**Override Hub Methods:**
```csharp
OnConnectedAsync():
- Log connection
- Emit connection metric

OnDisconnectedAsync():
- Remove from campaign groups
- Log disconnection
- Emit disconnection metric
```

### CORS Configuration
Configure CORS for SignalR:
- Allow configured origins (development + production URLs)
- Support credentials (required for SignalR)
- Configure allowed headers and methods
- Add CORS middleware before SignalR routing

### SignalR Configuration
Configure in Program.cs:
- Add SignalR services with JSON protocol
- Configure message size limits
- Set up client timeout and keep-alive settings
- Enable detailed errors in development
- Configure Azure SignalR Service for production (future)

**Configuration Settings:**
```csharp
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 102400; // 100KB
});
```

### Backpressure Handling
Implement backpressure for streaming:
- Use `IAsyncEnumerable<T>` for streaming scenarios
- Implement `ChannelReader<T>` for buffered streaming
- Handle slow clients gracefully
- Drop events if client can't keep up (configurable)

### Error Handling
- Handle hub method exceptions gracefully
- Log all errors with connection context
- Return user-friendly error messages
- Emit error events to clients
- Don't expose internal errors to clients

### Metrics and Telemetry
Instrument the following metrics:
- Active connections count
- Events published per campaign
- Average event delivery latency
- Reconnection frequency
- Snapshot generation latency
- Group subscription counts
- Message size distribution
- Error rates

## Acceptance Criteria
- [ ] CampaignHub implemented with typed client interface
- [ ] SignalREventPublisher implements IOrchestrationEventPublisher
- [ ] Clients can subscribe/unsubscribe to campaigns
- [ ] Events broadcast to correct campaign groups
- [ ] Sequence IDs added to all events for ordering
- [ ] Snapshot recovery returns current campaign state in <500ms
- [ ] CORS configured for frontend origins
- [ ] Connection lifecycle events logged
- [ ] SignalR configured with appropriate timeouts
- [ ] Event delivery latency <2s measured

## Testing Requirements
- [ ] Unit tests for SignalREventPublisher (≥85% coverage)
- [ ] Unit tests for CampaignSnapshotService
- [ ] Integration tests for SignalR hub methods
- [ ] Test event broadcasting to subscribed clients
- [ ] Test group management (subscribe/unsubscribe)
- [ ] Test snapshot generation accuracy
- [ ] Test reconnection with snapshot recovery
- [ ] Test CORS configuration
- [ ] Test concurrent connections per campaign (10+)
- [ ] Test event ordering with sequence IDs
- [ ] Load test event throughput

## Non-Functional Requirements
- Event delivery latency ≤2s from emission to client render
- Missed event rate <1%
- Reconnection restoration <3s
- Snapshot generation <500ms
- Support 50+ concurrent connections
- Event delivery reliability ≥95%

## Out of Scope
- Token-level streaming of agent responses (future)
- Historical event replay beyond snapshot (future)
- Progress percentage estimation (future)
- Persistent event history storage (events are ephemeral in MVP)
- Azure SignalR Service integration (local only in MVP)

## Notes
- Follow AGENTS.md realtime communication patterns
- Use strongly-typed hub contracts
- Query Microsoft Docs MCP for SignalR best practices
- Configure backpressure handling appropriately
- Test reconnection scenarios thoroughly
- Document hub methods with XML comments
- Consider WebSocket fallback behavior (automatic in SignalR)
- Create MADR for event streaming architecture decisions
- Future: persist events for replay and analytics
- Ensure snapshot is authoritative state for consistency
