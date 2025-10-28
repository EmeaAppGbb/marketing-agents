# Development Setup

This guide helps you configure your local development environment for working on the Marketing Agents Platform.

## Prerequisites

Ensure you've completed the [Installation Guide](../getting-started/installation.md) before proceeding.

## IDE Configuration

### Visual Studio 2022 (Recommended for .NET Development)

#### Install Required Workloads

1. Open Visual Studio Installer
2. Modify your Visual Studio 2022 installation
3. Select the following workloads:
   - **ASP.NET and web development**
   - **.NET Aspire SDK** (under "Individual components")
   - **Azure development** (optional, for deployment)

#### Open Solution

```bash
# Open the solution file
open MarketingAgents.sln  # macOS
start MarketingAgents.sln  # Windows
```

Or from Visual Studio: **File > Open > Project/Solution** → `MarketingAgents.sln`

#### Set Startup Project

1. Right-click **MarketingAgents.AppHost** in Solution Explorer
2. Select **Set as Startup Project**
3. Press **F5** to run with debugging

The Aspire dashboard will launch automatically.

### Visual Studio Code (Cross-Platform)

#### Install Extensions

Install these recommended extensions:

```bash
# C# Dev Kit (includes C# extension)
code --install-extension ms-dotnettools.csdevkit

# .NET Aspire
code --install-extension ms-dotnettools.vscode-dotnet-aspire

# Azure Tools (optional)
code --install-extension ms-vscode.vscode-node-azure-pack
```

Or via Extensions view (`Cmd+Shift+X` / `Ctrl+Shift+X`):
- Search and install: **C# Dev Kit**
- Search and install: **.NET Aspire**

#### Open Workspace

```bash
cd marketing-agents
code .
```

#### Configure Launch Settings

VS Code should auto-detect the Aspire project. Press **F5** to launch, or:

1. Open Run and Debug view (`Cmd+Shift+D` / `Ctrl+Shift+D`)
2. Select **.NET Aspire** from dropdown
3. Click **Start Debugging** (F5)

### JetBrains Rider

#### Open Solution

```bash
rider MarketingAgents.sln
```

Or from Rider: **File > Open** → `MarketingAgents.sln`

#### Set Startup Project

1. Right-click **MarketingAgents.AppHost** in Explorer
2. Select **Set Startup Project**
3. Press **Shift+F10** to run

## Environment Configuration

### User Secrets

Configure .NET User Secrets for local development (optional overrides):

```bash
# Navigate to Api project
cd MarketingAgents.Api

# Initialize user secrets
dotnet user-secrets init

# Example: Override Cosmos DB connection string
dotnet user-secrets set "ConnectionStrings:CosmosDb" "AccountEndpoint=https://localhost:8081/;AccountKey=YOUR_KEY"

# Example: Configure Azure OpenAI
dotnet user-secrets set "AzureAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureAI:ApiKey" "your-api-key"
```

User secrets are stored in:
- **macOS/Linux**: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`

### Environment Variables

For settings shared across all projects, use environment variables:

```bash
# macOS/Linux (.zshrc or .bashrc)
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_ENVIRONMENT="Development"
```

### appsettings.Development.json

Each project has a `appsettings.Development.json` for local overrides:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Priority** (highest to lowest):
1. User Secrets
2. Environment Variables
3. `appsettings.Development.json`
4. `appsettings.json`

## Docker Configuration

### Ensure Docker is Running

```bash
# Check Docker status
docker ps

# If not running, start Docker Desktop
```

### Configure Docker Resources

For optimal Aspire performance:

1. Open **Docker Desktop Settings**
2. Navigate to **Resources**
3. Allocate:
   - **Memory**: At least 4 GB (8 GB recommended)
   - **CPUs**: At least 2 cores (4 recommended)
   - **Disk**: At least 20 GB

### Verify Containers

After running `dotnet run --project MarketingAgents.AppHost`, check containers:

```bash
docker ps
```

Expected output:
```
CONTAINER ID   IMAGE                            STATUS
abc123def456   redis:latest                     Up 10 seconds
def789ghi012   mcr.microsoft.com/cosmosdb/...   Up 15 seconds
```

## Database Setup

### Cosmos DB Emulator (Local)

Aspire automatically starts the Cosmos DB emulator in Docker.

**Access Emulator:**
- **Endpoint**: `https://localhost:8081`
- **Data Explorer**: https://localhost:8081/_explorer/index.html
- **Key**: `C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==`

