using TodoInFp.Api;
using TodoInFp.Domain;
using TodoInFp.Domain.Workflows;
using TodoInFp.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITodoItemStore>(svc => new TodoItemStore());
builder.Services.AddSingleton<GetTodoItemsWorkflow>(svc => new GetTodoItemsWorkflow(svc.GetRequiredService<ITodoItemStore>()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var mapGroup = app.MapGroup("/");
TodoApi.Setup(mapGroup, app.Services);
mapGroup
  .WithName("Todo App")
  .WithOpenApi();

app.Run();

public partial class Program { }
