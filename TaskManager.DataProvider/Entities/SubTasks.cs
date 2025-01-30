using TaskManager.DataProvider.Entities.BaseModel;

namespace TaskManager.DataProvider.Entities;

public class SubTasks:Base
{
    public string Name { get; set; }
    public string Priority { get; set; }
    public DateOnly? DeadLine { get; set; }
    public long UserId { get; set; }
    public long TaskId { get; set; }
    public bool IsCompleted { get; set; }

    public virtual Users? User { get; set; }
    public virtual Tasks? Task { get; set; }
}
