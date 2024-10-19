using AutoMapper;
using TodoInFp.DbClient;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Integrations;

using ModelItem = TodoInFp.DbClient.Models.TodoItem;

public class TodoItemStore : ITodoItemStore
{
  private readonly IMapper _todoMapper;
  private readonly TodoDbContext _todoDb;

  public TodoItemStore(TodoDbContext todoDb)
  {
    _todoDb = todoDb;
    _todoMapper = new MapperConfiguration(cfg => cfg.CreateMap<ModelItem, TodoItem>())
      .CreateMapper();
  }

  public IQueryable<TodoItem> GetTodoItems()
  {
    return _todoDb.TodoItems.Select(i => _todoMapper.Map<ModelItem, TodoItem>(i));
  }
}