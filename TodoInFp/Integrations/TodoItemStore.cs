using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoInFp.DbClient;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Integrations;

using CreateTodoError = OneOf.OneOf<DuplicateItemError, UnknownError>;

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

  public Result<int, CreateTodoError> CreateTodoItem(TodoItem item)
  {
    return Result.Try<int, CreateTodoError>(() =>
    {
      _todoDb.TodoItems.Add(_modelMapper.Map<TodoItem, ModelItem>(item));
      _todoDb.SaveChanges();
      return item.Id;
    }, exception => exception switch
    {
      InvalidOperationException e => e.Message switch
        {
          { } err when err.Contains("is already being tracked") => new DuplicateItemError(),
          _ => new UnknownError()
        },
      _ => new UnknownError()
    });
  }
}