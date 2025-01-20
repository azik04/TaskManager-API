using FluentValidation;
using TaskManager.Core.Dto.UserTask;

namespace TaskManager.Core.Validation.UserTask;

public class CreateUserTaskDtoValidation : AbstractValidator<CreateUserTaskDto>
{
    public CreateUserTaskDtoValidation()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("Tema ID düzgün olmalıdır.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("İstifadəçi ID düzgün olmalıdır.");
    }
}
