using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoInFp.DbClient;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Api;

public class CreateTodoApiTests(WebApplicationFactory<Program> factory)
  : IClassFixture<WebApplicationFactory<Program>>
{
  [Fact]
  public async void ShouldPersistOneTodo()
  {
    // Arrange
    List<TodoItem> expected = [new(1), new(3)];
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
    var json = new StringContent(JsonSerializer.Serialize(new TodoItem(3), jsonOpts));

    // Act
    var res = await client.PostAsync("/", json);

    // Assert
    var status = res.StatusCode;
    status.Should().Be(HttpStatusCode.Created);
    var getResponse = await client.GetAsync("/"); 
    var jsonString = await getResponse.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<List<TodoItem>>(jsonString, jsonOpts) ??
                 throw new InvalidOperationException();
    result.Should().BeEquivalentTo(expected);
  }
  
  [Fact]
  public async void ShouldNotPersistOneTodoWhenAlreadyExists()
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
    var json = new StringContent(JsonSerializer.Serialize(new TodoItem(1), jsonOpts));

    // Act
    var res = await client.PostAsync("/", json);

    // Assert
    var status = res.StatusCode;
    status.Should().Be(HttpStatusCode.BadRequest);
    var getResponse = await client.GetAsync("/"); 
    var jsonString = await getResponse.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<List<TodoItem>>(jsonString, jsonOpts) ??
                 throw new InvalidOperationException();
    result.Should().BeEquivalentTo(expected);
  }
}