using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comments>
{
    public void Configure(EntityTypeBuilder<Comments> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comment)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Tasks)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
