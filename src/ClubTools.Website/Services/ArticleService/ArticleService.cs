using ClubTools.Shared.Contracts;
using System.Net.Http.Json;

namespace ClubTools.Website.Services.ArticleService;

public sealed class ArticleService : IArticleService
{
    private readonly HttpClient _httpClient;

    public ArticleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ICollection<ArticleResponse>> GetArticlesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<ICollection<ArticleResponse>>($"api/v1/articles");

        return result ?? throw new InvalidOperationException("Failed to retrieve articles from the API.");
    }
}
