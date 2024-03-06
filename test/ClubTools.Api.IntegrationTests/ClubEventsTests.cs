using ClubTools.Api.Features.ClubEvents;

namespace ClubTools.Api.IntegrationTests;

public class ClubEventsTests : BaseIntegrationTest
{
    public ClubEventsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ShouldCreateClubEvent()
    {
        // Arrange
        var command = new CreateClubEvent.Command
        {
            Title = "Test",
            Description = "Test event",
            Venue = "Test",
            Capacity = 100,
            EntryPrice = 100,
            EventDateTime = DateTime.Now,
            ImageUrl = "Test",
            IsPlanned = true,
        };

        // Act
        var eventId = await Sender.Send(command);

        // Assert
        var clubEvent = DbContext.ClubEvents.FirstOrDefault(c => c.Id == eventId.Value);

        Assert.NotNull(clubEvent);
    }

    [Fact]
    public async Task Get_ShouldReturnClubEvent_WhenClubEventExists()
    {
        // Arrange
        var eventId = await CreateClubEvent();
        var query = new GetClubEvent.Query { Id = eventId };

        // Act
        var clubEventResponse = await Sender.Send(query);

        // Assert
        Assert.NotNull(clubEventResponse);
    }

    //[Fact]
    //public async Task Get_ShouldReturnError_WhenClubEventIsNull()
    //{
    //    // Arrange
    //    var query = new GetClubEvent.Query { Id = Guid.NewGuid() };

    //    // Act
    //    var clubEventResponse = Sender.Send(query);

    //    // Assert
    //    Assert.True(clubEventResponse.Result.IsFailure);
    //}

    private async Task<Guid> CreateClubEvent()
    {
        var command = new CreateClubEvent.Command
        {
            Title = "Test",
            Description = "Test event",
            Venue = "Test",
            Capacity = 100,
            EntryPrice = 100,
            EventDateTime = DateTime.Now,
            ImageUrl = "Test",
            IsPlanned = true,
        };

        var eventId = await Sender.Send(command);

        return eventId.Value;
    }
}
