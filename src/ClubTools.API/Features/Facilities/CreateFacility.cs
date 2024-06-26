using Carter;
using ClubTools.Api.Database;
using ClubTools.Api.Entities;
using ClubTools.Api.Shared;
using ClubTools.Shared.Contracts;
using FluentValidation;
using Mapster;
using MediatR;

namespace ClubTools.Api.Features.Facilities;

public static class CreateFacility
{
    public class Command : IRequest<Result<Guid>>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int MaxOccupancy { get; set; }

        public List<string> Amenities { get; set; } = new();
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Description).NotEmpty();
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
                    "CreateFacility.Validation",
                    validationResult.ToString()));
            }

            var facility = new Facility
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                MaxOccupancy = request.MaxOccupancy,
                Amenities = request.Amenities,
            };

            _dbContext.Add(facility);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return facility.Id;
        }
    }
}

public class CreateFacilityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("facilities", async (CreateFacilityRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateFacility.Command>();

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
