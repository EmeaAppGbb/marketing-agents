// Copyright (c) Marketing Agents. All rights reserved.

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Agent services will be registered here in future tasks
var app = builder.Build();

// Map health check endpoints
app.MapDefaultEndpoints();

app.MapGet("/", () => "MarketingAgents AgentHost Service")
    .WithName("GetRoot");

await app.RunAsync();
