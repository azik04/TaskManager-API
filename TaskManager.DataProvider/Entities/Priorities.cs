namespace TaskManager.DataProvider.Entities;

public class Priorities
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Tasks> Tasks { get; set; }

}
