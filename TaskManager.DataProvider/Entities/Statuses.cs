namespace TaskManager.DataProvider.Entities;

public class Statuses
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Tasks> Tasks { get; set; }   
}
