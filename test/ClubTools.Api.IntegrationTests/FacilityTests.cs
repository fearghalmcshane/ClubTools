using ClubTools.Api.Features.Facilities;

namespace ClubTools.Api.IntegrationTests;

public class FacilityTests : BaseIntegrationTest
{
    public FacilityTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ShouldCreateFacility()
    {
        // Arrange
        var command = new CreateFacility.Command
        {
            Name = "Test",
            Description = "Test facility",
            MaxOccupancy = 10,
            Amenities = new List<string> { "Test" },
        };

        // Act
        var facilityId = await Sender.Send(command);

        // Assert
        var facility = DbContext.Facilities.FirstOrDefault(c => c.Id == facilityId.Value);

        Assert.NotNull(facility);
    }

    [Fact]
    public async Task Get_ShouldReturnFacility_WhenFacilityExists()
    {
        // Arrange
        var facilityId = await CreateFacility();
        var query = new GetFacility.Query { Id = facilityId };

        // Act
        var facilityResponse = await Sender.Send(query);

        // Assert
        Assert.NotNull(facilityResponse);
    }

    [Fact]
    public async Task Get_ShouldReturnError_WhenFacilityIsNull()
    {
        // Arrange
        var query = new GetFacility.Query { Id = Guid.NewGuid() };

        // Act
        var facilityResponse = await Sender.Send(query);

        // Assert
        Assert.True(facilityResponse.IsFailure);
    }

    [Fact]
    public async Task Update_ShouldUpdateFacility()
    {
        // Arrange
        var facilityId = await CreateFacility();
        var command = new UpdateFacility.Command
        {
            Id = facilityId,
            Name = "Test",
            Description = "Test facility",
            MaxOccupancy = 10,
            Amenities = new List<string> { "Test" },
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_ShouldDeleteFacility()
    {
        // Arrange
        var facilityId = await CreateFacility();
        var command = new DeleteFacility.Command { Id = facilityId };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    private async Task<Guid> CreateFacility()
    {
        var command = new CreateFacility.Command
        {
            Name = "Test",
            Description = "Test facility",
            MaxOccupancy = 10,
            Amenities = new List<string> { "Test" },
        };

        var id = await Sender.Send(command);

        return id.Value;
    }
}
