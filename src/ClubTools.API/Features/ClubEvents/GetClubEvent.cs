using Carter;
using ClubTools.Api.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClubTools.Api.Features.ClubEvents;

public static class GetClubEvent
{
    public class Query : IRequest<Result<ClubEventResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ClubEventResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ClubEventResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var clubEventResponse = await _dbContext.ClubEvents
                .AsNoTracking()
                .Where(clubEvent => clubEvent.Id == request.Id)
                .Select(clubEvent => new ClubEventResponse
                {
                    Id = clubEvent.Id,
                    Title = clubEvent.Title,
                    Description = clubEvent.Description,
                    Venue = clubEvent.Venue,
                    EventDateTime = clubEvent.EventDateTime,
                    ImageUrl = clubEvent.ImageUrl,
                    EntryPrice = clubEvent.EntryPrice,
                    Capacity = clubEvent.Capacity,
                    IsPlanned = clubEvent.IsPlanned,
                    CreatedOnUtc = clubEvent.CreatedOnUtc,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (clubEventResponse == null)
            {
                return Result.Failure<ClubEventResponse>(new Error(
                    "GetClubEvent.Null",
                    "The club event with the specified ID was not found"));
            }

            return clubEventResponse;
        }
    }
}

public class GetClubEventEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("clubevents/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetClubEvent.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
