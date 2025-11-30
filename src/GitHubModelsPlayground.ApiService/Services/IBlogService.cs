namespace GitHubModelsPlayground.ApiService.Services;

/// <summary>
/// Defines the contract for fetching and parsing blog content from external sources.
/// </summary>
public interface IBlogService
{
    /// <summary>
    /// Fetches blog content from the specified slug and extracts the first paragraph.
    /// </summary>
    /// <param name="slug">The blog post slug or path to fetch.</param>
    /// <returns>The text content of the first paragraph from the blog post.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    Task<string> GetContentAsync(string slug);
}
