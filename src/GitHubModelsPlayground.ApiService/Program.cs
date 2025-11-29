using GitHubModelsPlayground.ApiService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

// Register the BlogService with a configured HttpClient
// The base address is a placeholder that will be resolved by Aspire's service discovery
builder.Services.AddHttpClient<IBlogService, BlogService>(client =>
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
builder.Services.AddScoped<IBlogSummarizer, BlogSummarizer>();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/summarize", async (string slug, IBlogService blogService, IBlogSummarizer blogSummarizer, ILogger<Program> logger) =>
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
}).WithName("SummarizeContent");

app.MapDefaultEndpoints();

app.Run();
