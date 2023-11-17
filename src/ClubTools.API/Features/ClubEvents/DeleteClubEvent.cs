using Carter;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using MediatR;

namespace ClubTools.Api.Features.ClubEvents;

public static class DeleteClubEvent
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
                    "DeleteClubEvent.Validation",
                    validationResult.ToString()));
            }

            var clubEvent = _dbContext.ClubEvents
                .Where(cEvent => cEvent.Id == request.Id)
                .First();

            if (clubEvent is null)
            {
                return Result.Failure<string>(new Error(
                    "DeleteclubEvent.Null",
                    "The club event with the specified ID was not found"));
            }

            _dbContext.Remove(clubEvent);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return "Delete Successful";
        }
    }
}

public class DeleteClubEventEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/clubevents/{id}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteClubEvent.Command() { Id = id };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}