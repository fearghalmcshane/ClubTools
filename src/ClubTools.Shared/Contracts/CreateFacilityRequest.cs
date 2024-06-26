namespace ClubTools.Shared.Contracts;

public class CreateFacilityRequest
{
    public String Name { get; set; } = string.Empty;

    public String Description { get; set; } = string.Empty;

    public int MaxOccupancy { get; set; }

    public List<String> Amenities { get; set; } = new();
}
