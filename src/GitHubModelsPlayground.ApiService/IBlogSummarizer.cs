namespace GitHubModelsPlayground.ApiService;

/// <summary>
/// Defines the contract for summarizing blog content using AI-powered chat completion.
/// </summary>
public interface IBlogSummarizer
{
    /// <summary>
    /// Generates a concise two-sentence summary of the provided blog content using AI.
    /// </summary>
    /// <param name="content">The blog content to summarize.</param>
    /// <returns>A two-sentence summary of the content, or a default message if generation fails.</returns>
    Task<string> SummarizeAsync(string content);
}
