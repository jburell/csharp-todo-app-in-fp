using CSharpFunctionalExtensions;
using TodoInFp.Domain.DomainObjs;

using CreateTodoError = OneOf.OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>;

namespace TodoInFp.Domain;

public class DuplicateItemError {};
public class UnknownError {};

public interface ITodoItemStore
{
  public IQueryable<TodoItem> GetTodoItems();
  public Result<TodoItem, CreateTodoError> CreateTodoItem(TodoItem item);
}