using TodoInFp.Domain;

namespace TodoInFp.Test.TestFixtures;

public class TestData(string description, List<TodoItem> expected)
{
  public readonly string _description = description;
  public readonly List<TodoItem> _expected = expected;
  public override string ToString() => _description;
}

public class FakeTodoItemStore(List<TodoItem> store) : ITodoItemStore
{
  public List<TodoItem> GetTodoItems() => store;
}