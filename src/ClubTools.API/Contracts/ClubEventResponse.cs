namespace ClubTools.Api.Contracts;

public class ClubEventResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Venue { get; set; } = string.Empty;

    public DateTime EventDateTime { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public double EntryPrice { get; set; }

    public int Capacity { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public bool IsPlanned { get; set; }
}
