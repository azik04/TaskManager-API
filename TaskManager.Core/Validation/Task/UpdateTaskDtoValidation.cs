using FluentValidation;
using TaskManager.Core.Dto.Tasks;

namespace TaskManager.Core.Validation.Task;

public class UpdateTaskDtoValidation : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidation()
    {
        RuleFor(x => x.TaskName)
             .NotEmpty()
             .WithMessage("Tapşırıq adı tələb olunur.");

        RuleFor(x => x.TaskDescription)
            .NotEmpty()
            .WithMessage("Tapşırıq təsviri tələb olunur.");

        RuleFor(x => x.Priority)
             .NotEmpty()
             .WithMessage("Tapşırıq adı tələb olunur.");


        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Tapşırıq adı tələb olunur.");



        RuleFor(x => x.DeadLine)
            .Must(deadline => deadline > DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Son tarix indiki vaxtdan sonra olmalıdır.");

    }
}
