namespace Assignment3.Entities;

public class User
{
    public virtual int Id { get; set; }

    public virtual string Name { get; set; } = null!;

    public virtual string Email { get; set; } = null!;

    public virtual List<Task> Tasks { get; set; } = new List<Task>{};
}
