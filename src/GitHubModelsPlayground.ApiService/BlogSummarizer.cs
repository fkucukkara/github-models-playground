using Microsoft.Extensions.AI;

namespace GitHubModelsPlayground.ApiService;

/// <summary>
/// Service for summarizing blog content using AI-powered chat completion.
/// </summary>
/// <param name="chatClient">The AI chat client used to generate summaries.</param>
public class BlogSummarizer(IChatClient chatClient)
{
    /// <summary>
    /// Generates a concise two-sentence summary of the provided blog content using AI.
    /// </summary>
    /// <param name="content">The blog content to summarize.</param>
    /// <returns>A two-sentence summary of the content, or a default message if generation fails.</returns>
    /// <remarks>
    /// This method uses an AI chat client (typically GPT-4o-mini via GitHub Models) to generate
    /// a concise summary. The AI is prompted to act as an expert blog summarizer.
    /// </remarks>
    public async Task<string> SummarizeAsync(string content)
    {
        var prompt = $"""
            You are an expert blog summarizer.
            Summarize the following blog content:{content} into two sentences.
            """;

        var response = await chatClient.GetResponseAsync(prompt);

        if (!response.Messages.Any())
        {
            return "No summary could be generated.";
        }

        return response.Messages.First().Text;
    }
}
