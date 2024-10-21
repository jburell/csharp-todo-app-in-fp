using System.Text.Json;
using TodoInFp.Domain;

namespace TodoInFp.Api;

public static class TodoApi
{
  public static void Setup(RouteGroupBuilder builder, IServiceProvider svc)
  {
    builder.MapGet("/", httpContext =>
    {
      var workflow = svc.GetRequiredService<Workflow>();
      var items = workflow.GetTodoItems();
      var json = JsonSerializer.Serialize(items);
      return httpContext.Response.WriteAsync(json);
    });
  }
}