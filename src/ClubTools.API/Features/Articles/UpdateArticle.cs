using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.Articles;

public static class UpdateArticle
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Content).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Result.Failure<Guid>(new Error(
                    "UpdateArticle.Validation",
                    validationResult.ToString()));
            }

            var article = _dbContext.Articles
                .Where(article => article.Id == request.Id)
                .First();

            if (article is null)
            {
                return Result.Failure<Guid>(new Error(
                    "UpdateArticle.Null",
                    "The article with the specified ID was not found"));
            }

            article.Title = request.Title;
            article.Content = request.Content;
            article.Tags = request.Tags;

            _dbContext.Update(article);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return article.Id;
        }
    }
}

public class UpdateArticleEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("articles", async (UpdateArticleRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateArticle.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
