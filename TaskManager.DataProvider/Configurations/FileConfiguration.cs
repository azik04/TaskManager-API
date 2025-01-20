using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<Files>
{
    public void Configure(EntityTypeBuilder<Files> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Tasks)
            .WithMany(u => u.Files)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
