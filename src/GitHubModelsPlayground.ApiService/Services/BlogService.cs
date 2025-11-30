namespace GitHubModelsPlayground.ApiService.Services;

using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Service for fetching and parsing blog content from external sources.
/// </summary>
/// <param name="httpClient">The HTTP client used to fetch blog content.</param>
public class BlogService(HttpClient httpClient) : IBlogService
{
    /// <summary>
    /// Fetches blog content from the specified slug and extracts the first paragraph.
    /// </summary>
    /// <param name="slug">The blog post slug or path to fetch.</param>
    /// <returns>The text content of the first paragraph from the blog post.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<string> GetContentAsync(string slug)
    {
        var response = await httpClient.GetAsync(slug);
        response.EnsureSuccessStatusCode();

        return ExtractFirstParagraph(await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Extracts the first paragraph from HTML content, optionally from within a markdown content div.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <returns>The plain text content of the first paragraph, with HTML tags removed. Returns an empty string if no paragraph is found.</returns>
    /// <remarks>
    /// This method first attempts to find content within a div with class "sl-markdown-content".
    /// If found, it searches for the first paragraph within that div; otherwise, it searches the entire HTML.
    /// All HTML tags are stripped from the result.
    /// </remarks>
    public string ExtractFirstParagraph(string htmlContent)
    {
        // First, try to extract content from div with class "sl-markdown-content"
        var divMatch = Regex.Match(htmlContent, @"<div[^>]*class=[""']?sl-markdown-content[""']?[^>]*>(.*?)</div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var contentToSearch = divMatch.Success ? divMatch.Groups[1].Value : htmlContent;

        // Then extract the first paragraph from the content
        var match = Regex.Match(contentToSearch, @"<p[^>]*>(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (match.Success)
        {
            var res = Regex.Replace(match.Groups[1].Value, @"<[^>]+>", string.Empty).Trim();
            return res;
        }
        return string.Empty;
    }
}
