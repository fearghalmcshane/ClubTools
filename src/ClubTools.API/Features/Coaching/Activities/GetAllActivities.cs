using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClubTools.Api.Features.Coaching.Activities;

public static class GetAllActivities
{
    public class Query : IRequest<Result<ICollection<ActivityResponse>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<ActivityResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ICollection<ActivityResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activityResponse = await _dbContext.Activites
                .AsNoTracking()
                .Select(activity => new ActivityResponse
                {
                    Id = activity.Id,
                    Title = activity.Title,
                    Detail = activity.Detail,
                    StepVariations = activity.StepVariations,
                    Equipment = activity.Equipment,
                    ImageUrl = activity.ImageUrl
                })
                .ToListAsync(cancellationToken);

            if (activityResponse.IsNullOrEmpty())
            {
                return Result.Failure<ICollection<ActivityResponse>>(new Error(
                    "GetAllActivities.Null",
                    "There are no activities found"));
            }

            return activityResponse;
        }
    }
}

public class GetAllActivitiesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("activities", async (ISender sender) =>
        {
            var query = new GetAllActivities.Query();

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
