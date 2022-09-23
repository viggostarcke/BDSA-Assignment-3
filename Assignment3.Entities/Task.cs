namespace Assignment3.Entities;

public class Task
{
    public virtual int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public virtual string Title { get; set; }

    public virtual User AssignedTo { get; set; }

    //dont know if making this property is same as making it nullable
    public virtual string? Description { get; set; }

    [Required]
    public virtual string State { get; set; }

    public virtual ICollection<Tag> Tags { get; set; }

}

public enum State{New, Active, Resolved, Closed, Removed}