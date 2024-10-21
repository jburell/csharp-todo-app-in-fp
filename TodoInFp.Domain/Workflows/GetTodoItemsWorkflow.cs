namespace TodoInFp.Domain.Workflows;

public class GetTodoItemsWorkflow(ITodoItemStore store)
{
  public List<TodoItem> GetTodoItems() => store.GetTodoItems();
}