namespace ClubTools.Shared.Contracts;

public class FacilityResponse
{
    public Guid Id { get; set; }

    public String Name { get; set; } = string.Empty;

    public String Description { get; set; } = string.Empty;

    public int MaxOccupancy { get; set; }

    public List<String> Amenities { get; set; } = new();
}
