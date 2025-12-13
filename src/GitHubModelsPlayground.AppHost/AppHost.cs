using Aspire.Hosting.GitHub;

var builder = DistributedApplication.CreateBuilder(args);

// Register an external service reference to the Aspire blog
// This creates a named service that can be discovered by other services
var blogService = builder.AddExternalService("aspire-blog", "https://aspire.dev/");

// Configure GitHub Models integration for AI-powered features
// This connects to GitHub's hosted GPT-4o-mini model
var aiModel = builder.AddGitHubModel("ai-model", GitHubModel.OpenAI.OpenAIGpt4oMini);

var apiService = builder.AddProject<Projects.GitHubModelsPlayground_ApiService>("apiservice", "https")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(blogService)
    .WithReference(aiModel);

builder.Build().Run();
