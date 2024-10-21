using System.Text.Json;
using TodoInFp.Domain;
using TodoInFp.Test.TestFixtures;

namespace TodoInFp.Test;

public class TodoInFpTests(WebApplicationFactory<Program> factory) 
  : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly TestUtils _utils = new(factory);
  
  public static IEnumerable<object[]> CreateTestData()
  {
    yield return
    [
      new TestData("Empty result", [])
    ];
    yield return
    [
      new TestData("One Todo item", [new(3)])
    ];
    yield return
    [
      new TestData("Two Todo items", [new(1), new(3)])
    ];
  }

  [Theory]
  [MemberData(nameof(CreateTestData))]
  public async void ShouldGetTodoItems(TestData testData)
  {
    // Arrange
    var client = _utils.CreateHttpClient(testData._expected);
    
    // Act
    var res = await client.GetAsync("/");

    // Assert
    res.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = await res.Content.ReadAsStringAsync();
    var item = JsonSerializer.Deserialize<List<TodoItem>>(json, JsonSerializerOptions.Default);
    item.Should().BeEquivalentTo(testData._expected);
  }
}