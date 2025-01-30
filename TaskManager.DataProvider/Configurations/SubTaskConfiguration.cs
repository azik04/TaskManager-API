using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class SubTaskConfiguration : IEntityTypeConfiguration<SubTasks>
{
    public void Configure(EntityTypeBuilder<SubTasks> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
       .ValueGeneratedOnAdd();

        builder.HasOne(c => c.User)
           .WithMany(u => u.CoTasks)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Task)
           .WithMany(u => u.CoTasks)
           .HasForeignKey(c => c.TaskId)
           .OnDelete(DeleteBehavior.Restrict);
    }

}
