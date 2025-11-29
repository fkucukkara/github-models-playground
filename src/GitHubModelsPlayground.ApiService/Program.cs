using GitHubModelsPlayground.ApiService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

// Register the BlogService with a configured HttpClient
// The base address is a placeholder that will be resolved by Aspire's service discovery
builder.Services.AddHttpClient<BlogService>(client =>
{
    // The base address is set to a placeholder;
    // the actual URL will be provided by the AppHost at runtime.
    client.BaseAddress = new Uri("https://aspire-blog");
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Configure the Azure AI chat client for GitHub Models integration
// This connects to GitHub's hosted AI models (GPT-4o-mini)
builder.AddAzureChatCompletionsClient("ai-model")
    .AddChatClient();

// Register the BlogSummarizer service for dependency injection
builder.Services.AddScoped<BlogSummarizer>();

var app = builder.Build();

// Add global exception handling middleware
app.UseExceptionHandler();

// Enable OpenAPI/Swagger in development environment for testing
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Define the /summarize endpoint using minimal API syntax
// This endpoint fetches blog content and generates an AI-powered summary
app.MapGet("/summarize", async (string slug, BlogService blogService, BlogSummarizer blogSummarizer, ILogger<Program> logger) =>
{
    try
    {
        // Step 1: Fetch the blog content using the provided slug
        var blogContent = await blogService.GetContentAsync(slug);

        // Step 2: Generate an AI summary of the content
        var summary = await blogSummarizer.SummarizeAsync(blogContent);

        return Results.Ok(summary);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error summarizing blog content for slug '{Slug}'", slug);
        return Results.Problem("Failed to summarize the blog content.");
    }
}).WithName("SummarizeBlogContent");

// Map default Aspire endpoints (health checks, metrics)
app.MapDefaultEndpoints();

app.Run();