**Create Database and Containers:**

The application will auto-create the database and containers on startup (in development mode).

**Manual Setup** (if needed):

```bash
# Install Azure Cosmos DB SDK CLI (optional)
dotnet tool install -g Microsoft.Azure.Cosmos.Table

# Or use Azure Storage Explorer to connect to local emulator
```

### Redis Cache (Local)

Aspire automatically starts Redis in Docker.

**Access Redis:**
```bash
# Connect via redis-cli
docker exec -it <redis-container-id> redis-cli

# Test connection
127.0.0.1:6379> PING
PONG
```

## Code Quality Tools

### EditorConfig

The repository includes `.editorconfig` for consistent code style. Ensure your IDE respects it:

- **Visual Studio**: Enabled by default
- **VS Code**: Install **EditorConfig for VS Code** extension
- **Rider**: Enabled by default

### Code Formatting

Format code before committing:

```bash
# Format entire solution
dotnet format

# Format specific project
dotnet format MarketingAgents.Api

# Verify formatting (CI mode)
dotnet format --verify-no-changes
```

### Analyzers

Roslyn analyzers run automatically during build:

- **StyleCop.Analyzers**: Code style enforcement
- **SonarAnalyzer.CSharp**: Code quality and security
- **Microsoft.CodeAnalysis.NetAnalyzers**: .NET-specific rules

Warnings are treated as errors. Suppress false positives in `GlobalSuppressions.cs`.

## Running the Application

### Start All Services

```bash
# From repository root
dotnet run --project MarketingAgents.AppHost
```

This starts:
- **AppHost** (Aspire dashboard)
- **Api service** (http://localhost:5001)
- **AgentHost service** (http://localhost:5002)
- **Redis** (localhost:6379)
- **Cosmos DB** (https://localhost:8081)

### Run Individual Services

For faster iteration when working on a single service:

```bash
# Run only the API service
dotnet run --project MarketingAgents.Api

# Run only the AgentHost
dotnet run --project MarketingAgents.AgentHost
```

**Note**: This skips service discovery and container orchestration.

### Hot Reload

.NET 9 supports hot reload for most code changes:

- **Visual Studio**: Automatic hot reload on save
- **CLI**: Use `dotnet watch` for auto-restart:
  ```bash
  dotnet watch --project MarketingAgents.Api
  ```

## Testing

### Run All Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
# Requires coverlet.collector package (already included)
dotnet test --collect:"XPlat Code Coverage"

# View coverage report (requires ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage -reporttypes:Html
open coverage/index.html
```

### Run Specific Tests

```bash
# Run tests in one project
dotnet test MarketingAgents.Api.IntegrationTests

# Run specific test method
dotnet test --filter "FullyQualifiedName~HealthCheckTests.HealthCheck_ReturnsOk"
```

### Debug Tests

- **Visual Studio**: Right-click test → **Debug Test**
- **VS Code**: Use **Test Explorer** sidebar
- **Rider**: Right-click test → **Debug**

## Troubleshooting

### Build Failures

If `dotnet build` fails:

```bash
# Clear build artifacts
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

### Aspire Dashboard Not Starting

If the dashboard doesn't appear:

```bash
# Check if port 15888 is in use
lsof -i :15888  # macOS/Linux
netstat -ano | findstr :15888  # Windows

# Kill the process or change the port in AppHost.cs
```

### Service Discovery Issues

If services can't communicate:

1. Ensure both services reference **ServiceDefaults**
2. Check `builder.AddServiceDefaults()` is called in Program.cs
3. Verify service names match in AppHost and HTTP client calls

### Cosmos DB Emulator SSL Issues

If you see SSL certificate errors:

```bash
# Trust the Cosmos DB emulator certificate (one-time setup)
# The emulator will prompt you to trust the cert on first run
```

Or disable SSL validation for local development (not recommended for production).

## Next Steps

- **[Architecture Overview](../architecture/overview.md)** - Understand system design
- **Backend Development Guide** (coming soon) - Build backend features
- **Testing Guide** (coming soon) - Write comprehensive tests
