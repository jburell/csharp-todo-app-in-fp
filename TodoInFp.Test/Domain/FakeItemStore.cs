using CSharpFunctionalExtensions;
using OneOf;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Domain;

public class FakeItemStore(IQueryable<TodoItem> items) : ITodoItemStore
{
  private IQueryable<TodoItem> _items = items;
  public IQueryable<TodoItem> GetTodoItems() => _items;
  public Result<int, OneOf<DuplicateItemError, UnknownError>> CreateTodoItem(TodoItem item)
  {
    _items = _items.Append(item);
    return item.Id;
  }
}
