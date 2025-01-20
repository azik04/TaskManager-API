using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManager.Core.Validation.AUTH;
using TaskManager.Core.Validation.User;
using TaskManager.Core.Validation.Comment;
using TaskManager.Core.Validation.Files;
using TaskManager.Core.Validation.SubTask;
using TaskManager.Core.Validation.Task;
using TaskManager.Core.Validation.Theme;
using TaskManager.Core.Validation.UserTheme;
using TaskManager.Core.Validation.UserTask;

namespace TaskManager.Infrastructure;

public static class FluentValidationServiceExtention
{
    public static void AddValidationService(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<AuthDtoValidation>();

        services.AddValidatorsFromAssemblyContaining<CreateCommentDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<UpdateCommentDtoValidation>();


        services.AddValidatorsFromAssemblyContaining<CreateFileDtoValidation>();

        services.AddValidatorsFromAssemblyContaining<CreateSubTaskDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<UpdateSubTaskDtoValidation>();


        services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<UpdateTaskDtoValidation>();

        services.AddValidatorsFromAssemblyContaining<CreateThemeDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<UpdateThemeDtoValidation>();

        services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidation>();
        services.AddValidatorsFromAssemblyContaining<UpdateUserDtoValidation>();


        services.AddValidatorsFromAssemblyContaining<CreateUserThemeDtoValidation>();

        services.AddValidatorsFromAssemblyContaining<CreateUserTaskDtoValidation>();
    }
}
