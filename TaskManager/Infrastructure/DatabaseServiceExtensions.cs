using Microsoft.EntityFrameworkCore;
using TaskManager.DataProvider.Context;

namespace TaskManager.Infrastructure
{
    public static class DatabaseServiceExtensions
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connection));
        }
    }
}
