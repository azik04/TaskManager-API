using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.DataProvider.Context;

namespace TaskManager.Infrastructure;

public static class ServiceRegistrationExtensions
{
    public static void AddServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IUserTaskService, UserTaskService>();
        //builder.Services.AddScoped<IMailService, MailService>();
        services.AddScoped<ISubTaskService, SubTaskService>();
        services.AddScoped<IUserThemeService, UserThemeService>();

        services.AddScoped<ApplicationDbContext>();
    }
}
