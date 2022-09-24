namespace Assignment3.Entities;

public class Task
{
    public virtual int Id { get; set; }

    public virtual string Title { get; set; } = null!;

    public virtual User? AssignedTo { get; set; }

    public virtual string? Description { get; set; }

    public virtual State State { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>{};

}

public enum State{New, Active, Resolved, Closed, Removed}