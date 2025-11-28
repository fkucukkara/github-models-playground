using Aspire.Hosting.GitHub;

// Create the distributed application builder for .NET Aspire orchestration
var builder = DistributedApplication.CreateBuilder(args);

// Register an external service reference to the Aspire blog
// This creates a named service that can be discovered by other services
var blogService = builder.AddExternalService("aspire-blog", "https://aspire.dev/");

// Configure GitHub Models integration for AI-powered features
// This connects to GitHub's hosted GPT-4o-mini model
var aiModel = builder.AddGitHubModel("ai-model", GitHubModel.OpenAI.OpenAIGpt4oMini);

// Register the API service project and configure its dependencies
// WithHttpHealthCheck() enables Aspire to monitor service health at the /health endpoint
// WithReference() creates service-to-service connections that are resolved at runtime:
//   - blogService: Makes the external blog URL available via service discovery
//   - aiModel: Injects GitHub PAT and AI model configuration from user secrets
var apiService = builder.AddProject<Projects.GitHubModelsPlayground_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")  // Enables Aspire to monitor service health at the /health endpoint
    .WithReference(blogService)   // Enables service discovery for the blog service
    .WithReference(aiModel);       // Injects AI model configuration and credentials

// Build and run the distributed application
// This starts the Aspire dashboard and all configured services
builder.Build().Run();
