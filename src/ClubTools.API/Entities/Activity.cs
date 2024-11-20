namespace ClubTools.Api.Entities;

public class Activity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public ICollection<string>? StepVariations { get; set; }

    public ICollection<string>? Equipment { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}
