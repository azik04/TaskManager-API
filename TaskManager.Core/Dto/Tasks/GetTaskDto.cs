using TaskManager.DataProvider.Enums;

namespace TaskManager.Core.Dto.Tasks;

public class GetTaskDto
{
    public long Id { get; set; }
    public bool isDeleted { get; set; }
    public DateTime CreateAt { get; set; }
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public int StatusId { get; set; }
    public int PriorityId { get; set; }
    public DateOnly? DeadLine { get; set; }
    public long ThemeId { get; set; }
    public DateTime? DateOfCompletion { get; set; }
    public string? Contact { get; set; }
    public bool IsCompleted { get; set; }

    public bool? isUpdated { get; set; }

    public long? ExecutiveUserId { get; set; }
    public string? ExecutiveUserName { get; set; }
    public IList<long>? UserId { get; set; }

    public ICollection<string>? UserNames { get; set; }
}
