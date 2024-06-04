using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Entities;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.Coaching.Activities;

public static class CreateActivity
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;

        public string Detail { get; set; } = string.Empty;

        public List<string> StepVariations { get; set; } = new();

        public List<string> Equipment { get; set; } = new();

        public string ImageUrl { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(a => a.Title).NotEmpty();
            RuleFor(a => a.Detail).NotEmpty();
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
                    "CreateActivity.Validation",
                    validationResult.ToString()));
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Detail = request.Detail,
                StepVariations = request.StepVariations,
                Equipment = request.Equipment,
            };

            _dbContext.Add(activity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return activity.Id;
        }
    }
}

public class CreateActivityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("activities", async (CreateActivityRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateActivity.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
