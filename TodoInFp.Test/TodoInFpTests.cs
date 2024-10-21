using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TodoInFp.Domain;

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

  public class FakeTodoItemStore(List<TodoItem> items) : ITodoItemStore
  {
    public List<TodoItem> GetTodoItems() => items;
  }
  
  [Fact]
  public async void ShouldGetATodoItem_WhenFetchingTodos()
  {
    // Arrange
    List<TodoItem> expected = [new(1)];
    var client = factory.WithWebHostBuilder(builder =>
      builder.ConfigureServices(services =>
        services.AddSingleton<ITodoItemStore>(_ => new FakeTodoItemStore(expected))))
      .CreateClient();
    
    // Act
    var res = await client.GetAsync("/");

    // Assert
    res.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = await res.Content.ReadAsStringAsync();
    var item = JsonSerializer.Deserialize<List<TodoItem>>(json, JsonSerializerOptions.Default);
    item.Should().BeEquivalentTo(expected);
  }
  
  [Fact]
  public async void ShouldGetTwoTodoItems_WhenFetchingTodos()
  {
    // Arrange
    List<TodoItem> expected = [new(1), new (3)];
    var client = factory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
          services.AddSingleton<ITodoItemStore>(_ => new FakeTodoItemStore(expected))))
      .CreateClient();
    
    // Act
    var res = await client.GetAsync("/");

    // Assert
    res.StatusCode.Should().Be(HttpStatusCode.OK);
    var json = await res.Content.ReadAsStringAsync();
    var item = JsonSerializer.Deserialize<List<TodoItem>>(json, JsonSerializerOptions.Default);
    item.Should().BeEquivalentTo(expected);
  }
}