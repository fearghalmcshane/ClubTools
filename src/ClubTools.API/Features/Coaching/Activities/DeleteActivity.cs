using Carter;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using MediatR;

namespace ClubTools.Api.Features.Coaching.Activities;

public static class DeleteActivity
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

            var activity = _dbContext.Activites
                .Where(activity => activity.Id == request.Id)
                .First();

            if (activity is null)
            {
                return Result.Failure<string>(new Error(
                    "DeleteActivity.Null",
                    "The activity with the specified ID was not found"));
            }

            _dbContext.Remove(activity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return "Delete Successful";
        }
    }
}

public class DeleteActivityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/activities/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteActivity.Command { Id = id };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
