using Carter;
using ClubTools.Api.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Entities;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.ClubEvents;

public static class CreateClubEvent
{

    public class Command : IRequest<Result<Guid>>
    {
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
                    "CreateEvent.Validation",
                    validationResult.ToString()));
            }

            var clubEvent = new ClubEvent
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Venue = request.Venue,
                EventDateTime = request.EventDateTime,
                ImageUrl = request.ImageUrl,
                EntryPrice = request.EntryPrice,
                Capacity = request.Capacity,
                IsPlanned = request.IsPlanned,
                CreatedOnUtc = DateTime.UtcNow,
            };

            _dbContext.Add(clubEvent);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return clubEvent.Id;
        }
    }
}

public class CreateClubEventEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/clubevents", async (CreateClubEventRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateClubEvent.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
