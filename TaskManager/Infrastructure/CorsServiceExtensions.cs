namespace TaskManager.Infrastructure
{
    public static class CorsServiceExtensions
    {
        public static void Cors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyCors", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://ittaskmanager.adra.gov.az")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }
    }
}
