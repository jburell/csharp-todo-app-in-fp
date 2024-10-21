using CSharpFunctionalExtensions;

namespace TodoInFp.Domain.Workflows;

public class CreateTodoWorkflow(ITodoItemStore store)
{
  public Result CreateTodoItem() => Result.Failure("Pang");
}