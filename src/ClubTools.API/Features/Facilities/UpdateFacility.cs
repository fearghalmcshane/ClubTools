using Carter;
using ClubTools.Shared.Contracts;
using ClubTools.Api.Database;
using ClubTools.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.Facilities;

public static class UpdateFacility
{
    public class Command : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int MaxOccupancy { get; set; }

        public List<string> Amenities { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Description).NotEmpty();
            RuleFor(c => c.MaxOccupancy).GreaterThan(0);
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
                    "UpdateFacility.Validation",
                    validationResult.ToString()));
            }

            var facility = _dbContext.Facilities
                .Where(facility => facility.Id == request.Id)
                .First();

            if (facility is null)
            {
                return Result.Failure<Guid>(new Error(
                    "UpdateFacility.NotFound",
                    $"Facility with ID {request.Id} not found."));
            }

            facility.Name = request.Name;
            facility.Description = request.Description;
            facility.MaxOccupancy = request.MaxOccupancy;
            facility.Amenities = request.Amenities;

            _dbContext.Update(facility);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return facility.Id;
        }
    }
}

public class UpdateFacilityEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("facilities", async(UpdateFacilityRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateFacility.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
