using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClubTools.Api.Features.ClubEvents;

public static class GetAllClubEvents
{
    public class Query : IRequest<Result<ICollection<ClubEventResponse>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<ClubEventResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ICollection<ClubEventResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var clubEventResponse = await _dbContext.ClubEvents
                .AsNoTracking()
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
                .ToListAsync(cancellationToken);

            if (clubEventResponse.IsNullOrEmpty())
            {
                return Result.Failure<ICollection<ClubEventResponse>>(new Error(
                    "GetAllClubEvents.Null",
                    "There are no club events found"));
            }

            return clubEventResponse;
        }
    }
}

public class GetAllClubEventsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("clubevents", async (ISender sender) =>
        {
            var query = new GetAllClubEvents.Query();

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
