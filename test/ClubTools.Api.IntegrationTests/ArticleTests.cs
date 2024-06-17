using ClubTools.Api.Features.Articles;

namespace ClubTools.Api.IntegrationTests;

public class ArticleTests : BaseIntegrationTest
{
    public ArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ShouldCreateArticle()
    {
        // Arrange
        var command = new CreateArticle.Command
        {
            Title = "Test",
            Content = "Test article",
            Tags = new List<string> { "Test" },
        };

        // Act
        var articleId = await Sender.Send(command);

        // Assert
        var article = DbContext.Articles.FirstOrDefault(c => c.Id == articleId.Value);

        Assert.NotNull(article);
    }

    [Fact]
    public async Task Get_ShouldReturnArticle_WhenArticleExists()
    {
        // Arrange
        var articleId = await CreateArticle();
        var query = new GetArticle.Query { Id = articleId };

        // Act
        var articleResponse = await Sender.Send(query);

        // Assert
        Assert.NotNull(articleResponse);
    }

    [Fact]
    public async Task Get_ShouldReturnError_WhenArticleIsNull()
    {
        // Arrange
        var query = new GetArticle.Query { Id = Guid.NewGuid() };

        // Act
        var articleResponse = await Sender.Send(query);

        // Assert
        Assert.True(articleResponse.IsFailure);
    }

    [Fact]
    public async Task Update_ShouldUpdateArticle()
    {
        // Arrange
        var articleId = await CreateArticle();
        var command = new UpdateArticle.Command
        {
            Id = articleId,
            Title = "Test",
            Content = "Test article",
            Tags = new List<string> { "Test" },
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Delete_ShouldDeleteArticle()
    {
        // Arrange
        var articleId = await CreateArticle();
        var command = new DeleteArticle.Command { Id = articleId };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    //[Fact]
    //public async Task Delete_ShouldReturnError_WhenArticleIsNull()
    //{
    //    // Arrange
    //    var command = new DeleteArticle.Command { Id = Guid.NewGuid() };

    //    // Act
    //    var result = await Sender.Send(command);

    //    // Assert
    //    Assert.True(result.IsFailure);
    //}

    private async Task<Guid> CreateArticle()
    {
        var command = new CreateArticle.Command
        {
            Title = "Test",
            Content = "Test article",
            Tags = new List<string> { "Test" },
        };

        var id = await Sender.Send(command);

        return id.Value;
    }
}
