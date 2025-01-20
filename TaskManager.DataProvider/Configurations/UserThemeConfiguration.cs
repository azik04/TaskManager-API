using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class UserThemeConfiguration : IEntityTypeConfiguration<UserThemes>
{
    public void Configure(EntityTypeBuilder<UserThemes> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserThemes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Theme)
           .WithMany(x => x.UserThemes)
           .HasForeignKey(x => x.ThemeId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
