# Task 015: SignalR Real-Time Integration (Frontend)

## Description
Integrate SignalR client for real-time campaign orchestration updates, event streaming, and connection management with automatic reconnection and snapshot recovery.

## Dependencies
- Task 002: Frontend Scaffolding
- Task 012: Real-Time Streaming & SignalR Integration (Backend)
- Task 014: Frontend API Client & SDK Generation

## Technical Requirements

### Package Installation
Install required packages:
- `@microsoft/signalr` - SignalR client library
- Latest stable version

### SignalR Client Configuration
Create SignalR connection manager:

**`lib/signalr/client.ts`:**
```typescript
import * as signalR from '@microsoft/signalr';

export const createCampaignHubConnection = () => {
  return new signalR.HubConnectionBuilder()
    .withUrl(process.env.NEXT_PUBLIC_WS_URL || 'http://localhost:5000/hubs/campaign', {
      withCredentials: false, // true when auth is added
    })
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        // Exponential backoff: 0s, 2s, 4s, 8s, 16s, 32s
        if (retryContext.previousRetryCount < 6) {
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 32000);
        }
        return null; // Stop retrying after 6 attempts
      },
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();
};
```

### Connection State Management
Create connection context with React Context API:

**`lib/signalr/CampaignHubContext.tsx`:**
- Provide connection instance to app
- Manage connection lifecycle
- Track connection state (Connected, Disconnected, Reconnecting)
- Provide connection status to UI
- Handle reconnection events

**Connection States:**
```typescript
type ConnectionState = 
  | 'Disconnected'
  | 'Connecting'
  | 'Connected'
  | 'Reconnecting'
  | 'Failed';

interface CampaignHubContextValue {
  connection: signalR.HubConnection | null;
  connectionState: ConnectionState;
  subscribeToEvent: (handler: EventHandler) => () => void;
  subscribeToCampaign: (campaignId: string) => Promise<void>;
  unsubscribeFromCampaign: (campaignId: string) => Promise<void>;
}
```

### Event Type Definitions
Create TypeScript types for SignalR events:

**`lib/signalr/types.ts`:**
```typescript
export type OrchestrationState = 
  | 'Queued'
  | 'GeneratingCopy'
  | 'GeneratingShortCopy'
  | 'GeneratingVisual'
  | 'GenerationComplete'
  | 'Auditing'
  | 'Completed'
  | 'PartiallyCompleted'
  | 'Failed'
  | 'Cancelled';

export interface OrchestrationEvent {
  eventId: string;
  runId: string;
  state: OrchestrationState;
  agentType?: string;
  message: string;
  timestamp: string;
  metadata?: Record<string, unknown>;
  sequenceId: number;
}

export interface ArtifactUpdate {
  campaignId: string;
  artifactType: 'Copy' | 'ShortCopy' | 'VisualConcept';
  versionId: string;
  timestamp: string;
}

export interface CampaignSnapshot {
  campaignId: string;
  currentState: OrchestrationState;
  completedArtifacts: string[];
  pendingArtifacts: string[];
  auditStatus: 'NotStarted' | 'InProgress' | 'Completed';
  lastEventSequence: number;
  timestamp: string;
}
```

### React Hooks for SignalR
Create custom hooks for SignalR interactions:

**`hooks/useCampaignEvents.ts`:**
```typescript
export function useCampaignEvents(campaignId: string | null) {
  const { connection, subscribeToCampaign, unsubscribeFromCampaign } = useCampaignHub();
  const [events, setEvents] = useState<OrchestrationEvent[]>([]);
  const [latestEvent, setLatestEvent] = useState<OrchestrationEvent | null>(null);

  useEffect(() => {
    if (!connection || !campaignId) return;

    // Subscribe to campaign
    subscribeToCampaign(campaignId);

    // Listen for events
    const handleEvent = (event: OrchestrationEvent) => {
      if (event.runId === campaignId) {
        setEvents(prev => [...prev, event].sort((a, b) => a.sequenceId - b.sequenceId));
        setLatestEvent(event);
      }
    };

    connection.on('ReceiveOrchestrationEvent', handleEvent);

    return () => {
      connection.off('ReceiveOrchestrationEvent', handleEvent);
      unsubscribeFromCampaign(campaignId);
    };
  }, [connection, campaignId]);

  return { events, latestEvent };
}
```

**`hooks/useConnectionStatus.ts`:**
```typescript
export function useConnectionStatus() {
  const { connectionState } = useCampaignHub();
  const [isOnline, setIsOnline] = useState(true);

  useEffect(() => {
    const handleOnline = () => setIsOnline(true);
    const handleOffline = () => setIsOnline(false);

    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    return () => {
      window.removeEventListener('online', handleOnline);
      window.removeEventListener('offline', handleOffline);
    };
  }, []);

  return {
    connectionState,
    isOnline,
    isConnected: connectionState === 'Connected',
  };
}
```

### Event Ordering and Deduplication
Implement client-side event ordering:
- Sort events by `sequenceId`
- Deduplicate events by `eventId`
- Handle out-of-order delivery
- Buffer events during reconnection

