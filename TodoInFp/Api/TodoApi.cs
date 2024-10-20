using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoInFp.Domain;
using TodoInFp.Domain.DomainObjs;

namespace TodoInFp.Api;

public class TodoApi(Workflow workflow)
{
  public void Setup(RouteGroupBuilder builder)
  {
    builder.MapGet("/", workflow.GetTodos);
    builder.MapPost("/", async httpCtx =>
    {
      var jsonOpts = httpCtx.RequestServices.GetRequiredService<JsonSerializerOptions>();
      var json = await GetRequestBodyAsync(httpCtx.Request);
      var item = JsonSerializer.Deserialize<TodoItem>(json, jsonOpts);
      var result = workflow.CreateTodo(item ?? throw new InvalidOperationException("TodoItem was null after deserialization"));
      if (result.IsSuccess)
      {
        httpCtx.Response.StatusCode = (int)HttpStatusCode.Created;
      }
      else
      {
        httpCtx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      }
    });
  }
  
  public async Task<string> GetRequestBodyAsync(HttpRequest request)
  {
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    request.Body.Position = 0;
    return body;
  }
}