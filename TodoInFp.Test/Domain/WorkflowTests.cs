using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Domain;

public class WorkflowTests
{
  public class FakeItemStore(IQueryable<TodoItem> items) : ITodoItemStore
  {
    public IQueryable<TodoItem> GetTodoItems() => items;
  }
  
  [Fact]
  public void WhenGettingTodosFromEmptyList_ShouldReturnEmptyList()
  {
    // Arrange
    var store = new FakeItemStore(new List<TodoItem>().AsQueryable());
    var workflow = new Workflow(store);
    var expected = new List<TodoItem>().AsQueryable();

    // Act
    var actual = workflow.GetTodos();

    // Assert
    actual.Should().BeEquivalentTo(expected);
  }
  
  [Fact]
  public void WhenGettingTodosFromListWithSingleItem_ShouldReturnListWithOneItem()
  {
    // Arrange
    var store = new FakeItemStore(new List<TodoItem>([new TodoItem(1)]).AsQueryable());
    var workflow = new Workflow(store);
    List<TodoItem> expected = [new(1)];

    // Act
    var actual = workflow.GetTodos();

    // Assert
    actual.Should().BeEquivalentTo(expected);
  }
}