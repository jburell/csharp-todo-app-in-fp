using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TodoInFp.Api;
using TodoInFp.DbClient;
using TodoInFp.Domain;
using TodoInFp.Integrations;

namespace TodoInFp.Setup;

public static class ServiceFactory
{
  public static IConfiguration GetConfiguration(IServiceProvider _) =>
    new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json")
      //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json")
      .AddEnvironmentVariables()
      .AddUserSecrets<Program>()
      .Build();

  public static TodoDbContext CreateTodoDbContext(IServiceProvider svc)
  {
    var connStr = svc.GetRequiredService<IConfiguration>().GetConnectionString("TodoDbConnection");
    var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>();
    optionsBuilder.UseSqlite(connStr);
    return new TodoDbContext(optionsBuilder.Options);
  }

  public static JsonSerializerOptions CreateJsonOptions(IServiceProvider _) => new()
  {
    IncludeFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static ITodoItemStore CreateTodoItemStore(IServiceProvider svc) =>
    new TodoItemStore(svc.GetRequiredService<TodoDbContext>());

  public static Workflow CreateWorkflow(IServiceProvider svc) => new(svc.GetRequiredService<ITodoItemStore>());

  public static TodoApi CreateApi(IServiceProvider svc) => new(svc.GetRequiredService<Workflow>());
}