namespace TodoInFp.Domain;

public class Workflow(ITodoItemStore store)
{
  public List<TodoItem> GetTodoItems()
  {
    return store.GetTodoItems();
  }
}