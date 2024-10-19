using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;

public class Workflow(ITodoItemStore store)
{
  public IQueryable<TodoItem> GetTodos() => store.GetTodoItems();
}