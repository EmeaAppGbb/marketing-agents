# Marketing Agents Platform

Welcome to the **Marketing Agents Platform** documentation. This platform enables AI-powered marketing campaign generation with built-in compliance and audit capabilities.

## Overview

The Marketing Agents Platform is a modern, cloud-native application that orchestrates multiple AI agents to:

- Generate creative copywriting for marketing campaigns
- Create visual concepts and poster designs
- Provide short-form copy for social media and ads
- Audit campaigns for brand compliance and regulatory requirements
- Enable iterative refinement through feedback loops
- Stream real-time campaign artifacts to users

## Key Features

✨ **AI Agent Orchestration**: Coordinate multiple specialized agents (copywriting, visual design, compliance) to create complete campaigns  
🔄 **Real-Time Streaming**: Live updates via SignalR as agents generate campaign artifacts  
✅ **Compliance-First**: Built-in audit and compliance agents ensure brand guidelines and regulatory adherence  
📊 **Campaign Dashboard**: Review, approve, or request revisions for generated campaigns  
🧪 **Iterative Refinement**: Feedback loops allow campaigns to be improved through multiple iterations  
🏗️ **Cloud-Native**: Built with .NET 9, ASP.NET Core, and .NET Aspire for modern cloud deployment  

## Technology Stack

### Backend
- **.NET 9** with ASP.NET Core Minimal APIs
- **.NET Aspire** for local development orchestration
- **Microsoft Agent Framework** for AI agent workflows
- **Azure Cosmos DB** for data persistence
- **SignalR** for real-time communication
- **OpenTelemetry** for observability

### Frontend
- **Next.js 14** with App Router
- **React 18** with TypeScript
- **TanStack Query** for state management
- **Zustand** for local state

### Infrastructure
- **Azure Container Apps** for deployment
- **Azure AI Foundry** for agent runtime
- **GitHub Actions** for CI/CD

## Quick Links

- [Installation Guide](getting-started/installation.md) - Set up your development environment
- [Quick Start Tutorial](getting-started/quick-start.md) - Build and run your first campaign
- [Architecture Overview](architecture/overview.md) - Understand the system design
- [Development Setup](development/setup.md) - Configure your local development environment


## Project Structure

```
marketing-agents/
├── MarketingAgents.AppHost/          # .NET Aspire orchestration
├── MarketingAgents.ServiceDefaults/  # Shared service configuration
├── MarketingAgents.Api/              # REST API service
├── MarketingAgents.AgentHost/        # AI agent execution service
├── MarketingAgents.Web/              # Next.js frontend (future)
├── docs/                             # MkDocs documentation
├── specs/                            # Product specs and ADRs
└── tests/                            # Test projects
```

## Getting Started

Follow these steps to get started with the Marketing Agents Platform:

1. **[Install Prerequisites](getting-started/installation.md)** - .NET 9 SDK, Docker, Azure CLI
2. **[Clone and Configure](getting-started/quick-start.md)** - Get the source code and configure environment
3. **[Run Locally](getting-started/quick-start.md)** - Start the Aspire dashboard and services
4. **[Explore the API](getting-started/quick-start.md)** - Test endpoints via Swagger UI

## Support

- **GitHub Issues**: Report bugs or request features via GitHub Issues
- **Documentation**: Browse this documentation for guides and references
- **Specifications**: Review product specs and ADRs for design decisions in the repository

## License

Copyright © 2025 Microsoft Corporation. All rights reserved.
