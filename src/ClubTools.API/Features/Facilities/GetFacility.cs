using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClubTools.Api.Features.Facilities;

public static class GetFacility
{
    public class Query : IRequest<Result<FacilityResponse>>
    {
        public Guid Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<FacilityResponse>>
    {
        private readonly ApplicationDbContext _dbContext;

        public Handler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<FacilityResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var facilityResponse = await _dbContext.Facilities
                .AsNoTracking()
                .Where(facility => facility.Id == request.Id)
                .Select(facility => new FacilityResponse
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    Description = facility.Description,
                    MaxOccupancy = facility.MaxOccupancy,
                    Amenities = facility.Amenities.ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (facilityResponse is null)
            {
                return Result.Failure<FacilityResponse>(new Error(
                    "GetFacility.Null",
                    "The facility with the specified ID was not found"));
            }

            return facilityResponse;
        }
    }
}

public class GetFacilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("facilities/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetFacility.Query { Id = id };
            
            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
