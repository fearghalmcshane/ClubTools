namespace ClubTools.Shared.Contracts;

public class UpdateArticleRequest
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
}
