# Quick Start

Get the Marketing Agents Platform running locally in under 5 minutes.

## Prerequisites

Before starting, ensure you've completed the [Installation Guide](installation.md).

You need:
- ✅ .NET 9 SDK installed
- ✅ Docker Desktop running
- ✅ Repository cloned

## Step 1: Clone Repository

If you haven't already cloned the repository:

```bash
git clone https://github.com/EmeaAppGbb/marketing-agents.git
cd marketing-agents
```

## Step 2: Restore Dependencies

### Backend Dependencies

Restore all NuGet packages using Central Package Management:

```bash
dotnet restore
```

Expected output:
```
Determining projects to restore...
Restored MarketingAgents.ServiceDefaults (in 1.2s)
Restored MarketingAgents.Api (in 1.5s)
Restored MarketingAgents.AgentHost (in 1.3s)
Restored MarketingAgents.AppHost (in 0.8s)
Restore succeeded.
```

### Frontend Dependencies

Install Node.js packages for the frontend application:

```bash
pnpm install
```

Expected output:
```
Scope: all 2 workspace projects
Lockfile is up to date, resolution step is skipped
Packages: +618
++++++++++++++++++++++++++++++++++++++++++++++++
Progress: resolved 618, reused 618, downloaded 0, added 618, done
Done in 5.3s
```

!!! note "Workspace Mode"
    The project uses pnpm workspace mode. Running `pnpm install` at the repository root installs dependencies for all workspace packages, including `MarketingAgents.Web`.

## Step 3: Run Aspire Dashboard

Start the .NET Aspire orchestrator, which will launch all services and dependencies:

```bash
dotnet run --project MarketingAgents.AppHost
```

!!! success "Expected Output"
    ```
    Building...
    info: Aspire.Hosting.DistributedApplication[0]
          Aspire version: 9.5.1
          Distributed application starting...
          
    Now listening on: http://localhost:15888
    ```

The Aspire dashboard will automatically open in your browser at **http://localhost:15888**

!!! tip "Dashboard Features"
    The Aspire dashboard provides:
    - **Resources Tab**: View all services (Api, AgentHost, Redis, Cosmos DB)
    - **Console Logs**: Real-time logs from each service
    - **Traces**: OpenTelemetry distributed traces
    - **Metrics**: Service metrics and health status
    - **Environment**: View environment variables and configuration

## Step 4: Verify Services

### Check Service Status in Dashboard

In the Aspire dashboard Resources tab, you should see:

| Resource | Type | State | Endpoint |
|----------|------|-------|----------|
| **apiservice** | Project | Running | http://localhost:5001 |
| **agenthost** | Project | Running | http://localhost:5002 |
| **webapp** | NPM App | Running | http://localhost:3000 |
| **cache** (Redis) | Container | Running | localhost:6379 |
| **cosmosdb** (Emulator) | Container | Running | https://localhost:8081 |

All resources should show **green status** (Running).

### Test Health Endpoints

Verify the API service is healthy:

```bash
curl http://localhost:5001/health
```

Expected response:
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456"
}
```

Test the alive endpoint:

```bash
curl http://localhost:5001/alive
```

Expected response:
```json
{
  "status": "Healthy"
}
```

## Step 5: Explore the Application

### Access the Frontend

Navigate to the web application in your browser:

**http://localhost:3000**

You'll see the Marketing Agents Platform home page with:
- Campaign creation interface
- Campaign listing and management
- Real-time updates via SignalR

!!! tip "Hot Module Replacement"
    The frontend uses Next.js Fast Refresh. Changes to React components are reflected instantly without losing component state.

### Open Swagger UI (Backend API)

Navigate to the API's Swagger documentation:

**http://localhost:5001/swagger**

You'll see the auto-generated OpenAPI documentation with available endpoints.

### Test Sample Endpoint

Try the sample weather forecast endpoint:

```bash
curl http://localhost:5001/weatherforecast
```

Expected response:
```json
[
  {
    "date": "2025-01-20",
    "temperatureC": 15,
    "temperatureF": 58,
    "summary": "Cool"
  },
  ...
]
```

## Step 6: View Telemetry

### OpenTelemetry Traces

1. In the Aspire dashboard, click the **Traces** tab
2. Make a request to the API: `curl http://localhost:5001/weatherforecast`
3. Refresh the Traces view
4. Click on a trace to see:
   - Request duration
   - HTTP method and status code
   - Span hierarchy showing service interactions

