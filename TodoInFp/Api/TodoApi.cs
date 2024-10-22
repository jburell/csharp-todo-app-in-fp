using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoInFp.Domain;
using TodoItem = TodoInFp.Domain.DomainObjs.TodoItem;

using CreateTodoError = OneOf.OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>;
using IResult = Microsoft.AspNetCore.Http.IResult;
using WorkflowError = OneOf.OneOf<
  TodoInFp.Api.ServiceNotFound, 
  TodoInFp.Api.CouldNotReadHttpBody,
  TodoInFp.Api.DeserializeError,
  TodoInFp.Api.ItemAlreadyExistsError,
  TodoInFp.Api.UnknownWorkflowError
>;

namespace TodoInFp.Api;

record CouldNotReadHttpBody;
record ServiceNotFound;
record DeserializeError;
record ItemAlreadyExistsError;
record UnknownWorkflowError;

public class TodoApi(GetTodoItemsWorkflow workflow)
{
  public void Setup(RouteGroupBuilder builder)
  {
      builder.MapGet("/", workflow.GetTodos);
      builder.MapPost("/", async httpCtx => 
        await GetJsonOptions(httpCtx)
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
            onSuccess: item => httpCtx.Response.StatusCode = (int)HttpStatusCode.Created,
            onFailure: e => e.Value switch
            {
              ItemAlreadyExistsError => httpCtx.Response.StatusCode = (int)HttpStatusCode.Conflict,
              _ => httpCtx.Response.StatusCode = (int)HttpStatusCode.InternalServerError,
            } 
          ));
      

    Result<JsonSerializerOptions, WorkflowError> GetJsonOptions(HttpContext httpCtx) =>
      Result.Try<JsonSerializerOptions, WorkflowError>(() => 
        httpCtx.RequestServices.GetRequiredService<JsonSerializerOptions>(), 
        ex => new ServiceNotFound());

    Task<Result<string, WorkflowError>> GetJson(JsonSerializerOptions opts, HttpContext httpCtx) =>
      Result.Try<string, WorkflowError>(async () => await GetRequestBodyAsync(httpCtx.Request), ex => new CouldNotReadHttpBody());

    Result<TodoItem, WorkflowError> DeserializeJson(JsonSerializerOptions jsonOpts, string json) =>
      Result.Try<TodoItem, WorkflowError>(
        () => JsonSerializer.Deserialize<TodoItem>(json, jsonOpts) ?? throw new InvalidOperationException(), 
        ex => new DeserializeError());
  }

  private async Task<string> GetRequestBodyAsync(HttpRequest request)
  {
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    request.Body.Position = 0;
    return body;
  }
}