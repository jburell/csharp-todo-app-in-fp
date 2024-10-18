using TodoInFp.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
app.UseHttpsRedirection();

var mapGroup = app.MapGroup("/");
TodoApi.Setup(mapGroup);
mapGroup
  .WithName("Todo App")
  .WithOpenApi();

app.Run();

public partial class Program { }
