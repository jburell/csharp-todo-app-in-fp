using CSharpFunctionalExtensions;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;

public interface ITodoItemStore
{
  public IQueryable<TodoItem> GetTodoItems();
  public Result CreateTodoItem(TodoItem item);
}