using TodoInFp.Domain;

namespace TodoInFp.Stores;


public class TodoItemStore : ITodoItemStore
{
  public List<TodoItem> GetTodoItems()
  {
    return [new TodoItem(1), new TodoItem(3)];
  }
}