namespace TodoInFp.Domain;

using DomainErrors = OneOf<NotFound, Unkown>;

public class Workflow(ITodoItemStore store)
{
  public Result<List<TodoItem>, DomainErrors> GetTodoItems()
  {
    return store.GetTodoItems();
  }
}