using Serilog;
using TaskManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocumentation();

builder.Services.Cors();

builder.Services.AddAuthenticationConfiguration();

builder.Services.AddDatabaseConfiguration(builder.Configuration);

builder.Services.AddServiceDependencies();

builder.Services.AddValidationService();

Log.Logger = new LoggerConfiguration()
     .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Hour)
     .CreateLogger();

Log.Information("Starting up!");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("MyCors");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "area",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllers();

app.Run();
