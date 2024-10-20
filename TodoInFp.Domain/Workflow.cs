using CSharpFunctionalExtensions;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;

public class Workflow(ITodoItemStore store)
{
  public IQueryable<TodoItem> GetTodos() => store.GetTodoItems();

  public Result CreateTodo(TodoItem item) => store.CreateTodoItem(item);
}