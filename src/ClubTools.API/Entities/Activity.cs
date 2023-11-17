namespace ClubTools.Api.Entities;

public class Activity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public List<string> StepVariations { get; set; } = new();

    public List<string> Equipment { get; set; } = new();

    public string ImageUrl { get; set; } = string.Empty;
}
