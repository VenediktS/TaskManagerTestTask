using Microsoft.EntityFrameworkCore;
using TaskManager.Common.Entities;

namespace TaskManager.DB;

public class TaskManagerDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<TaskStatusForChangeEntity> TaskStatusForChangeEntities => Set<TaskStatusForChangeEntity>();

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

