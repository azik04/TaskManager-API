using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Statuses>
{
    public void Configure(EntityTypeBuilder<Statuses> builder)
    {
        builder.HasKey(x => x.Id);  
    }
}
