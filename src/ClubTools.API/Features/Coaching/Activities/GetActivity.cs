using Carter;
using ClubTools.Api.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClubTools.Api.Features.Coaching.Activities;

public static class GetActivity
{
    public class Query : IRequest<Result<ActivityResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ActivityResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ActivityResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activityResponse = await _dbContext.Activites
                .AsNoTracking()
                .Where(activity => activity.Id == request.Id)
                .Select(activity => new ActivityResponse
                {
                    Id = activity.Id,
                    Title = activity.Title,
                    Detail = activity.Detail,
                    StepVariations = activity.StepVariations,
                    Equipment = activity.Equipment,
                    ImageUrl = activity.ImageUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (activityResponse is null)
            {
                return Result.Failure<ActivityResponse>(new Error(
                    "GetActivity.Null",
                    "The activity with the specified ID was not found"));
            }

            return activityResponse;
        }
    }
}

public class GetActivityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("activities/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetActivity.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
