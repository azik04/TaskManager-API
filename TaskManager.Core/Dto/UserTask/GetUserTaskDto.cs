namespace TaskManager.Core.Dto.UserTask;

public class GetUserTaskDto
{
    public long Id { get; set; }
    public bool isDeleted { get; set; }
    public DateTime CreateAt { get; set; }
    public long TaskId { get; set; }
    public string TaskName { get; set; }
    public bool isSeen { get; set; }
    public string CreatedBy { get; set; }

    public long UserId { get; set; }
    public string UserName { get; set; }
}
