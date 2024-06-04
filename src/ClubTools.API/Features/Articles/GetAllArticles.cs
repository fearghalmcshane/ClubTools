using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClubTools.Api.Features.Articles;

public static class GetAllArticles
{
    public class Query : IRequest<Result<ICollection<ArticleResponse>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<ArticleResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ICollection<ArticleResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var articleResponse = await _dbContext.Articles
                .AsNoTracking()
                .Select(article => new ArticleResponse
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    Tags = article.Tags,
                    CreatedOnUtc = article.CreatedOnUtc,
                    PublishedOnUtc = article.PublishedOnUtc
                })
                .ToListAsync(cancellationToken);

            if (articleResponse.IsNullOrEmpty())
            {
                return Result.Failure<ICollection<ArticleResponse>>(new Error(
                    "GetAllArticles.Null",
                    "There are no articles available"));
            }

            return articleResponse;
        }
    }
}

public class GetAllArticlesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("articles", async (ISender sender) =>
        {
            var query = new GetAllArticles.Query();

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
