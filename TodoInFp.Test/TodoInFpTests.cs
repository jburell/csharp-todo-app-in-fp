namespace TodoInFp.Test;

public class TodoInFpTests(WebApplicationFactory<Program> factory) 
  : IClassFixture<WebApplicationFactory<Program>>
{
  [Fact]
  public async void ShouldGetHttp200_WhenFetchingTodos()
  {
    // Arrange
    var client = factory.CreateClient();
    
    // Act
    var res = await client.GetAsync("/");

    // Assert
    res.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}