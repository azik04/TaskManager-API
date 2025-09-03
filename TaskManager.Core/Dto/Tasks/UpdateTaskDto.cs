using TaskManager.DataProvider.Enums;

namespace TaskManager.Core.Dto.Tasks;

public class UpdateTaskDto
{
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public int StatusId { get; set; }
    public int PriorityId { get; set; }
    public DateOnly? DeadLine { get; set; }
    public string? Contact { get; set; }

    public long? ExecutiveUserId { get; set; }
    public IList<long>? UserTasks { get; set; }
}
