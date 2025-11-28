# GitHub Copilot Instructions for GitHub Models Playground

## Project Overview
This is a .NET Aspire distributed application demonstrating GitHub Models (AI) integration for blog content summarization. It's an **educational project** focused on teaching modern cloud-native patterns.

## Architecture & Components

### Three-Project Structure (all in `src/`)
1. **AppHost** - Aspire orchestrator (entry point, run this)
2. **ApiService** - Minimal API with `/summarize` endpoint
3. **ServiceDefaults** - Shared Aspire configurations (health, telemetry, resilience)

### Key Architectural Patterns
- **Service Discovery**: External services referenced by name (e.g., `"aspire-blog"`) are resolved at runtime via Aspire
- **Primary Constructors**: All services use C# 12 primary constructor syntax for DI
- **File-Scoped Namespaces**: Use `namespace X;` not `namespace X { }`
- **Minimal APIs**: No controllers, use `app.MapGet()` with inline handlers

## Critical Workflows

### Build & Run (MUST start from AppHost)
```bash
cd src/GitHubModelsPlayground.AppHost
dotnet run
```
This launches the Aspire Dashboard (usually http://localhost:15888) which manages all services.

### Clean Build (when references break)
```bash
# From repo root
Get-ChildItem -Path "src" -Include "obj","bin" -Recurse -Directory -Force | Remove-Item -Recurse -Force
cd src/GitHubModelsPlayground.AppHost
dotnet build
```

### Testing the API
```bash
curl "http://localhost:5000/summarize?slug=blog/aspire-preview-6"
```

## Project-Specific Conventions

### XML Documentation Required
- ALL public APIs must have `<summary>` tags
- Projects have `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Use `<NoWarn>$(NoWarn);1591</NoWarn>` to suppress missing doc warnings

### Educational Code Style
- Prefer **clear over clever** - this teaches .NET Aspire
- Add **inline comments explaining WHY**, not what
- Example from BlogSummarizer:
  ```csharp
  // We use primary constructors here (C# 12 feature) to reduce boilerplate
  public class BlogSummarizer(IChatClient chatClient)
  ```

### Dependency Injection Patterns
- Use `builder.Services.AddScoped<T>()` for per-request services (e.g., BlogSummarizer)
- Use `builder.Services.AddHttpClient<T>()` for services that need HTTP clients
- Aspire injects AI client via: `builder.AddAzureChatCompletionsClient("ai-model").AddChatClient()`

### Service Reference Pattern
In AppHost.cs, services are wired using `.WithReference()`:
```csharp
var apiService = builder.AddProject<Projects.GitHubModelsPlayground_ApiService>("apiservice")
    .WithReference(blogService)   // Makes "aspire-blog" discoverable
    .WithReference(aiModel);       // Injects GitHub PAT and model config
```

## External Dependencies

### GitHub Models Configuration
- Requires GitHub PAT stored in user secrets: `dotnet user-secrets set "GitHub:Token" "your-token"`
- Uses `GitHubModel.OpenAI.OpenAIGpt4oMini` via Aspire.Hosting.GitHub package
- The `IChatClient` abstraction comes from Microsoft.Extensions.AI

### External Service Integration
- **aspire.dev blog**: Fetched via HttpClient with base address resolved by Aspire service discovery
- **HTML Parsing**: Uses Regex to extract `<div class="sl-markdown-content">` then first `<p>` tag

## Common Gotchas

1. **Must run AppHost, not ApiService** - ApiService alone won't have service discovery
2. **Build artifacts pollution** - Copied `obj/` folders cause duplicate AssemblyInfo errors; clean with above PowerShell
3. **Service names are case-sensitive** - `"aspire-blog"` in AppHost must match exactly in HttpClient registration
4. **GitHub PAT scope** - Token needs access to GitHub Models (typically requires repo scope)

## File Locations for Common Tasks

- Add new endpoints: `src/GitHubModelsPlayground.ApiService/Program.cs` (after `app.MapGet`)
- Configure observability: `src/GitHubModelsPlayground.ServiceDefaults/Extensions.cs`
- Add services to orchestration: `src/GitHubModelsPlayground.AppHost/AppHost.cs`
- Project dependencies: All `.csproj` files use relative paths (`../ProjectName/Project.csproj`)

## Development Philosophy

This codebase prioritizes:
- **Educational clarity** over production optimization
- **Complete XML docs** for learning
- **Inline comments** explaining modern C# features
- **Simple examples** over comprehensive error handling

When adding features, maintain the teaching-focused approach with clear, well-documented examples.
