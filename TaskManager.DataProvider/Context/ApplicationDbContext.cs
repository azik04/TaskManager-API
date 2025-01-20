using Microsoft.EntityFrameworkCore;
using TaskManager.DataProvider.Configurations;
using TaskManager.DataProvider.Entities;
namespace TaskManager.DataProvider.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<Files> Files { get; set; }
    public DbSet<Themes> Themes { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<SubTasks> SubTasks { get; set; }
    public DbSet<UserThemes> UserThemes { get; set; }
    public DbSet<UserTasks> UserTask { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        modelBuilder.ApplyConfiguration(new SubTaskConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new ThemeConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserTaskConfiguration());
        modelBuilder.ApplyConfiguration(new UserThemeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
