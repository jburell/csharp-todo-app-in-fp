using TodoInFp.Api;
using TodoInFp.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(ServiceFactory.GetConfiguration);
builder.Services.AddSingleton(ServiceFactory.CreateTodoDbContext);
builder.Services.AddSingleton(ServiceFactory.CreateJsonOptions);
builder.Services.AddSingleton(ServiceFactory.CreateTodoItemStore);
builder.Services.AddSingleton(ServiceFactory.CreateWorkflow);
builder.Services.AddSingleton(ServiceFactory.CreateApi);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var workflow = app.Services.GetRequiredService<TodoApi>();
workflow.Setup(app.MapGroup("/").WithOpenApi());

app.Run();

public partial class Program { }
