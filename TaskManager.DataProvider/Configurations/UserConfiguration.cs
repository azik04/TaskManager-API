using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasData(new Users
        {
            Id = 1,
            FullName = "Admin",
            Email = "admin@adra.gov.az",
            Role = Enums.Role.Admin,
            Password = "Admin2025"
        });

        builder.Property(x => x.Role)
            .HasConversion<int>();
    }
}
