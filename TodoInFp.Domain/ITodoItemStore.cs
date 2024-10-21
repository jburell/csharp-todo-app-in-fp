namespace TodoInFp.Domain;

public interface ITodoItemStore
{
  public List<TodoItem> GetTodoItems();
}