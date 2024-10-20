using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    
    return Result.Try(() =>
    {
      var i = _todoDb.TodoItems.Find(item.Id);
      if (i == null)
      {
        _todoDb.TodoItems.Add(_modelMapper.Map<TodoItem, ModelItem>(item));
        _todoDb.SaveChanges();
      }
      else
      {
        throw new DbUpdateException("Duplicate item");
        //Result.Failure("Duplicate item");
      }
    }, exception => exception switch
    {
      DbUpdateException e => e.InnerException?.Message /*switch
      {
        Exception a => Result.Failure(a.Message)
      },*/
    });
  }
}