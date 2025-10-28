# Installation

This guide walks you through installing all prerequisites for developing and running the Marketing Agents Platform.

## Prerequisites

### Required Software

#### 1. .NET 9 SDK

The Marketing Agents Platform is built with .NET 9. Install the latest SDK:

=== "macOS (Homebrew)"
    ```bash
    brew install dotnet@9
    ```

=== "macOS (Direct Download)"
    Download from [dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

=== "Windows"
    Download the installer from [dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

=== "Linux (Ubuntu/Debian)"
    ```bash
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 9.0
    ```

**Verify Installation:**
```bash
dotnet --version
# Should output 9.0.x
```

#### 2. Docker Desktop

Docker is required for running local emulators (Cosmos DB, Redis) via .NET Aspire.

=== "macOS"
    Download from [docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop)

=== "Windows"
    Download from [docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop)

=== "Linux"
    ```bash
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    ```

**Verify Installation:**
```bash
docker --version
# Should output Docker version 20.x or higher

docker compose version
# Should output Docker Compose version 2.x or higher
```

#### 3. Node.js and pnpm (for Frontend Development)

Required for Next.js frontend (Task 002).

=== "macOS (Homebrew)"
    ```bash
    brew install node@20
    npm install -g pnpm
    ```

=== "Windows (Chocolatey)"
    ```powershell
    choco install nodejs-lts
    npm install -g pnpm
    ```

=== "Linux (NodeSource)"
    ```bash
    curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
    sudo apt-get install -y nodejs
    npm install -g pnpm
    ```

**Verify Installation:**
```bash
node --version
# Should output v20.x or higher

pnpm --version
# Should output 9.x or higher
```

### Optional Tools

#### Azure CLI (for Deployment)

Required for deploying to Azure Container Apps.

=== "macOS (Homebrew)"
    ```bash
    brew install azure-cli
    ```

=== "Windows (Installer)"
    Download from [learn.microsoft.com/cli/azure/install-azure-cli](https://learn.microsoft.com/cli/azure/install-azure-cli)

=== "Linux (apt)"
    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
    ```

**Verify Installation:**
```bash
az --version
# Should output azure-cli 2.x or higher
```

#### Azure Developer CLI (azd)

Simplifies deployment to Azure Container Apps with Aspire.

=== "macOS (Homebrew)"
    ```bash
    brew tap azure/azd
    brew install azd
    ```

=== "Windows (PowerShell)"
    ```powershell
    powershell -ex AllSigned -c "Invoke-RestMethod 'https://aka.ms/install-azd.ps1' | Invoke-Expression"
    ```

=== "Linux (curl)"
    ```bash
    curl -fsSL https://aka.ms/install-azd.sh | bash
    ```

**Verify Installation:**
```bash
azd version
# Should output azd version 1.x or higher
```

#### Recommended IDE

- **Visual Studio 2022** (v17.9 or later) with .NET Aspire workload
- **Visual Studio Code** with C# Dev Kit extension
- **JetBrains Rider** 2024.1 or later

## Environment Setup

### Clone the Repository

```bash
git clone https://github.com/EmeaAppGbb/marketing-agents.git
cd marketing-agents
```

### Configure User Secrets (Optional)

For local development, configure .NET User Secrets if you need to override default settings:

```bash
# Navigate to the API project
cd MarketingAgents.Api

# Initialize user secrets
dotnet user-secrets init

# Set example secrets (optional)
dotnet user-secrets set "ConnectionStrings:CosmosDb" "your-connection-string"
dotnet user-secrets set "AzureAI:Endpoint" "https://your-endpoint.openai.azure.com/"
```

### Verify Installation

Run the following command to restore NuGet packages and verify the setup:

```bash
dotnet restore
```

Expected output:
```
Determining projects to restore...
Restored MarketingAgents.ServiceDefaults
Restored MarketingAgents.Api
Restored MarketingAgents.AgentHost
Restored MarketingAgents.AppHost
Restore succeeded.
```

## Next Steps

Once you've installed all prerequisites:

1. **[Quick Start Guide](quick-start.md)** - Run the application for the first time
2. **[Development Setup](../development/setup.md)** - Configure your development environment
3. **[Configuration](configuration.md)** - Learn about configuration options

## Troubleshooting

### .NET SDK Not Found

If `dotnet --version` fails:
- Ensure the SDK is installed (not just the runtime)
- Check your PATH environment variable includes the .NET installation directory

### Docker Not Running

If Aspire cannot start containers:
- Ensure Docker Desktop is running
- Check Docker is in your PATH: `docker ps`
- On Linux, add your user to the `docker` group: `sudo usermod -aG docker $USER`

### Port Conflicts

If you see "Address already in use" errors:
- Check for services using ports 5000, 5001, 6379 (Redis), 8081 (Cosmos DB emulator)
- Stop conflicting services or change ports in `appsettings.Development.json`

### Package Restore Failures

If NuGet restore fails:
- Clear NuGet caches: `dotnet nuget locals all --clear`
- Check internet connectivity
- Verify NuGet.org is accessible: `dotnet nuget list source`
