using System.Text.Json;
using TodoInFp.Domain;
using TodoInFp.Domain.Workflows;

namespace TodoInFp.Api;

public static class TodoApi
{
  public static void Setup(RouteGroupBuilder builder, IServiceProvider svc)
  {
    builder.MapGet("/", httpContext =>
    {
      var workflow = svc.GetRequiredService<GetTodoItemsWorkflow>();
      var items = workflow.GetTodoItems();
      var json = JsonSerializer.Serialize(items);
      return httpContext.Response.WriteAsync(json);
    });
  }
}