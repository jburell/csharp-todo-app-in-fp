using CSharpFunctionalExtensions;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Domain;

public class FakeItemStore(IQueryable<TodoItem> items) : ITodoItemStore
{
  private IQueryable<TodoItem> _items = items;
  public IQueryable<TodoItem> GetTodoItems() => _items;
  public Result CreateTodoItem(TodoItem item)
  {
    _items = _items.Append(item);
    return Result.Success();
  }
}
