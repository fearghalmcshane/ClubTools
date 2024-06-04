namespace ClubTools.Shared.Contracts;

public class CreateClubEventRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Venue { get; set; } = string.Empty;

    public DateTime EventDateTime { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public double EntryPrice { get; set; }

    public int Capacity { get; set; }

    public bool IsPlanned { get; set; }
}
