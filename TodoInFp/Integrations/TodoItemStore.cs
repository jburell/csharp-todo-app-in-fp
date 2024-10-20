using AutoMapper;
using TodoInFp.DbClient;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Integrations;

using ModelItem = TodoInFp.DbClient.Models.TodoItem;

public class TodoItemStore : ITodoItemStore
{
  private readonly IMapper _todoMapper;
  private readonly IMapper _modelMapper;
  private readonly TodoDbContext _todoDb;

  public TodoItemStore(TodoDbContext todoDb)
  {
    _todoDb = todoDb;
    _todoMapper = new MapperConfiguration(cfg => cfg.CreateMap<ModelItem, TodoItem>())
      .CreateMapper();
    _modelMapper = new MapperConfiguration(cfg => cfg.CreateMap<TodoItem, ModelItem>())
      .CreateMapper();
  }

  public IQueryable<TodoItem> GetTodoItems()
  {
    return _todoDb.TodoItems.Select(i => _todoMapper.Map<ModelItem, TodoItem>(i));
  }

  public Result CreateTodoItem(TodoItem item)
  {
    _todoDb.TodoItems.Add(_modelMapper.Map<TodoItem, ModelItem>(item));
    return Result.Try(() => _todoDb.SaveChanges());
  }
}