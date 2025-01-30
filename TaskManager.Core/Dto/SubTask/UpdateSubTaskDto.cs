using TaskManager.DataProvider.Enums;

namespace TaskManager.Core.Dto.SubTask;

public class UpdateSubTaskDto
{
    public string Name { get; set; }
    public string Priority { get; set; }
    public DateOnly? DeadLine { get; set; }
}
