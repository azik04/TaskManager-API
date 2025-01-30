using FluentValidation;
using TaskManager.Core.Dto.SubTask;

namespace TaskManager.Core.Validation.SubTask;

public class CreateSubTaskDtoValidation : AbstractValidator<CreateSubTaskDto>
{
    public CreateSubTaskDtoValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Alt tapşırıq adı tələb olunur.");

        RuleFor(x => x.Priority)
             .NotEmpty()
             .WithMessage("Prioritet tələb olunur.");

        RuleFor(x => x.DeadLine)
            .NotEmpty()
            .WithMessage("Son tarix adı tələb olunur.")
            .Must(deadline => deadline > DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Son tarix indiki vaxtdan sonra olmalıdır."); 

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("İstifadəçi ID düzgün olmalıdır."); 

        RuleFor(x => x.TaskId)
            .GreaterThan(0)
            .WithMessage("Task ID düzgün olmalıdır.");
    }
}