**Event Buffer:**
```typescript
class EventBuffer {
  private buffer: OrchestrationEvent[] = [];
  private processedIds = new Set<string>();

  add(event: OrchestrationEvent): OrchestrationEvent[] {
    if (this.processedIds.has(event.eventId)) {
      return []; // Duplicate
    }
    
    this.buffer.push(event);
    this.processedIds.add(event.eventId);
    
    // Sort and return in order
    return this.buffer.sort((a, b) => a.sequenceId - b.sequenceId);
  }

  clear() {
    this.buffer = [];
    this.processedIds.clear();
  }
}
```

### Reconnection with Snapshot Recovery
Implement snapshot recovery on reconnection:

**On Reconnect:**
1. Detect reconnection event
2. Request campaign snapshot from server
3. Process snapshot to determine current state
4. Update UI with authoritative state
5. Resume event streaming from last sequence

**Snapshot Handler:**
```typescript
connection.on('ReceiveCampaignSnapshot', (snapshot: CampaignSnapshot) => {
  // Update state from authoritative snapshot
  setCurrentState(snapshot.currentState);
  setCompletedArtifacts(snapshot.completedArtifacts);
  // Resume from last sequence
  setLastSequence(snapshot.lastEventSequence);
});

connection.onreconnected(async () => {
  if (currentCampaignId) {
    // Request snapshot for current campaign
    await connection.invoke('GetCampaignSnapshot', currentCampaignId);
  }
});
```

### Connection Lifecycle Management
Handle connection events:

**On Connected:**
- Log connection established
- Subscribe to active campaign if any
- Update UI connection indicator

**On Disconnected:**
- Log disconnection
- Show offline indicator
- Queue failed operations for retry

**On Reconnecting:**
- Show reconnecting indicator
- Disable user interactions with real-time features

**On Reconnected:**
- Request snapshot for recovery
- Resume subscriptions
- Re-enable interactions

### Error Handling
Handle SignalR errors:
- Connection failures
- Invoke errors (hub method calls)
- Message parsing errors
- Reconnection failures

**Error Boundaries:**
- Wrap SignalR components in error boundaries
- Graceful degradation to polling fallback (future)
- User-friendly error messages

### UI Connection Indicator
Create connection status component:
- Show connection state visually
- Display reconnection progress
- Show offline warning
- Provide manual reconnect button

### Integration with TanStack Query
Coordinate SignalR updates with React Query cache:
- Invalidate queries on artifact updates
- Update cache optimistically on events
- Prevent stale data issues

**Cache Invalidation on Events:**
```typescript
connection.on('ReceiveArtifactUpdate', (update: ArtifactUpdate) => {
  // Invalidate relevant queries
  queryClient.invalidateQueries({
    queryKey: ['campaign', update.campaignId],
  });
  queryClient.invalidateQueries({
    queryKey: ['artifacts', update.campaignId],
  });
});
```

### Performance Optimization
Optimize event handling:
- Throttle rapid event updates (100ms window)
- Debounce UI updates
- Virtualize event lists if large
- Cleanup event subscriptions properly

## Acceptance Criteria
- [ ] SignalR client configured with automatic reconnection
- [ ] CampaignHub context provides connection to app
- [ ] Connection state tracked and displayed in UI
- [ ] Campaign event subscription/unsubscription working
- [ ] Events received and ordered correctly by sequenceId
- [ ] Duplicate events filtered out
- [ ] Reconnection triggers snapshot recovery
- [ ] Snapshot updates UI with authoritative state
- [ ] Connection status indicator showing current state
- [ ] Integration with React Query cache invalidation
- [ ] Event delivery latency <2s measured
- [ ] Reconnection restoration <3s measured

## Testing Requirements
- [ ] Unit tests for connection manager
- [ ] Unit tests for event ordering and deduplication
- [ ] Unit tests for snapshot recovery logic
- [ ] Test connection lifecycle events
- [ ] Test reconnection scenarios
- [ ] Test event subscription/unsubscription
- [ ] Test integration with React Query
- [ ] Test offline/online transitions
- [ ] E2E tests for real-time updates
- [ ] Test connection failure recovery

## Non-Functional Requirements
- Event delivery latency <2s from backend to UI update
- Reconnection time <3s
- Event ordering accuracy 100%
- Support for 50+ events per campaign
- Memory usage <10MB for event buffers
- No memory leaks on connection cleanup

## Out of Scope
- Polling fallback mechanism (future)
- Historical event replay UI (future)
- Persistent event storage (future)
- Multi-tab synchronization (future)
- Custom reconnection strategies beyond exponential backoff

## Notes
- Follow AGENTS.md realtime communication patterns
- Use TypeScript strict mode for all SignalR code
- Test reconnection thoroughly in development
- Handle network failures gracefully
- Document SignalR integration in developer guide
- Create connection status indicator component early
- Consider WebSocket vs long-polling behavior (SignalR handles automatically)
- Future: add authentication token to connection
- Ensure proper cleanup to prevent memory leaks
- Test with network throttling and disconnections
