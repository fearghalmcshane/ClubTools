namespace ClubTools.Api.Entities;

public class Facility
{
    public Guid Id { get; set; }

    public String Name { get; set; } = string.Empty;

    public String Description { get; set; } = string.Empty;

    public int MaxOccupancy { get; set; }

    public ICollection<String>? Amenities { get; set; }
}
