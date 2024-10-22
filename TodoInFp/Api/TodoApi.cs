using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoInFp.Domain;
using TodoItem = TodoInFp.Domain.DomainObjs.TodoItem;

using CreateTodoError = OneOf.OneOf<TodoInFp.Domain.DuplicateItemError, TodoInFp.Domain.UnknownError>;
using IResult = Microsoft.AspNetCore.Http.IResult;
using WorkflowError = OneOf.OneOf<
  TodoInFp.Domain.ServiceNotFound, 
  TodoInFp.Domain.CouldNotReadHttpBody,
  TodoInFp.Domain.DeserializeError,
  TodoInFp.Domain.ItemAlreadyExistsError,
  TodoInFp.Domain.UnknownWorkflowError
>;

namespace TodoInFp.Api;

public class TodoApi(GetTodoItemsWorkflow workflow)
{
  public void Setup(RouteGroupBuilder builder)
  {
      builder.MapGet("/", workflow.GetTodos);
      builder.MapPost("/", httpCtx => 
        Task.FromResult(GetJsonOptions(httpCtx).Map(workflow.CreateTodoItem)));
      
      Result<JsonSerializerOptions, WorkflowError> GetJsonOptions(HttpContext httpCtx) =>
        Result.Try<JsonSerializerOptions, WorkflowError>(() => 
            httpCtx.RequestServices.GetRequiredService<JsonSerializerOptions>(), 
          ex => new ServiceNotFound());
  }
}