using CSharpFunctionalExtensions;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Test.Domain;

public class CreateTodoWorkflowTests
{
  [Fact]
  public void CanCreateTodo()
  {
    // Arrange
    var expected = Result.Success();
    var store = new FakeItemStore(new List<TodoItem>().AsQueryable());
    var workflow = new Workflow(store);

    // Act
    var actual = workflow.CreateTodo(new TodoItem(1));

    // Assert
    actual.Should().Be(expected);
    var items = workflow.GetTodos();
    items.Should().BeEquivalentTo(new List<TodoItem>([new TodoItem(1)]));
  }
}