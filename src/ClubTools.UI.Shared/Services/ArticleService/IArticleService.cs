using ClubTools.Shared.Contracts;

namespace ClubTools.UI.Shared.Services.ArticleService;

public interface IArticleService
{
    Task<ICollection<ArticleResponse>> GetArticlesAsync();
}