### Logs

1. Click the **Console Logs** tab in the dashboard
2. Select **apiservice** from the resource dropdown
3. See structured logs from the API service with log levels, timestamps, and context

### Metrics

1. Click the **Metrics** tab
2. Select metrics like:
   - `http.server.request.duration` - API response times
   - `process.cpu.usage` - CPU utilization
   - `dotnet.gc.collections.count` - Garbage collection metrics

## Step 7: Run Tests

Execute the test suite to verify everything is working:

### Backend Tests

```bash
# Run all backend tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

Expected output:
```
Test run for MarketingAgents.Api.IntegrationTests.dll (.NET 9.0)
Microsoft (R) Test Execution Command Line Tool Version 17.x

Starting test execution, please wait...
A total of 2 tests completed in 3.7s

Test Run Successful.
Total tests: 2
     Passed: 2
```

### Frontend Tests

Run unit tests for the frontend application:

```bash
cd MarketingAgents.Web
pnpm test
```

Expected output:
```
✓ src/components/campaign-card.test.tsx (6 tests) 237ms
   ✓ Campaign Card Component (6 tests) 236ms
     ✓ renders campaign information correctly
     ✓ displays correct status styling
     ✓ handles click events
     ✓ handles keyboard events
     ✓ renders error state correctly
     ✓ formats dates correctly

Test Files  1 passed (1)
     Tests  6 passed (6)
```

Run frontend tests with coverage:

```bash
pnpm test:coverage
```

Run E2E tests with Playwright (requires running application):

```bash
pnpm test:e2e
```

## What's Next?

Now that you have the platform running:

1. **[Configuration Guide](configuration.md)** - Customize settings and environment variables
2. **[Architecture Overview](../architecture/overview.md)** - Understand the system design
3. **[Development Setup](../development/setup.md)** - Set up your IDE and tools for development
4. **Frontend Development** - Build UI features with Next.js, TanStack Query, and Zustand
5. **Backend Development** - Create API endpoints and agent workflows
6. **Testing Guide** - Write unit, integration, and E2E tests

## Troubleshooting

### Dashboard Not Opening

If the Aspire dashboard doesn't open automatically:
- Manually navigate to **http://localhost:15888**
- Check the console output for the correct port
- Ensure no firewall is blocking localhost connections

### Services Not Starting

If services show "Failed" status in the dashboard:
1. Check console logs for error messages
2. Ensure Docker Desktop is running (for Redis and Cosmos DB)
3. Verify ports 5001, 5002, 6379, 8081 are not in use
4. Check Docker has sufficient resources (RAM/CPU)

### Cosmos DB Emulator Issues

If Cosmos DB emulator fails to start:
- **macOS/Linux**: The Cosmos DB emulator runs in Docker. Ensure Docker has at least 4GB RAM allocated
- **Windows**: Consider using the native Cosmos DB Emulator instead of the container
- Check Docker logs: `docker logs aspire-cosmosdb-container`

### Port Conflicts

If you see "Address already in use" errors:

```bash
# Check what's using the ports
lsof -i :3000  # Frontend (webapp)
lsof -i :5001  # API service
lsof -i :5002  # AgentHost service
lsof -i :6379  # Redis
lsof -i :8081  # Cosmos DB

# Kill conflicting processes or change ports in appsettings.Development.json
```

### Frontend Build Errors

If the frontend fails to start:
- Ensure all dependencies are installed: `pnpm install`
- Clear Next.js cache: `cd MarketingAgents.Web && rm -rf .next`
- Verify Node.js version: `node --version` (should be v20 or later)
- Check for TypeScript errors: `cd MarketingAgents.Web && pnpm tsc --noEmit`

### Performance Issues

If services are slow to start:
- Increase Docker Desktop memory allocation (Settings > Resources)
- Disable unnecessary services in `AppHost.cs` during development
- Use `dotnet build` before `dotnet run` to avoid build delays

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
