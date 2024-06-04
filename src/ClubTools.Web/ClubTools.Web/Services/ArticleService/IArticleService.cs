using ClubTools.Shared.Contracts;

namespace ClubTools.Web.Services.ArticleService;

public interface IArticleService
{
    Task<ICollection<ArticleResponse>> GetArticlesAsync();
}
