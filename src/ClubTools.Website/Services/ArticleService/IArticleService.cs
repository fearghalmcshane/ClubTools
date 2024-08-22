using ClubTools.Shared.Contracts;

namespace ClubTools.Website.Services.ArticleService;

public interface IArticleService
{
    Task<ICollection<ArticleResponse>> GetArticlesAsync();
}
