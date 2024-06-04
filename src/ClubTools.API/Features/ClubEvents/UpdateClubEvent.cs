using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.ClubEvents;

public static class UpdateClubEvent
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Venue { get; set; } = string.Empty;

        public DateTime EventDateTime { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public double EntryPrice { get; set; }

        public int Capacity { get; set; }

        public bool IsPlanned { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Description).NotEmpty();
            RuleFor(c => c.Venue).NotEmpty();
            RuleFor(c => c.EventDateTime).NotEmpty();
            RuleFor(c => c.ImageUrl).NotEmpty();
            RuleFor(c => c.EntryPrice).NotEmpty();
            RuleFor(c => c.Capacity).NotEmpty();
            RuleFor(c => c.IsPlanned).NotEmpty();
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
                    "UpdateClubEvent.Validation",
                    validationResult.ToString()));
            }

            var clubEvent = _dbContext.ClubEvents
                .Where(cEvent => cEvent.Id == request.Id)
                .First();

            if (clubEvent == null)
            {
                return Result.Failure<Guid>(new Error(
                    "UpdateClubEvent.Null",
                    "The club event with the specified ID was not found"));
            }

            clubEvent.Title = request.Title;
            clubEvent.Description = request.Description;
            clubEvent.Venue = request.Venue;
            clubEvent.EventDateTime = request.EventDateTime;
            clubEvent.ImageUrl = request.ImageUrl;
            clubEvent.EntryPrice = request.EntryPrice;
            clubEvent.Capacity = request.Capacity;
            clubEvent.IsPlanned = request.IsPlanned;

            _dbContext.Update(clubEvent);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return clubEvent.Id;
        }
    }
}

public class UpdateClubEventEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("clubevents", async (UpdateClubEventRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateClubEvent.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
