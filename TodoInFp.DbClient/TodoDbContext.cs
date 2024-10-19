using Microsoft.EntityFrameworkCore;
using TodoInFp.DbClient.Models;

namespace TodoInFp.DbClient;

public class TodoDbContext(DbContextOptions<TodoDbContext> optionsBuilderOptions) 
  : DbContext(optionsBuilderOptions)
{
  public DbSet<TodoItem> TodoItems { get; set; }
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    //var connStr = Environment.GetEnvironmentVariable("TODO_IN_FP_EF_CONNECTION_STRING");
    //optionsBuilder.UseSqlite(connStr);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TodoItem>(entity => entity.HasKey(e => e.Id));
  }
}