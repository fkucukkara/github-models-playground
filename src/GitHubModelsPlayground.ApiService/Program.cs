using GitHubModelsPlayground.ApiService;
using Microsoft.Extensions.AI;

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
app.UseStatusCodePages();
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
}).WithName("SummarizeContent")
  .WithSummary("Summarize blog content from aspire.dev")
  .WithDescription("Fetches a blog post from aspire.dev using the provided slug, extracts the first paragraph, " +
  "and generates a two-sentence AI summary using GitHub Models (GPT-4o-mini).");

app.MapPost("/chat", async (ChatRequest request, IChatClient chatClient, ILogger<Program> logger) =>
{
    try
    {
        // Send the user's message directly to the AI chat client
        var chatResponse = await chatClient.GetResponseAsync(request.Message);

        var responseText = chatResponse.Messages.FirstOrDefault()?.Text ?? "No response generated.";

        return Results.Ok(new { response = responseText });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing chat message");
        return Results.Problem("Failed to process the chat message.");
    }
}).WithName("Chat")
  .WithSummary("Send a message to the AI chat client")
  .WithDescription("Sends a message to the GitHub Models AI (GPT-4o-mini) and returns the response.");

app.MapDefaultEndpoints();

app.Run();

/// <summary>
/// Request model for the chat endpoint.
/// </summary>
/// <param name="Message">The message to send to the AI chat client.</param>
public record ChatRequest(string Message);
