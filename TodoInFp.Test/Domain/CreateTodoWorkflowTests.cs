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
    var expected = Result.Success(new TodoItem(2));
    var store = new FakeItemStore(new List<TodoItem>().AsQueryable());
    var workflow = new GetTodoItemsWorkflow(store);

    // Act
    var actual = workflow.CreateTodo(new TodoItem(2));

    // Assert
    actual.Value.Should().Be(expected.Value);
    var items = workflow.GetTodos();
    items.Should().BeEquivalentTo(new List<TodoItem>([new TodoItem(2)]));
  }
}