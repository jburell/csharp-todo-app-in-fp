using CSharpFunctionalExtensions;
using OneOf;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;
using CreateTodoError = OneOf<DuplicateItemError, UnknownError>;

public class Workflow(ITodoItemStore store)
{
  public IQueryable<TodoItem> GetTodos() => store.GetTodoItems();

  public Result<TodoItem, CreateTodoError> CreateTodo(TodoItem item) => store.CreateTodoItem(item);
}