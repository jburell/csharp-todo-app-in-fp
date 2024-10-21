using TodoInFp.Domain;

namespace TodoInFp.Stores;


public class TodoItemStore : ITodoItemStore
{
  public Result<List<TodoItem>, DomainErrors> GetTodoItems()
  {
    return Result.Success([new TodoItem(1), new TodoItem(3)]);
  }
}