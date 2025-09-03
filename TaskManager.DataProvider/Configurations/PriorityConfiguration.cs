using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.DataProvider.Entities;

namespace TaskManager.DataProvider.Configurations;

public class PriorityConfiguration : IEntityTypeConfiguration<Priorities>
{
    public void Configure(EntityTypeBuilder<Priorities> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
