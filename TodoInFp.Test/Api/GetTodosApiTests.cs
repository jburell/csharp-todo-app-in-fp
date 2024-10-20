using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoInFp.DbClient;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Api;

public class GetTodosApiTests(WebApplicationFactory<Program> factory)
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

  [Fact]
  public async void ShouldGetItemListWithOneItem_WhenFetchingTodos()
  {
    // Arrange
    List<TodoItem> expected = [new(1)];
    var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>();
    optionsBuilder.UseSqlite("Data Source=:memory:");
    await using var ctx = new TodoDbContext(optionsBuilder.Options);
    ctx.Database.OpenConnection();
    //ctx.Database.EnsureCreated(); // Kan inte kombineras med Migrate
    ctx.Database.Migrate();
    ctx.TodoItems.Add(new DbClient.Models.TodoItem { Id = 1 });
    ctx.SaveChanges();
    var client = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
      services.AddSingleton<TodoDbContext>(_ => ctx))).CreateClient();

    var jsonOpts = factory.Services.GetRequiredService<JsonSerializerOptions>();

    // Act
    var res = await client.GetAsync("/");

    // Assert
    var jsonString = await res.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<List<TodoItem>>(jsonString, jsonOpts) ??
                 throw new InvalidOperationException();
    result.Should().BeEquivalentTo(expected);
  }
}