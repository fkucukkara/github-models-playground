using Aspire.Hosting.GitHub;

// Create the distributed application builder for .NET Aspire orchestration
var builder = DistributedApplication.CreateBuilder(args);

// Register an external service reference to the Aspire blog
// This creates a named service that can be discovered by other services
var blogService = builder.AddExternalService("aspire-blog", "https://aspire.dev/");

// Configure GitHub Models integration for AI-powered features
// This connects to GitHub's hosted GPT-4o-mini model
var aiModel = builder.AddGitHubModel("ai-model", GitHubModel.OpenAI.OpenAIGpt4oMini);

// Register the API service project and wire up its dependencies
// The service will automatically discover the blog service and AI model at runtime
var apiService = builder.AddProject<Projects.GitHubModelsPlayground_ApiService>("apiservice")
    .WithReference(blogService)   // Enables service discovery for the blog service
    .WithReference(aiModel);       // Injects AI model configuration and credentials

// Build and run the distributed application
// This starts the Aspire dashboard and all configured services
builder.Build().Run();
