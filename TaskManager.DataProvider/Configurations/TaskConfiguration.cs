using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Tasks>
{
    public void Configure(EntityTypeBuilder<Tasks> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Theme)
            .WithMany(t => t.Tasks)
            .HasForeignKey(t => t.ThemeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ExecutiveUser)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.ExecutiveUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Status)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Priority)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
