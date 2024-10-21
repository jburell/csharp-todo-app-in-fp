using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;
using TodoItem = TodoInFp.Domain.DomainObjs.TodoItem;

using CreateTodoError = OneOf.OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>;
//susing WorkflowError = OneOf.OneOf<
//  TodoInFp.Domain.DomainObjs.TodoItem,
//  OneOf.OneOf<
//    TodoInFp.Domain.DuplicateItemError,
//    TodoInFp.Domain.UnknownError>>;

namespace TodoInFp.Api;

public class WorkflowError : OneOfBase<
  string, 
  OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>>
{
  public  WorkflowError(OneOf<string, CreateTodoError> input) : base(input)
  {
  }
}

public class TodoApi(Workflow workflow)
{
  public void Setup(RouteGroupBuilder builder)
  {
    builder.MapGet("/", workflow.GetTodos);
    builder.MapPost("/", httpCtx =>
    {
      var jsonOpts = httpCtx.RequestServices.GetRequiredService<JsonSerializerOptions>();
      return Result
        .Try(() => GetRequestBodyAsync(httpCtx.Request), exception => exception.Message)
        .Map<string, WorkflowError>(json => {
          try
          {
            // Deserialize the JSON into a TodoItem
            var todoItem = JsonSerializer.Deserialize<TodoItem>(json, jsonOpts);
            if (todoItem == null)
            {
              return Result.Failure<TodoItem, WorkflowError>(new WorkflowError(new UnknownError()));
            }
            return Result.Success<TodoItem, WorkflowError>(todoItem);
          }
          catch (JsonException ex)
          {
            // If JSON deserialization fails, map to an UnknownError inside WorkflowError
            return Result.Failure<TodoItem, WorkflowError>(new WorkflowError(new TodoInFp.Domain.UnknownError(ex.Message)));
          }
        })
        .MapError(ex => new TodoInFp.Domain.UnknownError())
        .Map<TodoItem, CreateTodoError>(item =>
        
          workflow.CreateTodo(item)
            //.MapError<CreateTodoError, WorkflowError>(
            //  err => err
            //)
        )
        .Match(
          onSuccess: item => httpCtx.Response.StatusCode = (int)HttpStatusCode.Created,
          onFailure: err => httpCtx.Response.StatusCode = 500);
    });
  }

  private string GetRequestBodyAsync(HttpRequest request)
  {
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
    var body = reader.ReadToEnd();
    request.Body.Position = 0;
    return body;
  }
}