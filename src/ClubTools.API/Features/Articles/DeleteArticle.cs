using Carter;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using MediatR;

namespace ClubTools.Api.Features.Articles;

public static class DeleteArticle
{
    public class Command : IRequest<Result<string>>
    {
        public Guid Id { get; set; } = Guid.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<string>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IValidator<Command> _validator;

        public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return Result.Failure<string>(new Error(
                    "DeleteArticle.Validation",
                    validationResult.ToString()));
            }

            var article = _dbContext.Articles
                .Where(article => article.Id == request.Id)
            .First();

            if (article is null)
            {
                return Result.Failure<string>(new Error(
                    "DeleteArticle.Null",
                    "The article with the specified ID was not found"));
            }

            _dbContext.Remove(article);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return "Delete Successful";
        }
    }
}

public class DeleteArticleEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/articles/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteArticle.Command { Id = id };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
