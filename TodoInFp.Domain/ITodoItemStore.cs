using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;

public interface ITodoItemStore
{
  public IQueryable<TodoItem> GetTodoItems();
}