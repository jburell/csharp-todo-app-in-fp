using System.Net;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using OneOf;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Domain;
using CreateTodoError = OneOf<DuplicateItemError, UnknownError>;
using WorkflowError = OneOf<
  ServiceNotFound, 
  CouldNotReadHttpBody,
  DeserializeError,
  ItemAlreadyExistsError,
  UnknownWorkflowError
>;

public record CouldNotReadHttpBody;
public record ServiceNotFound;
public record DeserializeError;
public record ItemAlreadyExistsError;
public record UnknownWorkflowError;

public class GetTodoItemsWorkflow(ITodoItemStore store)
{
  public IQueryable<TodoItem> GetTodos() => store.GetTodoItems();

  public Result<TodoItem, CreateTodoError> CreateTodoItem(JsonSerializerOptions jsonOpts) => 
  Result.Success(jsonOpts)
    .Bind(async jsonOpts =>
  (await GetJson(jsonOpts, httpCtx))
  .Map(json => (jsonOpts, json)))
  .Bind<(JsonSerializerOptions, string), TodoItem, WorkflowError>(
  val => DeserializeJson(val.Item1, val.Item2))
    .Bind<TodoItem, TodoItem, WorkflowError>(item =>
  workflow.CreateTodo(item)
  .MapError<TodoItem, CreateTodoError, WorkflowError>(err => err.Match<WorkflowError>(
  duplicate => new ItemAlreadyExistsError(),
  unknown => new UnknownWorkflowError()))
    ).Match(
    onSuccess: item => (int)HttpStatusCode.Created,
    onFailure: e => e.Value switch
    {
      ItemAlreadyExistsError => (int)HttpStatusCode.Conflict,
      _ => (int)HttpStatusCode.InternalServerError,
    } 
  ))
  
  private async Task<string> GetRequestBodyAsync(HttpRequest request)
  {
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    request.Body.Position = 0;
    return body;
  }
  
  private Result<TodoItem, CreateTodoError> StoreTodo(TodoItem item) => store.CreateTodoItem(item);

  private Task<Result<string, WorkflowError>> GetJson(JsonSerializerOptions opts, HttpContext httpCtx) =>
    Result.Try<string, WorkflowError>(async () => await GetRequestBodyAsync(httpCtx.Request), 
      ex => new CouldNotReadHttpBody());

  private Result<TodoItem, WorkflowError> DeserializeJson(JsonSerializerOptions jsonOpts, string json) =>
    Result.Try<TodoItem, WorkflowError>(
      () => JsonSerializer.Deserialize<TodoItem>(json, jsonOpts) ?? throw new InvalidOperationException(), 
      ex => new DeserializeError());
}