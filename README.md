# GitHub Models Playground

An educational .NET Aspire application demonstrating how to integrate GitHub Models (AI) with a blog summarization service and interactive chat capabilities. This project showcases modern cloud-native development practices using .NET 10, Aspire orchestration, and AI-powered content processing.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.0-blueviolet)](https://learn.microsoft.com/dotnet/aspire/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Overview

This project demonstrates:
- **GitHub Models Integration**: Using GitHub's AI models (GPT-4o-mini) through the Aspire framework
- **Blog Content Summarization**: Fetching and summarizing blog content using AI
- **Interactive AI Chat**: Direct chat interface with GitHub's AI models
- **Distributed Application Architecture**: Leveraging .NET Aspire for orchestration
- **Service Discovery**: Automatic service discovery between components
- **OpenTelemetry Integration**: Built-in observability with metrics, logs, and traces
- **Modern .NET Practices**: Minimal APIs, dependency injection, interface-based design, and configuration management

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│              GitHubModelsPlayground.AppHost             │
│                  (Orchestrator)                         │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────┐  │
│  │   External  │  │ GitHub Model │  │  API Service  │  │
│  │   Service   │  │  (GPT-4o)    │  │               │  │
│  └─────────────┘  └──────────────┘  └───────────────┘  │
└─────────────────────────────────────────────────────────┘
                            ↓
                 ┌──────────────────────┐
                 │    API Service       │
                 │  ┌────────────────┐  │
                 │  │ IBlogService   │  │
                 │  │ IBlogSummarizer│  │
                 │  │ IChatClient    │  │
                 │  └────────────────┘  │
                 └──────────────────────┘
                            ↓
                 ┌──────────────────────┐
                 │   External Blog      │
                 │   (aspire.dev)       │
                 └──────────────────────┘
```

### Components

1. **GitHubModelsPlayground.ApiService**
   - ASP.NET Core minimal API
   - Exposes `/summarize` endpoint for blog content summarization
   - Exposes `/chat` endpoint for interactive AI conversations
   - Interface-based design for improved testability

2. **GitHubModelsPlayground.AppHost**
   - .NET Aspire orchestrator
   - Manages service references and dependencies
   - Configures GitHub Models connection

3. **GitHubModelsPlayground.ServiceDefaults**
   - Shared service configurations
   - OpenTelemetry setup
   - Health checks and resilience patterns

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [GitHub Personal Access Token](https://github.com/settings/tokens) with access to GitHub Models
- [Azure Subscription](https://azure.microsoft.com/free/) (for deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/fkucukkara/github-models-playground.git
   cd github-models-playground
   ```

2. **Set up GitHub Models access**
   
   Configure your GitHub Personal Access Token (PAT):

   ```bash
   cd src/GitHubModelsPlayground.AppHost
   dotnet user-secrets set "GitHub:Token" "your-github-pat-token-here"
   ```

   **How to get a GitHub PAT:**
   - Go to [GitHub Settings > Developer settings > Personal access tokens](https://github.com/settings/tokens)
   - Generate a new token with access to GitHub Models

3. **Run locally**
   ```bash
   cd src/GitHubModelsPlayground.AppHost
   dotnet run
   ```

   The Aspire Dashboard will open automatically in your browser.

### Deploy to Azure

This project uses Azure Developer CLI (azd) for seamless deployment to Azure Container Apps.

1. **Initialize azd (first time only)**
   ```bash
   azd init
   ```

2. **Provision and deploy**
   ```bash
   azd up
   ```

   This single command will:
   - Provision Azure resources (Container Apps, Container Registry, etc.)
   - Build and containerize the application
   - Deploy to Azure Container Apps
   - Configure environment variables

3. **Set GitHub token for Azure deployment**
   ```bash
   azd env set GITHUB_TOKEN "your-github-pat-token-here"
   azd deploy
   ```

### Deployment Architecture

The application deploys to Azure Container Apps with:
- **Automatic scaling** based on load
- **Managed identity** for secure access
- **Built-in observability** with Application Insights
- **HTTPS endpoint** automatically provisioned

### Using the API

Once running, you can test the API endpoints:

#### Summarize Blog Content
```bash
# Example: Summarize a blog post from aspire.dev
curl "http://localhost:5000/summarize?slug=whats-new/aspire-13/"
```

**Response:**
```json
"This is a two-sentence summary of the blog content generated by AI..."
```

#### Chat with AI
```bash
# Example: Send a message to the AI chat endpoint
curl -X POST "http://localhost:5000/chat" \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello, what can you tell me about .NET Aspire?"}'
```

**Response:**
```json
{
  "response": ".NET Aspire is a cloud-native application framework that simplifies building distributed applications..."
}
```

#### Using the .http file
The project includes a `GitHubModelsPlayground.ApiService.http` file for easy testing in Visual Studio:
- Open the file in Visual Studio
- Click "Send Request" above any endpoint
- View responses directly in the editor

### Exploring the Aspire Dashboard

After starting the application:
1. Navigate to the Aspire Dashboard (typically `http://localhost:15888`)
2. View:
   - **Resources**: All running services and their status
   - **Logs**: Centralized logging from all services
   - **Traces**: Distributed tracing information
   - **Metrics**: Performance metrics and telemetry

## 📚 Project Structure

```
GitHubModelsPlayground/
├── src/
│   ├── GitHubModelsPlayground.ApiService/
│   │   ├── Services/
│   │   │   ├── BlogService.cs              # HTTP client for fetching blog content
│   │   │   ├── IBlogService.cs             # Interface for blog service
│   │   │   ├── BlogSummarizer.cs           # AI-powered summarization service
│   │   │   └── IBlogSummarizer.cs          # Interface for summarization service
│   │   ├── Program.cs                      # API configuration and endpoints
│   │   ├── appsettings.json                # Application configuration
│   │   ├── GitHubModelsPlayground.ApiService.http  # HTTP request examples
│   │   └── GitHubModelsPlayground.ApiService.csproj
│   ├── GitHubModelsPlayground.AppHost/
│   │   ├── AppHost.cs                      # Aspire orchestration setup
│   │   ├── appsettings.json                # Host configuration
│   │   └── GitHubModelsPlayground.AppHost.csproj
│   └── GitHubModelsPlayground.ServiceDefaults/
│       ├── Extensions.cs                   # Shared service extensions
│       └── GitHubModelsPlayground.ServiceDefaults.csproj
├── .gitignore
├── LICENSE
├── README.md
├── CONTRIBUTING.md
└── GitHubModelsPlayground.slnx             # Solution file
```

## 🔑 Key Concepts

### 1. GitHub Models Integration
The application uses GitHub's hosted AI models through the Aspire framework:

```csharp
var aiModel = builder.AddGitHubModel("ai-model", GitHubModel.OpenAI.OpenAIGpt4oMini);
```

### 2. Service Discovery
Services are referenced and discovered automatically:

```csharp
var apiService = builder.AddProject<Projects.GitHubModelsPlayground_ApiService>("apiservice")
    .WithReference(blogService)
    .WithReference(aiModel);
```

### 3. AI-Powered Summarization
The `BlogSummarizer` uses the AI chat client to generate summaries:

```csharp
var response = await chatClient.GetResponseAsync(prompt);
```

### 4. Interface-Based Design
The application follows SOLID principles with interface-based dependency injection:

```csharp
// Service registration
builder.Services.AddHttpClient<IBlogService, BlogService>();
builder.Services.AddScoped<IBlogSummarizer, BlogSummarizer>();

// Endpoint usage
app.MapGet("/summarize", async (IBlogService blogService, IBlogSummarizer blogSummarizer) =>
{
    // Implementation...
});
```

This approach provides:
- **Improved testability** - Easy to mock dependencies for unit testing
- **Loose coupling** - Depend on abstractions, not concrete implementations
- **Flexibility** - Swap implementations without changing consumers
- **Clear contracts** - Interfaces document expected behavior

### 5. Resilience and Observability
Built-in patterns through ServiceDefaults:
- Automatic retries with exponential backoff
- Circuit breaker patterns
- Distributed tracing
- Health checks

## 🛠️ Configuration

### Application Settings

**appsettings.json** (ApiService)
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

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `GITHUB_TOKEN` | GitHub Personal Access Token | Yes |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry endpoint | No |

## 🧪 Testing

### Using the .http File
The project includes `GitHubModelsPlayground.ApiService.http` for easy endpoint testing:

1. Open `src/GitHubModelsPlayground.ApiService/GitHubModelsPlayground.ApiService.http` in Visual Studio
2. Click "Send Request" above any endpoint
3. View the response in the editor

### Manual Testing
1. Start the application
2. Access the OpenAPI documentation at `http://localhost:5000/openapi` (in development mode)
3. Test both endpoints with different inputs

### Example Requests

**Summarize Blog Posts:**
```bash
# Test with different blog posts
curl "http://localhost:5000/summarize?slug=whats-new/aspire-13/"
curl "http://localhost:5000/summarize?slug=blog/aspire-ga"
```

**Chat with AI:**
```bash
# Simple greeting
curl -X POST "http://localhost:5000/chat" \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello, how are you?"}'

# Technical question
curl -X POST "http://localhost:5000/chat" \
  -H "Content-Type: application/json" \
  -d '{"message": "Explain the benefits of using .NET Aspire for cloud-native applications"}'

# Code-related question
curl -X POST "http://localhost:5000/chat" \
  -H "Content-Type: application/json" \
  -d '{"message": "What are the key differences between minimal APIs and controllers in ASP.NET Core?"}'
```

### API Endpoints

| Endpoint | Method | Description | Parameters |
|----------|--------|-------------|------------|
| `/summarize` | GET | Summarize blog content from aspire.dev | `slug` (query string) |
| `/chat` | POST | Interactive chat with GitHub Models AI | `message` (JSON body) |
| `/health` | GET | Health check endpoint | None |
