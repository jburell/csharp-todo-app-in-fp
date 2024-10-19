using Microsoft.EntityFrameworkCore;
using TodoInFp.DbClient.Models;

namespace TodoInFp.DbClient;

public class TodoDbContext : DbContext
{
  public DbSet<TodoItem> TodoItems { get; set; }
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlite(Environment.GetEnvironmentVariable("EF_CONNECTION_STRING"));
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TodoItem>(entity => entity.HasKey(e => e.Id));
  }
}