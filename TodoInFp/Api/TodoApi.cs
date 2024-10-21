using System.Net;
using System.Text;
using System.Text.Json;
using TodoInFp.Domain;
using TodoItem = TodoInFp.Domain.DomainObjs.TodoItem;

using CreateTodoError = OneOf.OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>;

namespace TodoInFp.Api;


public class TodoApi(Workflow workflow)
{
  public void Setup(RouteGroupBuilder builder)
  {
    builder.MapGet("/", workflow.GetTodos);
    builder.MapPost("/", httpCtx => Result
        .Try(() => httpCtx.RequestServices.GetRequiredService<JsonSerializerOptions>(), ex => new UnknownError())
        .MapTry<JsonSerializerOptions, (JsonSerializerOptions, string), CreateTodoError>(opts => (opts, GetRequestBodyAsync(httpCtx.Request)), exception => new UnknownError())
        .MapTry<(JsonSerializerOptions, string), TodoItem, CreateTodoError>((jsonOpts, json) =>
          JsonSerializer.Deserialize<TodoItem>(json, jsonOpts) ?? throw new ArgumentNullException(),
          ex => new UnknownError())
        .Bind<TodoItem, CreateTodoError>(item =>
          workflow.CreateTodo(item)
        )
        .Match(
          onSuccess: item => httpCtx.Response.StatusCode = (int)HttpStatusCode.Created,
          onFailure: err => httpCtx.Response.StatusCode = 500)
    );
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