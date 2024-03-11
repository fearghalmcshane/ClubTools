using Carter;
using ClubTools.Api.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Features.Articles;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.Coaching.Activities;

public static class UpdateActivity
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }

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
                    "UpdateActivity.Validation",
                    validationResult.ToString()));
            }

            var activity = _dbContext.Activites
                .Where(activity => activity.Id == request.Id)
                .First();

            if (activity is null)
            {
                return Result.Failure<Guid>(new Error(
                    "UpdateActivity.Null",
                    "The article with the specified ID was not found"));
            }

            activity.Title = request.Title;
            activity.Detail = request.Detail;
            activity.StepVariations = request.StepVariations;
            activity.Equipment = request.Equipment;
            activity.ImageUrl = request.ImageUrl;

            _dbContext.Update(activity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return activity.Id;
        }
    }
}

public class UpdateActivityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("activities", async (UpdateActivityRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateActivity.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
