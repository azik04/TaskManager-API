using TaskManager.DataProvider.Enums;

namespace TaskManager.Core.Dto.Tasks;

public class UpdateTaskDto
{
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateOnly? DeadLine { get; set; }
    public string? Contact { get; set; }

    public long? ExecutiveUserId { get; set; }
    public IList<long>? UserTasks { get; set; }
}
