using TodoInFp.Domain;

namespace TodoInFp.Api;

public class TodoApi(Workflow workflow)
{
  public void Setup(RouteGroupBuilder builder) => builder.MapGet("/", workflow.GetTodos);
}