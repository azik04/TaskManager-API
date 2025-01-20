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
           .WithMany(t => t.Tasks)
           .HasForeignKey(t => t.ExecutiveUserId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Priority)
          .HasConversion<int>();

        builder.Property(x => x.Status)
          .HasConversion<int>();
    }
}
