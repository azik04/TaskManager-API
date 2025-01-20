using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class UserTaskConfiguration : IEntityTypeConfiguration<UserTasks>
{
    public void Configure(EntityTypeBuilder<UserTasks> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Users)
            .WithMany(t => t.UserTasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Tasks)
            .WithMany(t => t.UserTasks)
            .HasForeignKey(t => t.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
