using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClubTools.Api.Features.Facilities;

public static class GetAllFacilities
{
    public class Query : IRequest<Result<ICollection<FacilityResponse>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<ICollection<FacilityResponse>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<ICollection<FacilityResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var facilityResponse = await _dbContext.Facilities
                .AsNoTracking()
                .Select(facility => new FacilityResponse
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    Description = facility.Description,
                    MaxOccupancy = facility.MaxOccupancy,
                    Amenities = facility.Amenities
                })
                .ToListAsync(cancellationToken);

            if (facilityResponse.IsNullOrEmpty())
            {
                return Result.Failure<ICollection<FacilityResponse>>(new Error(
                    "GetAllFacilities.Null",
                    "There are no facilities available"));
            }

            return facilityResponse;
        }
    }
}

public class GetAllFacilitiesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("facilities", async (ISender sender) =>
        {
            var query = new GetAllFacilities.Query();

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error.Message);
            }

            return Results.Ok(result.Value);
        });
    }
}
