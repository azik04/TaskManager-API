using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class ThemeConfiguration : IEntityTypeConfiguration<Themes>
{
    public void Configure(EntityTypeBuilder<Themes> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Users)
            .WithMany(x => x.Themes)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
