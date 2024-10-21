using Microsoft.Extensions.DependencyInjection;
using TodoInFp.Domain;

namespace TodoInFp.Test;

public class TestUtils(WebApplicationFactory<Program> _appFactory)
{
  public HttpClient CreateHttpClient(List<TodoItem> expected) =>
    _appFactory.WithWebHostBuilder(builder =>
        builder.ConfigureServices(services =>
          services.AddSingleton<ITodoItemStore>(_ => new TodoInFpTests.FakeTodoItemStore(expected))))
      .CreateClient();
}