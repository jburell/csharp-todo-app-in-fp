namespace TodoInFp.Api;

public static class TodoApi
{
  public static void Setup(RouteGroupBuilder builder)
  {
    builder.MapGet("/", () => "Hello World!");
  }
}